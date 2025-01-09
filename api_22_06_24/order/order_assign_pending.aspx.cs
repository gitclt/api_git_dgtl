using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();

    string json;
    static string querry;
    string emp_id = "", status = "";
    string id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();

    }

    public void chk_tocken()
    {
        CommFuncs CommFuncs = new CommFuncs();

        if (Request.Headers["Authorization"] != null)
        {
            id = CommFuncs.get_tocken_details(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
        }

        if (id == "Oops! Tocken Expired!")
        {
            json = "{'status':false,'Message' :'Oops! Tocken Expired!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.StatusCode = 403;
            Response.Write(json);
            Response.End();
            return;
        }
        else if (id != "")
        {

            jobstatus(id);
        }
        else
        {
            json = "{'status':false,'Message' :'Oops! Unauthorised Access!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.StatusCode = 403;
            Response.Write(json);
            Response.End();
            return;
        }
    }
    public void jobstatus(string empid)
    {
        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }

        if (Request.Form["status"] != null)
        {
            status = Request.Form["status"];//pending/ongoing/completed
        }

        string query1 = @"
        SELECT  
            oi.id as item_id,
            oi.pro_id, 
            p.product_name,
            p.color,
            p.gsm_id,
            g.name,
            p.size,
            o.target_date, 
            oi.qty,
            oa.job_type,
            p.product_name,
            o.production_status,
            o.packing_status,
            ot.id,
            c.company_name,
            ot.order_assign_id
        FROM 
            tbl_orders o
        INNER JOIN 
            tbl_order_items oi ON oi.order_id = o.id
        INNER JOIN 
            tbl_order_assigned oa ON oa.item_id = oi.id
        INNER JOIN 
            tbl_product p ON oi.pro_id = p.id
        INNER JOIN 
            tbl_customer c ON o.customer = c.id
        INNER JOIN 
            tbl_gsm g ON p.gsm_id = g.id	
        LEFT JOIN tbl_order_task ot ON ot.order_assign_id = oa.id
        WHERE 
            oa.emp_id = '" + empid + "'";

        if (status != null)
        {
            if (status.ToLower() == "pending")
            {
                query1 += " AND (oi.production_status = 'not started' OR oi.packing_status = 'not started')";
            }
            else if (status.ToLower() == "ongoing")
            {
                query1 += " AND (oi.production_status = 'ongoing' OR oi.packing_status = 'ongoing')";
            }
            else if (status.ToLower() == "completed")
            {
                query1 += " AND (oi.production_status = 'completed' AND oi.packing_status = 'completed')";
            }
        }

        query1 += @"
        GROUP BY 
            oi.id,
            oi.pro_id, 
            p.product_name,
            p.color,
            p.gsm_id,
            g.name,
            p.size,
            o.target_date, 
            oi.qty,
            oa.job_type,
            p.product_name,
            o.production_status,
            o.packing_status,
            c.company_name,
            ot.id,
            ot.order_assign_id";

        DataSet ds = cc.joinselect(query1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'itemid':'" + ds.Tables[0].Rows[i]["item_id"].ToString() + "','pro_id':'" + ds.Tables[0].Rows[i]["pro_id"].ToString() + "'";
                json += ",'product_name':'" + ds.Tables[0].Rows[i]["product_name"].ToString() + "'";
                json += ",'color':'" + ds.Tables[0].Rows[i]["color"].ToString() + "'";
                json += ",'gsm_id':'" + ds.Tables[0].Rows[i]["gsm_id"].ToString() + "'";
                json += ",'gsm':'" + ds.Tables[0].Rows[i]["name"].ToString() + "'";
                json += ",'size':'" + ds.Tables[0].Rows[i]["size"].ToString() + "'";
                json += ",'target_date':'" + ds.Tables[0].Rows[i]["target_date"].ToString() + "'";
                json += ",'qty':'" + ds.Tables[0].Rows[i]["qty"].ToString() + "'";
                json += ",'job_type':'" + ds.Tables[0].Rows[i]["job_type"].ToString() + "'";
                json += ",'production_status':'" + ds.Tables[0].Rows[i]["production_status"].ToString() + "'";
                json += ",'packing_status':'" + ds.Tables[0].Rows[i]["packing_status"].ToString() + "'";
                json += ",'company_name':'" + ds.Tables[0].Rows[i]["company_name"].ToString() + "'";
                json += ",'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "'";

                json += ",'output_per_sec':[";

                string query2 = @"
                SELECT grade_id, output_per_sec 
                FROM tbl_product_production_process_grade g 
                INNER JOIN tbl_production_process p ON g.production_process_id = p.id  
                WHERE pro_id = '" + ds.Tables[0].Rows[i]["pro_id"].ToString() + "' AND p.process_type = '" + ds.Tables[0].Rows[i]["job_type"].ToString() + "' AND delete_status = 0";
    

            DataSet dss = cc.joinselect(query2);
                if (dss.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dss.Tables[0].Rows.Count; j++)
                    {
                        json += "{'grade':'" + dss.Tables[0].Rows[j]["grade_id"].ToString() + "','output':'" + dss.Tables[0].Rows[j]["output_per_sec"].ToString() + "'},";
                    }
                    json = json.TrimEnd(',');
                }
                json += "]";

                json += ",'task':[";

                string query3 = @"
                SELECT t.id, t.fromdatetime, t.todatetime, t.remark, ta.activity 
                FROM tbl_order_task t 
                INNER JOIN tbl_order_task_activity ta ON ta.task_id = t.id  
                WHERE t.id = '" + ds.Tables[0].Rows[i]["id"].ToString() + "'";

                DataSet ds1 = cc.joinselect(query3);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < ds1.Tables[0].Rows.Count; j++)
                    {
                        json += "{'fromdatetime':'" + ds1.Tables[0].Rows[j]["fromdatetime"].ToString() + "','todatetime':'" + ds1.Tables[0].Rows[j]["todatetime"].ToString() + "','remark':'" + ds1.Tables[0].Rows[j]["remark"].ToString() + "','activity':'" + ds1.Tables[0].Rows[j]["activity"].ToString() + "'},";
                    }
                    json = json.TrimEnd(',');
                }
                json += "]},";
            }
            json = json.TrimEnd(',');
            json += "]}";
            ds.Dispose();

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
        {
            ds.Dispose();

            json = "{'status':false,'Message':'Nodata found.','data':[]}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }



}
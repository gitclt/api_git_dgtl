using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
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
    string job_type = "", job_status = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();

        //jobstatus();
    }

    public void chk_tocken()
    {
        CommFuncs CommFuncs = new CommFuncs();

        string id = "";
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
        if (Request.Form["job_type"] != null)
        {
            job_type = Request.Form["job_type"];
        }

        if (Request.Form["job_status"] != null)
        {
            job_status = Request.Form["job_status"];
        }


        string query = @"
            SELECT 
                oa.job_type,
                o.target_date, 
                ot.todatetime,
oa.id,
c.name,
                oi.qty,
                p.product_name,
                COUNT(DISTINCT te.emp_id) AS employee_count
            FROM 
                tbl_orders o
            INNER JOIN 
              tbl_customer c ON c.id = o.customer
INNER JOIN 
                tbl_order_items oi ON oi.order_id = o.id
            INNER JOIN 
                tbl_order_assigned oa ON oa.item_id = oi.id
            INNER JOIN 
                tbl_product p ON oi.pro_id = p.id
            INNER JOIN 
                tbl_order_task ot ON oa.id = ot.order_assign_id
            INNER JOIN 
                tbl_order_task_emp te ON te.task_id = ot.id
            INNER JOIN 
                tbl_employees e ON o.addedby = e.id
            WHERE oa.emp_id = '" + empid + "' ";
        if (!string.IsNullOrEmpty(job_type))
        {
            query += "AND oa.job_type = '" + job_type + "' ";
        }

        if (!string.IsNullOrEmpty(job_status))
        {
            if (job_status.ToLower() == "production completed")
            {
                query += "AND o.production_status = 'completed' ";
            }
            else if (job_status.ToLower() == "packing completed")
            {
                query += "AND o.packing_status = 'completed' ";
            }
         
        }

        query += @"
    GROUP BY 
        o.target_date, 
        oi.qty,
        p.product_name,
        ot.todatetime,
oa.job_type,
oa.id,
c.name,


o.addedby";

        DataSet ds = cc.joinselect(query);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'target_date':'" +ds.Tables[0].Rows[i]["target_date"].ToString() + "','order_assign_id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "'";

              

                json += ",'customer':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','qty':'" + ds.Tables[0].Rows[i]["qty"].ToString() + "','product_name':'" + ds.Tables[0].Rows[i]["product_name"].ToString() + "','achieved_quantity':'','pending_quantity':'','remark':'','employee_count':'','todatetime':'" + ds.Tables[0].Rows[i]["todatetime"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
        {
            json = "{'status':false,'Message' :'Success.','data':[]}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }

}
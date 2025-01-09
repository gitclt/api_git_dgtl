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
    string job_type = "", job_status = "", order_assign_id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();

       // jobstatus();
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
        string order_assign_id = null;

        if (Request.Form["order_assign_id"] != null)
        {
            order_assign_id = Request.Form["order_assign_id"];
        }
        else
        {
            string json = "{'status':false,'Message' :'Assign ID is required!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
            return;
        }

        string query1 = @"
    SELECT distinct
        oa.id,
        p.product_name,
        ot.todatetime,
        ot.qty,
        oi.qty AS total_quantity,
        SUM(ot.qty) AS achieved_quantity,
        COUNT(DISTINCT te.emp_id) AS employee_count,
        (
            SELECT 
                COUNT(DISTINCT emp_id)
            FROM 
                tbl_order_task_emp
            WHERE 
                task_id = ot.id
        ) AS team_size
    FROM 
        tbl_orders o
    INNER JOIN 
        tbl_order_items oi ON oi.order_id = o.id
    INNER JOIN 
        tbl_order_assigned oa ON oi.id = oa.item_id
    INNER JOIN 
        tbl_order_task ot ON oa.id = ot.order_assign_id
    INNER JOIN 
        tbl_order_task_emp te ON te.task_id = ot.id
    INNER JOIN 
        tbl_product p ON oi.pro_id = p.id
    WHERE 
        oa.emp_id ='" + empid + @"' AND oa.id ='" + order_assign_id + @"'
    GROUP BY 
        oa.id,
        p.product_name,
        ot.todatetime,
        ot.qty,
        oi.qty,
        ot.id";

   

        DataSet ds = cc.joinselect(query1);

        if (ds.Tables[0].Rows.Count > 0)
        {
           
            string json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'product_name':'" + ds.Tables[0].Rows[i]["product_name"].ToString() + "','employee_count':'" + ds.Tables[0].Rows[i]["employee_count"].ToString() + "','team_size':'" + ds.Tables[0].Rows[i]["team_size"].ToString() + "'";
                json += ",'todatetime':'" + ds.Tables[0].Rows[i]["todatetime"].ToString() + "','qty':'" + ds.Tables[0].Rows[i]["qty"].ToString() + "','total_quantity':'" + ds.Tables[0].Rows[i]["total_quantity"].ToString() + "','achieved_quantity':'" + ds.Tables[0].Rows[i]["achieved_quantity"].ToString() + "'},";
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
            string json = "{'status':false,'Message' :'No data found!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }

}
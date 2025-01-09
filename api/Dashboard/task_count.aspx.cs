using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    static string date;
    static string type;

    protected void Page_Load(object sender, EventArgs e)
    {
      // chk_tocken();
       getdata();
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

    public void getdata()
    {
        string date = null;

        // Retrieve the date from the form data
        if (Request.Form["date"] != null)
        {
            date = Request.Form["date"];
        }

        // Check if date is provided
        if (string.IsNullOrEmpty(date))
        {
            // If no date is provided, return "No data found!"
            json = "{'status':false,'Message':'No data found!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
            return;
        }

        string query1 = @"SELECT 
    e.id AS employee_id,
    e.name,
    (
        SELECT COUNT(te.emp_id)
        FROM tbl_task t
        LEFT JOIN tbl_task_emp te ON te.task_id = t.id
        WHERE t.startdate = '" + date + @"' 
          AND e.id = te.emp_id
    ) AS task_count,
    (
        SELECT COUNT(te.emp_id)
        FROM tbl_task t
        LEFT JOIN tbl_task_emp te ON te.task_id = t.id
        WHERE t.startdate = '" + date + @"' 
          AND e.id = te.emp_id 
          AND t.status = 'completed'
    ) AS completed_task_count,
    CONVERT(
        VARCHAR(8),
        DATEADD(
            SECOND,
            ISNULL(SUM(DATEDIFF(SECOND, 0, l.time_spent)), 0),
            0
        ),
        108
    ) AS total_time_spent
FROM 
    tbl_employee e
LEFT JOIN 
    tbl_log_time l ON l.created_user_id = e.id 
    AND l.date = '"+date+ @"'
GROUP BY 
    e.id, e.name";

        DataSet ds = cc.joinselect(query1);

        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'task_count':'" + ds.Tables[0].Rows[i]["task_count"].ToString() + "','total_time_spent':'" + ds.Tables[0].Rows[i]["total_time_spent"].ToString() + "','employee_id':'" + ds.Tables[0].Rows[i]["employee_id"].ToString() + "','employee_name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','completed_task_count':'" + ds.Tables[0].Rows[i]["completed_task_count"].ToString() + "'},";
            }

            json = json.TrimEnd(',');
            json += "]}";
        }
        else
        {
            json = "{'status':false,'Message':'No data found!'}";
        }

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

}
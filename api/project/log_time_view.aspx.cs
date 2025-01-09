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

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry = "",project_id = "",emp_id="";

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
            // Valid token logic here
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
        string project_id = null, emp_id = null;

        if (Request.Form["project_id"] != null)
        {
            project_id = Request.Form["project_id"];
        }
        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }

        string querry1 = @"SELECT 
        lt.id,
		lt.date,

        tl.pro_id,
        t.id AS task_id,
        e.name,
        t.taskname,
        tl.id AS tasklist_id,
        tl.name AS list_name,
        CONVERT(VARCHAR(8), DATEADD(SECOND, SUM(DATEDIFF(SECOND, '00:00:00', lt.time_spent)), '00:00:00'), 108) AS total_time_spent,
        CONVERT(TIME, DATEADD(SECOND, (
            SELECT SUM(DATEDIFF(SECOND, '00:00:00', l.time_spent)) 
            FROM tbl_log_time l
            INNER JOIN tbl_task t2 ON l.task_id = t2.id
            WHERE l.mark_billable = 1 AND t2.pro_id = tl.pro_id
        ), '00:00:00'), 108) AS total_time_spent_billable,
        lt.start_time,
        lt.end_time,
        lt.mark_billable,
        lt.is_complete,
        e.image
    FROM 
        tbl_log_time lt
    LEFT JOIN 
        tbl_task t ON lt.task_id = t.id
    LEFT JOIN 
        tbl_task_emp te ON lt.task_id = te.task_id
    LEFT JOIN 
        tbl_tasklist tl ON t.tasklist_id = tl.id
    LEFT JOIN 
        tbl_employee e ON te.emp_id = e.id 
    WHERE 
        tl.pro_id ='" + project_id + "' ";

        if (!string.IsNullOrEmpty(emp_id))
        {
            querry1 += "AND te.emp_id = '" + emp_id + "' ";
        }

        querry1 += @"
    GROUP BY 
        lt.id, tl.pro_id, t.id, e.name, t.taskname, tl.id, tl.name, lt.start_time, lt.end_time, lt.mark_billable, lt.is_complete, e.image,lt.date

    ORDER BY 
        lt.id desc";

        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "'," +
                       "'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "'," +
                        "'date':'" + ds.Tables[0].Rows[i]["date"].ToString() + "'," +

                        "'task_id':'" + ds.Tables[0].Rows[i]["task_id"].ToString() + "'," +
                        "'taskname':'" + ds.Tables[0].Rows[i]["taskname"].ToString() + "'," +
                        "'tasklist_id':'" + ds.Tables[0].Rows[i]["tasklist_id"].ToString() + "'," +
                        "'list_name':'" + ds.Tables[0].Rows[i]["list_name"].ToString() + "'," +
                        "'start_time':'" + ds.Tables[0].Rows[i]["start_time"].ToString() + "'," +
                        "'end_time':'" + ds.Tables[0].Rows[i]["end_time"].ToString() + "'," +
                        "'time_spent':'" + ds.Tables[0].Rows[i]["total_time_spent"].ToString() + "'," +
                        "'mark_billable':'" + ds.Tables[0].Rows[i]["mark_billable"].ToString() + "'," +
                        "'is_complete':'" + ds.Tables[0].Rows[i]["is_complete"].ToString() + "'," +
                        "'image':'" + ds.Tables[0].Rows[i]["image"].ToString() + "'," +
                        "'total_time_spent_billable':'" + ds.Tables[0].Rows[i]["total_time_spent_billable"].ToString() + "'},";
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
            json = "{'status':false,'Message' :'No data found!'}";
            ds.Dispose();
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }


}

using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry = "", list_id = "",date="", taskname="", task_status="";

    protected void Page_Load(object sender, EventArgs e)
    {
        //chk_tocken();
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
        if (Request.Form["list_id"] != null)
        {
            list_id = Request.Form["list_id"];
        }
        if (Request.Form["date"] != null)
        {
            date = Request.Form["date"];
        }

        if (Request.Form["taskname"] != null)
        {
            taskname = Request.Form["taskname"];
        }

        if (Request.Form["task_status"] != null)
        {
            task_status = Request.Form["task_status"];
        }

        string query1 = @"SELECT 
    t.id, 
    t.tasklist_id, 
    t.taskname AS task_name,
    t.startdate,
    t.duedate,
    t.estimated_time,
    tl.name,
    t.status,
    t.priority_id,
	p.name as priority,
    t.tag,
    COALESCE(
        RIGHT('0' + CAST(SUM(DATEDIFF(SECOND, '00:00:00', lt.time_spent)) / 3600 AS VARCHAR), 2) + ':' +
        RIGHT('0' + CAST((SUM(DATEDIFF(SECOND, '00:00:00', lt.time_spent)) % 3600) / 60 AS VARCHAR), 2) + ':' +
        RIGHT('0' + CAST(SUM(DATEDIFF(SECOND, '00:00:00', lt.time_spent)) % 60 AS VARCHAR), 2), 
    '00:00:00') AS total_time
FROM 
    tbl_task t
INNER JOIN 
    tbl_tasklist tl ON tl.id = t.tasklist_id
	LEFT JOIN 
    tbl_priority p ON p.id = t.priority_id
LEFT JOIN 
    tbl_log_time lt ON lt.task_id = t.id WHERE   t.tasklist_id='" + list_id + "'";
    
        if (!string.IsNullOrEmpty(taskname))
        {
            query1 += " AND t.taskname = '" + taskname + "' ";
        }
        if (!string.IsNullOrEmpty(task_status))
        {
            query1 += " AND t.status = '" + task_status + "' ";
        }
        if (!string.IsNullOrEmpty(date))
        {
            query1 += " AND cast(t.startdate as date)= '" + date + "' ";
        }
        query1 += @" GROUP BY  t.id, t.tasklist_id, t.taskname, t.startdate, t.duedate, t.estimated_time, tl.name, t.status, t.priority_id, t.tag,p.name";

        DataSet ds = cc.joinselect(query1);
       
        if (ds.Tables[0].Rows.Count > 0)
        {
           
            json = "{'status':true,'Message':'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                

                json += "{'task_id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','task_name':'" + ds.Tables[0].Rows[i]["task_name"].ToString().Replace("\"", "") + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "'," +
                        "'startdate':'" + ds.Tables[0].Rows[i]["startdate"].ToString() + "','duedate':'" + ds.Tables[0].Rows[i]["duedate"].ToString() + "','priority_id':'" + ds.Tables[0].Rows[i]["priority_id"].ToString() + "','priority':'" + ds.Tables[0].Rows[i]["priority"].ToString() + "',"+
                        "'logged_time':'" + ds.Tables[0].Rows[i]["total_time"].ToString() + "','tag':'" + ds.Tables[0].Rows[i]["tag"].ToString() + "','estimated_time':'" + ds.Tables[0].Rows[i]["estimated_time"].ToString() + "','tasklist_id':'" + ds.Tables[0].Rows[i]["tasklist_id"].ToString() + "',";
               
                json += "'employees':[";

                string query2 = @"SELECT te.emp_id,te.id,e.name FROM tbl_task_emp te
inner join tbl_employee e on e.id=te.emp_id WHERE te.task_id='" + ds.Tables[0].Rows[i]["id"].ToString() + "'";
                DataSet dss = cc.joinselect(query2);
                if (dss.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dss.Tables[0].Rows.Count; j++)
                    {
                        json += "{'emp_id':'" + dss.Tables[0].Rows[j]["emp_id"].ToString() + "','id':'" + dss.Tables[0].Rows[j]["id"].ToString() + "','name':'" + dss.Tables[0].Rows[j]["name"].ToString() + "'},";
                        //json += "{'id':'" + dss.Tables[0].Rows[j]["id"].ToString() + "'},";

                    }
                    json = json.TrimEnd(',');
                }
                json += "]";
                dss.Dispose();
                json += "},";
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
            json = "{'status':false,'Message':'No data found!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

public partial class api_project_Default : System.Web.UI.Page
{

    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    CommFuncs CommFuncs = new CommFuncs();
    string json;
    static string querry;
    string id = "", project_id = "";
    protected void Page_Load(object sender, EventArgs e)
    {
       chk_tocken();
        getdata();

    }


    public void chk_tocken()
    {
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
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
    public class DataResponse
    {
        public string type;
        public string emp_id;

    }
    public void getdata()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string json = "";
        string message = "";
        foreach (var data in DataResponse)
        {
            string curdate = DateTime.Now.ToString("yyyy-MM-dd");

            if (data.type == "month")
            {
                string query1 = @"SELECT 
    SUM(DATEDIFF(SECOND, '00:00:00', l.time_spent)) / 3600.0 AS total_time
FROM tbl_log_time l
INNER JOIN tbl_task_emp te ON te.task_id = l.task_id
WHERE te.emp_id = '"+data.emp_id+"' AND DATEPART(MONTH, l.date) = DATEPART(MONTH, GETDATE());";

                DataSet ds = cc.joinselect(query1);
              
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':true,'Message' :'Success.','data':[";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        json += "{'total_time':'" + ds.Tables[0].Rows[i]["total_time"].ToString() + "'},";
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
                    json = "{ \"status\": false, \"Message\": \"No data found!\" }";
                    Response.ContentType = "application/json";
                    ds.Dispose();
                    Response.Write(json);
                    Response.End();
                }
            }
            if (data.type == "week")
            {
                string query1 = @"SELECT 
    SUM(DATEDIFF(SECOND, '00:00:00', l.time_spent)) / 3600.0 AS total_time
FROM tbl_log_time l
INNER JOIN tbl_task_emp te ON te.task_id = l.task_id
WHERE te.emp_id = '" + data.emp_id+"' and  DATEPART(WEEK, l.date) = DATEPART(WEEK, GETDATE());";

                DataSet ds = cc.joinselect(query1);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':true,'Message' :'Success.','data':[";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        json += "{'total_time':'" + ds.Tables[0].Rows[i]["total_time"].ToString() + "'},";
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
                    json = "{ \"status\": false, \"Message\": \"No data found!\" }";
                    Response.ContentType = "application/json";
                    ds.Dispose();
                    Response.Write(json);
                    Response.End();
                }
            }
            if (data.type == "day")
            {
                string query1 = @"
SELECT 
    SUM(DATEDIFF(SECOND, '00:00:00', l.time_spent)) / 3600.0 AS total_time
FROM tbl_log_time l
INNER JOIN tbl_task_emp te ON te.task_id = l.task_id
WHERE te.emp_id = '" + data.emp_id+"'AND CAST(l.date AS DATE) = CAST(GETDATE() AS DATE);";

                DataSet ds = cc.joinselect(query1);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':true,'Message' :'Success.','data':[";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        json += "{'total_time':'" + ds.Tables[0].Rows[i]["total_time"].ToString() + "'},";
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
                    json = "{ \"status\": false, \"Message\": \"No data found!\" }";
                    Response.ContentType = "application/json";
                    ds.Dispose ();      
                    Response.Write(json);
                    Response.End();
                }
            }

        }
    }
}
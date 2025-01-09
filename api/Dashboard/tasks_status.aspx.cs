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
    ISNULL(SUM(CASE WHEN status = 'completed' THEN 1 ELSE 0 END), 0) AS Completed,
    ISNULL(SUM(CASE WHEN status = 'ongoing' THEN 1 ELSE 0 END), 0) AS Ongoing,
    ISNULL(SUM(CASE WHEN status = 'pending' THEN 1 ELSE 0 END), 0) AS Incomplete,
    COUNT(id) AS task_count
FROM 
    tbl_task
WHERE 
    DATEPART(MONTH, startdate) = DATEPART(MONTH, GETDATE());";

                DataSet ds = cc.joinselect(query1);
              
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':true,'Message' :'Success.','data':[";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        json += "{'task_count':'" + ds.Tables[0].Rows[i]["task_count"].ToString() + "','Completed':'" + ds.Tables[0].Rows[i]["Completed"].ToString() + "','Ongoing':'" + ds.Tables[0].Rows[i]["Ongoing"].ToString() + "','Incomplete':'" + ds.Tables[0].Rows[i]["Incomplete"].ToString() + "'},";
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
    ISNULL(SUM(CASE WHEN status = 'completed' THEN 1 ELSE 0 END), 0) AS Completed,
    ISNULL(SUM(CASE WHEN status = 'ongoing' THEN 1 ELSE 0 END), 0) AS Ongoing,
    ISNULL(SUM(CASE WHEN status = 'pending' THEN 1 ELSE 0 END), 0) AS Incomplete,
    COUNT(id) AS task_count
FROM 
    tbl_task
WHERE 
    DATEPART(WEEK, startdate) = DATEPART(WEEK, GETDATE());";

                DataSet ds = cc.joinselect(query1);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':true,'Message' :'Success.','data':[";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        json += "{'task_count':'" + ds.Tables[0].Rows[i]["task_count"].ToString() + "','Completed':'" + ds.Tables[0].Rows[i]["Completed"].ToString() + "','Ongoing':'" + ds.Tables[0].Rows[i]["Ongoing"].ToString() + "','Incomplete':'" + ds.Tables[0].Rows[i]["Incomplete"].ToString() + "'},";
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
    ISNULL(SUM(CASE WHEN status = 'completed' THEN 1 ELSE 0 END), 0) AS Completed,
    ISNULL(SUM(CASE WHEN status = 'ongoing' THEN 1 ELSE 0 END), 0) AS Ongoing,
    ISNULL(SUM(CASE WHEN status = 'pending' THEN 1 ELSE 0 END), 0) AS Incomplete,
    COUNT(id) AS task_count
FROM 
    tbl_task
WHERE 
    CAST(startdate AS DATE) = CAST('"+curdate+"' AS DATE);";

                DataSet ds = cc.joinselect(query1);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':true,'Message' :'Success.','data':[";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        json += "{'task_count':'" + ds.Tables[0].Rows[i]["task_count"].ToString() + "','Completed':'" + ds.Tables[0].Rows[i]["Completed"].ToString() + "','Ongoing':'" + ds.Tables[0].Rows[i]["Ongoing"].ToString() + "','Incomplete':'" + ds.Tables[0].Rows[i]["Incomplete"].ToString() + "'},";
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    protected void Page_Load(object sender, EventArgs e)
    {
       // chk_tocken();

        insert();
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


    public class DataResponse
    {
        public string task_id;
        public string date;
        public string start_time;
        public string end_time;
        public string time_spent;
        public string mark_billable;
        public string is_complete;
        public string id;
        public string type;
        public string created_user_id;

    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string querry1 = "";
        string message = "";
        bool hasError = false;
        string errorMsg = "";

        foreach (var data in DataResponse)
        {
            try
            {
                string Date = DateTime.Parse(data.date).ToString("yyyy-MM-dd");
                string StartTime = DateTime.Parse(data.start_time).ToString("HH:mm:ss");
                string EndTime = DateTime.Parse(data.end_time).ToString("HH:mm:ss");

                if (!IsValidTimeSpent(data.time_spent))
                {
                    hasError = true;
                    errorMsg = "Invalid time spent format";
                    break; 
                }

                querry1 += @"insert into tbl_log_time (task_id,date,start_time,end_time,time_spent,mark_billable,is_complete,created_user_id)
                        values('" + data.task_id + "','" + Date + "','" + StartTime + "','" + EndTime + "','" + data.time_spent + "','" + data.mark_billable + "','" + data.is_complete + "','" + data.created_user_id + "') ";

                if (data.is_complete == "1")
                {
                    querry1 += @"update tbl_task set status='completed' where id='" + data.task_id + "'";
                }
            }
            catch (FormatException ex)
            {
                hasError = true;
                break;
            }
        }

        if (hasError)
        {
            json = "{'status':false,'Message' :'" + errorMsg + "'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
            return;
        }

        int status = cc.Insert(querry1);
        if (status > 0)
        {
            message = "Data added successfully.";
            json = "{'status':true,'Message' :'" + message + "'}";
        }
        else
        {
            json = "{'status':false,'Message' :'Failed'}";
        }

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

    private bool IsValidTimeSpent(string timeSpent)
    {
        // Check if time_spent matches hh:mm:ss format
        return Regex.IsMatch(timeSpent, @"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$");
    }
}

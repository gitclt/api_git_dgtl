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
    string query1 = "";

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
        public string emp_id;
        public string type;
       // public string id;
      
    }

  
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string json = "";
        string message = "";
        int count = 0;

        foreach (var data in DataResponse)
        {
            string query1 = "";

            if (data.type == "add")
            {
                query1 += @"insert into tbl_task_emp (task_id, emp_id) 
                        values('" + data.task_id + "','" + data.emp_id + "');";
                      message = "Data added successfully.";
            }
           
            else if (data.type == "delete")
            {
                query1 += @"delete from tbl_task_emp where task_id='" + data.task_id + "' and emp_id='" + data.emp_id + "'";
                            message = "Data added successfully.";
            }

            int status = cc.Insert(query1);

            if (status > 0)
            {
                count++;
            }
            else
            {
                // If any operation fails, return a failed response.
                json = "{'status':false,'Message' :'Failed to process data.'}";
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;
            }
        }

        if (count > 0)
        {
            json = "{'status':true,'Message':'" + message + "'}";
        }
        else
        {
            json = "{'status':false,'Message':'No data was processed.'}";
        }

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }


}





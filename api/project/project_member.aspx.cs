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
    protected void Page_Load(object sender, EventArgs e)
    {
         //chk_tocken();

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
        public string project_id;
        public string emp_id;
        public string type;
        public string id;
        public string workspace_id;
        public string delete_status;
        public string deleted_on;
    }

    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        string query = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string qry = "";
        int status = 0;
        string message = "";
        bool deleteExecuted = false;

        foreach (var data in DataResponse)
        {
            string query1 = "";

            if (data.type == "add")
            {
                query1 += @" insert into tbl_projectmember (project_id, emp_id, workspace_id, delete_status) 
                        values('" + data.project_id + "','" + data.emp_id + "','" + data.workspace_id + "', 0);";
                message = "Data added successfully.";
            }
            else if (data.type == "edit")
            {
                if (!deleteExecuted)
                {
                    query1 += @"delete from tbl_projectmember where project_id=" + data.project_id + ";";
                    deleteExecuted = true;
                }
                query1 += @"insert into tbl_projectmember (project_id, emp_id, workspace_id, delete_status) 
                        values('" + data.project_id + "','" + data.emp_id + "','" + data.workspace_id + "', 0);";
                message = "Data updated successfully.";
            }
            else if (data.type == "delete")
            {
                query1 = @"delete from tbl_projectmember where id=" + data.id + ";";
                message = "Data deleted successfully.";
            }

            query += query1;
        }

        status = cc.Insert(query);

        string json;
        if (status > 0)
        {
            json = "{'status':true,'Message' :'" + message + "'}";
        }
        else
        {
            json = "{'status':false,'Message' :'Oops! Something went wrong'}";
        }

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

}





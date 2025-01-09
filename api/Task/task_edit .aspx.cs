using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
//using System.ServiceModel.Channels;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry1 = "";
    protected void Page_Load(object sender, EventArgs e)
    {
         chk_tocken();

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
        public string tasklist_id;
        public string taskname;
        public string emp_id;
        public string startdate;
        public string duedate;
        public string id;
        public string type;
        public string estimated_time;
        public string status;


    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();



            List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

            foreach (var data in DataResponse)
            {
                string query = "UPDATE tbl_employees SET ";
                if (!string.IsNullOrEmpty(data.tasklist_id)) query += "tasklist_id='" + data.tasklist_id + "',";
                if (!string.IsNullOrEmpty(data.taskname)) query += "taskname='" + data.taskname + "',";
                if (!string.IsNullOrEmpty(data.emp_id)) query += "emp_code='" + data.emp_id + "',";
                if (!string.IsNullOrEmpty(data.startdate)) query += "startdate='" + data.startdate + "',";
                if (!string.IsNullOrEmpty(data.duedate)) query += "duedate='" + data.duedate + "',";
                if (!string.IsNullOrEmpty(data.status)) query += "grade='" + data.status + "',";
                if (!string.IsNullOrEmpty(data.estimated_time)) query += "duedate='" + data.estimated_time + "',";
              

                query = query.TrimEnd(',');

                query += " WHERE id=" + data.id;



            }
            int status = cc.Insert(querry1);

        if (status > 0)
        {
            json = "{'status':true,'Message' :'Data updated successfully.'}";
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
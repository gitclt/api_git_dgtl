using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public string project_id;
        public string note;
        public string title;
        public string deleted_status;
        public string emp_id;
        public string color;
        public string deleted_by;
        public string type;     
        public string id;     
    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string qry = "";
        int count = 0;
        string message = "";

        foreach (var data in DataResponse)
        {
            string querry1 = "";


            if (data.type == "add")
            {
                 querry1 = @"insert into tbl_stickynote (project_id,title,note,color,deleted_status,emp_id) values('" + data.project_id + "','" + data.title + "','"+data.note+"','"+data.color+"',0,'"+data.emp_id+"')";
                message = "Data added successfully.";

            }
            if (data.type == "edit")
            {
                querry1 = @"update tbl_stickynote set project_id='"+data.project_id + "',title='"+data.title + "',note='"+data.note + "',color='"+data.color + "',emp_id='"+data.emp_id + "' where id=" + data.id + " ";
                message = "Data updated successfully.";

            }

            else if(data.type == "delete")
            {
                querry1 = @"update tbl_stickynote set deleted_status=1,deleted_on=getdate() where id=" + data.id + " ";
                message = "Data deleted successfully.";

            }
         
            int status = cc.Insert(querry1);
            if (status > 0)
            {
                //json = "{'status':true,'Message' :'Data updated successfully.'}";

                json = "{'status':true,'Message' :'" + message + "'}";
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
            else
                json = "{'status':false,'Message' :'Failed'}";
        }

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
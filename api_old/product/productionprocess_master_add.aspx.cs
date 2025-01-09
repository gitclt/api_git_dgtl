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
        public string process_type;
        public string process_name;

        public string type;
        public string id;
    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        querry = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        string qry = "";
        string message = "";

        int count = 0;
        foreach (var data in DataResponse)
        {
            string querry1 = "";
            if (data.type == "add")
            {

                    querry1 = @"insert into tbl_production_process (process_type,process_name,delete_status) values('" + data.process_type + "','" + data.process_name + "',0)";
                    message = "Data added successfully.";
           

            }
            else if (data.type == "edit")
            {
                querry1 = @"update tbl_production_process set process_type='" + data.process_type + "',process_name='" + data.process_name + "' where id=" + data.id + " ";
                message = "Data updated successfully.";

            }
            else if (data.type == "delete")
            {
                querry1 = @"update tbl_production_process set delete_status=1 where id=" + data.id + " ";

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
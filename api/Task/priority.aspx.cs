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
        public string priority;
         
        public string task_id;     
    }
    public void insert()
    {
        //string r = "", s = "";
        //r = "type=admin&username=admin&password=123456";
        //s = EncodeDecode.base64Encode(r);
        //Response.Write(s);
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


                 querry1 = @"update tbl_task set priority_id='"+data.priority+"' where id='"+data.task_id+"'";
                message = "Data updated successfully.";

            
          

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
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();

        Insert();
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
        public string password;
        public string confirmpassword;
        public string id;
    }

    public void Insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        querry = "";
        List<DataResponse> dataResponses = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string message = "";

        foreach (var data in dataResponses)
        {
            if (data.password == data.confirmpassword)
            {
             
                string query = @"UPDATE tbl_employees SET password='" + data.password + "' WHERE id=" + data.id;
                //Response.Write("aaa");
                //return;
                int status = cc.Insert(query);
                if (status > 0)
                {
                    message = "Password updated successfully.";
                    json = "{\"status\":true,\"Message\":\"" + message + "\"}";
                }
                else
                {
                    message = "Oops! Something went wrong.";
                    json = "{\"status\":false,\"Message\":\"" + message + "\"}";
                }
            }
            else
            {
                message = "Passwords do not match.";
                json = "{\"status\":false,\"Message\":\"" + message + "\"}";
            }
        }

        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}
using DocumentFormat.OpenXml.Drawing.Charts;
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
    string id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
       chk_tocken();

        getdata();
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

    public void getdata()
    {
        string id = "";
        string empid = "";
        string ip_address = "";
        if (Request.Form["id"] != null)
        {
            id = Request.Form["id"];
        }
        if (Request.Form["empid"] != null)
        {
            empid = Request.Form["empid"];
        }
        if (Request.Form["ip_address"] != null)
        {
            ip_address = Request.Form["ip_address"];
        }

        string query1 = @"UPDATE tbl_employees 
SET delete_status = 1, 
    deleted_on = GETDATE(), 
    deleted_by = '"+ empid + "',ip_address='"+ip_address+"' WHERE id = '"+id+"'";
       
        int status = cc.Insert(query1);
        string json;

        if (status > 0)
        {
            json = "{'status':true,'Message' :'Data deleted successfully.'}";
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
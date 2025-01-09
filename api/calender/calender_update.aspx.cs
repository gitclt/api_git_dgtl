using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();

    string json;
    static string querry;
    string is_holiday = "";
    string date = "", holiday_name="";

    protected void Page_Load(object sender, EventArgs e)
    {
       //chk_tocken();
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
        if (Request.Form["date"] != null)
        {
            date = Request.Form["date"];
        }
        //if (Request.Form["is_holiday"] != null)
        //{
        //    is_holiday = Request.Form["is_holiday"];
        //}
        if (Request.Form["holiday_name"] != null)
        {
            holiday_name = Request.Form["holiday_name"];
        }

        string querry1 = @"update tbl_calendar set is_holiday=1,holiday_name='"+ holiday_name + "' where calendar_date='"+date+"'";

        string message = "Data updated successfully";
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


        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

}
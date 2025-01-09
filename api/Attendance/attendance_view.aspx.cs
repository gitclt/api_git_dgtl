using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
    string month = "";
    string date = "";

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
        if (Request.Form["month"] != null)
        {
            month = Request.Form["month"];
        }
        if (Request.Form["date"] != null)
        {
            date = Request.Form["date"];
        }

        string querry1 = @"select a.emp_id,e.name,a.addedon,a.check_in,a.check_out,a.location,a.work_hour,a.id
from 
tbl_attendance a 
inner join tbl_employee e on e.id=a.emp_id";


        if (!string.IsNullOrEmpty(date))
        {
            querry1 += " AND CAST(a.addedon AS DATE) = '" + date + "' ";
        }
        if (!string.IsNullOrEmpty(month))
        {
            querry1 += " AND MONTH(a.addedon) = '" + month + "' ";
        }
        DataSet ds = cc.joinselect(querry1);
        
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "'," +
                    "'addedon':'" + ds.Tables[0].Rows[i]["addedon"].ToString() + "','check_in':'" + ds.Tables[0].Rows[i]["check_in"].ToString() + "'," +
                    "'check_out':'" + ds.Tables[0].Rows[i]["check_out"].ToString() + "','location':'" + ds.Tables[0].Rows[i]["location"].ToString() + "'," +
                    "'work_hour':'" + ds.Tables[0].Rows[i]["work_hour"].ToString() + "','emp_id':'" + ds.Tables[0].Rows[i]["emp_id"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";
            ds.Dispose();
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
            json = "{'status':false,'Message' :'No data found!'}";

        ds.Dispose();

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

}
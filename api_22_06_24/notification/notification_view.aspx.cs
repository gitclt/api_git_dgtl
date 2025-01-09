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
    CommFuncs CommFuncs = new CommFuncs();
    string json;
    static string querry;
    string name = "", mobile = "",email="",location="",district="",state="",no_of_emp="";
    string id = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
    }
    public void chk_tocken()
    {
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
        }
        else if (id != "")
        {

            
            getdata(id);

        }
        else
        {
            json = "{'status':false,'Message' :'Oops! Unauthorised Access!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.StatusCode = 403;
            Response.Write(json);
            Response.End();
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
    public void getdata(string empid)
    {
        string querry1 = @"
        SELECT  title,description,date_time,type
        FROM tbl_notification
        WHERE emp_id = '" + empid + "'";

        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'title':'" + ds.Tables[0].Rows[i]["title"].ToString() + "','type':'" + ds.Tables[0].Rows[i]["type"].ToString() + "','date_time':'" + Convert.ToDateTime(ds.Tables[0].Rows[i]["date_time"]).ToString("yyyy-MM-dd HH:mm") + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
            ds.Dispose();   
        }
        else
        {
            json = "{'status':false,'Message' :'No data found!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}
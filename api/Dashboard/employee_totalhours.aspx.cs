using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    static string emp_id;
    static string project_id;
    static string type;

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

        if (Request.Form["project_id"] != null)
        {
            project_id = Request.Form["project_id"];
        }
        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }
       
      
            string querry1 = @"SELECT 
    SUM(DATEDIFF(SECOND, '00:00:00', l.time_spent)) / 3600.0 AS total_time_spent_hours
FROM tbl_log_time l
INNER JOIN tbl_task t ON t.id = l.task_id
INNER JOIN tbl_task_emp te ON t.id = te.task_id where te.emp_id='" + emp_id+"' and t.pro_id='"+project_id+"'";
        DataSet ds = cc.joinselect(querry1);



        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'totalhours':'" + ds.Tables[0].Rows[i]["total_time_spent_hours"].ToString() + "'},";
            }

            json = json.TrimEnd(',');
            json += "]}";
            ds.Dispose();
        }
        else
        {
            json = "{'status':false,'Message':'No data found!'}";
            ds.Dispose();

        }

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

}
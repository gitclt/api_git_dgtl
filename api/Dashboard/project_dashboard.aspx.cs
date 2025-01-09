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
        string project_id = "";
        string emp_id = "";
        string json = "";
        string type = "";

        if (Request.Form["project_id"] != null)
        {
            project_id = Request.Form["project_id"];
        }
        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }
        if (Request.Form["type"] != null)
        {
            type = Request.Form["type"];
        }

        DataSet ds = null;
        string curdate = DateTime.Now.ToString("yyyy-MM-dd");

        if (type == "today")
        {
            string querry1 = @"
select distinct taskname,duedate from tbl_task t
inner join tbl_task_emp te on te.task_id=t.id where t.pro_id='" + project_id + "' and CAST(startdate AS DATE) = '" + curdate + "' and te.emp_id='" + emp_id + "' order by taskname";
            ds = cc.joinselect(querry1);
        }
        else if (type == "ongoing")
        {
            string querry1 = @"
select distinct taskname,duedate from tbl_task t
inner join tbl_task_emp te on te.task_id=t.id where t.pro_id='" + project_id + "' and t.status='ongoing' and te.emp_id='" + emp_id + "' order by taskname";
            ds = cc.joinselect(querry1);
        }
        else if (type == "upcoming")
        {
            string querry1 = @"
select distinct taskname,duedate from tbl_task t
inner join tbl_task_emp te on te.task_id=t.id where t.pro_id='" + project_id + "' and t.status<>'completed' and te.emp_id='" + emp_id + "' order by taskname";
            ds = cc.joinselect(querry1);
        }

        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'taskname':'" + ds.Tables[0].Rows[i]["taskname"].ToString() + "','date':'" + ds.Tables[0].Rows[i]["duedate"].ToString() + "'},";
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
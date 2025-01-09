using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry = "", list_id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
       // chk_tocken();
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
            // Valid token logic here
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
        if (Request.Form["list_id"] != null)
        {
            list_id = Request.Form["list_id"];
        }

        string query1 = @"SELECT t.id, t.tasklist_id, t.taskname AS task_name,t.startdate,t.duedate,t.estimated_time,tl.name,t.status
                      FROM tbl_task t
					 --  INNER JOIN tbl_status s ON t.status = s.id
                      INNER JOIN tbl_tasklist tl ON tl.id = t.tasklist_id
                      WHERE t.tasklist_id='" + list_id + "'";

        DataSet ds = cc.joinselect(query1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'task_id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','task_name':'" + ds.Tables[0].Rows[i]["task_name"].ToString().Replace("\"", "") + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "'," +
                        "'startdate':'" + ds.Tables[0].Rows[i]["startdate"].ToString() + "','duedate':'" + ds.Tables[0].Rows[i]["duedate"].ToString() + "',";
                json += "'employees':[";
                string query2 = @"SELECT emp_id,id FROM tbl_task_emp WHERE task_id='" + ds.Tables[0].Rows[i]["id"].ToString() + "'";
                DataSet dss = cc.joinselect(query2);
                if (dss.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dss.Tables[0].Rows.Count; j++)
                    {
                        json += "{'emp_id':'" + dss.Tables[0].Rows[j]["emp_id"].ToString() + "','id':'" + dss.Tables[0].Rows[j]["id"].ToString() + "'},";
                        //json += "{'id':'" + dss.Tables[0].Rows[j]["id"].ToString() + "'},";

                    }
                    json = json.TrimEnd(',');
                }
                json += "]";
                dss.Dispose();
                json += "},";
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
        {
            ds.Dispose();
            json = "{'status':false,'Message':'No data found!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }


}

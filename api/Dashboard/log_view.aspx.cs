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
using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry = "",project_id = "",emp_id="", type="";

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
        if (Request.Form["project_id"] != null)
        {
            project_id = Request.Form["project_id"];
        }
       
    

        string querry1 = @"select distinct l.task_id,l.start_time,l.end_time,l.time_spent,t.pro_id,l.created_user_id,t.taskname,e.name
from tbl_log_time l
inner join tbl_task t on l.task_id=t.id 
 left join tbl_employee e on e.id=l.created_user_id
where t.pro_id='" + project_id+"'";
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'task_id':'" + ds.Tables[0].Rows[i]["task_id"].ToString() + "','taskname':'" + ds.Tables[0].Rows[i]["taskname"].ToString() + "'," +
                    "'start_time':'" + ds.Tables[0].Rows[i]["start_time"].ToString() + "','end_time':'" + ds.Tables[0].Rows[i]["end_time"].ToString() + "','time_spent':'" + ds.Tables[0].Rows[i]["time_spent"].ToString() + "'," +
                    "'pro_id':'" + ds.Tables[0].Rows[i]["pro_id"].ToString() + "','created_user_id':'" + ds.Tables[0].Rows[i]["created_user_id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
        {
            json = "{'status':false,'Message' :'No data found!'}";
        }

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}

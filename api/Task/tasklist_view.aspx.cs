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
    static string pro_id;



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
       
            if (Request.Form["pro_id"] != null)
            {
                pro_id = Request.Form["pro_id"];
            }


        string query1 = @"
    SELECT 
        COUNT(ts.id) AS incomplete_task_count,
        t.id,
        t.pro_id,
        t.name,
        t.status,
        t.user_id,
        t.color,
        p.name AS projectname
    FROM 
        tbl_tasklist t
    LEFT JOIN 
        tbl_projectmaster p ON p.id = t.pro_id 
    LEFT JOIN 
        tbl_task ts ON ts.pro_id = p.id AND ts.status != 'completed'
    WHERE 
        t.pro_id = '"+pro_id+"' AND p.delete_status = 0";

        query1 += @"GROUP BY t.id,t.pro_id,t.name,t.status,t.user_id,t.color,p.name order by t.name";

        DataSet ds = cc.joinselect(query1);
       
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'project_id':'" + ds.Tables[0].Rows[i]["pro_id"].ToString() + "','task_name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "','user_id':'" + ds.Tables[0].Rows[i]["user_id"].ToString() + "','color':'" + ds.Tables[0].Rows[i]["color"].ToString() + "','projectname':'" + ds.Tables[0].Rows[i]["projectname"].ToString() + "','incomplete_task_count':'" + ds.Tables[0].Rows[i]["incomplete_task_count"].ToString() + "'},";
            }

            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
            json = "{'status':false,'Message' :'No data found!'}";


        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
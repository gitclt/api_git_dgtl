using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
//using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
     string querry;
     string pro_id="",status="";



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

        if (Request.Form["status"] != null)
        {
            status = Request.Form["status"];
        }

        string querry1 = @"
        SELECT 
            t.id, 
            t.pro_id, 
            t.name, 
            t.status, 
            t.user_id, 
            t.color, 
            p.name AS projectname,
            COUNT(ts.id) AS task_count
        FROM 
            tbl_tasklist t
        INNER JOIN 
            tbl_projectmaster p ON p.id = t.pro_id 
        LEFT JOIN 
            tbl_task ts ON ts.tasklist_id = t.id     
WHERE t.pro_id = '" + pro_id + "' AND p.delete_status = 0 and t.status='pending' ";


        if (!string.IsNullOrEmpty(status))
        {
            querry1 += " AND ts.status = '" + status + "' ";
        }

        querry1 += @" GROUP BY t.id, t.pro_id, t.name, t.status, t.user_id, t.color, p.name";
    
        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                // Get task count directly from the dataset
                string taskCount = ds.Tables[0].Rows[i]["task_count"].ToString();

                json += "{'project_id':'" + ds.Tables[0].Rows[i]["pro_id"].ToString() +
                        "','task_name':'" + ds.Tables[0].Rows[i]["name"].ToString() +
                        "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() +
                        "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() +
                        "','user_id':'" + ds.Tables[0].Rows[i]["user_id"].ToString() +
                        "','color':'" + ds.Tables[0].Rows[i]["color"].ToString() +
                        "','projectname':'" + ds.Tables[0].Rows[i]["projectname"].ToString() +
                        "','task_count':'" + taskCount + "'},"; // Include the task count
            }

            json = json.TrimEnd(','); // Remove trailing comma
            json += "]}"; // Close the JSON structure
            json = json.Replace("'", "\""); // Convert single quotes to double quotes

            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
        {
            json = "{'status':false,'Message' :'No data found!'}";
            json = json.Replace("'", "\""); // Ensure double quotes
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }

}
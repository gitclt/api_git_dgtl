using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Newtonsoft.Json;
using System.IO;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry, emp_id, project_id;

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
        select();
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

    public void select()
    {
        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }
        if (Request.Form["project_id"] != null)
        {
            project_id = Request.Form["project_id"];
        }

        // Modify the SQL query to use ISNULL or COALESCE
        querry = @"SELECT s.id, s.project_id, s.title, s.note, s.color, 
                      ISNULL(m.name, NULL) AS name 
               FROM tbl_stickynote s
               LEFT JOIN tbl_projectmaster m ON m.id = s.project_id 
               WHERE s.emp_id = '" + emp_id + "' AND s.deleted_status = 0";

        if (!string.IsNullOrEmpty(project_id))
        {
            querry += " AND s.project_id = '" + project_id + "'";
        }
      //Response.Write (querry);
      //  return;
        DataSet ds = cc.joinselect(querry);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'success' ,'data':[";

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"] + "','project_id':'" + ds.Tables[0].Rows[i]["project_id"] + "','title':'" + ds.Tables[0].Rows[i]["title"] + "','note':'" + ds.Tables[0].Rows[i]["note"] + "','color':'" + ds.Tables[0].Rows[i]["color"] + "','project_name':'" + ds.Tables[0].Rows[i]["name"] + "'},";
            }

            // Remove the trailing comma and close the JSON array
            json = json.TrimEnd(',') + "]}";
        }
        else
        {
            json = "{'status':false,'Message' :'No data found'}";
        }

        // Replace single quotes with double quotes
        json = json.Replace("'", "\"");

        // Set the content type and output the JSON
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();

        ds.Dispose();
    }
}
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
using System.Runtime.InteropServices.ComTypes;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using System.ServiceModel.Syndication;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;

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
        string project_id = null;
        if (Request.Form["project_id"] != null)
        {
            project_id = Request.Form["project_id"];
        }

        string querry = @"SELECT distinct e.id, e.name, r.name as role
        FROM tbl_employee e
        INNER JOIN tbl_user u ON u.emp_id = e.id
        LEFT JOIN tbl_hierarchy h ON u.hierarchy_id = h.id
        LEFT JOIN tbl_role r ON r.hierarchy_id = h.id
          WHERE u.type != 'admin' and e.delete_status=0
	and NOT EXISTS (
            SELECT 1
            FROM tbl_projectmember pm
         WHERE pm.emp_id = e.id AND pm.project_id = '" + project_id+"')";

        if (!string.IsNullOrEmpty(project_id))
        {
            querry = querry.Replace("@project_id", project_id); // Replace the placeholder with actual project_id

            DataSet ds = cc.joinselect(querry);
            if (ds.Tables[0].Rows.Count > 0)
            {
                json = "{'status':true,'Message' :'Success.','data':[";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //json += "{'name':'" + ds.Tables[0].Rows[i]["name"].ToString() + @"'
                    //    ,'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "' ,";
                    json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','role':'" + ds.Tables[0].Rows[i]["role"].ToString() + "'},";


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
                json = "{'status':false,'Message' :'No data found!'}";
                ds.Dispose();
            }

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
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
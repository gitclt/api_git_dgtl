using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Protocols.WSTrust;
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
    static string status = "";

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
        if (Request.Form["status"] != null)
        {
            status = Request.Form["status"];
        }
        string querry1 = @"SELECT 
p.id,
    p.name,
    p.target_date,
    p.status,
    COUNT(t.id) AS total_tasks,
    SUM(CASE WHEN t.status = 'pending' and t.status='ongoing' THEN 1 ELSE 0 END) AS pending_tasks,
    COALESCE(SUM(CASE WHEN t.status = 'completed' THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(t.id), 0), 0) AS progress
FROM 
    tbl_projectmaster p
LEFT JOIN 
    tbl_task t ON p.id = t.pro_id
	inner JOIN 
    tbl_workspace w ON w.id = p.workspace_id
 where p.delete_status=0  and w.delete_status=0";
        if (!string.IsNullOrEmpty(status))
        {
            querry1 += " AND p.status='" + status + "'";
        }
        querry1 += @"GROUP BY p.name, p.target_date, p.status,p.id";



        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','target_date':'" + ds.Tables[0].Rows[i]["target_date"].ToString() + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "','progress':'" + ds.Tables[0].Rows[i]["progress"].ToString() + "'},";
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

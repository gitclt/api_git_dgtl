using DocumentFormat.OpenXml.Drawing.Spreadsheet;
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
    static string pro_id = "";

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
        if (Request.Form["pro_id"] != null)
        {
            pro_id = Request.Form["pro_id"];
        }
        string querry1 = @"SELECT 
    ISNULL(CONVERT(TIME, DATEADD(SECOND, SUM(DATEDIFF(SECOND, '00:00:00', l.time_spent)), '00:00:00')), '00:00:00') AS total_time_spent,
    ISNULL(CONVERT(TIME, DATEADD(SECOND, SUM(DATEDIFF(SECOND, '00:00:00', t.estimated_time)), '00:00:00')), '00:00:00') AS total_estimated_time
FROM 
    tbl_log_time l
INNER JOIN 
    tbl_task t ON l.task_id = t.id
INNER JOIN 
    tbl_projectmaster p ON p.id = t.pro_id
WHERE 
    t.pro_id = '" + pro_id+"' ";
  
        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'logtime':'" + ds.Tables[0].Rows[i]["total_time_spent"].ToString() + "','total_estimated_time':'" + ds.Tables[0].Rows[i]["total_estimated_time"].ToString() + "'},";
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

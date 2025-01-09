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
    static string querry = "", workspace_id = "";

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
        if (Request.Form["workspace_id"] != null)
        {
            workspace_id = Request.Form["workspace_id"];
        }

        string query1 = @"SELECT p.id, p.name, p.description,p.target_date,p.workspace_id,p.status,w.work_space
                      FROM tbl_projectmaster p
                      INNER JOIN tbl_workspace w ON p.workspace_id = w.id
                      WHERE p.delete_status=0 and w.delete_status=0 and p.workspace_id='"+workspace_id+"'";
       
        DataSet ds = cc.joinselect(query1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','description':'" + ds.Tables[0].Rows[i]["description"].ToString() + "'," +
                        "'target_date':'" + ds.Tables[0].Rows[i]["target_date"].ToString() + "','workspace_id':'" + ds.Tables[0].Rows[i]["workspace_id"].ToString() + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "','work_space':'" + ds.Tables[0].Rows[i]["work_space"].ToString() + "',";
                json += "'employees':[";
                string query2 = @"
SELECT pm.emp_id,pm.id,e.name 
FROM tbl_projectmember pm
inner join tbl_employee e
on e.id=pm.emp_id
WHERE  pm.delete_status=0 and e.delete_status=0 and pm.project_id='" + ds.Tables[0].Rows[i]["id"].ToString() + "'";

              
                DataSet dss = cc.joinselect(query2);
                if (dss.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dss.Tables[0].Rows.Count; j++)
                    {
                        json += "{'emp_id':'" + dss.Tables[0].Rows[j]["emp_id"].ToString() + "','name':'" + dss.Tables[0].Rows[j]["name"].ToString() + "'},";
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

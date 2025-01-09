using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string id = "", project_id="";

   
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

        if (Request.Form["id"] != null)
        {
            id = Request.Form["id"];
        }
        if (Request.Form["project_id"] != null)
        {
            project_id = Request.Form["project_id"];
        }

        //string querry1 = @"select * from tbl_employees  where role='supervisor' and delete_status=0  ";

        string querry = @"
WITH TimeSpent AS(
    SELECT
        SUM(DATEDIFF(SECOND, '00:00:00', l.time_spent)) / 3600.0 AS total_hours,
        t.pro_id
    FROM
        tbl_log_time l
    INNER JOIN
        tbl_task t ON t.id = l.task_id
    WHERE
        t.pro_id = '" + project_id + @"'
    GROUP BY
        t.pro_id
),
ProjectDetails AS(
    SELECT distinct
        p.name AS project_name,
        p.target_date,
        p.id,
p.status,
        pm.workspace_id
    FROM
        tbl_projectmaster p
    INNER JOIN
        tbl_projectmember pm ON p.id = pm.project_id
    INNER JOIN
        tbl_workspace w ON pm.workspace_id = w.id
    WHERE
        pm.workspace_id = '" + id + @"'
        AND pm.delete_status = 0";

if (!string.IsNullOrEmpty(project_id))
        {
            querry += @" AND pm.project_id = '" + project_id + @"'";
        }

        querry += @"
)
SELECT  
    pd.project_name, 
    pd.target_date, 
    pd.id, 
    pd.workspace_id,
pd.status,
    COALESCE(ts.total_hours, 0) AS total_hours 
FROM 
    ProjectDetails pd 
LEFT JOIN 
    TimeSpent ts ON pd.id = ts.pro_id;";

      

        DataSet ds = cc.joinselect(querry);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'project_name':'" + ds.Tables[0].Rows[i]["project_name"] + "','target_date':'" + ds.Tables[0].Rows[i]["target_date"] + "','id':'" + ds.Tables[0].Rows[i]["id"] + "','status':'" + ds.Tables[0].Rows[i]["status"] + "','total_hours':'" + ds.Tables[0].Rows[i]["total_hours"] + "',";
                json += "'employees':[";
                string query2 = @"SELECT  te.emp_id,te.id,e.name
FROM tbl_projectmember te
inner join tbl_employee e on e.id=te.emp_id
WHERE te.project_id='" + ds.Tables[0].Rows[i]["id"] + "'  and  te.workspace_id='"+id+"'";
               
                DataSet dss = cc.joinselect(query2);
              
                if (dss.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dss.Tables[0].Rows.Count; j++)
                    {
                        json += "{'emp_id':'" + dss.Tables[0].Rows[j]["emp_id"].ToString() + "','id':'" + dss.Tables[0].Rows[j]["id"].ToString() + "','name':'" + dss.Tables[0].Rows[j]["name"].ToString() + "'},";
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
            json = "{'status':false,'Message' :'No data found!'}";
        ds.Dispose();


        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
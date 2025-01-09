using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_project_Default : System.Web.UI.Page
{

    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    CommFuncs CommFuncs = new CommFuncs();
    string json;
    static string querry;
    string id = "", project_id="";
    protected void Page_Load(object sender, EventArgs e)
    {
       // chk_tocken();
        getdata();

    }


    public void chk_tocken()
    {
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
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
    public void getdata()
    {
        if (Request.Form["project_id"] != null)
        {
            project_id = Request.Form["project_id"];
        }

        string querry1 = @"WITH TimeSpent AS (
    SELECT 
        SUM(DATEDIFF(SECOND, '00:00:00', l.time_spent)) / 3600.0 AS total_hours,
        t.pro_id
    FROM tbl_log_time l
    INNER JOIN tbl_task t ON t.id = l.task_id
    WHERE t.pro_id = 1
    GROUP BY t.pro_id
),
ProjectDetails AS (
    SELECT 
        name,
        target_date,
        status,
        id
    FROM tbl_projectmaster
    WHERE delete_status = 0 AND id = '"+project_id+"') SELECT pd.name,pd.target_date,pd.status,pd.id,ts.total_hours FROM ProjectDetails pd LEFT JOIN TimeSpent ts ON pd.id = ts.pro_id;";

        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','target_date':'" + ds.Tables[0].Rows[i]["target_date"].ToString() + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "','total_hours':'" + ds.Tables[0].Rows[i]["total_hours"].ToString() + "'},";
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
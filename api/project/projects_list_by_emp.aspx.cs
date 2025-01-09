using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using System.ServiceModel.Syndication;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string id = "";

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

    protected void Page_Load(object sender, EventArgs e)
    {
       // chk_tocken();

        getdata();
    }
    public void getdata()
    {
        string emp_id = "";
        string workspace_id = "";
        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }
        if (Request.Form["workspace_id"] != null)
        {
            workspace_id = Request.Form["workspace_id"];
        }

        //string querry1 = @"select * from tbl_employees  where role='supervisor' and delete_status=0  ";

        string querry = @"
       SELECT 
    p.name AS project_name,
    p.workspace_id,
    w.work_space,
    subquery.total_employee_count,
    p.id,
    (SELECT COUNT(*) FROM tbl_task WHERE status = 'completed' AND pro_id = p.id) AS completed_tasks_count,
    (SELECT COUNT(*) FROM tbl_task WHERE pro_id = p.id) AS total_tasks_count
FROM 
    tbl_projectmaster p
LEFT JOIN 
    (SELECT pm.project_id, COUNT(DISTINCT pm.emp_id) AS total_employee_count
     FROM tbl_projectmember pm
     WHERE pm.workspace_id ='"+ workspace_id + "' GROUP BY pm.project_id) subquery ON p.id = subquery.project_id left JOIN  tbl_projectmember pm ON p.id = pm.project_id left JOIN tbl_user u ON u.emp_id = pm.emp_id LEFT JOIN tbl_workspace w ON p.workspace_id = w.id WHERE p.delete_status = 0  AND w.delete_status = 0 and p.workspace_id='"+workspace_id+"' and u.emp_id='"+emp_id+"' GROUP BY   p.name, p.id, p.workspace_id, w.work_space, subquery.total_employee_count";
        DataSet ds = cc.joinselect(querry);
        //Response.Write(querry);
        //return;
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'project_name':'" + ds.Tables[0].Rows[i]["project_name"] + "'," +
                    "'employee_count':'" + ds.Tables[0].Rows[i]["total_employee_count"] + "'," +
                    "'project_id':'" + ds.Tables[0].Rows[i]["id"] + "','completed_tasks_count':'" + ds.Tables[0].Rows[i]["completed_tasks_count"] + "'," +
                    "'total_tasks_count':'" + ds.Tables[0].Rows[i]["total_tasks_count"] + "','workspace_id':'" + ds.Tables[0].Rows[i]["workspace_id"] + "'," +
                    "'work_space':'" + ds.Tables[0].Rows[i]["work_space"] + "'},";

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
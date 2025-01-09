using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();

    string json;
    static string querry;
    string date = "";

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
        if (Request.Form["date"] != null)
        {
            date = Request.Form["date"];
        }
        string querry1 = @"	SELECT 
    a.emp_id,
    e.name AS supervisor_name,
    SUM(ot.qty) AS production_quantity,
CONVERT(VARCHAR(5), DATEADD(SECOND, SUM(DATEDIFF(SECOND, '00:00:00', ot.workhour)), '00:00'), 108) AS total_workhour
FROM 
    tbl_employees e
LEFT JOIN 
    tbl_order_assigned a ON a.emp_id = e.id
LEFT JOIN 
    tbl_order_task ot ON ot.order_assign_id = a.id 
LEFT JOIN  
    tbl_order_task_emp te ON te.task_id = ot.id
WHERE 
    CAST(ot.addedon AS DATE) = '" + date+ "' AND a.job_type = 'production' AND ot.production_status = 'completed' GROUP BY  e.name,a.emp_id";
      
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','total_workhour':'" + ds.Tables[0].Rows[i]["total_workhour"].ToString() + "'},";
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
            ds.Dispose();

        json = "{'status':false,'Message' :'No data found!'}";


        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

}
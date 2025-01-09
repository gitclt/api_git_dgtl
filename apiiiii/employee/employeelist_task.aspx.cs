using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string grade = "", unit_id="",designation_id;
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
            Response.Write(json);
            Response.End();
            return;
        }
    }

    public void getdata()
    {
        if (Request.Form["grade"] != null)
        {
            grade = Request.Form["grade"];
        }
        if (Request.Form["unit_id"] != null)
        {
            unit_id = Request.Form["unit_id"];
        }
        if (Request.Form["designation_id"] != null)
        {
            designation_id = Request.Form["designation_id"];
        }
        string querry1 = @"SELECT distinct
    e.id,
    e.name,e.grade,e.unit_id,e.designation_id,e.emp_code,e.email,e.mobile
   -- g.name AS grade,
   -- g.id AS grade_id,
  --  ISNULL(SUM(a.qty), 0) - 
   -- (SELECT ISNULL(SUM(t.qty), 0) FROM tbl_order_task t WHERE t.addedby = e.id) AS pending_quantity
FROM 
    tbl_employees e
--LEFT JOIN tbl_order_assigned a ON a.emp_id = e.id
INNER JOIN tbl_role r ON e.role = r.id
INNER JOIN tbl_hierarchy h ON r.hierarchy_id = h.id
INNER JOIN 
    tbl_grade g ON e.grade = g.id WHERE
    r.hierarchy_id=6 and e.delete_status=0
    AND e.id NOT IN
    (
        SELECT distinct te.emp_id
        FROM tbl_order_task_emp te
        INNER JOIN tbl_order_task ot ON te.task_id = ot.id
        WHERE ot.start_end = 'start'
    )";
       if (!string.IsNullOrEmpty(grade))
        {
            querry1 += " AND e.grade = '" + grade + "' ";
        }

        if (!string.IsNullOrEmpty(unit_id))
        {
            querry1 += " AND e.unit_id = '" + unit_id + "' ";
        }

        if (!string.IsNullOrEmpty(designation_id))
        {
            querry1 += " AND e.designation_id = '" + designation_id + "' ";
        }
       
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','grade':'" + ds.Tables[0].Rows[i]["grade"].ToString() + "','unit':'" + ds.Tables[0].Rows[i]["unit_id"].ToString() + "','designation':'" + ds.Tables[0].Rows[i]["designation_id"].ToString() + "','empCode':'" + ds.Tables[0].Rows[i]["emp_code"].ToString() + "','email':'" + ds.Tables[0].Rows[i]["email"].ToString() + "','mobile':'" + ds.Tables[0].Rows[i]["mobile"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            ds.Dispose();
            Response.End();
        }
        else
            json = "{'status':false,'Message' :'No data found!'}";
        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        ds.Dispose( );  
        Response.End();
    }
}
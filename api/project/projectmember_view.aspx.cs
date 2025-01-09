using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string workspace_id = "";
    string project_id = "";


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
        if (Request.Form["project_id"] != null)
        {
            project_id = Request.Form["project_id"];
        }

        string query = @"select pm.emp_id, pm.workspace_id, w.work_space, p.id as project_id, p.name as project_name, e.id as emp_id, e.name as emp_name 
                     from tbl_projectmember pm
                     inner join tbl_projectmaster p on p.id = pm.project_id
                     inner join tbl_employee e on e.id = pm.emp_id
                     inner join tbl_workspace w on w.id = p.workspace_id 
                     where pm.delete_status=0 and p.delete_status=0 and e.delete_status=0 and pm.project_id = '" + project_id + "' and pm.workspace_id = '" + workspace_id + "'";

        DataSet ds = cc.joinselect(query);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','data':[";
            var groupedData = ds.Tables[0].AsEnumerable()
                                 .GroupBy(row => new
                                 {
                                     workspace_id = row.Field<object>("workspace_id"),
                                     work_space = row.Field<string>("work_space"),
                                     project_id = row.Field<object>("project_id"),
                                     project_name = row.Field<string>("project_name")
                                 })
                                 .Select(group => new
                                 {
                                     workspace_id = group.Key.workspace_id.ToString(),
                                     work_space = group.Key.work_space,
                                     project_id = group.Key.project_id.ToString(),
                                     project_name = group.Key.project_name,
                                     employees = group.Select(row => new
                                     {
                                         emp_id = row.Field<object>("emp_id").ToString(),
                                         emp_name = row.Field<string>("emp_name")
                                     }).ToList()
                                 }).ToList();

            foreach (var data in groupedData)
            {
                json += "{'workspace_id':'" + data.workspace_id + "','work_space':'" + data.work_space + "','project_id':'" + data.project_id + "','name':'" + data.project_name + "',";
                json += "'employees':[";
                foreach (var employee in data.employees)
                {
                    json += "{'emp_id':'" + employee.emp_id + "','name':'" + employee.emp_name + "'},";
                }
                json = json.TrimEnd(',') + "]},";
            }
            json = json.TrimEnd(',') + "]}";
        }
        else
        {
            json = JsonConvert.SerializeObject(new { status = false, Message = "No data found!" });
        }

        ds.Dispose();
        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}

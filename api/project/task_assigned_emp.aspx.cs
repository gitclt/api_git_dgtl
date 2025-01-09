using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
//using System.ServiceModel.Channels;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry1 = "", task_id="";
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
        string emp_id = "", type = "", task_id = "", json = "";

        if (Request.Form["task_id"] != null)
        {
            task_id = Request.Form["task_id"];
        }
        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }
        if (Request.Form["type"] != null)
        {
            type = Request.Form["type"];
        }

        if (type == "list")
        {
            string querry = @"
            select te.emp_id, te.task_id, e.name 
            from tbl_task_emp te
            inner join tbl_employee e on te.emp_id = e.id
            inner join tbl_user u on e.id = u.emp_id
            where e.delete_status = 0 
            and u.type != 'admin' 
            and te.task_id = '" + task_id + "'";

            DataSet ds = cc.joinselect(querry);

            if (ds.Tables[0].Rows.Count > 0)
            {
                json = "{'status':true,'Message' :'Success.','data':[";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    json += "{'emp_id':'" + ds.Tables[0].Rows[i]["emp_id"] + "','task_id':'" + ds.Tables[0].Rows[i]["task_id"] + "','name':'" + ds.Tables[0].Rows[i]["name"] + "'},";
                }
                json = json.TrimEnd(',') + "]}";
            }
            else
            {
                json = "{'status':false,'Message' :'No data found!'}";
            }
            ds.Dispose();
        }
        else if (type == "delete")
        {
            string querry = @"delete from tbl_task_emp where task_id = '" + task_id + "' and emp_id = '" + emp_id + "'";
            cc.joinselect(querry); // No need to handle a DataSet for deletion

            json = "{'status':true,'Message':'Data deleted successfully.'}";
        }
        else
        {
            json = "{'status':false,'Message':'Invalid type provided.'}";
        }

        // Convert to proper JSON format
        json = json.Replace("'", "\"");

        // Return the JSON response
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}
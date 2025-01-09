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
        if (Request.Form["task_id"] != null)
        {
            task_id = Request.Form["task_id"];
        }


        string querry = @"
SELECT e.id,e.name
FROM tbl_employee e 
inner join tbl_user u on u.emp_id=e.id 
WHERE u.type!='admin' and e.delete_status=0 and e.id NOT IN (SELECT te.emp_id FROM tbl_task_emp  te where te.task_id='" + task_id + "') ";
        DataSet ds = cc.joinselect(querry);
       
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'emp_id':'" + ds.Tables[0].Rows[i]["id"] + "','name':'" + ds.Tables[0].Rows[i]["name"] + "'},";

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
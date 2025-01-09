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
    static string querry1 = "";
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
        string emp_id = "";
        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }

        string querry = @"
						 	  SELECT DISTINCT workspace_id ,w.work_space
FROM tbl_projectmember p
inner join tbl_workspace w on w.id=p.workspace_id
                      WHERE p.emp_id = '" + emp_id+"';";

        DataSet ds = cc.joinselect(querry);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'workspace_id':'" + ds.Tables[0].Rows[i]["workspace_id"] + "','work_space':'" + ds.Tables[0].Rows[i]["work_space"] + "'},";
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
            json = "{'status':false,'Message' :'No data found!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }

        ds.Dispose();
    }


}
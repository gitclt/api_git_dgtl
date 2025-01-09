using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
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
     string hierarchy_id = "";

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
        if (Request.Form["hierarchy_id"] != null)
        {
            hierarchy_id = Request.Form["hierarchy_id"];
        }

        string querry1 = @"select * from tbl_privilage where delete_status=0 ";


        if (hierarchy_id!=null)
        {
            querry1 += "and ','+hierarchy_id+',' like '%," + hierarchy_id + ",%'";
        }
        DataSet ds = cc.joinselect(querry1);
            if (ds.Tables[0].Rows.Count > 0)
            {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'module':'" + ds.Tables[0].Rows[i]["module"].ToString() + "','menu':'" + ds.Tables[0].Rows[i]["menu"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','hierarchy_id':'" + ds.Tables[0].Rows[i]["hierarchy_id"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";
                
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
            else
                json = "{'status':false,'Message' :'No data found!'}";


        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
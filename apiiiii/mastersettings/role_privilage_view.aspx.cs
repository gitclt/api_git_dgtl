using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
     string role_id = "";

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
        if (Request.Form["role_id"] != null)
        {
            role_id = Request.Form["role_id"];
        }

        string querry1 = @"SELECT p.module,rp.role_id,rp.id,p.menu,rp.is_add,rp.is_delete,rp.is_edit,rp.is_view
                 FROM tbl_privilage p
                 INNER JOIN tbl_role_privilage rp ON rp.privilage_id = p.id 
                where 1=1";


        if (role_id != null)
        {
            querry1 += "AND role_id = '" + role_id + "' ";
        }
        DataSet ds = cc.joinselect(querry1);
            if (ds.Tables[0].Rows.Count > 0)
            {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'role_id':'" + ds.Tables[0].Rows[i]["role_id"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','is_add':'" + ds.Tables[0].Rows[i]["is_add"].ToString() + "','is_edit':'" + ds.Tables[0].Rows[i]["is_edit"].ToString() + "','is_delete':'" + ds.Tables[0].Rows[i]["is_delete"].ToString() + "','is_view':'" + ds.Tables[0].Rows[i]["is_view"].ToString() + "','module':'" + ds.Tables[0].Rows[i]["module"].ToString() + "','menu':'" + ds.Tables[0].Rows[i]["menu"].ToString() + "'},";
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
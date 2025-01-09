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
    string name = "";
    string state_id = "";
    string country_id = "";

    string type = "";

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
        if (Request.Form["country_id"] != null)
        {
            country_id = Request.Form["country_id"];
        }

        if (Request.Form["state_id"] != null)
        {
            state_id = Request.Form["state_id"];
        }

        string querry1 = @"select d.id, c.name as country, s.name as state, d.name, d.state_id, d.country_id as Countryid 
                           from tbl_district d  
                           inner join tbl_country c on d.country_id = c.id 
                           inner join tbl_states s on d.state_id = s.id 
                           where d.delete_status=0 ";

        if (!string.IsNullOrEmpty(country_id))
        {
            querry1 += "AND d.country_id = '" + country_id + "' ";
        }

        if (!string.IsNullOrEmpty(state_id))
        {
            querry1 += "AND d.state_id = '" + state_id + "' ";
        }

        querry1 += "order by d.name";

        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','state':'" + ds.Tables[0].Rows[i]["state"].ToString() + "','country':'" + ds.Tables[0].Rows[i]["country"].ToString() + "','stateid':'" + ds.Tables[0].Rows[i]["state_id"].ToString() + "','countryid':'" + ds.Tables[0].Rows[i]["Countryid"].ToString() + "'},";
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

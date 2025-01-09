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
    string name = "",country_id="";
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

        //string querry1 = @"select * from tbl_states where (1=1) ";
        if (Request.Form["country_id"] != null)
        {
            country_id = Request.Form["country_id"];
        }
        string querry1 = @"select s.id,s.name,s.short_code,s.country_id,c.name as country,c.country_code,c.short_code as country_short_code    from tbl_states s inner join tbl_country c on s.country_id=c.id where s.delete_status=0";

        if (!string.IsNullOrEmpty(country_id))
        {
            querry1 += "AND country_id = '" + country_id + "' ";
        }

        querry1 += "order by name asc";
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','short_code':'" + ds.Tables[0].Rows[i]["short_code"].ToString() + "','country':'" + ds.Tables[0].Rows[i]["country"].ToString() + "','countryid':'" + ds.Tables[0].Rows[i]["country_id"].ToString() + "','country_code':'" + ds.Tables[0].Rows[i]["country_code"].ToString() + "','country_short_code':'" + ds.Tables[0].Rows[i]["country_short_code"].ToString() + "'},";
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



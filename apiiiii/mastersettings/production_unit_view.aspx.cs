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
    string unit_incharge = "";
    string mobile = "";
    string address = "";
    string name = "";
    string district = "";

    string state = "";


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

       // string querry1 = @"select * from tbl_production_unit where (1=1) ";
       string querry1= @"select pu.id,pu.unit_incharge,pu.mobile,pu.address,pu.name,pu.district_id,pu.state_id,d.name as district,s.name as state from tbl_production_unit pu inner join  tbl_district d on pu.district_id=d.id inner join tbl_states s on pu.state_id=s.id  where (1=1)";
        querry1 += "order by unit_incharge";
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'unit_incharge':'" + ds.Tables[0].Rows[i]["unit_incharge"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','mobile':'" + ds.Tables[0].Rows[i]["mobile"].ToString() + "','address':'" + ds.Tables[0].Rows[i]["address"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','state':'" + ds.Tables[0].Rows[i]["state"].ToString() + "','district':'" + ds.Tables[0].Rows[i]["district"].ToString() + "','district_id':'" + ds.Tables[0].Rows[i]["district_id"].ToString() + "','state_id':'" + ds.Tables[0].Rows[i]["state_id"].ToString() + "'},";
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
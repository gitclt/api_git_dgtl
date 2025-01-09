using DocumentFormat.OpenXml.Drawing.Spreadsheet;
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
    string pro_id = "",type="";
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
        if (Request.Form["pro_id"] != null)
        {
            pro_id = Request.Form["pro_id"];
        }
        if (Request.Form["type"] != null)
        {
            type = Request.Form["type"];
        }


        string querry1 = @"select distinct pp.process_type,pp.id,pp.process_name 
		from tbl_production_process pp
		inner join  tbl_product_production_process_grade pg on pp.id=pg.production_process_id
		 where pg.pro_id='"+pro_id+"' and pp.process_type='"+type+"'";
             
            DataSet ds = cc.joinselect(querry1);
            if (ds.Tables[0].Rows.Count > 0)
            {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'process_type':'" + ds.Tables[0].Rows[i]["process_type"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','process_name':'" + ds.Tables[0].Rows[i]["process_name"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";
                
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                ds.Dispose();       
            }
            else
                json = "{'status':false,'Message' :'No data found!'}";
        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        ds.Dispose();
        Response.End();
    }
}
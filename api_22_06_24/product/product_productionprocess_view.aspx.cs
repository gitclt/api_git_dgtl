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
    string product_id;




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
        if (Request.Form["product_id"] != null)
        {
            product_id = Request.Form["product_id"];

        }

        string querry1= @"select pp.id,pp.product_id,ps.id as productionprocess_id,ps.process_name,pp.gradeA_output_per_second,pp.gradeB_output_per_second,pp.gradeC_output_per_second,pp.gradeD_output_per_second,p.product_name from tbl_product_production_process pp inner join  tbl_product p on pp.product_id=p.id inner join tbl_production_process ps on pp.production_process_id=ps.id  where pp.product_id=  '" + product_id + "'";
        Response.Write(querry1);
        return;
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','product_id':'" + ds.Tables[0].Rows[i]["product_id"].ToString() + "','productionprocess_id':'" + ds.Tables[0].Rows[i]["productionprocess_id"].ToString() + "','process_name':'" + ds.Tables[0].Rows[i]["process_name"].ToString() + "','gradeA_output_per_second':'" + ds.Tables[0].Rows[i]["gradeA_output_per_second"].ToString() + "','gradeB_output_per_second':'" + ds.Tables[0].Rows[i]["gradeB_output_per_second"].ToString() + "','gradeC_output_per_second':'" + ds.Tables[0].Rows[i]["gradeC_output_per_second"].ToString() + "','gradeD_output_per_second':'" + ds.Tables[0].Rows[i]["gradeD_output_per_second"].ToString() + "','product_name':'" + ds.Tables[0].Rows[i]["product_name"].ToString() + "'},";
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
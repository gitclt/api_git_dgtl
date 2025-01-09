using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
   


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
        

        // string querry1 = @"select * from tbl_product where (1=1) ";
        string querry1 = @"SELECT p.product_name,p.product_market,p.pattern_id,p.gradeA_output_per_second,p.gradeB_output_per_second,P.gradeC_output_per_second,P.gradeD_output_per_second,pt.name AS product_type, p.material_id, p.gsm_id, p.size, p.color,p.id,p.producttype_id,p.hsn_sac,m.name as material,pn.name as pattern,g.name as gsm
                 FROM tbl_product p
                 INNER JOIN tbl_product_type pt ON p.producttype_id = pt.id 
				 INNER JOIN tbl_material m ON p.material_id = m.id 
		         INNER JOIN tbl_gsm g ON p.gsm_id = g.id 
		         INNER JOIN tbl_pattern pn ON p.pattern_id = pn.id 

                 WHERE p.delete_status=0";
       
        querry1 += "order by product_name";
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'product_name':'" + ds.Tables[0].Rows[i]["product_name"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','product_market':'" + ds.Tables[0].Rows[i]["product_market"].ToString() + "','product_type':'" + ds.Tables[0].Rows[i]["product_type"].ToString() + "','material':'" + ds.Tables[0].Rows[i]["material"].ToString() + "','hsn_sac':'" + ds.Tables[0].Rows[i]["hsn_sac"].ToString() + "','pattern':'" + ds.Tables[0].Rows[i]["pattern"].ToString() + "','producttype_id':'" + ds.Tables[0].Rows[i]["producttype_id"].ToString() + "','gsm':'" + ds.Tables[0].Rows[i]["gsm"].ToString() + "','size':'" + ds.Tables[0].Rows[i]["size"].ToString() + "','color':'" + ds.Tables[0].Rows[i]["color"].ToString() + "','material_id':'" + ds.Tables[0].Rows[i]["material_id"].ToString() + "','pattern_id':'" + ds.Tables[0].Rows[i]["pattern_id"].ToString() + "','gsm_id':'" + ds.Tables[0].Rows[i]["gsm_id"].ToString() + "'},";
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
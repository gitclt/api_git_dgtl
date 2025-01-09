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
    string name = "";
    string logo = "";
    string sort_no = "";
    string short_name = "";
   

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
        string querry1 = @"select * from tbl_brand where delete_status=0";
      
        querry1 += "order by name";
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string id = ds.Tables[0].Rows[i]["id"].ToString();
                string imagePath = imgpath(id);
                string path = "";
                if (imagePath != "")
                    path = "http://" + HttpContext.Current.Request.Url.Authority + "/uploads/brand/" + imagePath + "";

                json += "{'name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','logo':'"+path+"','sort_no':'" + ds.Tables[0].Rows[i]["sort_no"].ToString() + "','short_name':'" + ds.Tables[0].Rows[i]["short_name"].ToString() + "'},";
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

    private string imgpath(string id)
    {
        string folder = Server.MapPath("../uploads/brand/");
        if (Directory.Exists(folder))
        {
            string[] files = Directory.GetFiles(folder);
            if (files.Length > 0)
            {
                return Path.GetFileName(files[0]); // Return the first image found
            }
        }
        return ""; // Return an empty string if no image is found
    }

}
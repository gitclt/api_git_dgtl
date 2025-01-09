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
    protected void Page_Load(object sender, EventArgs e)
    {
        getdata();
    }
    public void getdata()
    {
        string id = "";
        string type = "";

        if (Request.Form["id"] != null)
        {
            id = Request.Form["id"];
        }
        if (Request.Form["type"] != null)
        {
            type = Request.Form["type"];
        }

       
        
        string folder = "";
        if (type == "country")
        {
            folder = "../uploads/country/";
        }
        else if (type == "employee")
        {
            folder = "../uploads/employee/"+id;
        }





        HttpFileCollection MyFileCollection = Request.Files;
        if (MyFileCollection.Count > 0)
        {
           
            if (!Directory.Exists(Server.MapPath(folder)))
            {
                Directory.CreateDirectory(Server.MapPath(folder));
            }

            for (int i = 0; i < MyFileCollection.Count; i++)
            {
                string oimg = MyFileCollection[i].FileName;
                string countryImagePath = folder + "/" + oimg; // Renamed to avoid conflict
                MyFileCollection[i].SaveAs(Server.MapPath(countryImagePath));
            }
        }
        json = "{'status':true,'Message' :'Image Uploaded Successfully'}";


        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}
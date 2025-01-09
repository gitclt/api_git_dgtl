using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Newtonsoft.Json;
using System.IO;
using DocumentFormat.OpenXml.Math;
using Org.BouncyCastle.Ocsp;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry, ref_id;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Form["id"] != null)
        {
            ref_id = Request.Form["id"].ToString();
            insert();
        }
        
    }
	
	public void insert()
    {


        HttpFileCollection MyFileCollection = Request.Files;
        //Response.Write("file count:"+ MyFileCollection.Count + " <br/>");
        if (MyFileCollection.Count > 0)
        {
            //Response.Write("file exist <br/>");
            string folder = "../../uploads/shop_requirements/"+ref_id, imagepath = "", images = "";
            if (!Directory.Exists(Server.MapPath(folder)))
                System.IO.Directory.CreateDirectory(Server.MapPath(folder));
            for (int i = 0; i < MyFileCollection.Count; i++)
            {
                //Response.Write("inside loop <br/>");
                string oimg = MyFileCollection[i].FileName;
                imagepath = folder + "/" + oimg;
                // Save the File
                MyFileCollection[i].SaveAs(Server.MapPath(imagepath));
            }
        }



      
         json = "{'status':true,'Message' :'Inserted Successfully','id':'" + ref_id + "'}";


        json = json.Replace("'", "\"");


        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

}
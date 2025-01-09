using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using iTextSharp.text.html.simpleparser;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json, data = "", id = "", type = "", image = "";
    static string querry;
    protected void Page_Load(object sender, EventArgs e)
    {
       

        if (Request.Form["data"] != null)
        {
            data = Request.Form["data"];

           
        }
        if (Request.Form["id"] != null)
        {
            id = Request.Form["id"];
        }
       
        if (Request.Form["type"] != null)
        {
            type = Request.Form["type"];
        }
        if (Request.Form["image"] != null)
        {
            image = Request.Form["image"];
        }

      
        string folder = "";
       
         if (type == "employee")
        {
            
            folder = "../uploads/employee/" + id;
           

        }
       
        else if (type == "documents")
        {
            folder = "../uploads/documents/" + id;
        }
        if (!Directory.Exists(Server.MapPath(folder)))
        {
           

            Directory.CreateDirectory(Server.MapPath(folder));
        }


        byte[] imgBytes = Convert.FromBase64String(data);



        string countryImagePath = folder + "/" + image; // Renamed to avoid conflict
        // Save byte array as image file
        string savePath = Server.MapPath(countryImagePath);
       

        File.WriteAllBytes(savePath, imgBytes);

        

        json = "{'status':true,'Message' :'Image Uploaded Successfully'}";


        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
       

      

    }
   
    public byte[] imageToByteArray(System.Drawing.Image imageIn)
    {
        MemoryStream ms = new MemoryStream();
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
        return ms.ToArray();
    }
    public Image byteArrayToImage(byte[] byteArrayIn)
    {
        MemoryStream ms = new MemoryStream(byteArrayIn);
        Image returnImage = Image.FromStream(ms);
        return returnImage;
    }

}
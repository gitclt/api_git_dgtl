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

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    protected void Page_Load(object sender, EventArgs e)
    {
		insert();
    }
	public class DataResponse
	{
		public string id;
		public string empid;
		public string user_id;
		public string user_type;
		public string retailer_id;
		public string creditlimit_total;		
		public string creditlimit_vkc;		
		public string creditdays;		
	}
	public void insert()
    {
		var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
		bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
		var bodyText = bodyStream.ReadToEnd();

		querry = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        string qry = "";
        int count = 0;
        foreach (var data in DataResponse)
        {
            querry += " update tbl_retailer_credit_limit ";
            querry += " set empid='" + data.empid + "',creditlimit_total='" + data.creditlimit_total + "',creditlimit_vkc='" + data.creditlimit_vkc + "',creditdays='" + data.creditdays + "'  ";
            querry += " where id='" + data.id + "' ";
        }
        int q = cc.Insert(querry);
        if (q > 0)
            json = "{'status':true,'Message' :'Updated Successfully'}";
        else
            json = "{'status':false,'Message' :'Failed'}";


        json = json.Replace("'", "\"");


        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

}
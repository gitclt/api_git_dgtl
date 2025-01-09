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
			string querry1 = @"
select id from tbl_retailer_credit_limit 
where user_id=" + data.user_id + " and  user_type='" + data.user_type + "' and retailer_id='" + data.retailer_id + "'";
			DataTable dt = cc.joinselect(querry1);
            if (dt.Rows.Count > 0)
            {
				json = "{'status':false,'Message' :'Already Exist! Please update if needed.'}";
				json = json.Replace("'", "\"");
				Response.ContentType = "application/json";
				Response.Write(json);
				Response.End();
			}
            else
            {
                querry += " insert into  tbl_retailer_credit_limit (empid, user_id, user_type, retailer_id,creditlimit_total,creditlimit_vkc,creditdays,addedon) ";
                querry += " values('" + data.empid + "','" + data.user_id + "','" + data.user_type + "','" + data.retailer_id + "','" + data.creditlimit_total + "','" + data.creditlimit_vkc + "','" + data.creditdays + "',getdate())  ";
            }
            dt.Dispose();
        }
        int q = cc.Insert(querry);
        if (q > 0)
            json = "{'status':true,'Message' :'Inserted Successfully'}";
        else
            json = "{'status':false,'Message' :'Failed'}";


        json = json.Replace("'", "\"");


        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

}
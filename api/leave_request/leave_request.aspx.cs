using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;

    protected void Page_Load(object sender, EventArgs e)
    {
        //chk_tocken();

        insert();
    }

    public class DataResponse
    {
        public string emp_id;
        public string from_date;
        public string to_date;
        public string reason;
        public string remark;
        public string status;
        public string type;
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
        string message = "";

        foreach (var data in DataResponse)
        {
            string querry1 = "";

            if (data.type == "add")
            {
                querry1 = @"INSERT INTO tbl_leave_request (emp_id,from_date,to_date,reason,remark,status) 
            VALUES ('" + data.emp_id + "', '" + data.from_date + "', '" + data.to_date + "', '" + data.reason + "', '" + data.remark + "', '" + data.status + "')";
                message = "Data added successfully";
                 


            }

            int status = cc.Insert(querry1);
            if (status > 0)
            {
                //json = "{'status':true,'Message' :'Data updated successfully.'}";

                json = "{'status':true,'Message' :'" + message + "'}";
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
            else
                json = "{'status':false,'Message' :'Failed'}";
        }

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

}
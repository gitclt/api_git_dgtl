using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        insert();
    }
    public class DataResponse
    {
        public string name;
        public string type;
        public string id;


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
            string querry1 = "";
            string message = "";

            if (data.type == "add")
            {
                querry1 = @"insert into tbl_product_type (name,delete_status) values('" + data.name + "',0)";
                message = "Data added successfully.";

            }
            else if (data.type == "edit")
            {
                querry1 = @"update tbl_product_type set name='" + data.name + "' where id=" + data.id + " ";
                message = "Data updated successfully.";

            }
            else if (data.type == "delete")
            {
                querry1 = @"update tbl_product_type set delete_status=1 where id=" + data.id + " ";
                message = "Data deleted successfully.";

            }
            int status = cc.Insert(querry1);
            if (status > 0)
            {
                //json = "{'status':true,'Message' :'Success.'}";
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
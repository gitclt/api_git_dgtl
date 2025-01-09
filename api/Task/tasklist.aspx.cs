using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
//using System.ServiceModel.Channels;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry1 = "";
    protected void Page_Load(object sender, EventArgs e)
    {
         chk_tocken();

        insert();
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

    public class DataResponse
    {
        public string pro_id;
        public string name;
        public string status;
        public string user_id;
        public string color;
        public string id;
        public string type;
        public string delete_status;

    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        string qry = "";
        int count = 0;
        string message = "";


        foreach (var data in DataResponse)
        {
            if (data.type == "add")
            {

                querry1 = @"insert into tbl_tasklist (pro_id,name,status,user_id,color,delete_status)values('" + data.pro_id + "','" + data.name + "','" + data.status + "','" + data.user_id + "','" + data.color + "',0)";
                message = "Data added successfully.";

            }

            else if (data.type == "edit")
            {
                querry1 = @"update tbl_tasklist set pro_id='" + data.pro_id + "', name='" + data.name + "',status='" + data.status + "',user_id='" + data.user_id + "',color='" + data.color + "' where id=" + data.id + " ";
                message = "Data updated successfully.";

            }

            else if (data.type == "delete")
            {
                querry1 = @"UPDATE tbl_tasklist 
SET delete_status = 1,deleted_on = GETDATE() WHERE id = '"+data.id+"'";
                message = "Data deleted successfully.";
            }
        }
        int status = cc.Insert(querry1);

        if (status > 0)
        {
            // json = "{'status':true,'Message' :'Data updated successfully.'}";
            json = "{'status':true,'Message' :'" + message + "'}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }

        else
            json = "{'status':false,'Message' :'Oops! Something went wrong'}";


        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
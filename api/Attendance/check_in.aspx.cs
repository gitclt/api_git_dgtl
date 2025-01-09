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
        public string emp_id;
        public string check_in;
        public string checkin_place;
        public string checkin_latitude;
        public string checkin_longitude;
        public string is_nearest_checkin;
        public string location;
        public string addedon;
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

          
                querry1 = @"INSERT INTO tbl_attendance (emp_id,check_in,checkin_place,location,addedon,checkin_latitude,checkin_longitude,is_nearest_checkin) 
            VALUES ('" + data.emp_id + "', '" + data.check_in + "', '" + data.checkin_place + "', '" + data.location + "',GETDATE(),'"+data.checkin_latitude+"','"+data.checkin_longitude+"','"+data.is_nearest_checkin + "')";
                message = "Data added successfully";
           
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
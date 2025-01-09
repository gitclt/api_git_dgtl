using DocumentFormat.OpenXml.Spreadsheet;
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
       // chk_tocken();

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
        public string privilage_id;
        public string role_id;

        public string type;
        public string is_add;
        public string is_edit;
        public string is_delete;
        public string is_view;
        public string id;

       
  }

    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        querry = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        string json = ""; // Moved declaration here
        bool success = true; // Track overall success
        bool dataAdded = false; // Track if any data was added
        bool dataDeleted = false; // Track if any data was deleted

        foreach (var data in DataResponse)
        {
            string querry1 = "";

            if (data.type == "add")
            {
                querry1 = @"insert into tbl_role_privilage(privilage_id,role_id,[is_add],[is_edit],[is_delete],[is_view]) values('" + data.privilage_id + "','" + data.role_id + "','" + data.is_add + "','" + data.is_edit + "','" + data.is_delete + "','" + data.is_view + "')";

                int status = cc.Insert(querry1);
                if (status > 0)
                {
                    dataAdded = true;
                }
                else
                {
                    success = false;
                    break; // Stop processing further if any operation fails
                }
            }
            else if (data.type == "delete")
            {
                querry1 = @"delete from tbl_role_privilage  where id=" + data.id;

                int status = cc.Insert(querry1);
                if (status > 0)
                {
                    dataDeleted = true;
                }
                else
                {
                    success = false;
                    break; // Stop processing further if any operation fails
                }
            }
        }

        // If all operations were successful, send a single success response
        if (success)
        {
            string message = "";
            if (dataAdded)
            {
                message += "Data added successfully.\n";
            }
            if (dataDeleted)
            {
                message += "Data deleted successfully.";
            }
            json = "{'status':true,'Message' :'" + message + "'}";
        }
        else
        {
            json = "{'status':false,'Message' :'Failed'}";
        }

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

}
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
        public string tag;
        public string task_id;     
    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string qry = "";
        string json = "";
        string message = "";
        int count = 0;

        foreach (var data in DataResponse)
        {
            // Concatenate new tag with the existing tags (assuming comma-separated tags)
            string query = @"update tbl_task 
                         set tag = CASE 
                                      WHEN tag IS NULL OR tag = '' 
                                      THEN '" + data.tag + @"' 
                                      ELSE tag + ',' + '" + data.tag + @"' 
                                   END 
                         where id='" + data.task_id + "'";
            message = "Tags updated successfully.";

            int status = cc.Insert(query);

            if (status > 0)
            {
                count++;
            }
            else
            {
                // If any operation fails, return a failure response
                json = "{'status':false,'Message':'Failed to update tags.'}";
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;
            }
        }

        if (count > 0)
        {
            json = "{'status':true,'Message':'" + message + "'}";
        }
        else
        {
            json = "{'status':false,'Message':'No tags were updated.'}";
        }

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

}
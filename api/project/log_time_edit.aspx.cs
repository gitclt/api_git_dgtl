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
       //  chk_tocken();

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
        public string task_id;
        public string date;
        public string start_time;
        public string end_time;
        public string time_spent;
        public string mark_billable;
        public string is_complete;
        public string id;
       // public string type;
        public string created_user_id;
    }
    public void insert()
    {
        // Reading the request body to get the JSON payload
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        // Deserializing the JSON into a list of DataResponse objects
        List<DataResponse> dataResponseList = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        // Initialize a flag to check whether the update was successful
        int totalUpdatedRecords = 0;

        // Loop through each item in the list
        foreach (var data in dataResponseList)
        {
            string query = "UPDATE tbl_log_time SET ";
            bool isFieldUpdated = false; // Flag to ensure at least one field is updated

            // Building the query dynamically based on the provided fields
            if (!string.IsNullOrEmpty(data.task_id))
            {
                query += "task_id = '" + data.task_id + "', ";
                isFieldUpdated = true;
            }
            if (!string.IsNullOrEmpty(data.date))
            {
                query += "date = '" + data.date + "', ";
                isFieldUpdated = true;
            }
            if (!string.IsNullOrEmpty(data.start_time))
            {
                query += "start_time = '" + data.start_time + "', ";
                isFieldUpdated = true;
            }
            if (!string.IsNullOrEmpty(data.end_time))
            {
                query += "end_time = '" + data.end_time + "', ";
                isFieldUpdated = true;
            }
            if (!string.IsNullOrEmpty(data.time_spent))
            {
                query += "time_spent = '" + data.time_spent + "', ";
                isFieldUpdated = true;
            }
            if (!string.IsNullOrEmpty(data.mark_billable))
            {
                query += "mark_billable = '" + data.mark_billable + "', ";
                isFieldUpdated = true;
            }
            if (!string.IsNullOrEmpty(data.created_user_id))
            {
                query += "created_user_id = '" + data.created_user_id + "', ";
                isFieldUpdated = true;
            }
            if (!string.IsNullOrEmpty(data.is_complete))
            {
                query += "is_complete = '" + data.is_complete + "', ";
                isFieldUpdated = true;
            }

            // If at least one field is updated, finalize the query
            if (isFieldUpdated)
            {
                query = query.TrimEnd(',', ' '); // Remove trailing comma
                query += " WHERE id = " + data.id; // Add WHERE clause with ID

                // Execute the query
                int status = cc.Insert(query);

                // Increment the count of updated records if successful
                if (status > 0)
                {
                    totalUpdatedRecords++;
                }
            }
        }

        // Construct the JSON response based on whether updates were successful
        string json;
        if (totalUpdatedRecords > 0)
        {
            json = "{'status':true,'Message':'Data updated successfully.'}";
        }
        else
        {
            json = "{'status':false,'Message':'No records updated or something went wrong.'}";
        }

        // Send the JSON response
        json = json.Replace("'", "\""); // Replace single quotes with double quotes for valid JSON
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

}
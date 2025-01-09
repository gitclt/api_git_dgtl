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
        public string name;
        public string hierarchy_id;
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
        bool success = true; // Track overall success
        bool dataAdded = false; // Track if any data was added
        bool dataDeleted = false; // Track if any data was deleted
        bool dataedit = false;
        foreach (var data in DataResponse)
        {

            if (data.type == "add")
            {
                querry = @"select * from tbl_role where name='" + data.name + "'";
                DataSet ds = cc.joinselect(querry);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':false,'Message' :'Already Exist! Please update if needed.'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
                else
                {

                    qry += @"insert into tbl_role (name,hierarchy_id,delete_status)values('" + data.name + "','" + data.hierarchy_id + "','0') ";

                    int status = cc.Insert(qry);
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
            }
            else if(data.type == "edit")
            {

                // qry = @"update tbl_role set name='" + data.name + "',hierarchy_id='"+ data.hierarchy_id + "' where id='" + data.id + "' ";
                qry = "UPDATE tbl_role SET ";

                // Track if we've added any fields to update
                bool isFirstField = true;

                // Conditionally add fields to the update clause
                if (!string.IsNullOrEmpty(data.name))
                {
                    qry += "name = '" + data.name + "'";
                    isFirstField = false; // Mark that a field has been added
                }

                if (!string.IsNullOrEmpty(data.hierarchy_id))
                {
                    if (!isFirstField)
                    {
                        qry += " ,"; // Add comma if it's not the first field
                    }
                    qry += "hierarchy_id = '" + data.hierarchy_id + "'";
                }

                qry += " WHERE id = '" + data.id + "'";
                int status = cc.Insert(qry);
                if (status > 0)
                {
                    dataedit = true;
                }
                else
                {
                    success = false;
                    break; // Stop processing further if any operation fails
                }

            }
            else if (data.type == "delete")
            {

                qry = @"update tbl_role set delete_status='1' where id='" + data.id + "' ";
                int status = cc.Insert(qry);
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
        if (success)
        {
            string message = "";
            if (dataAdded)
            {
                message += "Data added successfully.";
            }
            if (dataDeleted)
            {
                message += "Data deleted successfully.";
            }


            if (dataedit)
            {
                message += "Data Update successfully.";
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
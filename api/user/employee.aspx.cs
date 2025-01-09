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
using System.Runtime.InteropServices.ComTypes;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    CommFuncs com = new CommFuncs();

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
        public string name;
        public string last_name;
        public string employee_id;
        public string email;
        public string type;
        public string id;
        public string phone;
        public string designation_id;
        public string hierarchy_id;
        public string image;
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
        bool dataedit = false;
        bool successtrue = true;

        string qry = "";
        int count = 0;
        foreach (var data in DataResponse)
        {
            if (data.type == "add")
            {
                querry = @"select * from tbl_employee where name='" + data.name + "' or email='" + data.email + "' or  phone='" + data.phone + "' ";

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
                    qry += @"insert into tbl_employee (name,email,phone,designation_id,active_status,join_date,delete_status,image,last_name,employee_id)values('" + data.name + "','" + data.email + "','" + data.phone + "','" + data.designation_id + "','0',GETDATE(),'0','" + data.image + "','" + data.last_name + "','" + data.employee_id + "') ";

                    int status = cc.Insert(qry);

                    if (status > 0)
                    {
                        //get return respons id after adding user table
                        String querry2 = @"select max(id) as id from tbl_employee";

                        DataSet ds1 = cc.joinselect(querry2);

                        int task_id = Convert.ToInt32(ds1.Tables[0].Rows[0]["id"]);

                        querry = @"insert into tbl_user (type,username,password,emp_id,hierarchy_id,last_log,delete_status)values('employee','" + data.phone + "','" + EncodeDecode.base64Encode(data.phone) + "','" + task_id + "','" + data.hierarchy_id + "',GETDATE(),'0') ";

                        int status1 = cc.Insert(querry);

                        dataAdded = true;
                    }
                    else
                    {
                        success = false;
                        break; // Stop processing further if any operation fails
                    }
                }
            }
            else if (data.type == "edit")
            {
                qry = "UPDATE tbl_employee SET ";
                querry = "UPDATE tbl_user SET ";

                // Track if we've added any fields to update
                bool isFirstField = true;
                bool isFirstFieldUser = true;

                // Conditionally add fields to the update clause for tbl_employee
                if (!string.IsNullOrEmpty(data.name))
                {
                    qry += "name = '" + data.name + "'";
                    isFirstField = false; // Mark that a field has been added
                }

                if (!string.IsNullOrEmpty(data.phone))
                {
                    if (!isFirstField)
                    {
                        qry += ", "; // Add comma if it's not the first field
                    }
                    qry += "phone = '" + data.phone + "'";
                    isFirstField = false;
                }

                if (!string.IsNullOrEmpty(data.designation_id))
                {
                    if (!isFirstField)
                    {
                        qry += ", "; // Add comma if it's not the first field
                    }
                    qry += "designation_id = '" + data.designation_id + "'";
                    isFirstField = false;
                }

                if (!string.IsNullOrEmpty(data.email))
                {
                    if (!isFirstField)
                    {
                        qry += ", "; // Add comma if it's not the first field
                    }
                    qry += "email = '" + data.email + "'";
                    isFirstField = false;
                }

                if (!string.IsNullOrEmpty(data.last_name))
                {
                    if (!isFirstField)
                    {
                        qry += ", "; // Add comma if it's not the first field
                    }
                    qry += "last_name = '" + data.last_name + "'";
                    isFirstField = false;
                }

                if (!string.IsNullOrEmpty(data.employee_id))
                {
                    if (!isFirstField)
                    {
                        qry += ", "; // Add comma if it's not the first field
                    }
                    qry += "employee_id = '" + data.employee_id + "'";
                    isFirstField = false;
                }

                if (!string.IsNullOrEmpty(data.image))
                {
                    if (!isFirstField)
                    {
                        qry += ", "; // Add comma if it's not the first field
                    }
                    qry += "image = '" + data.image + "'";
                    isFirstField = false;
                }

                // Conditionally add fields to the update clause for tbl_user
                if (!string.IsNullOrEmpty(data.hierarchy_id))
                {
                    querry += "hierarchy_id = '" + data.hierarchy_id + "'";
                    isFirstFieldUser = false; // Mark that a field has been added
                }

                // Add WHERE clauses if any fields were added
                if (!isFirstField)
                {
                    qry += " WHERE id = '" + data.id + "'";
                }

                if (!isFirstFieldUser)
                {
                    querry += " WHERE emp_id = '" + data.id + "'";
                }

                int status = 0;
                int status1 = 0;

                // Execute queries only if they are not empty
                if (!isFirstField)
                {
                    status = cc.Insert(qry);
                }

                if (!isFirstFieldUser)
                {
                    status1 = cc.Insert(querry);
                }

                if (status > 0 || status1 > 0)
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
                qry = @"update tbl_employee set delete_status='1',active_status='1' where id='" + data.id + "' ";
                int status = cc.Insert(qry);
                if (status > 0)
                {
                    qry = @"update tbl_user set delete_status='1' where emp_id='" + data.id + "' ";
                    int status1 = cc.Insert(qry);
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
                message += "Data updated successfully.";
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
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
using System.ServiceModel.Syndication;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.IdentityModel.Protocols.WSTrust;

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
        public string description;
        public string workspace_id;
        public string id;
        public string type;
        public string status;
        public string target_date;
        public string completed_date;
        public List<projectmember> pro_member;

    }

    public class projectmember 
    {
        public string emp_id;
        public string workspace_id;
    }

    // In the insert method
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        List<DataResponse> dataResponseList = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        string query1 = "";
        string query2 = "";
        string message = "";
        string json = "";

        foreach (var data in dataResponseList)
        {
            if (data.type == "add")
            {
                querry = @"select id from tbl_projectmaster where name='" + data.name + "'";
                DataSet ds = cc.joinselect(querry);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    message = "name already exists.";
                    json = "{'status':false,'Message' :'" + message + "'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
                else
                {
                    query1 = @"INSERT INTO tbl_projectmaster (name, description, workspace_id, status, target_date, delete_status, completed_date)
                       VALUES ('" + data.name + "', '" + data.description + "', '" + data.workspace_id + "', '" + data.status + "', '" + data.target_date + "', 0, '" + data.completed_date + "')";


                }
                int status = cc.Insert(query1);


                if (status > 0)
                {
                    query2 = @"SELECT TOP 1 id
                           FROM tbl_projectmaster
                           WHERE name = '" + data.name + "'";

                    DataSet dss = cc.joinselect(query2);

                    if (dss.Tables[0].Rows.Count > 0)
                    {
                        int project_id = Convert.ToInt32(dss.Tables[0].Rows[0]["id"]);
                        query2 = ""; 

                        if (data.pro_member != null) 
                        {
                            foreach (var member in data.pro_member) 
                            {
                               
                                    query2 += @"INSERT INTO tbl_projectmember (project_id, emp_id, workspace_id, delete_status) 
                                            VALUES ('" + project_id + "','" + member.emp_id + "','" + member.workspace_id + "', 0); ";
                                
                            }

                            if (!string.IsNullOrEmpty(query2))
                            {
                                int status1 = cc.Insert(query2);

                                if (status1 > 0)
                                {
                                    message = "Data added successfully.";
                                }
                                else
                                {
                                    message = "Failed to insert project members.";
                                }
                            }
                        }

                        //tasklist

                        string querry3 = "";
                        querry3 += @"INSERT INTO tbl_tasklist (pro_id, name, status, color,delete_status) 
                                            VALUES ('" + project_id + "','To do','pending', 'ffB1B1B1',0); ";

                            
                            if (!string.IsNullOrEmpty(querry3))
                            {
                                int status1 = cc.Insert(querry3);

                                if (status1 > 0)
                                {
                                    message = "Data added successfully.";
                                }
                                else
                                {
                                    message = "Failed to insert";
                                }
                            }
                        

                    }
                }
                else
                {
                    message = "Failed to insert";
                }
            }

            else if (data.type == "edit")
            {
                string qry = "";
                // qry = @"update tbl_role set name='" + data.name + "',hierarchy_id='"+ data.hierarchy_id + "' where id='" + data.id + "' ";
                qry = "UPDATE tbl_projectmaster SET ";

                // Track if we've added any fields to update
                bool isFirstField = true;

                // Conditionally add fields to the update clause
                if (!string.IsNullOrEmpty(data.name))
                {
                    qry += "name = '" + data.name + "'";
                    isFirstField = false; // Mark that a field has been added
                }

                if (!string.IsNullOrEmpty(data.description))
                {
                    if (!isFirstField)
                    {
                        qry += " ,"; // Add comma if it's not the first field
                        isFirstField = false;
                    }
                    qry += "description = '" + data.description + "'";
                }

                if (!string.IsNullOrEmpty(data.completed_date))
                {
                    if (!isFirstField)
                    {
                        qry += " ,"; // Add comma if it's not the first field
                        isFirstField = false;
                    }
                    qry += "completed_date = '" + data.completed_date + "'";
                }
                if (!string.IsNullOrEmpty(data.status))
                {
                    if (!isFirstField)
                    {
                        qry += " ,"; // Add comma if it's not the first field
                        isFirstField = false;
                    }
                    qry += "status = '" + data.status + "'";
                }
                if (!string.IsNullOrEmpty(data.workspace_id))
                {
                    if (!isFirstField)
                    {
                        qry += " ,"; // Add comma if it's not the first field
                        isFirstField = false;
                    }
                    qry += "workspace_id = '" + data.workspace_id + "'";
                }

                qry += " WHERE id = '" + data.id + "'";


                int status = cc.Insert(qry);

                if (status > 0)
                {
                    message = "Data updated successfully.";
                }
                else
                {
                    message = "Failed to update data.";
                }

            }
            else if (data.type == "delete")
            {
                // Update the delete status for the project
                query1 = @"UPDATE tbl_projectmaster 
                       SET delete_status = 1, deleted_on = GETDATE() 
                       WHERE id = '" + data.id + "'";
                int status = cc.Insert(query1);
                if (status > 0)
                {
                    message = "Data deleted successfully.";
                }
                else
                {
                    message = "Failed to delete data.";
                }
            }
        }

        json = "{'status':true,'Message' :'" + message + "'}";
        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }


}








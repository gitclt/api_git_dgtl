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
    string json;
    static string querry;

    protected void Page_Load(object sender, EventArgs e)
    {

        insert();
    }

    public class DataResponse
    {
        public string project_id;
        public string emp_id;
        public string type;
        public string id;
        public string workspace_id;
        public string delete_status;
        
        
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
                querry = @"select * from tbl_projectmember where project_id='" + data.project_id + "' and emp_id='" + data.emp_id + "' ";
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

                    qry += @"insert into tbl_projectmember (project_id,emp_id,workspace_id,delete_status)values('" + data.project_id + "','" + data.emp_id + "','" + data.workspace_id + "','0') ";

                    int status = cc.Insert(qry);
                   // Response.Write(status);
                   // Response.End();
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
                qry = "UPDATE tbl_projectmember SET ";
              

                // Track if we've added any fields to update
                bool isFirstField = true;

                // Conditionally add fields to the update clause
                if (!string.IsNullOrEmpty(data.project_id))
                {
                    qry += "project_id = '" + data.project_id + "'";
                    isFirstField = false; // Mark that a field has been added
                }

                if (!string.IsNullOrEmpty(data.emp_id))
                {
                    if (!isFirstField)
                    {
                        qry += " ,"; // Add comma if it's not the first field
                        isFirstField = false;
                    }
                    qry += "emp_id = '" + data.emp_id + "'";
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
                int status1 = cc.Insert(querry);

                if (status > 0|| status1>0)
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

                qry = @"update tbl_projectmember set delete_status='1',deleted_on=GETDATE() where id='" + data.id + "' ";
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

            else if (data.type == "view")
            {

                qry = @"select p.*,e.name,w.work_space,w.description,pm.name as projectname from tbl_projectmember p 
join tbl_employee e on  p.emp_id=e.id
join tbl_workspace w on p.workspace_id=w.id
join tbl_projectmaster pm on p.project_id=pm.id where p.id='" + data.id + "' ";
 
                DataSet ds = cc.joinselect(qry);
               
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':true,'Message' :'success' ,'data':[";

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        json += "{'id':'" + ds.Tables[0].Rows[i]["id"] + "','project_id':'" + ds.Tables[0].Rows[i]["project_id"] + "','emp_id':'" + ds.Tables[0].Rows[i]["emp_id"] + "','workspace_id':'" + ds.Tables[0].Rows[i]["workspace_id"] + "','name':'" + ds.Tables[0].Rows[i]["name"] + "','work_space':'" + ds.Tables[0].Rows[i]["work_space"] + "','description':'" + ds.Tables[0].Rows[i]["description"] + "','projectname':'" + ds.Tables[0].Rows[i]["projectname"] +"'},";
                    }
                    json = json.TrimEnd(',');
                    json += "]}";
                   
                }
                else
                    json = "{'status':false,'Message' :'Failed'}";


                json = json.Replace("'", "\"");


                Response.ContentType = "application/json";

                Response.Write(json);

                Response.End();
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
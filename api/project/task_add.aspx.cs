using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
        public string tasklist_id;
        public string taskname;
        public string startdate;
        public string duedate;
        public string id;
        public string type;
        public string estimated_time;
        public string status;
        public string pro_id;
        public string priority_id;
        public List<task_emp> items; 
    }

    public class task_emp
    {
        public string emp_id;
    }

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

                // Insert into tbl_task
                query1 = @"INSERT INTO tbl_task (tasklist_id, taskname, startdate, duedate, estimated_time, status, pro_id, priority_id) 
                       VALUES ('" + data.tasklist_id + "', '" + data.taskname + "', '" + data.startdate + "', '" + data.duedate + "', '" + data.estimated_time + "', '" + data.status + "', '" + data.pro_id + "', '" + data.priority_id + "');";

                // Execute insert query and get the status
                int status = cc.Insert(query1);

                if (status > 0)
                {
                    // Get the inserted task ID
                    query2 = @"SELECT TOP 1 id
                           FROM tbl_task
                           WHERE taskname = '" + data.taskname + "' ORDER BY id DESC";

                    DataSet dss = cc.joinselect(query2);

                   
                    if (dss.Tables[0].Rows.Count > 0)
                    {
                        int task_id = Convert.ToInt32(dss.Tables[0].Rows[0]["id"]);
                        query2 = "";
                       
                        if (data.items != null)
                        {
                            foreach (var item in data.items)
                            {
                                if (!string.IsNullOrEmpty(item.emp_id))
                                {
                                    query2 += @"INSERT INTO tbl_task_emp (task_id, emp_id) 
                                            VALUES ('" + task_id + "', '" + item.emp_id + "');";
                                   
                                }
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
                                    message = "Failed to insert.";
                                }
                            }
                          
                        }
                     
                    }
                  
                }
                else
                {
                    message = "Failed to insert into tbl_task.";
                }
            }
            else if (data.type == "delete")
            {
                query1 = @"delete from tbl_task
                       WHERE id = '" + data.id + "'";
                query1 += @"delete from tbl_log_time
                       WHERE task_id = '" + data.id + "'";
              
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
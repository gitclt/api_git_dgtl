using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
//using System.IdentityModel.Protocols.WSTrust;
using System.IO;
using System.Web;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    CommFuncs commFuncs = new CommFuncs();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string assign_status = "";

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
        public string emp_id;
        public string item_id;
        public string qty;
        public string remark;
        public string addedby;
        public string packing_status;
        public string target_date;
        public string id;
        public string type;
        public string job_type;
        public string production_status;

    }
    public class items
    {
        public string title;
        public string description;
        public string type;
        public string emp_id;
        public string doc_id;
        public string date_time;
        public string view_status;

    }

    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string json = "";
        string message = "";

        foreach (var data in DataResponse)
        {
            string querry1 = "";
            string jobType = data.job_type.ToLower();
            bool success = false;



            if(jobType!= "production" && jobType!="packing")
            {
                // Handle the case where production is not completed yet
                message = "Job type mismatch.";
                json = "{'status':false,'Message' :'" + message + "'}";
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;
            }


            //get total qty
            int totalQuantity = commFuncs.get_order_item_totalQuantity(data.item_id);
       
            //get assigned qty
            int assignedQuantity = commFuncs.get_order_assignedQuantity(data.item_id,data.job_type);
           

            // Calculate remaining quantity
            int remainingQuantity = totalQuantity - assignedQuantity;
            
            // Check if quantity is sufficient
            if (remainingQuantity >= int.Parse(data.qty))
            {
                // Insert query for tbl_order_assigned
                string insertQuery = @"INSERT INTO tbl_order_assigned (emp_id, item_id, qty, remark, addedby, addedon, target_date, production_status, packing_status, job_type) " +
                                     "VALUES('" + data.emp_id + "','" + data.item_id + "','" + data.qty + "','" + data.remark + "','" + data.addedby + "', GETDATE(),'" + data.target_date + "','not started','not started','" + data.job_type + "')";


               
                // Calculate new total assigned quantity
                int newAssignedQuantity = assignedQuantity + int.Parse(data.qty);
              
                string status = "completed";
                if (newAssignedQuantity < totalQuantity)
                {
                    status = "partially assigned";
                }
               

                //order_item status update code
                insertQuery += commFuncs.update_order_items_assign_status(jobType, data.item_id,status);

                int status2 = cc.Insert(insertQuery);



                //order tabkle status update
                insertQuery = commFuncs.update_order_assign_status(jobType, data.item_id,status);

                // Execute the insert query for tbl_order_assigned
                int status1 = cc.Insert(insertQuery);
                if (status1 > 0)
                {
                    json = "{'status':false,'Message' :'Data added successfully.'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
                else
                {
                    json = "{'status':false,'Message' :'Insufficient quantity'}";


                    //message = "Insufficient quantity";
                    //json = "{'status':false,'Message' :'" + message + "'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
                if (status1 > 0)
                {
                    // Query to get the product name
                    string proQuery = @"SELECT p.product_name as item_name
                        FROM tbl_order_assigned o
                        INNER JOIN tbl_order_items oi ON o.item_id = oi.id
                        INNER JOIN tbl_product p ON oi.pro_id = p.id
                        WHERE o.item_id = '" + data.item_id + "'";
                  
                    DataSet proDs = cc.joinselect(proQuery);

                    if (proDs.Tables[0].Rows.Count > 0)
                    {
                        string itemName = proDs.Tables[0].Rows[0]["item_name"].ToString();

                        string notificationQuery = @"INSERT INTO tbl_notification (emp_id, title, date_time, description, type, doc_id,view_status) " +
                                                   "VALUES('" + data.emp_id + "', '" + itemName + " " + data.job_type + "', GETDATE(), '', 'pending', 0,0)";

                        cc.Insert(notificationQuery);

                        json = "{'status':true,'Message' :'Data added successfully.'}";
                        json = json.Replace("'", "\"");
                        Response.ContentType = "application/json";
                        Response.Write(json);
                        Response.End();
                    }
                    else
                    {
                        json = "{'status':false,'Message' :'Failed to update data.'}";
                        json = json.Replace("'", "\"");
                        Response.ContentType = "application/json";
                        Response.Write(json);
                        Response.End();
                    }
                }
                else
                {
                    json = "{'status':false,'Message' :'Failed to insert data.'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
             

                // Generate response based on the insert status
               
            }

            json = "{'status':false,'Message' :'Insufficient quantity'}";
            json = json.Replace("'", "\"");

            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }

    }

    
}
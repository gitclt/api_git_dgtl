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
     //   chk_tocken();

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
        public string order_id;
        public string emp_id;
        public string item_id;
        public string qty;
        public string remark;
        public string addedby;
        public string target_date;
        public string job_type;
        public string item_status;
        public string order_status;
    }
    //public class items
    //{
    //    public string title;
    //    public string description;
    //    public string type;
    //    public string emp_id;
    //    public string doc_id;
    //    public string date_time;
    //    public string view_status;

    //}

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
                json = "{'status':true,'Message' :'" + message + "'}";
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;
            }


            string insertQuery = @"INSERT INTO tbl_order_assigned (emp_id, item_id, qty, remark, addedby, addedon, target_date, production_status, packing_status, job_type) " +
                                     "VALUES('" + data.emp_id + "','" + data.item_id + "','" + data.qty + "','" + data.remark + "','" + data.addedby + "', GETDATE(),'" + data.target_date + "','not started','not started','" + data.job_type + "')";

            if (data.job_type.ToLower() == "production")
            {
                insertQuery += @" UPDATE tbl_order_items SET production_assign_status = '" + data.item_status + "' WHERE id = '" + data.item_id + "'";
                insertQuery += @" UPDATE tbl_orders SET production_assign_status = '" + data.order_status + "' WHERE id = '" + data.order_id + "'";
            }
            else if (data.job_type.ToLower() == "packing")
            {
                insertQuery += @" UPDATE tbl_order_items SET packing_assign_status = '" + data.item_status + "' WHERE id = '" + data.item_id + "'";
                insertQuery += @" UPDATE tbl_orders SET packing_assign_status = '" + data.order_status + "' WHERE id = '" + data.order_id + "'";
            }
           

            int status1 = cc.Insert(insertQuery);

            if (status1 > 0)
                {
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
               
            json = "{'status':false,'Message' :'Oops! Something went wrong!'}";
            json = json.Replace("'", "\"");

            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }

    }

    
}
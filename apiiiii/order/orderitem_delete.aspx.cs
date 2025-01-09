using Newtonsoft.Json;
using System;
using System.Data;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    CommFuncs commFuncs = new CommFuncs();
    string json;

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
        if (Request.Form["id"] != null)
        {
            string id = Request.Form["id"].ToString();
            delete_order_item(id);
        }
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

    public void delete_order_item(string id)
    {
        string message = "Data deleted successfully.";
        var response = new
        {
            status = true,
            Message = message
        };

        try
        {
            // Check the status of the order item
            string q = @"SELECT order_id 
                         FROM tbl_order_items 
                         WHERE packing_assign_status='pending' 
                         AND production_assign_status='pending' 
                         AND id=" + id;

            DataSet ds = cc.joinselect(q);

            if (ds.Tables[0].Rows.Count > 0)
            {
                string orderId = ds.Tables[0].Rows[0]["order_id"].ToString();

                // Check if there are other items with the same order_id
                q = @"SELECT id 
                      FROM tbl_order_items 
                      WHERE order_id='" + orderId + "'";
                DataSet dss = cc.joinselect(q);

                if (dss.Tables[0].Rows.Count == 1)
                {
                    // If no other items exist, do not delete
                    response = new
                    {
                        status = false,
                        Message = "Cannot delete the only item of an order."
                    };
                }
                else
                {
                    // Delete from tbl_order_items
                    q = @"DELETE FROM tbl_order_items WHERE id=" + id;
                    int aa1 = cc.Insert(q);

                    if (aa1 > 0)
                    {
                        message = "Deleted successfully";
                    }
                    else
                    {
                        response = new
                        {
                            status = false,
                            Message = "Failed to delete the order item."
                        };
                    }

                    // Update order items assign status
                    string jobtypeQuery = @"SELECT job_type FROM tbl_order_assigned WHERE item_id=" + id;
                    DataSet jobtypeDS = cc.joinselect(jobtypeQuery);
                    if (jobtypeDS.Tables[0].Rows.Count > 0)
                    {
                        string jobtype = jobtypeDS.Tables[0].Rows[0]["job_type"].ToString();
                        string status = ""; // Replace with the actual status
                        string updateQuery = commFuncs.update_order_items_assign_status(jobtype, id, status);
                        int status2 = cc.Insert(updateQuery);

                        if (status2 <= 0)
                        {
                            response = new
                            {
                                status = false,
                                Message = "Failed to update order items assign status."
                            };
                        }
                    }
                }
            }
            else
            {
                response = new
                {
                    status = false,
                    Message = "Delete Assigned details before deleting order item."
                };
            }
        }
        catch (Exception ex)
        {
            response = new
            {
                status = false,
                Message = "An error occurred: " + ex.Message
            };
        }

        string json = JsonConvert.SerializeObject(response);

        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}

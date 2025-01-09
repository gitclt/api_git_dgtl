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
        if (Request.Form["id"] != null)
        {
            string id = Request.Form["id"].ToString();
            delete_order_item(id);
        }
    }

    public void delete_order_item(string id)
    {
        string message = "Data deleted successfully.";
        var errorResponse = new
        {
            status = true,
            Message = message
        };

        try
        {
            // Select query to check the status
            string q = @"SELECT order_id 
                         FROM tbl_order_items 
                         WHERE packing_assign_status='pending' 
                         AND production_assign_status='pending' 
                         AND id=" + id;

            string jobtype = @"select job_type from tbl_order_assigned item_id="+id;
            DataSet ds = cc.joinselect(q);

            if (ds.Tables[0].Rows.Count > 0)
            {
                string orderId = ds.Tables[0].Rows[0]["order_id"].ToString();

                // Delete from tbl_order_items
                q = @"DELETE FROM tbl_order_items WHERE id=" + id;
                int aa = cc.Insert(q);

                // Check if there are other items with the same order_id
                q = @"SELECT id 
                      FROM tbl_order_items 
                      WHERE order_id='" + orderId + "'";
                DataSet dss = cc.joinselect(q);

                if (dss.Tables[0].Rows.Count == 0)
                {
                    // Delete from tbl_orders if no other items exist
                    //q = @"DELETE FROM tbl_orders WHERE id='" + orderId + "'";
                    message = "single item in order so it cannot be deleted";

                    errorResponse = new
                    {
                        status = false,
                        Message = message
                    };
                    //int aa1 = cc.Insert(q);


                }

                // Update order items assign status
              //  string jobType = ""; // Replace with the actual job type
                string itemId = id; // Use the current item id
               string status = ""; // Replace with the actual status
                q = commFuncs.update_order_items_assign_status(jobtype, itemId, status);

                int status2 = cc.Insert(q);
            }
            else
            {
                message = "Delete Assigned details before deleting order item.";
                errorResponse = new
                {
                    status = false,
                    Message = message
                };
            }
        }
        catch (Exception ex)
        {
            errorResponse = new
            {
                status = false,
                Message = "An error occurred: " + ex.Message
            };
        }

        json = JsonConvert.SerializeObject(errorResponse);

        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}

using Newtonsoft.Json;
using System;
using System.Data;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    string json;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Form["id"] != null)
        {
            string id = Request.Form["id"].ToString();
            delete_order(id);
        }
    }

    public void delete_order(string id)
    {
        string message = "Data deleted successfully.";
        var errorResponse = new
        {
            status = true,
            Message = message
        };

        try
        {
            string q = @"SELECT id 
                         FROM tbl_order_items 
                         WHERE packing_assign_status='pending' 
                         AND production_assign_status='pending' 
                         AND order_id=" + id;
            DataSet ds = cc.joinselect(q);
            if (ds.Tables[0].Rows.Count > 0)
            {
                q = @"DELETE FROM tbl_order_items WHERE order_id=" + id + "; ";
                q += @"DELETE FROM tbl_orders WHERE id=" + id;
                int aa = cc.Insert(q);
            }
            else
            {
                message = "Delete Assigned details before deleting order.";
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

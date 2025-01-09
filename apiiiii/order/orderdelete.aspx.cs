using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Data;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    string json, empid, ip_address;

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();

        if (Request.Form["empid"] != null)
        {
            empid = Request.Form["empid"];
        }
        if (Request.Form["ip_address"] != null)
        {
            ip_address = Request.Form["ip_address"];
        }

        if (Request.Form["id"] != null)
        {
            string id = Request.Form["id"].ToString();
            delete_order(id);
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
            // Token is valid
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
                q += @"UPDATE tbl_orders 
                       SET delete_status = 1,deleted_on = GETDATE(),deleated_by = '" + empid + "', ip_address = '" + ip_address + "' WHERE id = " + id;

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

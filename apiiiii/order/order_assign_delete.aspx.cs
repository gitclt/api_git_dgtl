using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    CommFuncs commFuncs = new CommFuncs();
    string json;
    static string querry, id = "",item_status,order_status;

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
        delete_order_assigned();
    }

    public void chk_tocken()
    {
        CommFuncs CommFuncs = new CommFuncs();

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

            delete_order_assigned();
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

  

public void delete_order_assigned()
{
    string message = "Data deleted successfully.";
    var errorResponse = new
    {
        status = true,
        Message = message
    };

    try
    {
            if (Request.Form["id"] != null)
            {
                id = Request.Form["id"];
            }

            if (Request.Form["item_status"] != null)
            {
                item_status = Request.Form["item_status"];
            }
            if (Request.Form["order_status"] != null)
            {
                order_status = Request.Form["order_status"];
            }
            string q = @"select item_id, job_type, qty
                     from tbl_order_assigned 
                     where packing_status='not started' 
                     and production_status='not started' 
                     and id='" + id + "'";
           
            DataSet ds = cc.joinselect(q);

        if (ds.Tables[0].Rows.Count > 0)
        {
            string itemId = ds.Tables[0].Rows[0]["item_id"].ToString();
            string jobType = ds.Tables[0].Rows[0]["job_type"].ToString();
            string qty = ds.Tables[0].Rows[0]["qty"].ToString();

            q = @"delete from tbl_order_assigned where id='" + id + "';";

            string qq = @"SELECT order_id FROM tbl_order_items WHERE id = '" + itemId + "'";
               

            DataSet dss = cc.joinselect(qq);

            if (dss.Tables[0].Rows.Count > 0)
            {
                string orderid = dss.Tables[0].Rows[0]["order_id"].ToString();

                if (jobType.ToLower() == "production")
                {
                    q += @"UPDATE tbl_order_items SET production_assign_status = '" + item_status + "' WHERE id = '" + itemId + "';";
                    q += @"UPDATE tbl_orders SET production_assign_status = '" + order_status + "' WHERE id = '" + orderid + "';";
                }
                else if (jobType.ToLower() == "packing")
                {
                    q += @"UPDATE tbl_order_items SET packing_assign_status = '" + item_status + "' WHERE id = '" + itemId + "';";
                    q += @"UPDATE tbl_orders SET packing_assign_status = '" + order_status + "' WHERE id = '" + orderid + "';";
                }
            }

            int aa = cc.Insert(q);
            q = @"select id from tbl_order_assigned where item_id='" + itemId + "'";
            dss = cc.joinselect(q);

            if (dss.Tables[0].Rows.Count == 0)
            {
                q = @"delete from tbl_order_task where id='" + itemId + "'";
                int aa1 = cc.Insert(q);
            }
        }
        else
        {
            message = "Delete task details before deleting order assigned.";
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
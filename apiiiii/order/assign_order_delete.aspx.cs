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
    static string querry, id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (Request.Form["id"] != null)
        {
            id = Request.Form["id"].ToString();
            delete_order_assigned();
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
            string q = @"select item_id, job_type,qty
                         from tbl_order_assigned 
                         where packing_status='not started' 
                         and production_status='not started' 
                         and id=" + id;
            DataSet ds = cc.joinselect(q);

            if (ds.Tables[0].Rows.Count > 0)
            {

                string itemId = ds.Tables[0].Rows[0]["item_id"].ToString();
                string jobType = ds.Tables[0].Rows[0]["job_type"].ToString();
                string qty = ds.Tables[0].Rows[0]["qty"].ToString();
              


                int totalQuantity = commFuncs.get_order_item_totalQuantity(itemId);
                int assignedQuantity = commFuncs.get_order_assignedQuantity(itemId,jobType);
                int newAssignedQuantity = assignedQuantity - int.Parse(qty);

                string status = "partially assigned";
                if (newAssignedQuantity < totalQuantity)
                {
                    status = "pending";
                }
                q = @"delete from tbl_order_assigned where id=" + id;
                int aa = cc.Insert(q);

                q = commFuncs.update_order_items_assign_status(jobType, itemId, status);
                int status2 = cc.Insert(q);

                q = commFuncs.update_order_assign_status(jobType, itemId, status);
                int status1 = cc.Insert(q);

                q = @"select id from tbl_order_assigned where item_id='" + itemId + "'";
                DataSet dss = cc.joinselect(q);

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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    string json;

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_token();
    }

    public void chk_token()
    {
        CommFuncs CommFuncs = new CommFuncs();
        string id = "";

        if (Request.Headers["Authorization"] != null)
        {
            id = CommFuncs.get_tocken_details(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
        }

        if (id == "Oops! Tocken Expired!")
        {
            SendErrorResponse("Oops! Token Expired!", 403);
            return;
        }
        else if (id == "")
        {
            SendErrorResponse("Oops! Unauthorized Access!", 403);
            return;
        }

        getdata(id);
    }

    public void getdata(string empid)
    {
        if (Request.Form["type"] != null)
        {
            string type = Request.Form["type"];
            if (type == "add")
            {
                string curdate = DateTime.Now.ToString("yyyy-MM-dd");
                string query1 = @"
                SELECT COUNT(order_no) as count
                FROM tbl_orders
                WHERE CAST(addedon AS DATE) = @curdate
                AND addedby = @empid";

                DataSet ds = cc.joinselect(query1, new Dictionary<string, object> { { "@curdate", curdate }, { "@empid", empid } });

                if (ds.Tables[0].Rows.Count > 0)
                {
                    int totalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["count"]);
                    decimal percentage = 0;

                    if (totalCount > 0)
                    {
                        string query2 = @"
                        SELECT COUNT(order_no) as total_count
                        FROM tbl_orders
                        WHERE addedby = @empid";

                        DataSet totalDs = cc.joinselect(query2, new Dictionary<string, object> { { "@empid", empid } });

                        int totalOrders = Convert.ToInt32(totalDs.Tables[0].Rows[0]["total_count"]);
                        percentage = Math.Round(((decimal)totalCount / totalOrders) * 100, 2);
                    }

                    json = JsonConvert.SerializeObject(new { status = true, Message = "Success", data = new[] { new { total_count = totalCount, date = curdate, percentage = percentage } } });
                    SendResponse(json);
                }
                else
                {
                    SendErrorResponse("No data found!", 404);
                }
            }
            else
            {
                // Handle other types of requests
            }
        }
        else
        {
            SendErrorResponse("Type parameter not provided", 400);
        }
    }

    public void SendResponse(string message)
    {
        Response.ContentType = "application/json";
        Response.Write(message);
        Response.End();
    }

    public void SendErrorResponse(string message, int statusCode)
    {
        json = JsonConvert.SerializeObject(new { status = false, Message = message });
        Response.ContentType = "application/json";
        Response.StatusCode = statusCode;
        Response.Write(json);
        Response.End();
    }
}

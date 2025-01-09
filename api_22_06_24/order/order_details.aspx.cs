using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string order_id = "", pro_id = "", production_status = "", packing_status = "", production_assign_status = "", packing_assign_status = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
        getdata();
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

    public void getdata()
    {
        if (Request.Form["order_id"] != null)
        {
            order_id = Request.Form["order_id"];
        }

        string querry1 = @" 
                      SELECT
s.id,
                s.order_no,
                s.addedby,
                e.name AS employee,
                s.addedon,
                s.customer,
                c.first_name AS customername,
                c.email,
                c.mobile,
s.production_assign_status,
s.packing_assign_status,
s.production_status,
s.packing_status,
c.billing_address,
c.billing_city,
st.name as state,
d.name as district,

                s.total_items,
                s.total_amount,
                o.id as order_item_id,
                o.order_id,
                o.pro_id,
                o.qty,
                o.rate,
                o.remark,
                o.total,
                p.id AS product_id,
                p.product_name,
o.production_assign_status as itemsproduction_assign_status,
o.packing_assign_status as itemspacking_assign_status,
o.production_status as itemsproduction_status,
o.packing_status as itemspacking_status,
                s.id AS order_id
            FROM tbl_orders s
            INNER JOIN tbl_order_items o ON o.order_id = s.id
            INNER JOIN tbl_product p ON o.pro_id = p.id
            INNER JOIN tbl_customer c ON s.customer = c.id
            INNER JOIN tbl_employees e ON s.addedby = e.id
			            INNER JOIN tbl_states st ON c.billing_state=st.id
									            INNER JOIN tbl_district d ON c.billing_district=d.id";

        if (!string.IsNullOrEmpty(order_id))
        {
            querry1 += " WHERE o.order_id = '" + order_id + "' ";
        }

        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','data':[";

            Dictionary<string, List<Dictionary<string, string>>> ordersDict = new Dictionary<string, List<Dictionary<string, string>>>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string orderId = row["order_id"].ToString();

                if (!ordersDict.ContainsKey(orderId))
                {
                    ordersDict[orderId] = new List<Dictionary<string, string>>();
                }

                Dictionary<string, string> item = new Dictionary<string, string>
                {
                    { "pro_id", row["pro_id"].ToString() },
                                        { "product_name", row["product_name"].ToString() },
                                        { "order_id", row["order_id"].ToString() },
                                                                                
                    
                    { "order_item_id", row["order_item_id"].ToString() },

                    { "qty", row["qty"].ToString() },
                    { "rate", row["rate"].ToString() },
                    { "total", row["total"].ToString() },
                    { "remark", row["remark"].ToString() },
                    { "production_assign_status", row["itemsproduction_assign_status"].ToString() },
                     { "packing_assign_status", row["itemspacking_assign_status"].ToString() },
                                        

                     { "production_status", row["itemsproduction_status"].ToString() },
                     { "packing_status", row["itemspacking_status"].ToString() },



                };

                ordersDict[orderId].Add(item);
            }

            foreach (var order in ordersDict)
            {
                if (json.Length > 23) // Check if it's not the first iteration
                {
                }

                json += "{";
                json += "'order_no':'" + ds.Tables[0].Rows[0]["order_no"].ToString() + "',";
                json += "'id':'" + ds.Tables[0].Rows[0]["id"].ToString() + "',";

                json += "'customer_id':'" + ds.Tables[0].Rows[0]["customer"].ToString() + "',";
                json += "'total_amount':'" + ds.Tables[0].Rows[0]["total_amount"].ToString() + "',";
                json += "'remark':'" + ds.Tables[0].Rows[0]["remark"].ToString() + "',";
                json += "'added_by':'" + ds.Tables[0].Rows[0]["addedby"].ToString() + "',";
                json += "'employee':'" + ds.Tables[0].Rows[0]["employee"].ToString() + "',";
                json += "'date':'" + ds.Tables[0].Rows[0]["addedon"].ToString() + "',";
                json += "'customer_id':'" + ds.Tables[0].Rows[0]["customer"].ToString() + "',";
                json += "'customername':'" + ds.Tables[0].Rows[0]["customername"].ToString() + "',";
                json += "'email':'" + ds.Tables[0].Rows[0]["email"].ToString() + "',";
                json += "'mobile':'" + ds.Tables[0].Rows[0]["mobile"].ToString() + "',";
                json += "'address':'" + ds.Tables[0].Rows[0]["billing_address"].ToString() + "',";
                json += "'location':'" + ds.Tables[0].Rows[0]["billing_city"].ToString() + "',";
                json += "'state':'" + ds.Tables[0].Rows[0]["state"].ToString() + "',";
                json += "'district':'" + ds.Tables[0].Rows[0]["district"].ToString() + "',";

                json += "'total_items':'" + ds.Tables[0].Rows[0]["total_items"].ToString() + "',";
                json += "'total_amount':'" + ds.Tables[0].Rows[0]["total_amount"].ToString() + "',";
                json += "'packing_assign_status':'" + ds.Tables[0].Rows[0]["packing_assign_status"].ToString() + "',";
                
                                    json += "'packing_status':'" + ds.Tables[0].Rows[0]["packing_status"].ToString() + "',";
                json += "'production_assign_status':'" + ds.Tables[0].Rows[0]["production_assign_status"].ToString() + "',";
                json += "'production_status':'" + ds.Tables[0].Rows[0]["production_status"].ToString() + "',";

                json += "'remark':'" + ds.Tables[0].Rows[0]["remark"].ToString() + "',";
                json += "'items':[";

                foreach (var item in order.Value)
                {
                    if (json.EndsWith("[")) // Check if it's not the first iteration
                    {
                        json += "{";
                    }
                    else
                    {
                        json += ",{";
                    }

                    foreach (var pair in item)
                    {
                        json += "'" + pair.Key + "':'" + pair.Value + "',";
                    }

                    json = json.TrimEnd(',') + "}";
                }

                json += "]}";
            }

            json += "]}";
        }
        else
        {
            json = "{'status':false,'Message':'No data found!'}";
        }

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}

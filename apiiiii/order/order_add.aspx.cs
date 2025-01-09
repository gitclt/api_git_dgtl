using DocumentFormat.OpenXml.Drawing.Charts;
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
        public string order_no;
        public string customer_id;
        public string total_items;
        public string total_amount;
        public string remark;
        public string added_by;
        public string target_date;
        public List<items> items;
    }
    public class items
    {
        public string pro_id;
        public string qty;
        public string rate;
        public string total;
        public string job_status;
        public string assign_status;
        public string remark;
    }

    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        DataResponse DataResponse = JsonConvert.DeserializeObject<DataResponse>(bodyText);

        int count = 0;
        string message = "Added Successfully";
        bool isValid = true;


        string orderno = generate_orderno();



        int total_items = DataResponse.items.Count;


        string querry2 = @"insert into tbl_orders  
(order_no,addedon,customer,total_items,total_amount,packing_assign_status,packing_status,production_assign_status,production_status,remark,addedby,target_date,delete_status) 
   values('" + orderno + "',getdate(),'" + DataResponse.customer_id + "','" + total_items + "','" + DataResponse.total_amount + "','pending'" +
 ",'not started','pending','not started','" + DataResponse.remark + "','" + DataResponse.added_by + "','" + DataResponse.target_date + "',0)";
        int status = cc.Insert(querry2);
        if (status > 0)
        {
            querry2 = @"SELECT TOP 1 id
FROM tbl_orders
WHERE order_no='" + orderno + "' and addedby='" + DataResponse.added_by + "' order by id desc ";
            //Response.Write(querry2);
            //return;
            DataSet dss = cc.joinselect(querry2);
            if (dss.Tables[0].Rows.Count > 0)
            {
                int order_id = Convert.ToInt32(dss.Tables[0].Rows[0]["id"]);
                //Response.Write(order_id);
                //return;
                querry2 = "";
                foreach (var item in DataResponse.items)
                {
                    querry2 += @"insert into tbl_order_items  
(order_id,pro_id,qty,rate,total,packing_assign_status,packing_status,production_assign_status,production_status,remark,delete_status) 
values('" + order_id + "','" + item.pro_id + "','" + item.qty + "','" + item.rate + "','"+item.total+"','pending','not started','pending','not started','" + item.remark + "',0)";

                }

                int status1 = cc.Insert(querry2);
                //Response.Write(querry2);
                //return;
                if (status1 > 0)
                {


                    //json = "{'status':true,'Message' :'Data updated successfully.'}";

                    json = "{'status':true,'Message' :'" + message + "'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                    dss.Dispose();

                }
                else
                    json = "{'status':false,'Message' :'Oops! Something went wrong'}";

                json = json.Replace("'", "\"");

                Response.ContentType = "application/json";

                Response.Write(json);
                dss.Dispose();
                Response.End();
            }

        }

        else
        {
            message = "Oops! Something went wrong.";
            json = "{'status':false,'Message' :'" + message + "'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";

            Response.Write(json);
            Response.End();
        }





    }

   

    public string generate_orderno()
    {
        // Generate a new GUID
        Guid orderno = Guid.NewGuid();

        // Format the GUID and add the prefix
        string ordernoString = "FAS/ORD/" + orderno.ToString().Replace("-", "").Substring(0, 10);

        // Check if the generated order number exists in the database
        querry = @"select id from tbl_orders where order_no='" + ordernoString + "' ";
        DataSet ds = cc.joinselect(querry);

        // If a duplicate is found, generate a new order number
        if (ds.Tables[0].Rows.Count > 0)
        {
            return generate_orderno();
        }

        // Return the unique order number
        return ordernoString;
    }



    //    return orderno + "";
    //}
}
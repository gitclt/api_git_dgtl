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
    string order_id = "", pro_id = "", production_status = "",packing_status="", production_assign_status = "", packing_assign_status="";
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
       

        string querry1 = @"SELECT 
o.id,
o.pro_id,
o.qty,
   o.rate,
    o.production_status,
    o.packing_status,

   o.production_assign_status,
   o.packing_assign_status,

   o.remark,
o.total,
   p.id as product_id,
   p.product_name,
   s.id as order_id,
   s.order_no
from tbl_order_items o
 
INNER JOIN 
    tbl_orders s ON  o.order_id= s.id 
	inner join tbl_product p on o.pro_id=p.id 
";
        if (!string.IsNullOrEmpty(order_id))
        {
            querry1 += "AND o.order_id = '" + order_id + "' ";
        }
       

        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','quantity':'" + ds.Tables[0].Rows[i]["qty"].ToString() + "','rate':'" + ds.Tables[0].Rows[i]["rate"].ToString() + "','packing_status':'" + ds.Tables[0].Rows[i]["packing_status"].ToString() + "','production_status':'" + ds.Tables[0].Rows[i]["production_status"].ToString() + "','production_assign_status':'" + ds.Tables[0].Rows[i]["production_assign_status"].ToString() + "','packing_assign_status':'" + ds.Tables[0].Rows[i]["packing_assign_status"].ToString() + "','remark':'" + ds.Tables[0].Rows[i]["remark"].ToString() + "','product_id':'" + ds.Tables[0].Rows[i]["product_id"].ToString() + "','product_name':'" + ds.Tables[0].Rows[i]["product_name"].ToString() + "','order_id':'" + ds.Tables[0].Rows[i]["order_id"].ToString() + "','order_no':'" + ds.Tables[0].Rows[i]["order_no"].ToString() + "','total':'" + ds.Tables[0].Rows[i]["total"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
            json = "{'status':false,'Message' :'No data found!'}";


        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

}
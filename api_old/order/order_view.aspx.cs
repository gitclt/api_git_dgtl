using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string order_no = "", addedby = "", customer = "", fromdate = "",todate="",job_status="", assign_status="";
    int index, size;

    protected void Page_Load(object sender, EventArgs e)
    {
        //chk_tocken();
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
        string addedby = null;
        string customer = null;
        string fromdate = null;
        string todate = null;
        string job_status = null;
        string assign_status = null;
        int index = 0;
        int size = 10; // Default value if not provided

        if (Request.Form["addedby"] != null)
        {
            addedby = Request.Form["addedby"];
        }
        if (Request.Form["customer"] != null)
        {
            customer = Request.Form["customer"];
        }
        if (Request.Form["fromdate"] != null)
        {
            fromdate = Request.Form["fromdate"];
        }
        if (Request.Form["todate"] != null)
        {
            todate = Request.Form["todate"];
        }
        if (Request.Form["production_status"] != null)
        {
            job_status = Request.Form["production_status"];
        }
        if (Request.Form["packing_status"] != null)
        {
            job_status = Request.Form["packing_status"];
        }
        if (Request.Form["size"] != null && Request.Form["index"] != null)
        {
            index = int.Parse(Request.Form["index"]);
            size = int.Parse(Request.Form["size"]);
        }

        string query = @"
    WITH Results AS (
        SELECT 
            o.id AS oid,
            o.order_no,
            o.addedon,
            o.addedby,
            o.customer,
            o.total_items,
            o.total_amount,
            o.production_assign_status,
            o.packing_assign_status,
            o.packing_status,
            o.production_status,
            o.remark,
            e.id AS empid,
            e.name AS employee,
            c.email,
            c.mobile,
         
            c.id AS custid,
            o.target_date
        FROM tbl_orders o
        INNER JOIN tbl_employees e ON o.addedby = e.id
        INNER JOIN tbl_customer c ON o.customer = c.id
        WHERE 1=1";

        if (!string.IsNullOrEmpty(addedby))
        {
            query += " AND o.addedby = '" + addedby + "' ";
        }
        if (!string.IsNullOrEmpty(customer))
        {
            query += " AND o.customer = '" + customer + "' ";
        }
        if (!string.IsNullOrEmpty(fromdate))
        {
            query += " AND CAST(o.addedon AS DATE) >= CAST('" + fromdate + "' AS DATE) ";
        }
        if (!string.IsNullOrEmpty(todate))
        {
            query += " AND CAST(o.addedon AS DATE) <= CAST('" + todate + "' AS DATE) ";
        }
        if (!string.IsNullOrEmpty(job_status))
        {
            query += " AND (o.packing_status = '" + job_status + "' OR o.production_status = '" + job_status + "') ";
        }
        if (!string.IsNullOrEmpty(assign_status))
        {
            query += " AND o.assign_status = '" + assign_status + "' ";
        }

        query += @"
    ), ResultCount AS (
        SELECT COUNT(*) AS count 
        FROM Results
    )
    SELECT r.*, rc.count 
    FROM Results r, ResultCount rc 
    ORDER BY order_no DESC 
    OFFSET " + index * size + " ROWS FETCH NEXT " + size + " ROWS ONLY";

        //Response.Write(query);
        //return;
    
    DataSet ds = cc.joinselect(query);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','count':'" + ds.Tables[0].Rows[0]["count"].ToString() + "','size':'" + size + "','index':'" + index + "','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'order_number':'" + ds.Tables[0].Rows[i]["order_no"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["oid"].ToString() + "','addedby':'" + ds.Tables[0].Rows[i]["addedby"].ToString() + "','employee':'" + ds.Tables[0].Rows[i]["employee"].ToString() + "','date':'" + ds.Tables[0].Rows[i]["addedon"].ToString() + "','customer_id':'" + ds.Tables[0].Rows[i]["customer"].ToString() + "','email':'" + ds.Tables[0].Rows[i]["email"].ToString() + "','mobile':'" + ds.Tables[0].Rows[i]["mobile"].ToString() + "','total_items':'" + ds.Tables[0].Rows[i]["total_items"].ToString() + "','total_amount':'" + ds.Tables[0].Rows[i]["total_amount"].ToString() + "','packing_assign_status':'" + ds.Tables[0].Rows[i]["packing_assign_status"].ToString() + "','packing_status':'" + ds.Tables[0].Rows[i]["packing_status"].ToString() + "','production_assign_status':'" + ds.Tables[0].Rows[i]["production_assign_status"].ToString() + "','production_status':'" + ds.Tables[0].Rows[i]["production_status"].ToString() + "','remark':'" + ds.Tables[0].Rows[i]["remark"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
            ds.Dispose();
        }
        else
        {
            json = "{'status':false,'Message':'No data found!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}
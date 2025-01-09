using DocumentFormat.OpenXml.Office2010.Excel;
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
    string job_type = "", job_status = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();

       // jobstatus();
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
            jobstatus(id);
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

    public void jobstatus(string empid)
    {
        if (Request.Form["job_type"] != null)
        {
            job_type = Request.Form["job_type"];

        }


        if (Request.Form["job_status"] != null)
        {
            job_status = Request.Form["job_status"];

        }
        string querry1 = @"
SELECT 
c.name,
    p.product_name,
    o.production_status,
    o.packing_status,
	oa.id ,
o.target_date,
    oi.qty AS total_quantity,
    SUM(ot.qty) AS achieved_quantity,
    (oi.qty - SUM(ot.qty)) AS pending_quantity
FROM 
    tbl_orders o
INNER JOIN 
    tbl_order_items oi ON oi.order_id = o.id
INNER JOIN 
    tbl_order_assigned oa ON oi.id = oa.item_id
INNER JOIN 
    tbl_order_task ot ON oa.id = ot.order_assign_id
   INNER JOIN 
                tbl_order_task_emp te ON te.task_id = ot.id
INNER JOIN 
    tbl_product p ON oi.pro_id = p.id
INNER JOIN 
    tbl_employees e ON o.addedby = e.id 
INNER JOIN 
    tbl_customer c ON c.id = o.customer
where oa.emp_id=  '" + empid + "'";
 

        if (!string.IsNullOrEmpty(job_type))
        {
            querry1 += "AND oa.job_type  = '" + job_type + "' ";
        }
        if (!string.IsNullOrEmpty(job_status))
        {
            if (job_status.ToLower() == "production ongoing")
            {
                querry1 += " AND o.production_status = 'ongoing' ";
            }
            else if (job_status.ToLower() == "packing ongoing")
            {
                querry1 += " AND o.packing_status = 'ongoing' ";

            }

        }


        querry1 += @"
    GROUP BY 
     p.product_name,
                oa.job_type,
 o.production_status,
o.target_date,
 o.packing_status,
oa.id,
 oi.qty,
c.name

";



        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'remark':''";

                //if (job_type == "production")
                //{
                //    json += ",'status':'" + ds.Tables[0].Rows[i]["production_status"].ToString() + "'";
                //}
                //else
                //{
                //    json += ",'status':'" + ds.Tables[0].Rows[i]["packing_status"].ToString() + "'";
                //}

                json += ",'customer':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','qty':'" + ds.Tables[0].Rows[i]["total_quantity"].ToString() + "','product_name':'" + ds.Tables[0].Rows[i]["product_name"].ToString() + "','achieved_quantity':'" + ds.Tables[0].Rows[i]["achieved_quantity"].ToString() + "','pending_quantity':'" + ds.Tables[0].Rows[i]["pending_quantity"].ToString() + "','target_date':'" + ds.Tables[0].Rows[i]["target_date"].ToString() + "','order_assign_id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','todatetime':''},";


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
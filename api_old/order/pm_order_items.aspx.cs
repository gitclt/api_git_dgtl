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
    string id = "", order_id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        //chk_tocken();
        jobstatus();
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

    public void jobstatus()
    {
      

        if (Request.Form["order_id"] != null)
        {
            order_id = Request.Form["order_id"];
        }

        string querry1 = @"
 SELECT  
    c.company_name,
    o.target_date, 
    oi.qty,
    oa.job_type,
    p.product_name,
    p.color,
    p.gsm_id,
    p.size,
    g.name,
    o.production_status,
    o.packing_status,
    oi.id,
    o.order_no,
    o.total_items,
    (SELECT e1.name 
     FROM tbl_order_assigned oa1
     INNER JOIN tbl_employees e1 ON oa1.emp_id = e1.id
     WHERE oa1.item_id = oa.item_id AND oa1.job_type = 'production') AS production_assigned_employee,
    (SELECT e2.name 
     FROM tbl_order_assigned oa2
     INNER JOIN tbl_employees e2 ON oa2.emp_id = e2.id
     WHERE oa2.item_id = oa.item_id AND oa2.job_type = 'packing') AS packing_assigned_employee
FROM 
    tbl_orders o
INNER JOIN tbl_order_items oi ON oi.order_id = o.id
left JOIN tbl_order_assigned oa ON oa.item_id = oi.id
left JOIN tbl_employees e ON oa.emp_id = e.id
INNER JOIN tbl_product p ON oi.pro_id = p.id
INNER JOIN tbl_gsm g ON p.gsm_id = g.id
INNER JOIN tbl_customer c ON o.customer = c.id
WHERE 
    o.id ='" + order_id+"' ";

     
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'order_no':'" + ds.Tables[0].Rows[i]["order_no"].ToString() + "','target_date':'" + ds.Tables[0].Rows[i]["target_date"].ToString() + "'";

                //if (job_type == "production")
                //{
                //    json += ",'status':'" + ds.Tables[0].Rows[i]["production_status"].ToString() + "'";
                //}
                //else
                //{
                //    json += ",'status':'" + ds.Tables[0].Rows[i]["packing_status"].ToString() + "'";
                //}

                json += ",'company_name':'" + ds.Tables[0].Rows[i]["company_name"].ToString() + "','qty':'" + ds.Tables[0].Rows[i]["qty"].ToString() + "','total_items':'" + ds.Tables[0].Rows[i]["total_items"].ToString() + "','pm_remark':'" + ds.Tables[0].Rows[i]["pm_remark"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
        {
            json = "{'status':false,'Message' :'Success.','data':[]}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}

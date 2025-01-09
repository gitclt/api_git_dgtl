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
    string id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
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

    public void jobstatus( string empid)
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
    oi.pro_id, 
	p.product_name,
	p.color,
	p.gsm_id,
	p.size,
    o.target_date, 
    oi.qty,
    oa.job_type,
    p.product_name,
    o.production_status,
    o.packing_status,
c.company_name

FROM 
    tbl_orders o
INNER JOIN 
    tbl_order_items oi ON oi.order_id = o.id
INNER JOIN 
    tbl_order_assigned oa ON oa.item_id = oi.id
	
INNER JOIN 
    tbl_employees e ON oa.emp_id = e.id
INNER JOIN 
    tbl_product p ON oi.pro_id = p.id

INNER JOIN 
    tbl_customer c ON o.customer=c.id
WHERE 
    oa.emp_id = 27";

        if ((job_type)!=null)
        {
            querry1 += " AND oa.job_type = '" + job_type + "'";
        }

        if ((job_status)!=null)
        {
            if (job_status.ToLower() == "production pending")
            {
                querry1 += " AND o.production_status = 'not started'";
            }
            else if (job_status.ToLower() == "packing pending")
            {
                querry1 += " AND o.packing_status = 'not started'";
            }
         
        }



        querry1 += @"
    GROUP BY 
     o.remark, 
    oi.pro_id, 
                o.target_date, 
                oi.qty,
                oa.job_type,
oa.id,
c.company_name,	p.product_name,p.color,p.gsm_id,p.size,o.production_status,o.packing_status


";
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'remark':'" + ds.Tables[0].Rows[i]["remark"].ToString() + "','target_date':'" + ds.Tables[0].Rows[i]["target_date"].ToString() + "','achieved_qty':'','pending_quantity':'','employee_count':''";

                //if (job_type == "production")
                //{
                //    json += ",'status':'" + ds.Tables[0].Rows[i]["production_status"].ToString() + "'";
                //}
                //else
                //{
                //    json += ",'status':'" + ds.Tables[0].Rows[i]["packing_status"].ToString() + "'";
                //}

                json += ",'company_name':'" + ds.Tables[0].Rows[i]["company_name"].ToString() + "','qty':'" + ds.Tables[0].Rows[i]["qty"].ToString() + "','product_name':'" + ds.Tables[0].Rows[i]["product_name"].ToString() + "','order_assign_id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','production_status':'" + ds.Tables[0].Rows[i]["production_status"].ToString() + "'},";
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

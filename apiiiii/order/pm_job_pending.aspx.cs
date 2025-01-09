using DocumentFormat.OpenXml.Drawing.Charts;
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
    string id = "", pm_id="", status="";

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
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
      

        //if (Request.Form["job_status"] != null)
        //{
        //    job_status = Request.Form["job_status"];
        //}
        if (Request.Form["pm_id"] != null)
        {
            pm_id = Request.Form["pm_id"];
        }
        if (Request.Form["status"] != null)
        {
            status = Request.Form["status"];//pending/ongoing
        }
        string querry1 = @"
         					select o.order_no,c.first_name as company_name,o.target_date,o.id as order_id,o.production_assign_status,o.packing_assign_status,
				o.total_items,o.pm_remark,sum(oi.qty) as qty
				from 
				tbl_orders o
				inner join tbl_order_items oi on oi.order_id=o.id
             
				 INNER JOIN 
              tbl_customer c ON c.id = o.customer where o.pm_id='" + pm_id+ "'";
        if (status != null)
        {
            if (status.ToLower() == "pending")
            {
                querry1 += @" AND ( 
                                    (o.production_assign_status = 'pending' and o.packing_assign_status = 'pending') 
                                    or
                                    (o.production_assign_status = 'partially completed' or o.packing_assign_status = 'partially completed')                                     
                                    )";
            }
            else if (status.ToLower() == "ongoing")
            {
                querry1 += @" AND (
                                    
                                    (o.production_status = 'ongoing' and o.packing_status = 'not started')  
                                    or
                                    (o.production_status = 'not started' and o.packing_status = 'ongoing')  
or
                                    (o.production_status = 'ongoing' and o.packing_status = 'ongoing')  
or                                    (o.production_status = 'not started' and o.packing_status = 'not started')  

                                    )";
            }
            else if(status.ToLower()=="completed")
            {
                querry1 += @" AND (    (o.production_status = 'completed' and o.packing_status = 'completed')
                                    )";
            }
           
        }
        querry1 += "GROUP BY o.order_no,c.first_name,o.target_date,o.id,o.production_assign_status,o.packing_assign_status,o.total_items,o.pm_remark";
       
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {

         
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'order_no':'" + ds.Tables[0].Rows[i]["order_no"].ToString() + "','target_date':'" + ds.Tables[0].Rows[i]["target_date"].ToString() + "'";

                json += ",'company_name':'" + ds.Tables[0].Rows[i]["company_name"].ToString() + "','qty':'" + ds.Tables[0].Rows[i]["qty"].ToString() + "'," +
                    "'total_items':'" + ds.Tables[0].Rows[i]["total_items"].ToString() + "','pm_remark':'" + ds.Tables[0].Rows[i]["pm_remark"].ToString() + "'," +
                    "'order_id':'" + ds.Tables[0].Rows[i]["order_id"].ToString() + "','packing_assign_status':'" + ds.Tables[0].Rows[i]["packing_assign_status"].ToString() + "'," +
                    "'production_assign_status':'" + ds.Tables[0].Rows[i]["production_assign_status"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";
            ds.Dispose();
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
        {
            ds.Dispose();

            json = "{'status':false,'Message' :'Success.','data':[]}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}

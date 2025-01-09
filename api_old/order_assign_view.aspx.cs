using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;

    string job_type = "", item_id="";

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

        if (Request.Form["job_type"] != null)
        {
            job_type = Request.Form["job_type"];
           
        }
        if (Request.Form["item_id"] != null)
        {
            item_id = Request.Form["item_id"];
         
        }

        string  querry1 = @"

SELECT 
                os.id,
                os.qty,
                os.addedon,
                os.job_type,
                os.packing_status,
                os.production_status,
				i.packing_assign_status,
				i.production_assign_status,
os.item_id,
                e.name,
                e.id AS emp_id
            FROM tbl_order_assigned os
            INNER JOIN tbl_employees e ON e.id = os.emp_id
			            INNER JOIN tbl_order_items i ON os.item_id= i.id

     ";
       
        if (!string.IsNullOrEmpty(job_type))
        {
            querry1 += "AND os.job_type  = '" + job_type + "' ";
        }
        if (!string.IsNullOrEmpty(item_id))
        {
            querry1 += "AND os.item_id  = '" + item_id + "' ";
        }
        //Response.Write(querry1);
        //return;

        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','emp_id':'" + ds.Tables[0].Rows[i]["emp_id"].ToString() + "'";

                if (job_type == "production" )
                {
                    json += ",'status':'" + ds.Tables[0].Rows[i]["production_status"].ToString() + "','status':'" + ds.Tables[0].Rows[i]["production_assign_status"].ToString() + "'";
                }
                else 
                {
                    json += ",'status':'" + ds.Tables[0].Rows[i]["packing_status"].ToString() + "','status':'" + ds.Tables[0].Rows[i]["packing_assign_status"].ToString() + "'";
                }

                json += ",'qty':'" + ds.Tables[0].Rows[i]["qty"].ToString() + "','addedon':'" + ds.Tables[0].Rows[i]["addedon"].ToString() + "','item_id':'" + ds.Tables[0].Rows[i]["item_id"].ToString() + "'},";
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
            json = "{'status':false,'Message' :'No data found!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}    


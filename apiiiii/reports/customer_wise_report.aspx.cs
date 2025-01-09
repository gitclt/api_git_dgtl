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
    string date = "";

    protected void Page_Load(object sender, EventArgs e)
    {
      // chk_tocken();

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
        if (Request.Form["date"] != null)
        {
            date = Request.Form["date"];
        }
        string querry1 = @"SELECT 
    o.order_no,
    o.total_items,
    o.target_date,
    c.company_name,
    o.addedon,
    CASE 
        WHEN o.packing_status = 'completed' AND o.production_status = 'completed' THEN 'completed'
        WHEN o.packing_status  <> 'completed' OR o.production_status<> 'completed' THEN 'pending'
        ELSE 'unknown'
    END AS overall_status
FROM 
    tbl_orders o
INNER JOIN 
    tbl_customer c ON o.customer = c.id
WHERE 
    CAST(o.addedon AS DATE)='" + date+"'";
      
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'order_no':'" + ds.Tables[0].Rows[i]["order_no"].ToString() + "','total_items':'" + ds.Tables[0].Rows[i]["total_items"].ToString() + "'," +
                    "'target_date':'" + ds.Tables[0].Rows[i]["target_date"].ToString() + "','overall_status':'" + ds.Tables[0].Rows[i]["overall_status"].ToString() + "'," +
           "'company_name':'" + ds.Tables[0].Rows[i]["company_name"].ToString() + "','addedon':'" + ds.Tables[0].Rows[i]["addedon"].ToString() + "'},";
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
            ds.Dispose();

        json = "{'status':false,'Message' :'No data found!'}";


        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

}
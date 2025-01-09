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
    string name = "";
    string code = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
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
            getdata(id);
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

    public void getdata(string empid)
    {
        string curdate = DateTime.Now.ToString("yyyy-MM-dd");

        string querry1 = @"SELECT COUNT(order_no) as count
                           FROM tbl_orders  o
						   inner join tbl_order_items oi on  o.id=oi.id

						   inner join tbl_order_assigned oa on  oi.id=oa.item_id
                           WHERE CAST(oa.addedon AS DATE) = '"+curdate+"'  AND  oa.emp_id = '" + empid + "'";

        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            string querry2 = @"SELECT COUNT(order_no) as total_count 
                               FROM tbl_orders o
	   inner join tbl_order_items oi on  o.id=oi.id

						   inner join tbl_order_assigned oa on  oi.id=oa.item_id
                               WHERE oa.emp_id = '" + empid + "'";
          

            DataSet totalDs = cc.joinselect(querry2);
            int totalOrders = 0;
            int.TryParse(totalDs.Tables[0].Rows[0]["total_count"].ToString(), out totalOrders);

            int totalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["count"]);
            decimal percentage = 0;
            if (totalCount > 0)
            {
                percentage = Math.Round(((decimal)totalCount / totalOrders) * 100, 2);
            }

            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'total_count':'" + ds.Tables[0].Rows[i]["count"].ToString() + "','date':'" + curdate + "','percentage':'" + percentage + "'},";
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

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
    string name = "",district_id="",state_id="";
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
            Response.Write(json);
            Response.End();
            return;
        }
    }

    public void getdata()
    {
      

        string querry1 = @"SELECT 
        c.id,
        c.first_name as first_name,
        c.last_name as last_name,
        c.email,
c.customer_type,
c.code,
		c.gstno,
ct.name as shipping_country,
				cr.name as billing_country,
s.name as shipping_state,
st.name as billing_state,
dt.name as shipping_district,
d.name as billing_district,
c.mobile,
c.billing_address,
c.billing_city,
c.billing_state as billing_state_id,
        c.billing_district as billing_district_id,
        c.billing_pin,
     c.billing_country as billing_country_id,
	 c.company_name,
	 c.billing_phone,
	 c.shipping_address,
	 c.shipping_country as shipping_country_id,
	 c.shipping_state as shipping_state_id,
	 c.shipping_district as shipping_district_id,
	 c.shipping_city,
	 c.shipping_pin,
c.shipping_phone,
c.added_by

    FROM 
        tbl_customer c 
    left JOIN 
        tbl_country ct ON c.shipping_country = ct.id 
 left JOIN 
        tbl_country cr ON c.billing_country = cr.id 
		
    left JOIN 
        tbl_states s ON c.shipping_state = s.id 
 left JOIN 
        tbl_states st ON c.billing_state = st.id
     left JOIN 
        tbl_district dt ON c.shipping_district = dt.id 
		 left JOIN 
        tbl_district d ON c.billing_district = d.id
    WHERE 
        c.delete_status = 0 ";
       
      
       

        querry1 += " ORDER BY c.first_name ASC";
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                json += "{'first_name':'" + ds.Tables[0].Rows[i]["first_name"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "'," +
                    "'last_name':'" + ds.Tables[0].Rows[i]["last_name"].ToString() + "','email':'" + ds.Tables[0].Rows[i]["email"].ToString() + "'," +
                    "'mobile':'" + ds.Tables[0].Rows[i]["mobile"].ToString() + "','billing_address':'" + ds.Tables[0].Rows[i]["billing_address"].ToString() + "'," +
                    "'billing_city':'" + ds.Tables[0].Rows[i]["billing_city"].ToString() + "','billing_state_id':'" + ds.Tables[0].Rows[i]["billing_state_id"].ToString() + "','billing_district_id':'" + ds.Tables[0].Rows[i]["billing_district_id"].ToString() + "'," +
                    "'billing_pin':'" + ds.Tables[0].Rows[i]["billing_pin"].ToString() + "','billing_country_id':'" + ds.Tables[0].Rows[i]["billing_country_id"].ToString() + "','company_name':'" + ds.Tables[0].Rows[i]["company_name"].ToString() + "','billing_phone':'" + ds.Tables[0].Rows[i]["billing_phone"].ToString() + "'," +
                    "'shipping_address':'" + ds.Tables[0].Rows[i]["shipping_address"].ToString() + "','shipping_country_id':'" + ds.Tables[0].Rows[i]["shipping_country_id"].ToString() + "','shipping_state_id':'" + ds.Tables[0].Rows[i]["shipping_state_id"].ToString() + "','shipping_district_id':'" + ds.Tables[0].Rows[i]["shipping_district_id"].ToString() + "'," +
                    "'shipping_city':'" + ds.Tables[0].Rows[i]["shipping_city"].ToString() + "','shipping_pin':'" + ds.Tables[0].Rows[i]["shipping_pin"].ToString() + "','added_by':'" + ds.Tables[0].Rows[i]["added_by"].ToString() + "','shipping_phone':'" + ds.Tables[0].Rows[i]["shipping_phone"].ToString() + "'," +
                    "'shipping_country':'" + ds.Tables[0].Rows[i]["shipping_country"].ToString() + "','billing_country':'" + ds.Tables[0].Rows[i]["billing_country"].ToString() + "'," +
                    "'shipping_state':'" + ds.Tables[0].Rows[i]["shipping_state"].ToString() + "','billing_state':'" + ds.Tables[0].Rows[i]["billing_state"].ToString() + "','shipping_district':'" + ds.Tables[0].Rows[i]["shipping_district"].ToString() + "'," +
                    "'billing_district':'" + ds.Tables[0].Rows[i]["billing_district"].ToString() + "','code':'" + ds.Tables[0].Rows[i]["code"].ToString() + "','gstno':'" + ds.Tables[0].Rows[i]["gstno"].ToString() + "','customer_type':'" + ds.Tables[0].Rows[i]["customer_type"].ToString() + "'},";
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
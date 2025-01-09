using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.IdentityModel.Protocols.WSTrust;
using System.Drawing;

public partial class api_catlog_product_details : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    GetBaseURL objbaseurl = new GetBaseURL();
    string querry, id, ret_id = "", user_id = "", user_type = "";   
    string json, url;
	int index = 0, size = 50;
	protected void Page_Load(object sender, EventArgs e)
    {
        url = objbaseurl.Geturl();
        if (Request.Form["user_id"] != null && Request.Form["user_type"] != null && Request.Form["size"] != null && Request.Form["index"] != null)
        {
			user_id = Request.Form["user_id"].ToString();
			user_type = Request.Form["user_type"].ToString();
			index = int.Parse(Request.Form["index"]);
			size = int.Parse(Request.Form["size"]);
			View();
		}
	}


    public void View()
    {
        if (Request.Form["ret_id"] != null)
        {
			ret_id= Request.Form["ret_id"].ToString();

		}

			querry = @"
select c.*,r.name,r.code,r.address,r.location,r.state,r.district,r.mobile,'retailer' type from tbl_retailer_credit_limit c
inner join tbl_retailers r on c.retailer_id=r.id
where c.user_id=" + user_id+" and  c.user_type='"+user_type+"' ";
        if(ret_id!="")
        {
			querry += " and c.retailer_id="+ret_id;
		}

		querry += " order by id desc ";
		querry += " offset " + index * size + "  rows fetch next " + size + " rows only ";


		string querry1 = @"
select count(id) total from tbl_retailer_credit_limit c
where c.user_id=" + user_id + " and  c.user_type='" + user_type + "'";


		DataTable dt = cc.joinselect1(querry);
		DataSet dss = cc.joinselect(querry1);
		// Response.Write(querry);

		if (dt.Rows.Count > 0)
        {
            json += "{'status':true,'Message':'success','Count':'" + dss.Tables[0].Rows[0]["total"] + "','data':[";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                json += "{'id':'" + dt.Rows[i]["id"].ToString() + "'";
                json += ",'empid':'" + dt.Rows[i]["empid"].ToString() + "'";
                json += ",'retailer_id':'" + dt.Rows[i]["retailer_id"].ToString() + "'";
                json += ",'ret_type':'" + dt.Rows[i]["type"].ToString() + "'";
                json += ",'name':'" + dt.Rows[i]["name"].ToString() + "'";
                json += ",'code':'" + dt.Rows[i]["code"].ToString() + "'";
                json += ",'location':'" + dt.Rows[i]["location"].ToString() + "'";
                json += ",'mobile':'" + dt.Rows[i]["mobile"].ToString() + "'";
                json += ",'address':'" + dt.Rows[i]["address"].ToString() + "'";
                json += ",'district':'" + dt.Rows[i]["district"].ToString() + "'";
                json += ",'creditlimit_total':'" + dt.Rows[i]["creditlimit_total"].ToString() + "'";
                json += ",'creditlimit_vkc':'" + dt.Rows[i]["creditlimit_vkc"].ToString() + "'";
                json += ",'creditdays':'" + dt.Rows[i]["creditdays"].ToString() + "'";                
                json += ",'addedon':'" + dt.Rows[i]["addedon"].ToString() + "'";                
                json += "},";
            }
            json = json.Remove(json.Length - 1);
            json += "]}";
        }
        else
        {
            json = "{'status':false,'Message' :'No Items Exists'}";
        }
        dt.Dispose();
      

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
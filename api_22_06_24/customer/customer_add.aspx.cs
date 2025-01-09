using AjaxControlToolkit.HTMLEditor.ToolbarButton;
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
    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();

        insert();
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

    public class DataResponse
    {
        public string id;
        public string first_name;
        public string last_name;
        public string company_name;
        public string mobile;
        public string email;
        public string billing_address;
        public string billing_city;
        public string billing_state;
        public string billing_district;
        public string billing_country;
        public string billing_phone;
        public string billing_pin;

        public string shipping_address;
        public string shipping_city;
        public string shipping_state;
        public string shipping_district;
        public string shipping_country;
        public string shipping_phone;
        public string shipping_pin;

        public string gstno;
        public string code;
        public string customer_type;

        public string delete_status;
        public DateTime deleted_on;
        public string deleted_by;
        public DateTime added_on;
        public string type;
        public string added_by;
        //public string added_type;
        public string modified_by;
        public DateTime modified_on;
        public string modified_type;

    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        querry = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        string qry = "";
        int count = 0;
        string message = "";

        foreach (var data in DataResponse)
        {
            string querry1 = "";
            bool isValid = true;

            if (isValid)
            {
                if (data.type == "add")
                {
                    querry = @"select id from tbl_customer where mobile='" + data.mobile + "' or email='" + data.email + "'";
                    DataSet ds = cc.joinselect(querry);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        message = "Mobile number or email already exists.";
                        json = "{'status':false,'Message' :'" + message + "'}";
                        json = json.Replace("'", "\"");
                        Response.ContentType = "application/json";
                        Response.Write(json);
                        Response.End();
                    }
                    else
                    {
                        querry1 = @"insert into tbl_customer   
                    (first_name,last_name,company_name,mobile,email,billing_address,billing_city,billing_state,billing_district,billing_country,billing_phone,billing_pin,gstno,delete_status,added_on,added_by,modified_by,modified_on,modified_type,
                    shipping_address,shipping_city,shipping_state,shipping_country,shipping_district,shipping_phone,shipping_pin,code,customer_type) 
                    values('" + data.first_name + "','" + data.last_name + "','" + data.company_name + "','" + data.mobile + "','" + data.email + "','" + data.billing_address + "','" +
                        data.billing_city + "','" + data.billing_state + "','" + data.billing_district + "','" + data.billing_country + "','" + data.billing_phone + "','" + data.billing_pin + "','" +
                        data.gstno + "',0,getdate(),'" + data.added_by + "','" + data.added_by + "',getdate(),'" + data.added_by + "','" + data.shipping_address + "','" + data.shipping_city + "','" +
                        data.shipping_state + "','" + data.shipping_country + "','" + data.shipping_district + "','" + data.shipping_phone + "','" + data.shipping_pin + "','" + data.code + "','"+data.customer_type + "')";

                     
                      
                        message = "Data added successfully.";
                    }
                }
                else if (data.type == "edit")
                {
                  
                    querry1 = "UPDATE tbl_customer SET ";
                    if (!string.IsNullOrEmpty(data.first_name)) querry1 += "first_name='" + data.first_name + "',";
                    if (!string.IsNullOrEmpty(data.last_name)) querry1 += "last_name='" + data.last_name + "',";
                    if (!string.IsNullOrEmpty(data.company_name)) querry1 += "company_name='" + data.company_name + "',";
                    if (!string.IsNullOrEmpty(data.mobile)) querry1 += "mobile='" + data.mobile + "',";
                    if (!string.IsNullOrEmpty(data.email)) querry1 += "email='" + data.email + "',";
                    if (!string.IsNullOrEmpty(data.billing_address)) querry1 += "billing_address='" + data.billing_address + "',";
                    if (!string.IsNullOrEmpty(data.billing_city)) querry1 += "billing_city='" + data.billing_city + "',";

                    if (!string.IsNullOrEmpty(data.billing_state)) querry1 += "billing_state='" + data.billing_state + "',";
                    if (!string.IsNullOrEmpty(data.billing_district)) querry1 += "billing_district='" + data.billing_district + "',";
                    if (!string.IsNullOrEmpty(data.billing_country)) querry1 += "billing_country='" + data.billing_country + "',";
                    if (!string.IsNullOrEmpty(data.modified_by)) querry1 += "modified_by='" + data.modified_by + "',";
                    querry1 += "modified_on=GETDATE(),";
                    if (!string.IsNullOrEmpty(data.modified_type)) querry1 += "modified_type='" + data.modified_type + "',";
                    if (!string.IsNullOrEmpty(data.billing_phone)) querry1 += "billing_phone='" + data.billing_phone + "',";
                    if (!string.IsNullOrEmpty(data.billing_pin)) querry1 += "billing_pin='" + data.billing_pin + "',";
                    if (!string.IsNullOrEmpty(data.gstno)) querry1 += "gstno='" + data.gstno + "',";
                    if (!string.IsNullOrEmpty(data.added_by)) querry1 += "added_by='" + data.added_by + "',";
                    if (!string.IsNullOrEmpty(data.modified_by)) querry1 += "modified_by='" + data.modified_by + "',";
                    if (!string.IsNullOrEmpty(data.modified_type)) querry1 += "modified_type='" + data.modified_type + "',";
                    if (!string.IsNullOrEmpty(data.shipping_address)) querry1 += "shipping_address='" + data.shipping_address + "',";
                    if (!string.IsNullOrEmpty(data.shipping_city)) querry1 += "shipping_city='" + data.shipping_city + "',";
                    if (!string.IsNullOrEmpty(data.shipping_state)) querry1 += "shipping_state='" + data.shipping_state + "',";
                    if (!string.IsNullOrEmpty(data.shipping_country)) querry1 += "shipping_country='" + data.shipping_country + "',";
                    if (!string.IsNullOrEmpty(data.shipping_district)) querry1 += "shipping_district='" + data.shipping_district + "',";
                    if (!string.IsNullOrEmpty(data.shipping_phone)) querry1 += "shipping_phone='" + data.shipping_phone + "',";
                    if (!string.IsNullOrEmpty(data.shipping_pin)) querry1 += "shipping_pin='" + data.shipping_pin + "',";
                    if (!string.IsNullOrEmpty(data.code)) querry1 += "code='" + data.code + "',";
                    if (!string.IsNullOrEmpty(data.customer_type)) querry1 += "customer_type='" + data.customer_type + "',";


                    querry1 = querry1.TrimEnd(',');

                    querry1 += " WHERE id=" + data.id;

                  
                  
                    message = "Data updated successfully.";
                }
                else if (data.type == "delete")
                {
                    querry1 = @"update  tbl_customer  set delete_status=1 where id=" + data.id + " ";
                    message = "Data deleted successfully.";
                }

                int status = cc.Insert(querry1);
                if (status > 0)
                {
                    querry1 = @"select max(id) as id from  tbl_customer";
                    DataSet ds1 = cc.joinselect(querry1);

                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        json = "{'status':true,'Message' :'Success.','data':[";

                        for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                        {
                            json += "{'id':'" + ds1.Tables[0].Rows[i]["id"].ToString() + "'}";
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
                        json = "{'status':false,'Message' :'Oops! Something went wrong'}";
                    }
                }
                else
                {
                    json = "{'status':false,'Message' :'Oops! Something went wrong'}";
                }
            }
            else
            {
                json = "{'status':false,'Message' :'" + message + "'}";
            }
        }

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

}
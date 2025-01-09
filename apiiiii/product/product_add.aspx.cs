using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
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
            // Continue processing
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
        public string productname;
        public int producttype_id;
        public string material_id;
        public string gsm_id;
        public string size;
        public string color;
        public string hsn_sac;
        public string product_market;
        public string pattern_id;
        public string type;
        public string id;
        public string delete_status;
        public string empid;
        public string ip_address;

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

            if (data.type == "add")
            {
                querry = @"select id from tbl_product where product_name='" + data.productname + "'  ";
                DataSet ds = cc.joinselect(querry);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    message = "Product already exists.";
                    json = "{'status':false,'Message' :'" + message + "'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    ds.Dispose();
                    Response.End();
                }
                else
                {
                    querry1 = @"insert into tbl_product (product_name,producttype_id,material_id,gsm_id,size,color,hsn_sac,product_market,delete_status,pattern_id) values('" + data.productname + "','" + data.producttype_id + "','" + data.material_id + "','" + data.gsm_id + "','" + data.size + "','" + data.color + "','" + data.hsn_sac + "','" + data.product_market + "',0,'" + data.pattern_id + "')";
                    message = "Data added successfully.";
                    ds.Dispose();

                }
            }
            else if (data.type == "edit")
            {
                querry1 = @"update tbl_product set product_name='" + data.productname + "',producttype_id='" + data.producttype_id + "',material_id='" + data.material_id + "',gsm_id='" + data.gsm_id + "',size='" + data.size + "',color='" + data.color + "',hsn_sac='" + data.hsn_sac + "',product_market='" + data.product_market + "',pattern_id='" + data.pattern_id + "' where id=" + data.id + " ";
                message = "Data updated successfully.";
            }
            else if (data.type == "delete")
            {
                querry1 = @"update tbl_product set delete_status=1,deleted_on = GETDATE(),deleted_by = '"+ data.empid + "',ip_address='" + data.ip_address+"' where id=" + data.id + " ";
                message = "Data deleted successfully.";
            }

            int status = cc.Insert(querry1);
            if (status > 0)
            {
                if (data.type == "add")
                {
                    String querry2 = @"select max(id) as id from tbl_product";
                    DataSet ds1 = cc.joinselect(querry2);

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
                        ds1.Dispose();  
                        Response.End();
                    }
                }
                else if (data.type == "edit")
                {
                    json = "{'status':true,'Message' :'Data updated successfully.'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);

                    Response.End();
                }
                else if (data.type == "delete")
                {
                    json = "{'status':true,'Message' :'Data deleted successfully.'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
            }
            else
            {
                json = "{'status':false,'Message' :'Oops! Something went wrong'}";
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
        }
    }
}

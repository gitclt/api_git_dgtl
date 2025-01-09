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
        public string productname;
        public int producttype_id;
        public string material;
        public string gsm;
        public string size;
        public string color;
        public string item_code;
        public string hsn_sac;   
        public string rate;    
        public string product_category;    
        public string type;
        public string id;
        public string delete_status;


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
                    querry = @"select id from tbl_product where product_name='" + data.productname + "'  ";
                    DataSet ds = cc.joinselect(querry);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        message = "Product  already exists.";
                        json = "{'status':false,'Message' :'" + message + "'}";
                        json = json.Replace("'", "\"");
                        Response.ContentType = "application/json";
                        Response.Write(json);
                        Response.End();
                    }
                    else

                    {
                        querry1 = @"insert into tbl_product (product_name,producttype_id,material,gsm,size,color,item_code,hsn_sac,rate,delete_status,product_category) values('" + data.productname + "','" + data.producttype_id + "','" + data.material + "','" + data.gsm + "','" + data.size + "','" + data.color + "','" + data.item_code + "','" + data.hsn_sac + "','" + data.rate + "',0,'"+data.product_category + "')";
                       // querry1 = @"INSERT INTO tbl_product (product_name, output_perhr, producttype_id, material, gsm, size, color)
         

                        message = "Data added successfully.";

                    }
                }
                else if (data.type == "edit")
                {
                    querry1 = @"update tbl_product set product_name='" + data.productname + "',producttype_id='" + data.producttype_id + "',material='" + data.material + "',gsm='" + data.gsm + "',size='" + data.size + "',color='" + data.color + "',item_code='" + data.item_code + "',hsn_sac='" + data.hsn_sac + "',rate='" + data.rate + "',product_category='"+data.product_category + "' where id=" + data.id + " ";
                    message = "Data updated successfully.";

                }
                else if (data.type == "delete")
                {
                    //querry1 = @"delete from tbl_product  where id=" + data.id + " ";
                    querry1 = @"update tbl_product set delete_status=1 where id=" + data.id + " ";

                    message = "Data deleted successfully.";

                }
                int status = cc.Insert(querry1);
                if (status > 0)
                {

                    String querry2 = @"select max(id) as id from  tbl_product";
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


                        // json = "{'status':true,'Message' :'Data updated successfully.'}";

                        json = json.Replace("'", "\"");
                        Response.ContentType = "application/json";
                        Response.Write(json);
                        Response.End();
                    }




                    else
                        json = "{'status':false,'Message' :'Oops! Something went wrong'}";
                }
                else
                    json = "{'status':false,'Message' :'Oops! Something went wrong'}";
            }
        }

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}


using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
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
    string assign_id = "",status="";
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
        public string assign_id;
        public string production_packing_status; //packingonly /productiononly/productionandpacking

    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        List<DataResponse> dataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string json = "";
        string message = "";

        foreach (var data in dataResponse)
        {


            string packing_status = "null";
            string production_status = "null";
            if (data.production_packing_status.ToLower() == "packingonly")
            {
                packing_status = "'not started'";
            }
            else if (data.production_packing_status.ToLower() == "productiononly")
            {
                production_status = "'not started'";
            }
            else
            {
                packing_status = "'not started'";
                production_status = "'not started'";

            }
            string query = @"UPDATE tbl_order_assigned  set production_packing_status = '" + data.production_packing_status +"',production_status="+ production_status + ",packing_status="+ packing_status + " where id='"+data.assign_id+"'";
           


                int status = cc.Insert(query);

                if (status > 0)
                {
                    message = "Data updated successfully.";
                    json = "{'status':true,'Message' :'" + message + "'}";
                }
                else
                {
                    message = "Data updation  failed.";
                    json = "{'status':false,'Message' :'" + message + "'}";
                }
           

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}
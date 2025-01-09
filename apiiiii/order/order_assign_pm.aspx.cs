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
    static string querry, order_id,pm_id;
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
        public string pm_id;
        public string order_id;
        public string pm_remark;
        public DateTime pm_delivery;

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
            if (!string.IsNullOrEmpty(data.order_id))
            {
                string query = @"UPDATE tbl_orders 
                             SET pm_id = '" + data.pm_id + "',  pm_remark = '" + data.pm_remark + "', pm_delivery = '" + data.pm_delivery.ToString("yyyy-MM-dd HH:mm:ss") + "',pm_addedon = GETDATE() where id = " + data.order_id;
    
            int status = cc.Insert(query);

                if (status > 0)
                {
                    message = "Data updated successfully.";
                    json = "{'status':true,'Message' :'" + message + "'}";
                }
                else
                {
                    message = "Data update failed.";
                    json = "{'status':false,'Message' :'" + message + "'}";
                }
            }
            else
            {
                message = "order_id is required.";
                json = "{'status':false,'Message' :'" + message + "'}";
            }

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }

}
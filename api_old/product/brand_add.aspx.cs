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
        public string name;
        public string logo;
        public string sort_no;
        public string short_name;
        public string delete_status;
       public string type;   
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
           // bool isValid = ;

                if (data.type == "add")
                {


                    querry = @"select id from tbl_brand where name='" + data.name + "'";
                    DataSet ds = cc.joinselect(querry);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        message = "name already exists.";
                        json = "{'status':false,'Message' :'" + message + "'}";
                        json = json.Replace("'", "\"");
                        Response.ContentType = "application/json";
                        Response.Write(json);
                        Response.End();
                    }
                    else
                    {

                        querry1 = @"insert into tbl_brand   
            (name,logo,sort_no,short_name,delete_status) 
            values('" + data.name + "','" + data.logo + "','" + data.sort_no + "','" + data.short_name + "',0)";

                        message = "Data added successfully.";
                    }
                    //Response.Write(querry1);
                    //return;
                }
                else if (data.type == "edit")
                {
                    querry1 = @"update tbl_brand set name='" + data.name + "',logo='" + data.logo + "',sort_no='" + data.sort_no + "',short_name='" + data.short_name + "'";
                                message = "Data updated successfully.";
                }

                else if (data.type == "delete")
                {
                    querry1 = @"update  tbl_brand  set delete_status=1 where id=" + data.id + " ";
                    message = "Data deleted successfully.";
                }

                int status = cc.Insert(querry1);
                if (status > 0)
                {
                    json = "{'status':true,'Message' :'" + message + "'}";
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

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

}
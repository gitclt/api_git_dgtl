using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
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
        public string name;
        public string country_code;
        public string short_code;
        public string img;
        public string type;     
        public string id;     
    }

    public void insert()
    {



        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        querry = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
       // List<DataResponse> updatedData = new List<DataResponse>();

        string qry = "";
        int count = 0;
        string message = "";

        foreach (var data in DataResponse)
        {
            string querry1 = "";

            if (data.type == "add")
            {
                querry1 = @"insert into tbl_country (name,country_code,short_code,img,delete_status) values('" + data.name + "','"+data.country_code+"','"+data.short_code+"','"+data.img+"',0)";
                message = "Data added successfully.";
            }
            else if (data.type == "edit")
            {
                querry1 = @"update tbl_country set name='" + data.name + "',country_code='" + data.country_code + "',short_code='" + data.short_code + "',img='" + data.img + "' where id=" + data.id + " ";
                message = "Data updated successfully.";
               // updatedData.Add(new DataResponse { name = data.name, id = data.id });
            }
            else if (data.type == "delete")
            {
                querry1 = @"update tbl_country set delete_status=1 where id=" + data.id + " ";

                message = "Data deleted successfully.";

            }
            int status = cc.Insert(querry1);
            if (status > 0)
            {
                json = "{'status':true,'Message' :'" + message + "','data':[";
                foreach (var item in DataResponse)
                {
                    //json += "{" + item.name + "," + item.id + "},";
                    json += "{'name':'" + item.name + "','country_code':'" + item.country_code + "','short_code':'" + item.short_code + "','img':'"+item.img+"'},";

                }
                json = json.TrimEnd(',') + "]}";

                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
            else
            {
                json = "{'status':false,'Message' :'Failed'}";
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
        }

        json = "{'status':false,'Message' :'" + message + "','data':" + JsonConvert.SerializeObject(DataResponse) + "}";
        
        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);

        Response.End();

      
    }
}
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
//using System.ServiceModel.Channels;
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
        public string hierarchy_id;  
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

        string qry = "";
        int count = 0;
        string message = "";


        foreach (var data in DataResponse)
        {
            string querry1 = "";
            if (data.type == "add")
            {
                
                querry1 = @"insert into tbl_role (name,delete_status,hierarchy_id) values('" + data.name + "',0,'"+data.hierarchy_id+"')";
                message = "Data added successfully.";
              
            }
            else if (data.type == "edit")
            {
                querry1 = @"update tbl_role set name='" + data.name + "',hierarchy_id='" + data.hierarchy_id + "' where id=" + data.id + " ";
                message = "Data updated successfully.";


            }
            else if (data.type == "delete")
            {
                querry1 = @"update tbl_role set delete_status=1 where id=" + data.id + " ";

               // querry1 = @"delete from tbl_designation  where id=" + data.id + " ";
                message = "Data deleted successfully.";


            }

            int status = cc.Insert(querry1);
            // int id = cc.Insert(querry1);  // Assuming Insert method returns the ID of the newly inserted record

            if (status > 0)
            {

                String querry2 = @"select max(id) as id from  tbl_role";
                DataSet ds1 = cc.joinselect(querry2);

                if (ds1.Tables[0].Rows.Count > 0)
                {

                    json = "{'status':true,'Message' :'Success.','data':[";

                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        json += "{'id':'" + ds1.Tables[0].Rows[i]["id"].ToString() +"'}";
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
                {

                }
                ds1.Dispose();

            }
            else
                json = "{'status':false,'Message' :'Oops! Something went wrong'}";
        }

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
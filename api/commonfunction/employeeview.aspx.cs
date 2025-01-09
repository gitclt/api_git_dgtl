using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Newtonsoft.Json;
using System.IO;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;

    protected void Page_Load(object sender, EventArgs e)
    {
      //  chk_tocken();
        select();

       
    }

    public class DataResponse
    {
       
        public string type;
       

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

    public void select()
    {

       // var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
       // bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
       // var bodyText = bodyStream.ReadToEnd();

       // List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
       // string json = "";


        //foreach (var data in DataResponse)
        //{
            if (Request.Form["type"] == "employee_list")
            {
                querry = @"
select e.*,a.name as hierarchy,d.name as designation_name from tbl_employee e 
inner join tbl_designation d on  e.designation_id = d.id  
inner join tbl_user u on  e.id = u.emp_id 
inner join tbl_hierarchy a on  a.id = u.hierarchy_id 
order by e.name
            ";

                DataSet ds = cc.joinselect(querry);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':true,'Message' :'success' ,'data':[";

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        json += "{'id':'" + ds.Tables[0].Rows[i]["id"] + "','name':'" + ds.Tables[0].Rows[i]["name"] + "','email':'" + ds.Tables[0].Rows[i]["email"] + "','phone':'" + ds.Tables[0].Rows[i]["phone"] + "','designation_name':'" + ds.Tables[0].Rows[i]["designation_name"] + "','join_date':'" + ds.Tables[0].Rows[i]["join_date"] + "','designation_id':'" + ds.Tables[0].Rows[i]["designation_id"] + "','hierarchy':'" + ds.Tables[0].Rows[i]["hierarchy"] + "'},";
                    }
                    json = json.TrimEnd(',');
                    json += "]}";
                }
                else
                    json = "{'status':false,'Message' :'Failed'}";


                json = json.Replace("'", "\"");


                Response.ContentType = "application/json";

                Response.Write(json);

                Response.End();
            }

            else if (Request.Form["type"] == "employee_count")
            {
                querry = @"
select count(id) as count  from tbl_user where type='employee' 
            ";

                DataSet ds = cc.joinselect(querry);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':true,'Message' :'success' ,'data':[";

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        json += "{'count':'" + ds.Tables[0].Rows[i]["count"] + "'},";
                    }
                    json = json.TrimEnd(',');
                    json += "]}";
                }
                else
                    json = "{'status':false,'Message' :'Failed'}";


                json = json.Replace("'", "\"");


                Response.ContentType = "application/json";

                Response.Write(json);

                Response.End();
            }
    }
}
   

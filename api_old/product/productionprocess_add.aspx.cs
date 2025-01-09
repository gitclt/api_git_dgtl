using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

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
        }
        else if (id == "")
        {
            json = "{'status':false,'Message' :'Oops! Unauthorised Access!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.StatusCode = 403;
            Response.Write(json);
            Response.End();
        }
    }

    public class DataResponse
    {
        public int pro_id { get; set; }
        public ProductProductionProcess product_production_process { get; set; }
    }

    public class ProductProductionProcess
    {
        public string product_production_process_id { get; set; }
        public List<Grade> grade { get; set; }
    }

    public class Grade
    {
        public int grade_id { get; set; }
        public float output_per_sec { get; set; }
    }

    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        DataResponse data = JsonConvert.DeserializeObject<DataResponse>(bodyText);

        if (data == null)
        {
            json = "{'status':false,'Message' :'No data received'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
            return;
        }

        bool allSuccessful = true;

        if (data.product_production_process != null && data.product_production_process.grade != null)
        {
            foreach (var grade in data.product_production_process.grade)
            {
                string querry1 = @"INSERT INTO tbl_product_production_process_grade (pro_id, production_process_id, output_per_sec, grade_id) 
                                   VALUES('" + data.pro_id + "', '" + data.product_production_process.product_production_process_id + "', '" + grade.output_per_sec + "', '" + grade.grade_id + "')";

                int status = cc.Insert(querry1);
                if (status <= 0)
                {
                    allSuccessful = false;
                    break;
                }
            }
        }

        if (allSuccessful)
        {
            json = "{'status':true,'Message':'Data added successfully.'}";
        }
        else
        {
            json = "{'status':false,'Message':'Failed to insert some entries.'}";
        }

        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}

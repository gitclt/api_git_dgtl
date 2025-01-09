﻿using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
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
        public string product_id;
        public string production_process_id;
        public string gradeA_output_per_second;
        public string gradeB_output_per_second;
        public string gradeC_output_per_second;
        public string gradeD_output_per_second;
        public string id;
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
       


        foreach (var data in DataResponse)
        {
            string querry1 = "";
            string message = "";

            if (data.type == "add")
            {
                querry1 = @"insert into tbl_product_production_process (product_id,production_process_id,gradeA_output_per_second,gradeB_output_per_second,gradeC_output_per_second,gradeD_output_per_second) values('" + data.product_id + "','" + data.production_process_id + "','" + data.gradeA_output_per_second + "','"+data.gradeB_output_per_second + "','"+ data.gradeC_output_per_second + "','"+ data.gradeD_output_per_second + "')";
                message = "Data added successfully.";

            }
            else if (data.type == "edit")
            {
                querry1 = @"update tbl_product_production_process set product_id='" + data.product_id + "', production_process_id='" + data.production_process_id + "',gradeA_output_per_second='" + data.gradeA_output_per_second + "',gradeB_output_per_second='" + data.gradeB_output_per_second + "',gradeC_output_per_second='" + data.gradeC_output_per_second + "',gradeD_output_per_second='" + data.gradeD_output_per_second + "' where id=" + data.id + " ";
                message = "Data updated successfully.";

            }
            else if (data.type == "delete")
            {
                querry1 = @"delete from tbl_product_production_process  where id=" + data.id + " ";
                message = "Data deleted successfully.";

            }
            int status = cc.Insert(querry1);
            if (status > 0)
            {
                //json = "{'status':true,'Message' :'Success.'}";
                json = "{'status':true,'Message' :'" + message + "'}";

                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
            else
                json = "{'status':false,'Message' :'Failed'}";
        }
        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
//    }
//}

﻿using Newtonsoft.Json;
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
       // chk_tocken();
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
        public string task_id;
        public string file;

        // public string project_id;
        //public string type;
        public string id;
        public string deleted_by;



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
       

        foreach (var data in DataResponse)
        {
            string querry1 = "";
            string message = "";

            //if (data.type == "add")
            //{
                querry1 = @"insert into tbl_documents (task_id,delete_status,[file]) values('"+data.task_id +"',0,'"+data.file+"')";
                message = "Data added successfully.";

            //}
            //else if (data.type == "edit")
            //{
            //    querry1 = @"update tbl_documents set task_id='"+data.task_id + "'[file] = '" + data.file + "' where id=" + data.id + " ";
            //    message = "Data updated successfully.";

            //}
            //else if (data.type == "delete")
            //{
            //    querry1 = @"update tbl_documents set delete_status=1,deleted_by='"+data.deleted_by+"',deleted_on=getdate() where id=" + data.id + " ";
            //    message = "Data deleted successfully.";

            //}
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
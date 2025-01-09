using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
//using System.IdentityModel.Protocols.WSTrust;
using System.IO;
//using System.Linq;
using System.Web;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    CommFuncs commFuncs = new CommFuncs();
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
        public string expected_qty;
        public string workhour;
        public string remark;
        public string qty;

        public string addedby;
        public string order_assign_id;
        public string datetime;
        public string task_start_end_flag;       
        public string no_of_emp;
        public List<items> items;
        public List<activities> activities;
    }
    public class items
    {
        public string task_id;
        public string emp_id;
    }
    public class activities
    {
        public string task_id;
        public string activity;
    }

    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();
        querry = "";
        List<DataResponse> dataResponses = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string message = "";
        string querry1 = "";
        foreach (var data in dataResponses)
        {
            if (data.task_start_end_flag.ToLower() == "start")
            {
                querry1 = @"INSERT INTO tbl_order_task (addedby, order_assign_id,fromdatetime, addedon,no_of_emp) " +
                          "VALUES('" + data.addedby + "','" + data.order_assign_id + "','" + data.datetime + "', GETDATE(),'" + data.no_of_emp + "')";
            }
            else
            {
                querry1 = @"update tbl_order_task set expected_qty='" + data.expected_qty + "',workhour='"+data.workhour+"',remark='"+data.remark+"',todatetime='"+data.datetime+"',qty='"+data.qty+"' where id='"+data.task_id+"'";
            }
            int status = cc.Insert(querry1);
            if (status > 0)
            {


                string stat = commFuncs.update_order_job_status(data.order_assign_id);

                if (data.task_start_end_flag.ToLower() == "start")
                {
                    querry1 = @"SELECT TOP 1 id
FROM tbl_order_task
WHERE addedby='" + data.addedby + "' order by id desc";

                    DataSet dss = cc.joinselect(querry1);
                    if (dss.Tables[0].Rows.Count > 0)
                    {
                        int task_id = Convert.ToInt32(dss.Tables[0].Rows[0]["id"]);
                        string querry2 = "";
                        foreach (var item in data.items)
                        {
                            querry2 += @"insert into tbl_order_task_emp 
(task_id,emp_id) 
   values('" + task_id + "','" + item.emp_id + "')";
                        }
                        int status1 = cc.Insert(querry2);

                        //activity
                        foreach (var activity in data.activities)
                        {
                          string querry3 = @"insert into tbl_order_task_activity 
(task_id,activity) 
   values('" + task_id + "','" + activity.activity + "')";
                            int status2 = cc.Insert(querry3);

                        }


                        if (status1 > 0)
                        {
                            json = "{'status':true,'Message' :'Data added successfully.','task_id':'"+task_id+"'}";
                            json = json.Replace("'", "\"");
                            Response.ContentType = "application/json";
                            Response.Write(json);
                            Response.End();
                        }
                        else
                            json = "{'status':false,'Message' :'Oops! Something went wrong'}";

                        json = json.Replace("'", "\"");
                        Response.ContentType = "application/json";
                        Response.Write(json);
                        Response.End();
                    }
                }
                else
                {
                    json = "{'status':true,'Message' :'Updated successfully.'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
            }
        }
        json = "{'status':true,'Message' :' Oops! Something went wrong'}";
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();



    }
}

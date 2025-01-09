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
        insert();
    }

    public class DataResponse
    {
        public string addedby;
        public string order_assign_id;
        public string qty;
        public string fromdatetime;
        public string todatetime;
        public string workhour;
        public string remark;
        public string type;
        public string id;
        public string no_of_emp;
        public List<items> items;

    }
    public class items
    {
        public string task_id;
        public string emp_id;

    }

    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();
        querry = "";
        List<DataResponse> dataResponses = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string message = "";

        foreach (var data in dataResponses)
        {
            string querry1 = "";
            if (data.type == "add")
            {
                // string s = @"select count(emp_id) from tbl_order_task_emp where task_id='"++"'";
                int totalQuantity = commFuncs.get_order_assigned_totalQuantity(data.order_assign_id);
                int assignedQuantity = commFuncs.get_order_taskQuantity(data.order_assign_id);

                if (totalQuantity >= assignedQuantity)
                {

                    querry1 = @"INSERT INTO tbl_order_task (addedby, order_assign_id, qty, fromdatetime, todatetime, workhour, remark, addedon,no_of_emp) " +
                              "VALUES('" + data.addedby + "','" + data.order_assign_id + "','" + data.qty + "','" + data.fromdatetime + "','" + data.todatetime + "','" + data.workhour + "','" + data.remark + "', GETDATE(),'" + data.no_of_emp + "')";
                    //  message = "Data added successfully.";


                    int status = cc.Insert(querry1);

                    if (status > 0)
                    {
                        querry1 = @"SELECT TOP 1 id
FROM tbl_order_task
WHERE addedby='" + data.addedby + "' order by id desc";

                        DataSet dss = cc.joinselect(querry1);
                        if (dss.Tables[0].Rows.Count > 0)
                        {
                            int task_id = Convert.ToInt32(dss.Tables[0].Rows[0]["id"]);
                            // int emp_id = Convert.ToInt32(dss.Tables[0].Rows[0]["emp_id"]);

                            //Response.Write(emp_id);
                            //return;
                            string querry2 = "";
                            foreach (var item in data.items)
                            {
                                querry2 += @"insert into tbl_order_task_emp 
(task_id,emp_id) 
   values('" + task_id + "','" + item.emp_id + "')";
                            }
                            int status1 = cc.Insert(querry2);




                            if (status1 > 0)
                            {
                                //json = "{'status':true,'Message' :'Data updated successfully.'}";
                                string stat = commFuncs.update_order_job_status(data.order_assign_id);

                              

                                if (stat == "success")
                                {
                                    message = "Data added successfully.";
                                    json = "{'status':true,'Message' :'" + message + "'}";
                                    json = json.Replace("'", "\"");
                                    Response.ContentType = "application/json";
                                    Response.Write(json);
                                    Response.End();
                                    return;

                                }
                                else
                                {
                                    json = "{'status':false,'Message' :'Failed to update job status'}";
                                    Response.ContentType = "application/json";
                                    Response.Write(json);
                                    Response.End();
                                    return;
                                }
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
                       // message = "Oops! Something went wrong.";
                        // Handle other types if needed
                    }

                    json = "{'status':true,'Message' :' Oops! Something went wrong'}";
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
                else
                {
                    string json = "{\"status\":false,\"Message\":\"Insufficient Quantity\"}";
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                    return;

                }



            }
        }



    }
}

using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
    string job_type = "", job_status = "";
    string id = "", task_id = "", item_id="";

    protected void Page_Load(object sender, EventArgs e)
    {
        //chk_tocken();
        jobstatus();
    }

    public void chk_tocken()
    {
        CommFuncs CommFuncs = new CommFuncs();

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

    public void jobstatus()
    {
        //if (Request.Form["job_status"] != null)
        //{
        //    job_status = Request.Form["job_status"];
        //}
        if (Request.Form["task_id"] != null)
        {
            task_id = Request.Form["task_id"];
        }
        if (Request.Form["item_id"] != null)
        {
            item_id = Request.Form["item_id"];
        }
        if (Request.Form["job_type"] != null)
        {
            job_type = Request.Form["job_type"];
        }
        
        string querry1 = @"select t.id, t.istakenfromstock, t.todatetime, t.fromdatetime, t.order_assign_id, sum(t.qty) as qty, o.pm_addedon
                       from tbl_order_task t  
                       inner join tbl_order_assigned a on a.id = t.order_assign_id
                       inner join tbl_order_items i on i.id = a.item_id
                       inner join tbl_orders o on o.id = i.order_id
                       where t.id = '" + task_id + "'  group by t.id, t.istakenfromstock, t.todatetime, t.fromdatetime, t.order_assign_id, o.pm_addedon";
    
    DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'pm_addedon':'" + ds.Tables[0].Rows[i]["pm_addedon"].ToString() + "','istakenfromstock':'" + ds.Tables[0].Rows[i]["istakenfromstock"].ToString() + "','task_id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','fromdatetime':'" + ds.Tables[0].Rows[i]["fromdatetime"].ToString() + "','todatetime':'" + ds.Tables[0].Rows[i]["todatetime"].ToString() + "','qty':'" + ds.Tables[0].Rows[i]["qty"].ToString() + "'";
                json += ",'activity':[";
                string query2 = @"select t.id, ta.activity, pp.process_name, pp.process_type, ta.pro_id as pro_id
                              from tbl_order_task t 
                              inner join tbl_order_task_activity ta on t.id = ta.task_id 
                              --inner join tbl_product_production_process_grade pg on ta.activity = pg.id
inner join tbl_production_process pp on ta.activity = pp.id
                              where ta.task_id = '" + ds.Tables[0].Rows[i]["id"].ToString() + "'";
              
                DataSet dss = cc.joinselect(query2);
                if (dss.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dss.Tables[0].Rows.Count; j++)
                    {
                        json += "{'product_process_grade_id':'" + dss.Tables[0].Rows[j]["activity"].ToString() + "','process_name':'" + dss.Tables[0].Rows[j]["process_name"].ToString() + "','process_type':'" + dss.Tables[0].Rows[j]["process_type"].ToString() + "'},";
                    }
                    json = json.TrimEnd(',');
                }
                json += "]";
                json += ",'employees':[";

                string query3 = @"select t.id, te.emp_id, te.task_id, e.grade, e.name, e.unit_id, e.designation_id, e.emp_code, e.email, e.mobile
                              from tbl_order_task t 
                              inner join tbl_order_task_emp te on t.id = te.task_id
                              inner join tbl_employees e on e.id = te.emp_id 
                              where te.task_id = '" + ds.Tables[0].Rows[i]["id"].ToString() + "'";

                DataSet ds1 = cc.joinselect(query3);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < ds1.Tables[0].Rows.Count; j++)
                    {
                        json += "{'id':'" + ds1.Tables[0].Rows[j]["emp_id"].ToString() + "','grade':'" + ds1.Tables[0].Rows[j]["grade"].ToString() + "','name':'" + ds1.Tables[0].Rows[j]["name"].ToString() + "','unit':'" + ds1.Tables[0].Rows[j]["unit_id"].ToString() + "','designation':'" + ds1.Tables[0].Rows[j]["designation_id"].ToString() + "','empCode':'" + ds1.Tables[0].Rows[j]["emp_code"].ToString() + "','email':'" + ds1.Tables[0].Rows[j]["email"].ToString() + "','mobile':'" + ds1.Tables[0].Rows[j]["mobile"].ToString() + "'},";
                    }
                    json = json.TrimEnd(',');
                }
                json += "]";

                //ongoing list
                json += ",'task':[";

                string query4 = @"select t.id, t.expected_qty, t.qty, t.todatetime, t.fromdatetime, ta.activity, t.no_of_emp, oi.production_status, oi.packing_status, oa.item_id
                              from tbl_order_task t
                              inner join tbl_order_task_activity ta on ta.task_id = t.id
                              inner join tbl_order_assigned oa on t.order_assign_id = oa.id
                              inner join tbl_order_items oi on oa.item_id = oi.id
                              where t.id = '" + task_id + "' ";
             
                DataSet ds11 = cc.joinselect(query4);
                if (ds11.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds11.Tables[0].Rows.Count; k++)
                    {
                        json += "{'id':'" + ds11.Tables[0].Rows[k]["id"].ToString() + "','fromdatetime':'" + ds11.Tables[0].Rows[k]["fromdatetime"].ToString() + "','todatetime':'" + ds11.Tables[0].Rows[k]["todatetime"].ToString() + "','expected_qty':'" + ds11.Tables[0].Rows[k]["expected_qty"].ToString() + "','qty':'" + ds11.Tables[0].Rows[k]["qty"].ToString() + "','teamsize':'" + ds11.Tables[0].Rows[k]["no_of_emp"].ToString() + "'},";
                    }
                    json = json.TrimEnd(',');
                }
                json += "]";
                ds11.Dispose();

                //orderassign
                json += ",'logtime':[";

                string query5 = @"select t.id, t.expected_qty, t.qty, t.todatetime, t.fromdatetime, ta.activity, t.no_of_emp, oi.production_status, oi.packing_status, t.order_assign_id
                              from tbl_order_task t
                              inner join tbl_order_task_activity ta on ta.task_id = t.id
                              inner join tbl_order_assigned oa on t.order_assign_id = oa.id
                              inner join tbl_order_items oi on oa.item_id = oi.id
                              where oa.item_id = '" + item_id + "'and job_type='"+job_type+"'";
               
                DataSet dst = cc.joinselect(query5);
                if (dst.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < dst.Tables[0].Rows.Count; k++)
                    {
                        json += "{'id':'" + dst.Tables[0].Rows[k]["id"].ToString() + "','fromdatetime':'" + dst.Tables[0].Rows[k]["fromdatetime"].ToString() + "','todatetime':'" + dst.Tables[0].Rows[k]["todatetime"].ToString() + "','expected_qty':'" + dst.Tables[0].Rows[k]["expected_qty"].ToString() + "','qty':'" + dst.Tables[0].Rows[k]["qty"].ToString() + "','teamsize':'" + dst.Tables[0].Rows[k]["no_of_emp"].ToString() + "','order_assign_id':'" + dst.Tables[0].Rows[k]["order_assign_id"].ToString() + "'},";
                    }
                    json = json.TrimEnd(',');
                }
                json += "]";
                dst.Dispose();

                //production_processes
                json += ",'production_processes':[";
                string a = @"select p.process_type, p.process_name, g.pro_id, g.production_process_id, g.grade_id, g.output_per_sec
                         from tbl_production_process p
                         inner join tbl_product_production_process_grade g on p.id = g.production_process_id
                         inner join tbl_order_task_activity a on g.pro_id = a.pro_id and a.task_id = '" + task_id + "' where p.delete_status = 0 ";

               
                DataSet dsw = cc.joinselect(a);
                if (dsw.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dsw.Tables[0].Rows.Count; j++)
                    {
                        json += "{'process_type':'" + dsw.Tables[0].Rows[j]["process_type"].ToString() + "','process_name':'" + dsw.Tables[0].Rows[j]["process_name"].ToString() + "','production_process_id':'" + dsw.Tables[0].Rows[j]["production_process_id"].ToString() + "','grade_id':'" + dsw.Tables[0].Rows[j]["grade_id"].ToString() + "','output_per_sec':'" + dsw.Tables[0].Rows[j]["output_per_sec"].ToString() + "'},";
                    }
                    json = json.TrimEnd(',');
                }
                dsw.Dispose();
                json += "]}";
            }
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
        {
            json = "{'status':false,'Message' :'Success.','data':[]}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}

using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    static string emp_id;
    static string type;



    protected void Page_Load(object sender, EventArgs e)
    {
      // chk_tocken();

        getdata();
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

    public void getdata()
    {
               string querry1 = @"
        
select distinct t.taskname, t.duedate,t.status,t.id,pm.name as projectname
        from tbl_task t 
        inner join tbl_task_emp te on te.task_id=t.id
inner join tbl_projectmaster pm  on t.pro_id=pm.id where pm.delete_status=0";

        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message':'Success.','data':[";

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "',";
                json += "'taskname':'" + ds.Tables[0].Rows[i]["taskname"].ToString() + "',";
                json += "'duedate':'" + ds.Tables[0].Rows[i]["duedate"].ToString() + "',";
                json += "'status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "',";
                json += "'projectname':'" + ds.Tables[0].Rows[i]["projectname"].ToString() + "',";


                // Employees
                json += "'employees':[";

                string query2 = @"
                select distinct te.emp_id, te.task_id, e.name 
                from tbl_task_emp te 
                inner join tbl_employee e on te.emp_id=e.id 
                where te.task_id='" + ds.Tables[0].Rows[i]["id"].ToString() + "' and e.delete_status=0";

                DataSet dss = cc.joinselect(query2);
               

                if (dss.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dss.Tables[0].Rows.Count; j++)
                    {
                        json += "{'emp_id':'" + dss.Tables[0].Rows[j]["emp_id"].ToString() + "',";
                        json += "'name':'" + dss.Tables[0].Rows[j]["name"].ToString() + "'},";
                    }
                    json = json.TrimEnd(','); 
                }
                json += "]"; 

                json += "},";
                dss.Dispose();
            }

            json = json.TrimEnd(','); 
            json += "]}";
            ds.Dispose();

            json = json.Replace("'", "\"");

            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
        {
            json = "{'status':false,'Message':'No data found!'}";
            json = json.Replace("'", "\"");
            ds.Dispose();

            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}

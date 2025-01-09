using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
    string month = "";
    string emp_id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
       //chk_tocken();
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

        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }
        string querry1 = @"select l.emp_id,c.calendar_date,l.status
from tbl_leave_request l 
inner join tbl_calendar c on c.calendar_date BETWEEN l.from_date AND l.to_date
where l.emp_id='"+ emp_id + "' and l.status='approved'";

        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'calendar_date':'" + ds.Tables[0].Rows[i]["calendar_date"].ToString() + "'," +
                        "'emp_id':'" + ds.Tables[0].Rows[i]["emp_id"].ToString() + "'," +
                        "'status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";
        }
        else
        {
            json = "{'status':false,'Message' :'No data found!'}";
        }

        ds.Dispose();
        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }


}
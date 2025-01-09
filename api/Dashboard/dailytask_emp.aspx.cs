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
using DocumentFormat.OpenXml.Drawing.Charts;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    static string emp_id;
    static string type;

    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
        getcounts();
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

    public void getcounts()
    {
        if (Request.Form["emp_id"] != null)
        {
            emp_id = Request.Form["emp_id"];
        }
       

        string curdate = DateTime.Now.ToString("yyyy-MM-dd");
        
            int taskcount = GetCount("SELECT COUNT(id) as total_count FROM tbl_task  where  CAST(startdate AS DATE) = '" + curdate + "' and emp_id='" + emp_id + "'");
            int completed = GetCount("select COUNT(id) as total_count from tbl_task where status='completed' and CAST(startdate AS DATE) = '" + curdate + "' and emp_id='" + emp_id + "'");
            int pending = GetCount("select COUNT(id) as total_count from tbl_task where status='pending' and CAST(startdate AS DATE) = '" + curdate + "' and emp_id='" + emp_id + "'");

        var data = new
        {
            status = true,
            Message = "Success.",
            data = new[] // Use an array to match the JSON array structure
            {
        new
        {
            taskcount = taskcount,
            completed = completed,
            pending = pending,
        }
    }
        };

        // Serialize the data object to JSON
        string jsonOutput = JsonConvert.SerializeObject(data);

        json = JsonConvert.SerializeObject(data);
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }

    private int GetCount(string query)
    {
        DataSet ds = cc.joinselect(query);
        if (ds.Tables[0].Rows.Count > 0)
        {
            return Convert.ToInt32(ds.Tables[0].Rows[0]["total_count"]);
        }
        return 0;
    }


}
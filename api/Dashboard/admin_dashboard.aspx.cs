using Newtonsoft.Json;
using System;
using System.Data;
using System.Web;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;

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
        int employeecount = GetCount("select COUNT(*) as total_count from tbl_employee e inner join  tbl_user u on e.id=u.emp_id where e.delete_status = 0 and u.type='employee'");
        int projectcount = GetCount("select COUNT(*) as total_count from tbl_projectmaster  where delete_status=0");
        int taskcount = GetCount("SELECT COUNT(*) AS total_count FROM tbl_task");
        int bugcount = 0;

        var data = new
        {
            status = true,
            Message = "Success.",
            data = new
            {
                employeecount = employeecount,
                projectcount = projectcount,
                taskcount = taskcount,
                bugcount = bugcount
            }
        };

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

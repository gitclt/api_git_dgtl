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
        int employeecount = GetCount("  SELECT COUNT(*) AS total_count FROM tbl_employees  e  INNER JOIN  tbl_states s ON e.state_id = s.id INNER JOIN tbl_country c ON e.country_id = c.id INNER JOIN tbl_designation d ON e.designation_id = d.id INNER JOIN tbl_district dt ON e.district_id = dt.id INNER JOIN tbl_production_unit pu ON e.unit_id = pu.id INNER JOIN tbl_grade g ON e.grade = g.id INNER JOIN tbl_dept de ON e.department_id = de.id INNER JOIN tbl_role r ON e.role = r.id WHERE e.delete_status = 0 ");
        int customercount = GetCount("SELECT COUNT(*) AS total_count FROM tbl_customer c  INNER JOIN tbl_country ct ON c.billing_country = ct.id INNER JOIN  tbl_states s ON c.billing_state = s.id  INNER JOIN tbl_district dt ON c.billing_district = dt.id WHERE  c.delete_status = 0 ");
        int ordercount = GetCount("SELECT COUNT(*) AS total_count FROM tbl_orders WHERE delete_status = 0");
        int productcount = GetCount("select COUNT(*) as total_count from tbl_product where delete_status = 0");

        var data = new
        {
            status = true,
            Message = "Success.",
            data = new
            {
                employeecount = employeecount,
                customercount = customercount,
                ordercount = ordercount,
                productcount = productcount
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

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
        int countrycount = GetCount("SELECT COUNT(*) AS total_count FROM tbl_country WHERE delete_status = 0");
        int statecount = GetCount("SELECT COUNT(*) AS total_count FROM tbl_states WHERE delete_status = 0");
        int gradecount = GetCount("select COUNT(*) as total_count from tbl_grade where delete_status = 0");
        int deptcount = GetCount("select COUNT(*) as total_count from tbl_dept where delete_status = 0");
        int rolecount = GetCount("select COUNT(*) as total_count from tbl_role where delete_status = 0");
        int designationcount = GetCount("select COUNT(*) as total_count from tbl_designation where delete_status = 0");
        int materialcount = GetCount("select COUNT(*) as total_count from tbl_material where delete_status = 0");
        int districtcount = GetCount("select COUNT(d.id) as total_count  from tbl_district d   inner join tbl_country c on d.country_id = c.id inner join tbl_states s on d.state_id = s.id where d.delete_status=0 ");
        int branchcount = GetCount("select COUNT(p.id) as total_count  from tbl_production_unit p inner join tbl_states s on p.state_id = s.id inner join tbl_district d on p.district_id = d.id");
        int producttypecount = GetCount("select COUNT(*) as total_count from tbl_product_type where delete_status = 0");
        int brandcount = GetCount("select COUNT(*) as total_count from tbl_brand where delete_status = 0");
        int documentcount = GetCount("select COUNT(*) as total_count from tbl_documents where delete_status = 0");
        int patterncount = GetCount("select COUNT(*) as total_count from tbl_pattern where delete_status = 0");
        int gsmcount = GetCount("select COUNT(*) as total_count from tbl_gsm where delete_status = 0");
        int productionprocesscount = GetCount("select COUNT(*) as total_count from tbl_production_process where delete_status = 0");


        var data = new
        {
            status = true,
            Message = "Success.",
            data = new
            {
                countrycount = countrycount,
                statecount = statecount,
                districtcount = districtcount,
                gradecount = gradecount,
                deptcount = deptcount,
                rolecount = rolecount,
                designationcount = designationcount,
                materialcount = materialcount,
                branchcount= branchcount,
                producttypecount= producttypecount,
                brandcount= brandcount,
                documentcount= documentcount,
                patterncount = patterncount,
                gsmcount = gsmcount,
                productionprocesscount = productionprocesscount,



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

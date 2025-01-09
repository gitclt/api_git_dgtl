using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string name = "", grade = "", unit_id = "", designation_id = "", state_id = "", district_id = "";
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
        if (Request.Form["name"] != null)
        {
            name = Request.Form["name"];
        }
        if (Request.Form["grade"] != null)
        {
            grade = Request.Form["grade"];
        }
        if (Request.Form["unit_id"] != null)
        {
            unit_id = Request.Form["unit_id"];
        }
        if (Request.Form["designation_id"] != null)
        {
            designation_id = Request.Form["designation_id"];
        }
        if (Request.Form["state_id"] != null)
        {
            state_id = Request.Form["state_id"];
        }
        if (Request.Form["district_id"] != null)
        {
            district_id = Request.Form["district_id"];
        }

        string querry1 = @"SELECT 
    e.id,
    e.name as employeename,
    e.grade,
    e.emp_code,
    e.email,
    e.mobile,
    e.password,
    e.role,
    e.department_id,
    e.employment_type,
    e.date_of_joining,
    e.report_manager,
    e.date_of_birth,
    e.gender,
    e.marital_status,
    e.uan,
e.img,
e.salary,
    e.pan,
    e.status,
    e.aadhaar,
    e.work_phone,
    e.personal_email,
    e.present_address,
    e.permenent_address,
    e.last_name,
    e.state_id as stateid,
    e.unit_id as unitid,
    e.country_id,
e.bank_name,
e.account_no,
e.branch,
e.IFSC,
e.account_holder,
pu.name as unit_name,
    c.name as countryname,
    e.district_id as districtid,
    e.designation_id as designationid,
    e.esi_no,
    s.name AS state_name,
    d.name as designation,
    dt.name as district,
    g.name as grade_name,
    de.name as department,
    r.name as role_name,
    (SELECT name FROM tbl_employees WHERE e.report_manager = id) as rep_manager_name
FROM 
    tbl_employees e 
INNER JOIN 
    tbl_states s ON e.state_id = s.id 
INNER JOIN 
    tbl_country c ON e.country_id = c.id 
INNER JOIN 
    tbl_designation d ON e.designation_id = d.id
INNER JOIN 
    tbl_district dt ON e.district_id = dt.id
INNER JOIN 
    tbl_production_unit pu ON e.unit_id = pu.id
INNER JOIN 
    tbl_grade g ON e.grade = g.id 
INNER JOIN 
    tbl_dept de ON e.department_id = de.id 
INNER JOIN 
    tbl_role r ON e.role = r.id 

WHERE 
    e.delete_status = 0";

        if (!string.IsNullOrEmpty(name))
        {
            querry1 += " AND e.name LIKE '%" + name + "%' ";
        }

        if (!string.IsNullOrEmpty(grade))
        {
            querry1 += " AND e.grade = '" + grade + "' ";
        }

        if (!string.IsNullOrEmpty(unit_id))
        {
            querry1 += " AND e.unit_id = '" + unit_id + "' ";
        }

        if (!string.IsNullOrEmpty(designation_id))
        {
            querry1 += " AND e.designation_id = '" + designation_id + "' ";
        }

        if (!string.IsNullOrEmpty(state_id))
        {
            querry1 += " AND e.state_id = '" + state_id + "' ";
        }

        if (!string.IsNullOrEmpty(district_id))
        {
            querry1 += " AND e.district_id = '" + district_id + "' ";
        }

        querry1 += " ORDER BY e.name ASC";

        DataSet ds = cc.joinselect(querry1);
       
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string id = ds.Tables[0].Rows[i]["id"].ToString();
                string imagePath = ds.Tables[0].Rows[i]["img"].ToString();


                string path = "";
                if (imagePath != "")
                    path = "http://" + HttpContext.Current.Request.Url.Authority + "/uploads/employee/" + ds.Tables[0].Rows[i]["id"].ToString() + "/" + imagePath + "";


                json += "{'name':'" + ds.Tables[0].Rows[i]["employeename"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "'," +
                    "'grade':'" + ds.Tables[0].Rows[i]["grade"].ToString() + "','emp_code':'" + ds.Tables[0].Rows[i]["emp_code"].ToString() + "'," +
                    "'email':'" + ds.Tables[0].Rows[i]["email"].ToString() + "','mobile':'" + ds.Tables[0].Rows[i]["mobile"].ToString() + "'," +
                    "'password':'" + EncodeDecode.base64Decode(ds.Tables[0].Rows[i]["password"].ToString()) + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "'," +
                    "'state_id':'" + ds.Tables[0].Rows[i]["stateid"].ToString() + "','designation_id':'" + ds.Tables[0].Rows[i]["designationid"].ToString() + "'," +
                    "'unit_id':'" + ds.Tables[0].Rows[i]["unitid"].ToString() + "','role':'" + ds.Tables[0].Rows[i]["role"].ToString() + "'," +
                    "'department_id':'" + ds.Tables[0].Rows[i]["department_id"].ToString() + "','employment_type':'" + ds.Tables[0].Rows[i]["employment_type"].ToString() + "'," +
                    "'date_of_joining':'" + Convert.ToDateTime(ds.Tables[0].Rows[i]["date_of_joining"]).ToString("yyyy-MM-dd") + "','report_manager':'" + ds.Tables[0].Rows[i]["report_manager"].ToString() + "',";

                if(ds.Tables[0].Rows[i]["date_of_birth"].ToString()=="")
                    json += "'date_of_birth':'',";
                else
                    json += "'date_of_birth':'" + Convert.ToDateTime(ds.Tables[0].Rows[i]["date_of_birth"]).ToString("yyyy-MM-dd") + "',";


                    json += "'country_id':'" + ds.Tables[0].Rows[i]["country_id"].ToString() + "'," +
                    "'countryname':'" + ds.Tables[0].Rows[i]["countryname"].ToString() + "','image_path':'" + path + "','gender':'" + ds.Tables[0].Rows[i]["gender"].ToString() + "'," +
                    "'marital_status':'" + ds.Tables[0].Rows[i]["marital_status"].ToString() + "','uan':'" + ds.Tables[0].Rows[i]["uan"].ToString() + "','pan':'" + ds.Tables[0].Rows[i]["pan"].ToString() + "'," +
                    "'aadhaar':'" + ds.Tables[0].Rows[i]["aadhaar"].ToString() + "','alternative_phone':'" + ds.Tables[0].Rows[i]["work_phone"].ToString() + "','personal_email':'" + ds.Tables[0].Rows[i]["personal_email"].ToString() + "'," +
                    "'present_address':'" + ds.Tables[0].Rows[i]["present_address"].ToString() + "','permenent_address':'" + ds.Tables[0].Rows[i]["permenent_address"].ToString() + "','last_name':'" + ds.Tables[0].Rows[i]["last_name"].ToString() + "','state_name':'" + ds.Tables[0].Rows[i]["state_name"].ToString() + "'," +
                    "'designation':'" + ds.Tables[0].Rows[i]["designation"].ToString() + "','grade_name':'" + ds.Tables[0].Rows[i]["grade_name"].ToString() + "','department':'" + ds.Tables[0].Rows[i]["department"].ToString() + "'," +
                    "'role_name':'" + ds.Tables[0].Rows[i]["role_name"].ToString() + "','district':'" + ds.Tables[0].Rows[i]["district"].ToString() + "'," +
                    "'districtid':'" + ds.Tables[0].Rows[i]["districtid"].ToString() + "','rep_manager_name':'" + ds.Tables[0].Rows[i]["rep_manager_name"].ToString() + "','esi_no':'" + ds.Tables[0].Rows[i]["esi_no"].ToString() + "','bank_name':'" + ds.Tables[0].Rows[i]["bank_name"].ToString() + "','account_no':'" + ds.Tables[0].Rows[i]["account_no"].ToString() + "','branch':'" + ds.Tables[0].Rows[i]["branch"].ToString() + "'," +
                    "'account_holder':'" + ds.Tables[0].Rows[i]["account_holder"].ToString() + "','img':'" + ds.Tables[0].Rows[i]["img"].ToString() + "','IFSC':'" + ds.Tables[0].Rows[i]["IFSC"].ToString() + "','salary':'" + ds.Tables[0].Rows[i]["salary"].ToString() + "','production_unit':'" + ds.Tables[0].Rows[i]["unit_name"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";
        
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
            ds.Dispose();
        }
        else
        {
            json = "{'status': false, 'Message': 'No data found!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }


}

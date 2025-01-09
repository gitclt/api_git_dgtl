using DocumentFormat.OpenXml.VariantTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_employee_Default : System.Web.UI.Page
{


    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    CommFuncs com = new CommFuncs();
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
        public string name;
        public string last_name;
        public string emp_code;
        public string email;
        public string password;
        public string img;
        public string grade;
        public string unit_id;
        public string role;
        public string department_id;
        public string designation_id;
        public string employement_type;
        public string date_of_joining;
        public string report_manager;
        public string date_of_birth;
        public string gender;
        public string marital_status;
        public string uan;
        public string pan;
        public string aadhaar;
        public string alternative_phone;
        public string mobile;
        public string personal_email;
        public string present_address;
        public string permenent_address;
        public string state_id;
        public string district_id;
        public string country_id;
        public string esi_no;
        public string modified_by;
        public DateTime added_on;
        public DateTime deleted_on;
        public string deleted_by;
        public DateTime modified_on;
        public string delete_status;
        public string modified_type;
        public string id;
        public string bank_name;
        public string IFSC;
        public string account_no;
        public string branch;
        public string account_holder;
        public string salary;




    }

    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        string enc_key = com.generate_tocken();

        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        foreach (var data in DataResponse)
        {
            string query = "UPDATE tbl_employees SET ";
            if (!string.IsNullOrEmpty(data.name)) query += "name='" + data.name + "',";
            if (!string.IsNullOrEmpty(data.last_name)) query += "last_name='" + data.last_name + "',";
            if (!string.IsNullOrEmpty(data.emp_code)) query += "emp_code='" + data.emp_code + "',";
            if (!string.IsNullOrEmpty(data.email)) query += "email='" + data.email + "',";
            if (!string.IsNullOrEmpty(data.password)) query += "password='" + EncodeDecode.base64Encode(data.password) + "',";
            if (!string.IsNullOrEmpty(data.mobile)) query += "mobile='" + data.mobile + "',";
            if (!string.IsNullOrEmpty(data.grade)) query += "grade='" + data.grade + "',";
            if (!string.IsNullOrEmpty(data.designation_id)) query += "designation_id='" + data.designation_id + "',";
            if (!string.IsNullOrEmpty(data.unit_id)) query += "unit_id='" + data.unit_id + "',";
            if (!string.IsNullOrEmpty(data.role)) query += "role='" + data.role + "',";
            if (!string.IsNullOrEmpty(data.modified_by)) query += "modified_by='" + data.modified_by + "',";
            query += "modified_on=GETDATE(),";
            if (!string.IsNullOrEmpty(data.modified_type)) query += "modified_type='" + data.modified_type + "',";
            if (!string.IsNullOrEmpty(data.department_id)) query += "department_id='" + data.department_id + "',";
            if (!string.IsNullOrEmpty(data.employement_type)) query += "employment_type='" + data.employement_type + "',";
            if (!string.IsNullOrEmpty(data.date_of_joining)) query += "date_of_joining='" + data.date_of_joining + "',";
            if (!string.IsNullOrEmpty(data.report_manager)) query += "report_manager='" + data.report_manager + "',";
            if (!string.IsNullOrEmpty(data.date_of_birth)) query += "date_of_birth='" + data.date_of_birth + "',";
            if (!string.IsNullOrEmpty(data.gender)) query += "gender='" + data.gender + "',";
            if (!string.IsNullOrEmpty(data.marital_status)) query += "marital_status='" + data.marital_status + "',";
            if (!string.IsNullOrEmpty(data.uan)) query += "uan='" + data.uan + "',";
            if (!string.IsNullOrEmpty(data.pan)) query += "pan='" + data.pan + "',";
            if (!string.IsNullOrEmpty(data.aadhaar)) query += "aadhaar='" + data.aadhaar + "',";
            if (!string.IsNullOrEmpty(data.alternative_phone)) query += "work_phone='" + data.alternative_phone + "',";
            if (!string.IsNullOrEmpty(data.personal_email)) query += "personal_email='" + data.personal_email + "',";
            if (!string.IsNullOrEmpty(data.present_address)) query += "present_address='" + data.present_address + "',";
            if (!string.IsNullOrEmpty(data.permenent_address)) query += "permenent_address='" + data.permenent_address + "',";
            if (!string.IsNullOrEmpty(data.state_id)) query += "state_id='" + data.state_id + "',";
            if (!string.IsNullOrEmpty(data.district_id)) query += "district_id='" + data.district_id + "',";
            if (!string.IsNullOrEmpty(data.country_id)) query += "country_id='" + data.country_id + "',";
            if (!string.IsNullOrEmpty(data.esi_no)) query += "esi_no='" + data.esi_no + "',";
            if (!string.IsNullOrEmpty(data.bank_name)) query += "bank_name='" + data.bank_name + "',";
            if (!string.IsNullOrEmpty(data.account_no)) query += "account_no='" + data.account_no + "',";
            if (!string.IsNullOrEmpty(data.IFSC)) query += "IFSC='" + data.IFSC + "',";
            if (!string.IsNullOrEmpty(data.account_holder)) query += "account_holder='" + data.account_holder + "',";
            if (!string.IsNullOrEmpty(data.branch)) query += "branch='" + data.branch + "',";
            if (!string.IsNullOrEmpty(data.img)) query += "img='" + data.img + "',";
            if (!string.IsNullOrEmpty(data.salary)) query += "salary='" + data.salary + "',";

            query = query.TrimEnd(',');

            query += " WHERE id=" + data.id;

            //Response.Write(query);
            //return;

            int status = cc.Insert(query);
            string json;
            if (status > 0)
            {
                json = "{'status':true,'Message' :'Data updated successfully.'}";
            }
            else
            {
                json = "{'status':false,'Message' :'Oops! Something went wrong'}";
            }
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }


}
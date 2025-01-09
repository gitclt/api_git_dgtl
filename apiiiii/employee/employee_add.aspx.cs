using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
        public string alternative_phone;
        public string present_address;
        public string permenent_address;
        public string salary; 
        public string mobile;

        public string grade;
        public string unit_id;
        public string role;
        public string department_id;
        public string designation_id;
        public string country_id;
        public string state_id;
        public string district_id;
        public string employement_type;
        public string status;
        public string date_of_joining;
        public string esi_no;
        public string report_manager;
        public string modified_by;
        public DateTime added_on;
        public DateTime deleted_on;
        public string deleted_by;
        public DateTime modified_on;
        public string delete_status;
        public string modified_type;
    }


    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        string enc_key = com.generate_tocken();

        string querry = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);
        string message = "Data added successfully.";
        string json = "";

        foreach (var data in DataResponse)
        {
            querry = @"SELECT id, emp_code, email, mobile 
                   FROM tbl_employees 
                   WHERE emp_code = '" + data.emp_code + "' OR mobile = '" + data.mobile + "'";

            if (!string.IsNullOrEmpty(data.email))
            {
                querry += " OR email = '" + data.email + "'";
            }

            DataSet dss = cc.joinselect(querry);

            if (dss.Tables[0].Rows.Count > 0)
            {
                DataRow row = dss.Tables[0].Rows[0];

                if (row["emp_code"].ToString() == data.emp_code)
                {
                    message = "Employee code already exists.";
                }
                else if (!string.IsNullOrEmpty(data.email) && row["email"].ToString() == data.email)
                {
                    message = "Employee email already exists.";
                }
                else if (row["mobile"].ToString() == data.mobile)
                {
                    message = "Employee mobile already exists.";
                }

                json = "{'status':false,'Message' :'" + message + "'}";
                json = json.Replace("'", "\"");
                dss.Dispose();
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
            else
            {
                string query = @"
                INSERT INTO tbl_employees
                (name, emp_code, email, password, added_on, modified_by, modified_on, delete_status, modified_type, 
added_by, added_type, enc_key, enc_key_date, status, last_name, present_address, permenent_address, work_phone, mobile, salary,
                grade, role, department_id, designation_id, country_id, state_id, district_id, date_of_joining, esi_no, report_manager, unit_id, employment_type) 
                VALUES
                ('" + data.name + "', '" + data.emp_code + "', '" + data.email + "','" + EncodeDecode.base64Encode(data.password) + "', GETDATE(), '" + data.modified_by + "', GETDATE(),0, '" + data.modified_type + "', " +
                    "'" + data.modified_by + "', '" + data.modified_type + "','" + enc_key + "', GETDATE(), 'active', '" + data.last_name + "','" + data.present_address + "','" + data.permenent_address + "','" + data.alternative_phone + "'," +
                    "'" + data.mobile + "','" + data.salary + "','" + data.grade + "','" + data.role + "','" + data.department_id + "','" + data.designation_id + "','" + data.country_id + "','" + data.state_id + "','" + data.district_id + "'," +
                    "'" + data.date_of_joining + "','" + data.esi_no + "','" + data.report_manager + "','" + data.unit_id + "','" + data.employement_type + "')";

                int status = cc.Insert(query);

                if (status > 0)
                {
                    query = @"SELECT TOP 1 id FROM tbl_employees WHERE emp_code = '" + data.emp_code + "'";

                    if (!string.IsNullOrEmpty(data.email))
                    {
                        query += " AND email = '" + data.email + "'";
                    }

                    DataSet ds = cc.joinselect(query);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int emp_id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"]);

                        if (emp_id != null)
                        {
                            json = "{'status':true,'Message':'Data added successfully','data':[";
                            foreach (var item in DataResponse)
                            {
                                json += "{'id':'" + emp_id + "','name':'" + item.name + "','emp_code':'" + item.emp_code + "','email':'" + item.email + "','password':'" + item.password + "','mobile':'" + item.mobile + "'," +
                                        "'alternative_phone':'" + item.alternative_phone + "','permenent_address':'" + item.permenent_address + "','present_address':'" + item.present_address + "','last_name':'" + item.last_name + "'," +
                                        "'salary':'" + item.salary + "','grade':'" + item.grade + "','role':'" + item.role + "','department_id':'" + item.department_id + "','designation_id':'" + item.designation_id + "'," +
                                        "'country_id':'" + item.country_id + "','state_id':'" + item.state_id + "','district_id':'" + item.district_id + "','date_of_joining':'" + item.date_of_joining + "','esi_no':'" + item.esi_no + "','report_manager':'" + item.report_manager + "','unit_id':'" + item.unit_id + "','employment_type':'" + item.employement_type + "'},";
                            }
                            json = json.TrimEnd(',') + "]}";
                            json = json.Replace("'", "\"");
                            Response.ContentType = "application/json";
                            Response.Write(json);
                            Response.End();
                        }
                    }
                    else
                    {
                        json = "{'status':false,'Message':'Oops! Something went wrong.'}";
                    }
                }
                dss.Dispose();
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
        }
    }
}
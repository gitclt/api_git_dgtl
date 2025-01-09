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
    CommFuncs com = new CommFuncs();
    string json;
    static string querry;
    protected void Page_Load(object sender, EventArgs e)
    {
        //chk_tocken();

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
        public string work_phone;
        public string mobile;
        public string personal_email;
        public string present_address;
        public string permenent_address;
        public string state_id;
        public string district_id;
        public string country_id;
        public string esi_no;

        public string type;
        public string id;
        public string modified_by;
        public DateTime added_on;
        public DateTime deleted_on;
        public string deleted_by;
       public DateTime modified_on;
        public string delete_status;
        public string modified_type;
        public string status;






    }

    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        string enc_key =com.generate_tocken();


        querry = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        string qry = "";
        int count = 0;
        string message = "";

        foreach (var data in DataResponse)
        {
            string querry1 = "";   
            if (data.type == "add")
            {
                querry1 = @"insert into tbl_employees   
(name,grade,emp_code,email,mobile,password,designation_id,unit_id,role
,added_on,modified_by,modified_on,delete_status,modified_type,
added_by,added_type,enc_key,enc_key_date,status,last_name,department_id,
employment_type,date_of_joining,report_manager,date_of_birth,gender,marital_status,
uan,pan,aadhaar,work_phone,personal_email,present_address,permenent_address,state_id,district_id,country_id,esi_no) 
   values('" + data.name + "','" + data.grade + "','" + data.emp_code + "','" + data.email + "','" + data.mobile + "'" +
   ",'" + EncodeDecode.base64Encode(data.password) + "','" + data.designation_id + "','" + data.unit_id + "'" +
   ",'" + data.role+ "',getdate(),'" + data.modified_by + "',getdate(),0,'" + data.modified_type+ "'," +
   "'" + data.modified_by+ "','" + data.modified_type+ "','"+ enc_key + "',getdate(),'active','"+data.last_name + "','"+data.department_id + "'," +
   "'"+data.employement_type + "','"+data.date_of_joining + "','"+data.report_manager + "','"+data.date_of_birth + "','"+data.gender + "','"+ data.marital_status + "'," +
   "'"+ data.uan + "','"+ data.pan + "','"+data.aadhaar + "','"+data.work_phone + "','"+data.personal_email + "','"+data.present_address + "','"+data.permenent_address + "','"+data.state_id+"','"+data.district_id+"','"+data.country_id+"','"+data.esi_no+"')";


                message = "Data added successfully.";


                int status = cc.Insert(querry1);
                if (status > 0)
                {
                    querry1 = @"select top 1 id from tbl_employees where ";

                    //json = "{'status':true,'Message' :'Data updated successfully.'}";
                    json = "{'status':true,'Message' :'" + message + "'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
                else
                    json = "{'status':false,'Message' :'Oops! Something went wrong'}";
            }
            else if (data.type == "edit")
            {
                querry1 = @"update tbl_employees set name='" + data.name + "',grade='" + data.grade + "',emp_code='" + data.emp_code + "'," +
      "mobile='" + data.mobile + "',password='" + EncodeDecode.base64Encode(data.password) + "',designation_id='" + data.designation_id + "',unit_id='" + data.unit_id + "'," +
                    "role='" + data.role + "',modified_by='" + data.modified_by + "'," +
                    "modified_on=getdate(),modified_type='" + data.modified_type + "',status='"+data.status+ "',last_name='"+data.last_name + "',department_id='"+data.department_id + "',employment_type='" + data.employement_type + "',date_of_joining='"+data.date_of_joining + "',report_manager='"+data.report_manager + "',date_of_birth='"+data.date_of_birth + "',gender='"+data.gender + "'," +
                    "marital_status='"+data.marital_status + "',uan='"+data.uan + "',pan='"+data.pan+ "',aadhaar='"+data.aadhaar + "',work_phone='"+data.work_phone + "',personal_email='"+data.personal_email + "',present_address='"+data.present_address + "',permenent_address='"+ data.permenent_address + "',state_id='"+data.state_id+"',district_id='"+data.district_id+"',country_id='"+data.country_id+"',email='"+data.email+ "',esi_no='"+data.esi_no + "' where id=" + data.id + " ";
                message = "Data updated successfully.";

                int status = cc.Insert(querry1);
                if (status > 0)
                {
                    //json = "{'status':true,'Message' :'Data updated successfully.'}";
                    json = "{'status':true,'Message' :'" + message + "'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
                else
                    json = "{'status':false,'Message' :'Oops! Something went wrong'}";

            }
            else if (data.type == "delete")
            {
                querry1 = @"update  tbl_employees  set delete_status=1 where id=" + data.id + " ";
                message = "Data deleted successfully.";
                int status = cc.Insert(querry1);
                if (status > 0)
                {
                    //json = "{'status':true,'Message' :'Data updated successfully.'}";
                    json = "{'status':true,'Message' :'" + message + "'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
                else
                    json = "{'status':false,'Message' :'Oops! Something went wrong'}";

            }
            //Response.Write(querry1);
            //return;

           
        }

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
   

}
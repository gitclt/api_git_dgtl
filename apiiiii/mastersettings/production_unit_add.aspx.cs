using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();

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
        public string unitincharge;
        public string mobile;
        public string address;
        public string name;
        public string districtid;
        public string stateid;
        public string id;
        public string type;


    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        querry = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        string qry = "";
       


        foreach (var data in DataResponse)
        {
            string querry1 = "";
            string message = "";

            if (data.type == "add")
            {
                querry1 = @"insert into tbl_production_unit (unit_incharge,mobile,address,name,district_id,state_id) values('" + data.unitincharge + "','" + data.mobile + "','" + data.address + "','"+data.name + "','"+ data.districtid + "','"+ data.stateid + "')";
                message = "Data added successfully.";

            }
            else if (data.type == "edit")
            {
                querry1 = @"update tbl_production_unit set name='" + data.name+"', state_id='" + data.stateid + "',unit_incharge='" + data.unitincharge + "',mobile='" + data.mobile + "',address='" + data.address + "',district_id='" + data.districtid + "' where id=" + data.id + " ";
                message = "Data updated successfully.";

            }
            else if (data.type == "delete")
            {
                querry1 = @"delete from tbl_production_unit  where id=" + data.id + " ";
                message = "Data deleted successfully.";

            }
            int status = cc.Insert(querry1);
            if (status > 0)
            {
                //json = "{'status':true,'Message' :'Success.'}";
                json = "{'status':true,'Message' :'" + message + "'}";

                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
            else
                json = "{'status':false,'Message' :'Failed'}";
        }
        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
//    }
//}


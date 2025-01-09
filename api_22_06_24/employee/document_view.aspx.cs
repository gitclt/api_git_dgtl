using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Xml.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string emp_id = "";

   

    protected void Page_Load(object sender, EventArgs e)
    {
       // chk_tocken();

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

        // string querry1 = @"select * from tbl_product where (1=1) ";
        string querry1 = @"select d.name,ed.emp_id,ed.document_id,ed.[file],ed.document_no,ed.id from tbl_employee_documents ed
inner join tbl_documents d on ed.document_id=d.id  where ed.emp_id='"+emp_id+"'";
        Response.Write(querry1);
        return;
        DataSet ds = cc.joinselect(querry1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string id = ds.Tables[0].Rows[i]["emp_id"].ToString();
                string imagePath = ds.Tables[0].Rows[i]["file"].ToString();
                string path = "";
                if (imagePath != "")

                path = "http://" + HttpContext.Current.Request.Url.Authority + "/uploads/documents/" + ds.Tables[0].Rows[i]["emp_id"].ToString() + "/" + imagePath + "";

                json += "{'emp_id':'" + ds.Tables[0].Rows[i]["emp_id"].ToString() + "','document_id':'" + ds.Tables[0].Rows[i]["document_id"].ToString() + "','file':'" + path+ "','document_no':'" + ds.Tables[0].Rows[i]["document_no"].ToString() + "','document_name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
            json = "{'status':false,'Message' :'No data found!'}";


        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }


}
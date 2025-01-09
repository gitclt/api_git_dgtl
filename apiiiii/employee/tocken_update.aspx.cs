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
    string id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        getdata();
    }
    public void getdata()
    {

        if (Request.Form["id"] != null)
        {
            id = Request.Form["id"];
            string enc_key = com.generate_tocken();

            string querry1 = @"update tbl_employees set enc_key='" + enc_key + "',enc_key_date=getdate() where id  = '" + id + "' ";
            int status = cc.Insert(querry1);
            if (status > 0)
            {
                // json = "{'status':true,'Message' :'Data updated successfully.'}";
                json = "{'status':true,'Message' :'updated successfully'}";

                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            }
            else
                json = "{'status':false,'Message' :'Oops! Something went wrong'}";
        }
    }

}
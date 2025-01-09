using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Newtonsoft.Json;
using System.IO;

public partial class api_catlog_add_wishlist : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;

    protected void Page_Load(object sender, EventArgs e)
    {

        select();
    }

   
    public void select()
    {
    
        querry = @"select * from tbl_hierarchy order by name";

        DataSet ds = cc.joinselect(querry);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'success' ,'data':[";

            for(int i = 0; i < ds.Tables[0].Rows.Count;i++)
            {
                json += "{'id':'" + ds.Tables[0].Rows[i]["id"] + "','name':'" + ds.Tables[0].Rows[i]["name"] + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";
        }
        else
            json = "{'status':false,'Message' :'Failed'}";


        json = json.Replace("'", "\"");


        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
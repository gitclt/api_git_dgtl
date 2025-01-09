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

        insert();
    }

    public class DataResponse
    {
        public string name;
        public string type;
        public string id;
    }
    public void insert()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();

        querry = "";
        List<DataResponse> DataResponse = JsonConvert.DeserializeObject<List<DataResponse>>(bodyText);

        string qry = "";
        int count = 0;
        foreach (var data in DataResponse)
        {

            if (data.type == "add")
            {
                querry = @"select * from tbl_designation where name='" + data.name + "'";
                DataSet ds = cc.joinselect(querry);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    json = "{'status':false,'Message' :'Already Exist! Please update if needed.'}";
                    json = json.Replace("'", "\"");
                    Response.ContentType = "application/json";
                    Response.Write(json);
                    Response.End();
                }
                else
                {

                    qry += @"insert into tbl_designation (name)values('" + data.name + "') ";
                }
            }
            else if(data.type == "edit")
            {

                qry = @"update tbl_designation set name='" + data.name + "' where id='" + data.id + "' ";
            }
            else if (data.type == "delete")
            {

                qry = @"delete from tbl_designation  where id='" + data.id + "' ";
            }
        }
        int q = cc.Insert(qry);
        if (q > 0)
            json = "{'status':true,'Message' :'Updated Successfully'}";
        else
            json = "{'status':false,'Message' :'Failed'}";


        json = json.Replace("'", "\"");


        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }
}
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Linq;

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
    string json;
    static string querry;
    string name = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
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
        string querry3 = @"select * from tbl_district where (1=1) ";
        querry3 += "order by state_id asc";
        DataSet dis = cc.joinselect(querry3);


        string querry2 = @"select * from tbl_states where (1=1) ";
        querry2 += "order by country_id asc";
        DataSet st = cc.joinselect(querry2);


        string querry1 = @"select * from tbl_country where (1=1) ";
        querry1 += "order by name asc";
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
                    path = "http://" + HttpContext.Current.Request.Url.Authority + "/uploads/country/" + imagePath + "";

                var rows = st.Tables[0].AsEnumerable().Where(x => x["country_id"].ToString() == ds.Tables[0].Rows[i]["id"].ToString());


                json += @"{'name':'" + ds.Tables[0].Rows[i]["name"].ToString() + @"'
                    ,'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + @"','country_code':'" + ds.Tables[0].Rows[i]["country_code"].ToString() + @"',
'short_code':'" + ds.Tables[0].Rows[i]["short_code"].ToString() + @"','img':'" + path + "','states':[";

                if (rows.Any())
                {
                    DataTable dt = rows.CopyToDataTable();
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        var rows1 = dis.Tables[0].AsEnumerable().Where(x => x["state_id"].ToString() == dt.Rows[j]["id"].ToString());

                        json += "{'state':'" + dt.Rows[j]["name"] + "','id':'" + dt.Rows[j]["id"] + "','districts':[";

                        if (rows1.Any())
                        {
                            DataTable dt1 = rows1.CopyToDataTable();
                            for (int k = 0; k < dt1.Rows.Count; k++)
                            {

                                json += "{'district':'" + dt1.Rows[k]["name"] + "','id':'" + dt1.Rows[k]["id"] + "'},";


                            }
                            dt1.Dispose();
                            json = json.TrimEnd(',');
                        }
                        json += "]},";

                    }
                    dt.Dispose();
                    json = json.TrimEnd(',');

                }


                json += "]},";


            }
            ds.Dispose();
            json = json.TrimEnd(',');
            json += "]}";

            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
        else
            json = "{'status':false,'Message' :'No data found!'}";

        ds.Dispose();
        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";

        Response.Write(json);

        Response.End();
    }

    private string imgpath(string id)
    {
        string folder = Server.MapPath("../uploads/country/");
        if (Directory.Exists(folder))
        {
            string[] files = Directory.GetFiles(folder);
            if (files.Length > 0)
            {
                return Path.GetFileName(files[0]); // Return the first image found
            }
        }
        return ""; // Return an empty string if no image is found
    }

}
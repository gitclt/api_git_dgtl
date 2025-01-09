using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Web;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry,id="";

    protected void Page_Load(object sender, EventArgs e)
    {
       // chk_tocken();
        getcounts();
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

    public void getcounts()
    {
        if (Request.Form["id"] != null)
        {
            id = Request.Form["id"];
        }

        int production_count = GetCount("select count(id) as total_count from tbl_orders  where production_status='not started' and addedby='"+ id + "' ");
          int packing_count= GetCount("select count(id) as total_count from tbl_orders  where packing_status='not started' and addedby='"+id+"'");
        int total_count = production_count + packing_count;


        var data = new
        {
            status = true,
            Message = "Success.",
            data = new
            {
                production_count = production_count,
                packing_count = packing_count,
                total_count = total_count


            }
        };

        json = JsonConvert.SerializeObject(data);
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }


    private int GetCount(string query)
    {
        DataSet ds = cc.joinselect(query);
        if (ds.Tables[0].Rows.Count > 0)
        {
            return Convert.ToInt32(ds.Tables[0].Rows[0]["total_count"]);
        }
        return 0;
    }

}

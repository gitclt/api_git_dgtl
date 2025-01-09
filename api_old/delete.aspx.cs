 using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Data;
//using System.IdentityModel.Protocols.WSTrust;
using System.Web;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    string json;
    string id = "";
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Request.Form["id"] != null && Request.Form["token"] != null)
        {

            string id = Request.Form["id"];
            string tocken = Request.Form["token"];

            getdata(id, tocken);
        }
    }

    public void getdata(string id, string token)
    {
        string querry = @"select * from tbl_employees where id='" + id + "' and enc_key='" + token + "'";
       
        DataSet ds = cc.joinselect(querry);
        //Response.Write(querry);
        //return;

        if (ds.Tables[0].Rows.Count > 0)
        {
          
                json = "{'status':true,'Message' :'Successfully logged in','data':[";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    json += "{'username':'" + ds.Tables[0].Rows[i]["mobile"].ToString() + "','password':'" + EncodeDecode.base64Decode(ds.Tables[0].Rows[i]["password"].ToString()) + "','role':'" + ds.Tables[0].Rows[i]["role"].ToString() + "' ,'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','grade':'" + ds.Tables[0].Rows[i]["grade"].ToString() + "','employee_code':'" + ds.Tables[0].Rows[i]["emp_code"].ToString() + "','enc_key':'" + ds.Tables[0].Rows[i]["enc_key"].ToString() + "','enc_key_date':'" + ds.Tables[0].Rows[i]["enc_key_date"].ToString() + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "'},";
                }
                json = json.TrimEnd(',');
                json += "]}";
            
           
        }
        else
        {
            // Modified this part to handle incorrect login attempts
            json = "{'status':false,'Message' :'Token or Id  mismatch or login not permitted for this role'}";
        }

        json = json.Replace("'", "\"");

        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}
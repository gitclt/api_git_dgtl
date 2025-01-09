using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
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
    string code = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        getdata();
    }
    public void getdata()
    {

        string sss = "username=admin";
        string encodedString = EncodeDecode.base64Encode(sss);
        Response.Write(encodedString);
        return;

        string id = "";
        string token = "";

        if (Request.Form["code"] != null)
        {

            string encryptedCode = Request.Form["code"];
            string decryptedCode = EncodeDecode.base64Decode(encryptedCode);
            string[] parts = decryptedCode.Split('-');

            if (parts.Length == 2)
            {
                token = parts[0];
                id = parts[1];
                //  Response.Write(id);
                //  return;
            }
            else
            {
                json = "{'status':false,'Message' :'Invalid encrypted code format.'}";
                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;
            }


            string curdate = DateTime.Now.ToString("yyyy-MM-dd");
            //Response.Write(curdate);
            //return;


            string querry = @"select * from tbl_employees where id='" + id + "' and enc_key='" + token + "' and cast(enc_key_date as date)= cast('" + curdate + "' as date)";

            DataSet ds = cc.joinselect(querry);
            //Response.Write(querry);
            //return;

            if (ds.Tables[0].Rows.Count > 0)
            {

                json = "{'status':true,'Message' :'Success','data':[";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    json += "{'username':'" + ds.Tables[0].Rows[i]["mobile"].ToString() + "','password':'" + EncodeDecode.base64Decode(ds.Tables[0].Rows[i]["password"].ToString()) + "','role':'" + ds.Tables[0].Rows[i]["role"].ToString() + "' ,'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','grade':'" + ds.Tables[0].Rows[i]["grade"].ToString() + "','employee_code':'" + ds.Tables[0].Rows[i]["emp_code"].ToString() + "','enc_key':'" + ds.Tables[0].Rows[i]["enc_key"].ToString() + "','enc_key_date':'" + ds.Tables[0].Rows[i]["enc_key_date"].ToString() + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "'},";
                }
                json = json.TrimEnd(',');
                json += "]}";
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;

            }
            else
            {
                json = "{'status':false,'Message' :'Token or Id  mismatch or login not permitted for this role'}";
            }
        }
    }
}
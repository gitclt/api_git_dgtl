using System;
using System.Data;
using System.Web;
using System.Web.UI;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json.Linq;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    string json;
    CommFuncs com =new CommFuncs();

    protected void Page_Load(object sender, EventArgs e)
    {
        string data = "";
        if (Request.Form["data"] != null)
        {
            try
            {
                data = EncodeDecode.base64Decode(Request.Form["data"]);
            }
            catch (Exception ex)
            {
                json = "{'status':false,'Message' :'Oops! Something went wrong!'}";

                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;
            }


                string username = data.Split('&')[0].Replace("username=", "").Replace("amp;", "&");
                string password = data.Split('&')[1].Replace("password=", "").Replace("amp;", "&");
              //  string role = data.Split('&')[2].Replace("role=", "");
                getdata(username, password);
            

           
            //return;
        }
       
    }

    public void getdata(string username, string password)
    {

        string curdate = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00"; // Append the time part to the date
        string querry = @"select e.id,e.enc_key,e.enc_key_date,e.role,e.status,r.hierarchy_id,e.mobile,e.password from tbl_employees e
inner join tbl_role r on e.role=r.id where e.mobile='" + username + "' and e.password='" + EncodeDecode.base64Encode(password) + "'";
        DataSet ds = cc.joinselect(querry);

        if (ds.Tables[0].Rows.Count > 0)
        {
            if (ds.Tables[0].Rows[0]["status"].ToString() == "inactive")
            {
                json = "{'status':false,'Message' :'Account is inactive please contact administrator'}";

                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;

            }
            
            if (DateTime.Parse(ds.Tables[0].Rows[0]["enc_key_date"].ToString()).ToString("dd-MM-yyyy") == DateTime.Parse(curdate).ToString("dd-MM-yyyy"))
            {
               
                json = "{'status':true,'Message' :'Successfully logged in','data':[";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    json += "{'role':'" + ds.Tables[0].Rows[i]["role"].ToString() + "','enc_key':'" + ds.Tables[0].Rows[i]["enc_key"].ToString() + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "','hierarchy_id':'" + ds.Tables[0].Rows[i]["hierarchy_id"].ToString() + "'},";
                }
                json = json.TrimEnd(',');
                json += "]}";

                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;

            }
            else
            {
              
                var tok = com.generate_tocken();
                json = "{'status':true,'Message' :'Successfully logged in','data':[";


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string aa = @"update tbl_employees set enc_key='" + tok + "',enc_key_date= '" + curdate + "' where id='" + ds.Tables[0].Rows[i]["id"].ToString() + "'";
                  
                    int status1 = cc.Insert(aa);
                   
                    json += "{'role':'" + ds.Tables[0].Rows[i]["role"].ToString() + "','enc_key':'" + tok + "','status':'" + ds.Tables[0].Rows[i]["status"].ToString() + "'},";
                }
              
                json = json.TrimEnd(',');
                json += "]}";

                json = json.Replace("'", "\"");
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;

            }
        }

        else
        {
            // Modified this part to handle incorrect login attempts
            json = "{'status':false,'Message' :'Username or password mismatch or login not permitted for this role'}";
            
            
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
            return;
        }
    }
}

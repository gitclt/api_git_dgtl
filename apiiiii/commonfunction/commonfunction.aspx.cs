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
using System.Data.SqlClient;

public class CommFuncs
{
    Country_DAL cc = new Country_DAL();
    public CommFuncs()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    private string connectionString = "Data Source=103.20.213.74;Initial Catalog=admin_gitprojects;User Id=gitprojects;Password=fitxyj@!222s;Connect Timeout=200; pooling='true'; Max Pool Size=200"; // Add your connection string here

   
   
    public string generate_tocken()
    {
        // Generate a new GUID
        Guid enc_key = Guid.NewGuid();

        // Format the GUID and add the prefix
        string ordernoString = "tocken" + enc_key.ToString().Replace("-", "").Substring(0, 10);

        // Check if the generated order number exists in the database
        string querry = @"select id from tbl_user where enc_key='" + ordernoString + "' ";
        DataSet ds = cc.joinselect(querry);

        // If a duplicate is found, generate a new order number
        if (ds.Tables[0].Rows.Count > 0)
        {
            return generate_tocken();
        }

        // Return the unique order number
        return ordernoString;
    }
    public string get_tocken_details(string enc_key)
    {
        string querry = @"select * from tbl_user where enc_key='" + enc_key + "'";
        DataSet ds = cc.joinselect(querry);
        if (ds.Tables[0].Rows.Count > 0)
        {
            if (Convert.ToDateTime(ds.Tables[0].Rows[0]["enc_key_date"].ToString()).ToString("dd-MM-yyyy") == DateTime.Now.ToString("dd-MM-yyyy"))
            {
                return ds.Tables[0].Rows[0]["id"].ToString();
            }
            else
                return "Oops! Tocken Expired!";
        }
        else
            return "";
    }

}
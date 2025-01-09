using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
//using System.Activities.Expressions;
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
   // string emp_id = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        getdata();
    }
    public void getdata()
    {
       

        //string querry1 = @"select * from tbl_employees  where role='supervisor' and delete_status=0  ";

        string querry= @"SELECT e.id,e.name,g.name grade,g.id grade_id,
    isnull(sum(a.qty),0)-(SELECT SUM(t.qty) FROM tbl_order_task t WHERE t.addedby = e.id) AS pending_quantity
FROM tbl_employees e
left JOIN tbl_order_assigned a ON a.emp_id = e.id 
inner JOIN tbl_role r ON e.role = r.id 
inner JOIN tbl_hierarchy h ON r.hierarchy_id = h.id 
inner JOIN tbl_grade g ON e.grade = g.id 
where h.id=5 group by e.id,e.name,g.name,g.id
";

        querry += "order by name";
        DataSet ds = cc.joinselect(querry);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'name':'" + ds.Tables[0].Rows[i]["name"].ToString() + @"'
                        ,'pending_quantity':'" + ds.Tables[0].Rows[i]["pending_quantity"].ToString() + @"'
                        ,'grade':'" + ds.Tables[0].Rows[i]["grade"].ToString() + @"'
                        ,'grade_id':'" + ds.Tables[0].Rows[i]["grade_id"].ToString() + @"'
                        ,'id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "'},";
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
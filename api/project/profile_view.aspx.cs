using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_project_Default : System.Web.UI.Page
{

    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    CommFuncs CommFuncs = new CommFuncs();
    string json;
    static string querry;
    string id = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        chk_tocken();
    }


    public void chk_tocken()
    {
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
        }
        else if (id != "")
        {


            getdata(id);

        }
        else
        {
            json = "{'status':false,'Message' :'Oops! Unauthorised Access!'}";
            json = json.Replace("'", "\"");
            Response.ContentType = "application/json";
            Response.StatusCode = 403;
            Response.Write(json);
            Response.End();
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
    public void getdata(string empid)
    {
       
        string querry1 = @" 
SELECT  distinct
e.id,e.name,d.name as designation,m.workspace_id,w.work_space
    FROM
        tbl_employee e
inner join
tbl_user u ON e.id = u.emp_id
  inner join
        tbl_designation d ON e.designation_id = d.id
			 left join
        tbl_projectmember pm ON pm.emp_id = e.id
		 left join
        tbl_projectmaster m ON m.id =pm.project_id
	
	
 	 left join
        tbl_workspace w ON m.workspace_id = w.id
     WHERE
        e.delete_status = 0 and m.delete_status=0
   ";

        querry1 += "AND u.id = '" + empid + "' ";


        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'name':'" + ds.Tables[0].Rows[i]["name"].ToString() + "','id':'" + ds.Tables[0].Rows[i]["id"].ToString() + "','designation':'" + ds.Tables[0].Rows[i]["designation"].ToString() + "',";
                json += "'work_space':[";
                string query2 = @"select pm.workspace_id,w.work_space 
		from tbl_projectmember pm
		inner join 
		tbl_workspace w on w.id=pm.workspace_id
		where pm.emp_id='" + ds.Tables[0].Rows[i]["id"].ToString() + "' and w.delete_status=0";
                DataSet dss = cc.joinselect(query2);
                if (dss.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dss.Tables[0].Rows.Count; j++)
                    {
                        json += "{'workspace_id':'" + dss.Tables[0].Rows[j]["workspace_id"].ToString() + "','work_space':'" + dss.Tables[0].Rows[j]["work_space"].ToString() + "'},";
                        //json += "{'id':'" + dss.Tables[0].Rows[j]["id"].ToString() + "'},";

                    }
                    json = json.TrimEnd(',');
                }
                json += "]";
                dss.Dispose();
                json += "},";
            }
            json = json.TrimEnd(',');
            json += "]}";
            ds.Dispose();
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
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using System.ServiceModel.Syndication;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry;
    string id = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        getdata();
    }
    public void getdata()
    {
        if (Request.Form["id"] != null)
        {
            id = Request.Form["id"];
        }

        //string querry1 = @"select * from tbl_employees  where role='supervisor' and delete_status=0  ";

        string querry = @"select p.name as project_name,e.name,p.target_date
from tbl_projectmaster p
inner join tbl_projectmember pm on p.id=pm.project_id
inner join tbl_employee e on e.id=pm.emp_id
inner join tbl_workspace w on pm.workspace_id=w.id where workspace_id='" + id + "'";
        DataSet ds = cc.joinselect(querry);
        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'project_name':'" + ds.Tables[0].Rows[i]["name"] + "','name':'" + ds.Tables[0].Rows[i]["name"] + "','target_date':'" + ds.Tables[0].Rows[i]["target_date"] + "'},";

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
﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
    string month = "";
    string date = "";

    protected void Page_Load(object sender, EventArgs e)
    {
       //chk_tocken();
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
        string querry1 = @"SELECT calendar_date, day_of_week, is_weekend, is_holiday, holiday_name 
                       FROM tbl_calendar 
                       WHERE MONTH(calendar_date) = MONTH(GETDATE())";

        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            json = "{'status':true,'Message' :'Success.','data':[";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                json += "{'calendar_date':'" + ds.Tables[0].Rows[i]["calendar_date"].ToString() + "'," +
                        "'day_of_week':'" + ds.Tables[0].Rows[i]["day_of_week"].ToString() + "'," +
                        "'is_weekend':'" + ds.Tables[0].Rows[i]["is_weekend"].ToString() + "'," +
                        "'is_holiday':'" + ds.Tables[0].Rows[i]["is_holiday"].ToString() + "'," +
                        "'holiday_name':'" + ds.Tables[0].Rows[i]["holiday_name"].ToString() + "'},";
            }
            json = json.TrimEnd(',');
            json += "]}";
        }
        else
        {
            json = "{'status':false,'Message' :'No data found!'}";
        }

        ds.Dispose();
        json = json.Replace("'", "\"");
        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }


}
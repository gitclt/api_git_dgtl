﻿using DocumentFormat.OpenXml.Bibliography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;

public partial class api_country_add : System.Web.UI.Page
{
    Country_DAL cc = new Country_DAL();
    SafeSqlLiteral safesql = new SafeSqlLiteral();
    string json;
    static string querry, pro_id;

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
            // Token is valid, continue processing
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
        if (Request.Form["pro_id"] != null)
        {
            pro_id = Request.Form["pro_id"];
        }

        string querry1 = @"SELECT p.id, p.pro_id, p.production_process_id, p.grade_id, p.output_per_sec, 
                                  pd.product_name, pp.process_name, g.name as grade
                           FROM tbl_product_production_process_grade p
                           INNER JOIN tbl_product pd ON p.pro_id = pd.id 
                           INNER JOIN tbl_production_process pp ON p.production_process_id = pp.id 
                           INNER JOIN tbl_grade g ON p.grade_id = g.id 
                           WHERE p.pro_id = '" + pro_id + "' ORDER BY pd.product_name";

        DataSet ds = cc.joinselect(querry1);

        if (ds.Tables[0].Rows.Count > 0)
        {
            var responseData = new
            {
                status = true,
                Message = "Success.",
                data = new List<object>()
            };

            int currentProId = -1; // Initialize with an invalid pro_id
            dynamic currentData = null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int proId = Convert.ToInt32(row["pro_id"]);
                if (proId != currentProId)
                {
                    // Add previous data if exists
                    if (currentData != null)
                    {
                        responseData.data.Add(currentData);
                    }

                    // Create new data object
                    currentData = new
                    {
                        pro_id = proId,
                        product_production_process = new
                        {
                            product_production_process_id = row["production_process_id"].ToString(),
                            process_name = row["process_name"],
                            grade = new List<object>()
                        }
                    };

                    currentProId = proId;
                }

                // Add grade to the current data object
                ((List<object>)currentData.product_production_process.grade).Add(new
                {
                    grade_id = Convert.ToInt32(row["grade_id"]),
                    grade= row["grade"],
                    output_per_sec = Convert.ToDecimal(row["output_per_sec"])
                });
            }

            // Add the last data object
            if (currentData != null)
            {
                responseData.data.Add(currentData);
            }

            json = JsonConvert.SerializeObject(responseData);
        }
        else
        {
            var errorResponse = new
            {
                status = false,
                Message = "No data found!"
            };
            json = JsonConvert.SerializeObject(errorResponse);
        }

        Response.ContentType = "application/json";
        Response.Write(json);
        Response.End();
    }
}
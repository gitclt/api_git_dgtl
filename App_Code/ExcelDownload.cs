using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using ClosedXML.Excel;

/// <summary>
/// Summary description for ExcelDownload
/// </summary>
public class ExcelDownload
{
    public ExcelDownload()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public void Download(DataTable dt,string name)
    {
        using (XLWorkbook wb = new XLWorkbook())
        {
            string filename = name + ".xlsx";
            var ws = wb.Worksheets.Add(dt, "Sheet1");
            ws.Tables.FirstOrDefault().Theme = XLTableTheme.None;
            ws.Tables.FirstOrDefault().ShowAutoFilter = false;

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "";
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            using (MemoryStream MyMemoryStream = new MemoryStream())
            {
                wb.SaveAs(MyMemoryStream);
                MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }
    }
}
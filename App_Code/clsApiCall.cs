using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Net;

/// <summary>
/// Summary description for clsApiCall
/// </summary>
public class clsApiCall
{
    public clsApiCall()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string ApiCall(string postData, string api)
    {
        string apiUrl = "http://vkcparivar.com/api";

        var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "/"+ api);

        //var postData = "index=0";
        //postData += "&size=50";
        //postData += "&state=Tamilnadu";
        //postData += "&user_id=54849";
        //postData += "&user_type=retailer";
        var Paramdata = Encoding.ASCII.GetBytes(postData);

        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        httpWebRequest.ContentLength = Paramdata.Length;

        using (var stream = httpWebRequest.GetRequestStream())
        {
            stream.Write(Paramdata, 0, Paramdata.Length);
        }

        var result = "";
        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            result = streamReader.ReadToEnd();
        }

        return result;
    }
}
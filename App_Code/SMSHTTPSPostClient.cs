using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
namespace esms_client
{
    public class SMSHttpPostClient
    {


        /// <summary>
        /// Method for sending OTP MSG.
        /// </summary>
        /// <param name="username"> Registered user name</param>
        /// <param name="password"> Valid login password</param>
        /// <param name="senderid">Sender ID </param>
        /// <param name="mobileNo"> valid single Mobile Number </param>
        /// <param name="message">Message Content </param>
        /// <param name="secureKey">Department generate key by login to services portal</param>
// Method for sending OTP MSG.
public String sendOTPMSG(String username, String password, String
senderid, String mobileNo, String message, String secureKey, String tempid)
        {
            Stream dataStream;
            System.Net.ServicePointManager.SecurityProtocol =SecurityProtocolType.Tls;
            HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create("https://msdgweb.mgov.gov.in/esms/sendsmsrequest");
            request.ProtocolVersion = HttpVersion.Version10;
            request.KeepAlive = false;
            request.ServicePoint.ConnectionLimit = 1;

            //((HttpWebRequest)request).UserAgent = ".NET Framework Example";
            ((HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible;MSIE 5.0; Windows 98; DigExt)";
            request.Method = "POST";
            System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
            String encryptedPassword = encryptedPasswod(password);
            String key = hashGenerator(username.Trim(), senderid.Trim(),
            message.Trim(), secureKey.Trim());
            String smsservicetype = "otpmsg"; //For OTP message.
            String query = "username=" +
            HttpUtility.UrlEncode(username.Trim()) +
            "&password=" + HttpUtility.UrlEncode(encryptedPassword) +
"&smsservicetype=" + HttpUtility.UrlEncode(smsservicetype) +
"&content=" + HttpUtility.UrlEncode(message.Trim()) +
"&mobileno=" + HttpUtility.UrlEncode(mobileNo) +
"&senderid=" + HttpUtility.UrlEncode(senderid.Trim()) +
"&key=" + HttpUtility.UrlEncode(key.Trim());
            byte[] byteArray = Encoding.ASCII.GetBytes(query);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            String Status = ((HttpWebResponse)response).StatusDescription;
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            String responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }


        /// <summary>
        /// Method to get Encrypted the password
        /// </summary>
        /// <param name="password"> password as String"</param>
        protected String encryptedPasswod(String password)
        {
            byte[] encPwd = Encoding.UTF8.GetBytes(password);
            //static byte[] pwd = new byte[encPwd.Length];
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA1");
            byte[] pp = sha1.ComputeHash(encPwd);
            // static string result =
            System.Text.Encoding.UTF8.GetString(pp);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in pp)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Method to Generate hash code
        /// </summary>
        /// <param name= "secure_key">your last generated Secure_key </param>
        protected String hashGenerator(String Username, String sender_id,
        String message, String secure_key)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Username).Append(sender_id).Append(message).Append(secure_key);
            byte[] genkey = Encoding.UTF8.GetBytes(sb.ToString());
            //static byte[] pwd = new byte[encPwd.Length];
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA512");
            byte[] sec_key = sha1.ComputeHash(genkey);
            StringBuilder sb1 = new StringBuilder();
            for (int i = 0; i < sec_key.Length; i++)
            {
                sb1.Append(sec_key[i].ToString("x2"));
            }
            return sb1.ToString();
        }
    }
}
class MyPolicy : ICertificatePolicy
{
    public bool CheckValidationResult(ServicePoint srvPoint,
    X509Certificate certificate, WebRequest request, int certificateProblem)
    {
        return true;
    }
}
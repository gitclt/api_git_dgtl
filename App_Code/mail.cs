using System;
using System.Data;
using System.Configuration;
//using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
//using System.Xml.Linq;
using System.Net.Mail;
using System.IO;

/// <summary>
/// Summary description for mail
/// </summary>
public class mail
{
	public mail()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public void sendMail(string to,string sub,string desc)
    {
        try
        {
            string from = "noreplygitgroup@gmail.com";
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            MailAddress tos = new MailAddress(to);
            mail.To.Add(tos);
            mail.From = new MailAddress(from, "Blackboard Online - Admin", System.Text.Encoding.UTF8);
            mail.Subject = "[ Blackboard Online ]" + sub;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            try
            {
                mail.Body += desc;                
            }
            catch (Exception ex)
            {
                mail.Body += desc;  
            }
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();

            client.Credentials = new System.Net.NetworkCredential(from, "globaltechnologies");

            client.Port = 25; // Gmail works on this port
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true; //Gmail works on Server Secured Layer

            client.Send(mail);

        




        }
        finally
        {

        }
    }
}

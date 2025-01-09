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
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for SafeSqlLiteral
/// </summary>
public class SafeSqlLiteral
{
	public SafeSqlLiteral()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string SafeSqlLiterall(System.Object theValue, System.Object theLevel)
    {
        // intLevel represent how thorough the value will be checked for dangerous code
        // intLevel (1) - Do just the basic. This level will already counter most of the SQL injection attacks
        // intLevel (2) -   (non breaking space) will be added to most words used in SQL queries to prevent unauthorized access to the database. Safe to be printed back into HTML code. Don't use for usernames or passwords

        string strValue = (string)theValue;
        int intLevel = (int)theLevel;

        if (strValue != null)
        {
            if (intLevel > 0)
            {
                strValue = strValue.Replace("'", "''"); // Most important one! This line alone can prevent most injection attacks
                strValue = strValue.Replace("--", "");
                strValue = strValue.Replace("[", "[[]");
                strValue = strValue.Replace("%", "[%]");
            }
            if (intLevel > 1)
            {
                string[] myArray = new string[] { "xp_ ", "update ", "insert ", "select ", "drop ", "alter ", "create ", "rename ", "delete ", "replace " };
                int i = 0;
                int i2 = 0;
                int intLenghtLeft = 0;
                for (i = 0; i < myArray.Length; i++)
                {
                    string strWord = myArray[i];
                    Regex rx = new Regex(strWord, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    MatchCollection matches = rx.Matches(strValue);
                    i2 = 0;
                    foreach (Match match in matches)
                    {
                        GroupCollection groups = match.Groups;
                        intLenghtLeft = groups[0].Index + myArray[i].Length + i2;
                        strValue = strValue.Substring(0, intLenghtLeft - 1) + "&nbsp;" + strValue.Substring(strValue.Length - (strValue.Length - intLenghtLeft), strValue.Length - intLenghtLeft);
                        i2 += 5;
                    }
                }
            }
            return strValue;
        }
        else
        {
            return strValue;
        }
    }

    public string SafeSqlLiterall(string p)
    {
        throw new NotImplementedException();
    }
}

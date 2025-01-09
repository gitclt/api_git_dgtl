using System;
using System.Data;
using System.Configuration;
//using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
//using System.Xml.Linq;
using System.Web.UI.WebControls.WebParts;

/// <summary>
/// Summary description for country_BAL
/// </summary>
public class country_BAL
{
	public country_BAL()
	{
		//
		// TODO: Add constructor logic here
		//
	}
   
    public int Insert(string parameters)
    {

        Country_DAL pDAL = new Country_DAL();

        try
        {

            return pDAL.Insert(parameters);

        }

        catch
        {

            throw;

        }

        finally
        {

            pDAL = null;

        }

    }
}

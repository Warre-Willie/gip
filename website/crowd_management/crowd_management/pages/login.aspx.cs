using crowd_management.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace crowd_management.pages;

public partial class login : System.Web.UI.Page
{
    private DbRepository db = new DbRepository();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["logout"] == "true")
            {
                Response.Redirect("Login.aspx");
            }

            // Add 'pb=true' to the URL if it's not already there
            if (Request.QueryString["pb"] != "true")
            {
                UriBuilder uriBuilder = new UriBuilder(Request.Url);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["pb"] = "true";
                uriBuilder.Query = query.ToString();
                Response.Redirect(uriBuilder.ToString());
            }
        }
    }

}
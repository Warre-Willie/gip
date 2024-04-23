﻿using crowd_management.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace crowd_management.pages
{
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
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            tbEmail.Text = tbEmail.Text.Trim().ToUpper();
            tbWW.Text = tbWW.Text.Trim();
            string hashedWW = ComputeSHA256(tbWW.Text);

            DataTable dt = db.SqlExecuteReader($"SELECT * FROM users WHERE email = '{tbEmail.Text}'");

            if (dt.Rows.Count > 0 )
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["password"].ToString() == hashedWW)
                    {
                        Session["User"] = row["username"];
                        Response.Redirect(Session["ReturnURL"].ToString());
                        break;
                    }
                }
                lbError.Visible = true;
            }
        }

        private string ComputeSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
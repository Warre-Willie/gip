using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Org.BouncyCastle.Bcpg;

namespace crowd_management.classes;

public class LoginHandler
{
    private readonly DbRepository _dbRepository = new DbRepository();
    private string _user;

    public string LoginUser(string email, string password)
    {
        string hashedWw = GetHash(password);

        DataTable dt = _dbRepository.SqlExecuteReader($"SELECT * FROM users WHERE email = '{email}'");

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
                if (row["password"].ToString() == hashedWw)
                {
                    _user = row["username"].ToString();
                }
                else
                {
                    _user = null;
                }
        }

        return _user;
    }

    private string GetHash(string input)
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
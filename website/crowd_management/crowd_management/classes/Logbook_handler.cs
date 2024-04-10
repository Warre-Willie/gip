using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace crowd_management.classes
{
    public class Logbook_handler
    {
        private DbRepository db = new DbRepository();

        public void AddLogbookEntry(string category, string user, string description)
        {
            string query = $"INSERT INTO website_logbook (category, user, description) VALUES ({category}, {user}, {description})";
            db.SQLExecute(query);
        }

    }
}
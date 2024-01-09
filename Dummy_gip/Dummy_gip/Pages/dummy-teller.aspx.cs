using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Dummy_gip.Pages
{
    public partial class dummy_teller : System.Web.UI.Page
    {
        //variables
        int count = 0;
        //database connection
        const string constring = "SERVER=localhost;DATABASE=crowd_management;UID=root;PASSWORD=gip-WJ;";
        MySqlConnection conn = new MySqlConnection(constring);

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if(Page.IsPostBack)
            {
                lblCount.Text = "";
            }
        }

        protected void btnIn_Click(object sender, EventArgs e)
        {
            if (Session["counter"] != null)
            {
                count = (int)Session["counter"] + 1;
                try
                {
                    string id = tbId.Text;
                    if(id == "")
                    {
                        throw new Exception("wrong input: ID");
                    }
                    update(1, Convert.ToInt32(tbId.Text));
                }
                catch(Exception ex)
                {
                    lblError.Text = ex.ToString();
                }
                
            }
            
            lblCount.Text = count.ToString();
            Session["counter"] = count;
        }

        protected void btnOut_Click(object sender, EventArgs e)
        {
            if (Session["counter"] != null)
            {
                count = (int)Session["counter"] - 1;

                try
                {
                    string id = tbId.Text;
                    if (id == "")
                    {
                        throw new Exception("wrong input: ID");
                    }
                    update(-1, Convert.ToInt32(tbId.Text));
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.ToString();
                }
            }

            lblCount.Text = count.ToString();
            Session["counter"] = count;
        }

        protected void btnTime_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(tbId.Text);
                int session =Convert.ToInt32(Session["counter"]);
                string query = $"INSERT INTO `zone_population_data`(`zone_id`, `people_count`) VALUES ({id},{Session["counter"]})";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch 
            {
                lblError.Text = "there was a error";
            }
        }

        void update (int value, int id)
        {
            //variables
            int db = 0;

            //select the value from the database
            string query = $"SELECT `people_count` FROM `zones` WHERE id = {id}";
            conn.Open();
            MySqlCommand cmd = new MySqlCommand( query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) 
            {
                db = Convert.ToInt32(reader["people_count"]) + value;
            }
            conn.Close();
            query = $"UPDATE `zones` SET `people_count`={db} WHERE id = {id}"; 
            conn.Open();
            MySqlCommand cmd2 = new MySqlCommand( query, conn);
            cmd2.ExecuteNonQuery();
            conn.Close();
        }

        protected void tbId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int user = Convert.ToInt32(tbId.Text);
                string query = $"SELECT `people_count` FROM `zones` WHERE id = '{user}'";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Session["counter"] = reader["people_count"];
                }
                conn.Close();
            }
            catch
            {

            }
        }

        protected void btnTeller_Click(object sender, EventArgs e)
        {
            Response.Redirect("dummy-teller.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("dummy-search.aspx");
        }
    }
}
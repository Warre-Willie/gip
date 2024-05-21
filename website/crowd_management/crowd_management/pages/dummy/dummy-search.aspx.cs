using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dummy_gip.Pages;

public partial class dummy_search : System.Web.UI.Page
{
    //database connection
    const string constring = "SERVER=localhost;DATABASE=crowd_management;UID=root;PASSWORD=gip-WJ;";
    MySqlConnection conn = new MySqlConnection(constring);

    protected void Page_Load(object sender, EventArgs e)
    {
            
    }

    protected void bttSearch_Click(object sender, EventArgs e)
    {
        //varibles
        string input;

        lblError.Text = "";

        try
        {
            input = tbTknb.Text;

            if(input == "")
            {
                throw new Exception("Please fill in an ticket");
            }

            getTicket(input);
        }
        catch(Exception ex) 
        {
            lblError.Text = ex.Message;
        }
        tbTknb.Text = "";
    }

    void getTicket (string ticketNr)
    {
        //variables
        string RFID, camping, VIP, backstage, artiest;

        string query = $"SELECT `RFID`,`camping`, `VIP`, `backstage`, `artiest` FROM `tickets` WHERE `barcode` = '{ticketNr}'";
        conn.Open();
        MySqlCommand cmd = new MySqlCommand(query, conn);
        MySqlDataReader reader = cmd.ExecuteReader();
           
        if(reader.HasRows) 
        {
            while (reader.Read())
            {
                RFID = reader["RFID"].ToString();

                if (RFID == "")
                {
                    tbOutput.Text += $"{ticketNr}: niet gescand.\n";
                }
                else
                {
                    tbOutput.Text += $"{ticketNr}: gescand --> camping:{reader["camping"]}, VIP:{reader["VIP"]}, backstage:{reader["backstage"]}, artiest:{reader["artiest"]}\n";
                }

            }

            conn.Close();
        }
        else
        {
            throw new Exception("Barcode niet gevonden");
        }
    }

    protected void btnTeller_Click(object sender, EventArgs e)
    {
        Response.Redirect("dummy-teller.aspx");
    }

    protected void btnZoekenPG_Click(object sender, EventArgs e)
    {
        Response.Redirect("dummy-search.aspx");
    }
}
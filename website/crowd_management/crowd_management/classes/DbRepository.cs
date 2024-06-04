/*
 * File: DbReposetory.cs
 * Author: Warre Willeme & Jesse UijtdeHaag
 * Date: 12-05-2024
 * Description: This file contains the DbRepository class. This class is used to connect to the database and execute queries.
 */

using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace crowd_management.classes;

public class DbRepository
{
  #region Variables and accessors

  private const string ConnString = "SERVER=192.168.0.101;DATABASE=crowd_management;UID=gip;PASSWORD=gip-WJ;";
  private readonly MySqlConnection _conn;
  private readonly MySqlCommand _cmd = new MySqlCommand();

  #endregion

  #region Methods

  public DbRepository()
  {
    try
    {
      _conn = new MySqlConnection(ConnString);
      _conn.Open();
    }
    catch (Exception e)
    {
      // ignored
    }
  }

  public void Dispose()
  {
    if (_conn != null)
    {
      _conn.Close();
      _conn.Dispose();
    }
  }

  public DataTable SqlExecuteReader(string query)
  {
    DataTable dt = new DataTable();

    try
    {
      _cmd.Connection = _conn;
      _cmd.CommandText = query;

      MySqlDataReader reader = _cmd.ExecuteReader();
      dt.Load(reader);
    }
    catch (Exception ex)
    {
      // ignored
    }

    return dt;
  }

  public void SqlExecute(string query)
  {
    try
    {
      _cmd.Connection = _conn;
      _cmd.CommandText = query;
      _cmd.ExecuteNonQuery();
    }
    catch (Exception ex)
    {
      // ignored
    }
  }

  #endregion
}
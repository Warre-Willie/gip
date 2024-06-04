/*
 * File: DbReposetory.cs
 * Author: Warre Willeme & Jesse UijtdeHaag
 * Date: 12-05-2024
 * Description: This file contains the DbRepository class. This class is used to connect to the database and execute queries.
 */

using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace crowd_management_live;

public class DbRepository
{
	#region Variables

	private const string ConnString = "SERVER=192.168.0.101;DATABASE=crowd_management;UID=gip;PASSWORD=gip-WJ;";
	private readonly MySqlConnection _conn;
	private readonly MySqlCommand _cmd = new MySqlCommand();

	#endregion

	#region Methods

	public DbRepository()
	{
		_conn = new MySqlConnection(ConnString);
		_conn.Open();
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
			throw new Exception(ex.ToString());
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
			throw new Exception(ex.ToString());
		}
	}

	#endregion
}
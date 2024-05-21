/*
* File: LogbookHandler.cs
* Author: Warre Willeme & Jesse UijtdeHaag
* Date: 12-05-2024
* Description: This file contains the LogbookHandler class. This class is used to add logbook entries to the database.
*/

using System;

namespace crowd_management.classes;

public class LogbookHandler
{
	#region Variables

	private readonly DbRepository _dbRepository = new DbRepository();

	#endregion

	#region Methods

	public void AddLogbookEntry(string category, string user, string description)
	{
		try // try to add the logbook entry to the database
		{
			string query = $"INSERT INTO website_logbook(category, user, description) VALUES ('{category}','{user}','{description}')";
			_dbRepository.SqlExecute(query);
		}
		catch (Exception ex) // if an exception occurs, write it to the console
		{
			Console.WriteLine(ex.ToString());
		}
	}

	#endregion
}
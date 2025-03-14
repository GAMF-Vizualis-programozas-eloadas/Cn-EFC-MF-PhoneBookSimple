using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PhoneBook;
namespace CnEFMF_PhoneBook
{
	class Program
	{	
		static void Main(string[] args)
		{	Console.WriteLine("Simple phonebook");
			cnPhoneBook cn = new cnPhoneBook();
			cn.Database.EnsureCreated();
			if (!cn.People.Any())
			{	var p1 = new Person { Name = "John Doe", Remark = "Test" };
				var p2 = new Person { Name = "Jane Smith", Remark = "" };
				var n1 = new Number { NumberString = "+36991234567" };
				var n2 = new Number { NumberString = "+36991234568" };
				var n3 = new Number { NumberString = "+36991234568" };
				p1.Numbers.Add(n1); p1.Numbers.Add(n3);
				p2.Numbers.Add(n2);	p2.Numbers.Add(n3);
				n1.People.Add(p1); n2.People.Add(p2);
				n3.People.Add(p1); n3.People.Add(p2);
				cn.People.Add(p1); cn.People.Add(p2);
				cn.Numbers.AddRange([n1,n2,n3]); // Collection expression
				try
				{	cn.SaveChanges();
					Console.WriteLine("All data saved to the database!");
					DetachDb(cn);
				}
				catch (Exception exc)
				{	Console.WriteLine(exc.Message); 
				}
			}
		}

		private static void DetachDb(cnPhoneBook cn)
		{
			cn.Database.OpenConnection();
			var connection = cn.Database.GetDbConnection();
			if (connection.State == System.Data.ConnectionState.Closed)
			{	connection.Open();
			}
			string[] commands =
			{ "USE master",
				$"ALTER DATABASE [{connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE",
				$"ALTER DATABASE [{connection.Database}] SET OFFLINE WITH ROLLBACK IMMEDIATE",
				$"EXEC sp_detach_db '{connection.Database}'"
			};
			using (var sqlCommand = new SqlCommand())
			{	sqlCommand.Connection = connection as SqlConnection;
				foreach (string command in commands)
				{	sqlCommand.CommandText = command;
					sqlCommand.ExecuteNonQuery();
				}
			}
		}

	}
}

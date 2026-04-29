using Microsoft.Data.Sqlite;
using QueueApp.Models;
using BCrypt.Net;

namespace QueueApp.Data;

public static class DatabaseContext
{
    private const string ConnectionString = "Data Source=queue.db";

    public static void Initialize()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = 
        @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                Role INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Counters (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CounterNumber INTEGER NOT NULL,
                StaffName TEXT,
                IsActive INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS Tickets (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TicketNumber INTEGER NOT NULL,
                CustomerName TEXT,
                CustomerPhone TEXT,
                Status INTEGER NOT NULL,
                CounterId INTEGER,
                CreatedAt DATETIME NOT NULL,
                CalledAt DATETIME,
                FOREIGN KEY (CounterId) REFERENCES Counters(Id)
            );
        ";
        command.ExecuteNonQuery();

        // Check if StaffName column exists, add if not
        command.CommandText = "PRAGMA table_info(Counters)";
        using (var reader = command.ExecuteReader())
        {
            bool hasStaffName = false;
            while (reader.Read())
            {
                if (reader.GetString(1) == "StaffName")
                {
                    hasStaffName = true;
                    break;
                }
            }
            if (!hasStaffName)
            {
                var alterCmd = connection.CreateCommand();
                alterCmd.CommandText = "ALTER TABLE Counters ADD COLUMN StaffName TEXT";
                alterCmd.ExecuteNonQuery();
            }
        }

        // Seed Admin if none exist
        command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
        var userCount = Convert.ToInt32(command.ExecuteScalar());
        if (userCount == 0)
        {
            var seedUserCmd = connection.CreateCommand();
            seedUserCmd.CommandText = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@user, @pass, @role)";
            seedUserCmd.Parameters.AddWithValue("@user", "admin");
            seedUserCmd.Parameters.AddWithValue("@pass", BCrypt.Net.BCrypt.HashPassword("admin123"));
            seedUserCmd.Parameters.AddWithValue("@role", (int)UserRole.Admin);
            seedUserCmd.ExecuteNonQuery();
        }

        // Seed counters if none exist
        command.CommandText = "SELECT COUNT(*) FROM Counters";
        var count = Convert.ToInt32(command.ExecuteScalar());
        if (count == 0)
        {
            for (int i = 1; i <= 5; i++)
            {
                var seedCmd = connection.CreateCommand();
                seedCmd.CommandText = "INSERT INTO Counters (CounterNumber, IsActive) VALUES (@num, 1)";
                seedCmd.Parameters.AddWithValue("@num", i);
                seedCmd.ExecuteNonQuery();
            }
        }
    }

    public static SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}

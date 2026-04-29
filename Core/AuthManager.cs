using Microsoft.Data.Sqlite;
using QueueApp.Data;
using QueueApp.Models;
using BCrypt.Net;

namespace QueueApp.Core;

public static class AuthManager
{
    public static User? Login(string username, string password)
    {
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Users WHERE Username = @user";
        cmd.Parameters.AddWithValue("@user", username);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var user = new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Role = (UserRole)reader.GetInt32(3)
            };

            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
        }
        return null;
    }

    public static void Register(string username, string password, UserRole role)
    {
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@user, @pass, @role)";
        cmd.Parameters.AddWithValue("@user", username);
        cmd.Parameters.AddWithValue("@pass", BCrypt.Net.BCrypt.HashPassword(password));
        cmd.Parameters.AddWithValue("@role", (int)role);
        cmd.ExecuteNonQuery();
    }
}

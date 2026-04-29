using Microsoft.Data.Sqlite;
using QueueApp.Data;
using QueueApp.Models;

namespace QueueApp.Core;

public static class QueueManager
{
    public static int GenerateTicket(string name, string phone)
    {
        using var connection = DatabaseContext.GetConnection();
        
        var lastNumCmd = connection.CreateCommand();
        lastNumCmd.CommandText = "SELECT MAX(TicketNumber) FROM Tickets WHERE date(CreatedAt) = date('now')";
        var result = lastNumCmd.ExecuteScalar();
        int nextNumber = (result == DBNull.Value || result == null) ? 1001 : Convert.ToInt32(result) + 1;

        var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = 
            "INSERT INTO Tickets (TicketNumber, CustomerName, CustomerPhone, Status, CreatedAt) VALUES (@num, @name, @phone, @status, @created)";
        insertCmd.Parameters.AddWithValue("@num", nextNumber);
        insertCmd.Parameters.AddWithValue("@name", name);
        insertCmd.Parameters.AddWithValue("@phone", phone);
        insertCmd.Parameters.AddWithValue("@status", (int)TicketStatus.Waiting);
        insertCmd.Parameters.AddWithValue("@created", DateTime.Now);
        insertCmd.ExecuteNonQuery();

        return nextNumber;
    }

    public static List<Ticket> GetWaitingTickets()
    {
        var tickets = new List<Ticket>();
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Tickets WHERE Status = @status ORDER BY CreatedAt ASC";
        cmd.Parameters.AddWithValue("@status", (int)TicketStatus.Waiting);
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tickets.Add(MapTicket(reader));
        }
        return tickets;
    }

    public static Ticket? GetNextWaitingTicket()
    {
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Tickets WHERE Status = @status ORDER BY CreatedAt ASC LIMIT 1";
        cmd.Parameters.AddWithValue("@status", (int)TicketStatus.Waiting);
        
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapTicket(reader);
        }
        return null;
    }

    public static void CallTicket(int ticketId, int counterId)
    {
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = 
            "UPDATE Tickets SET Status = @status, CounterId = @counterId, CalledAt = @calledAt WHERE Id = @id";
        cmd.Parameters.AddWithValue("@status", (int)TicketStatus.Calling);
        cmd.Parameters.AddWithValue("@counterId", counterId);
        cmd.Parameters.AddWithValue("@calledAt", DateTime.Now);
        cmd.Parameters.AddWithValue("@id", ticketId);
        cmd.ExecuteNonQuery();
    }

    public static void CompleteTicket(int ticketId)
    {
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE Tickets SET Status = @status WHERE Id = @id";
        cmd.Parameters.AddWithValue("@status", (int)TicketStatus.Served);
        cmd.Parameters.AddWithValue("@id", ticketId);
        cmd.ExecuteNonQuery();
    }

    public static void DeleteTicket(int ticketId)
    {
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Tickets WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", ticketId);
        cmd.ExecuteNonQuery();
    }

    public static void UpdateTicket(int ticketId, string name, string phone)
    {
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE Tickets SET CustomerName = @name, CustomerPhone = @phone WHERE Id = @id";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@phone", phone);
        cmd.Parameters.AddWithValue("@id", ticketId);
        cmd.ExecuteNonQuery();
    }

    public static void ResetQueue()
    {
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Tickets";
        cmd.ExecuteNonQuery();
    }

    public static List<Counter> GetCounters()
    {
        var counters = new List<Counter>();
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, CounterNumber, StaffName, IsActive FROM Counters WHERE IsActive = 1";
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            counters.Add(new Counter {
                Id = reader.GetInt32(0),
                CounterNumber = reader.GetInt32(1),
                StaffName = reader.IsDBNull(2) ? null : reader.GetString(2),
                IsActive = reader.GetInt32(3) == 1
            });
        }
        return counters;
    }

    public static void UpdateCounterStaff(int counterId, string staffName)
    {
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE Counters SET StaffName = @staffName WHERE Id = @id";
        cmd.Parameters.AddWithValue("@staffName", staffName);
        cmd.Parameters.AddWithValue("@id", counterId);
        cmd.ExecuteNonQuery();
    }


    public static List<Ticket> GetCallingTickets()
    {
        var tickets = new List<Ticket>();
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Tickets WHERE Status = @status ORDER BY CalledAt DESC";
        cmd.Parameters.AddWithValue("@status", (int)TicketStatus.Calling);
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tickets.Add(MapTicket(reader));
        }
        return tickets;
    }

    public static List<Ticket> GetServedTickets()
    {
        var tickets = new List<Ticket>();
        using var connection = DatabaseContext.GetConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Tickets WHERE Status = @status ORDER BY CalledAt DESC";
        cmd.Parameters.AddWithValue("@status", (int)TicketStatus.Served);
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tickets.Add(MapTicket(reader));
        }
        return tickets;
    }

    private static Ticket MapTicket(SqliteDataReader reader)
    {
        return new Ticket
        {
            Id = reader.GetInt32(0),
            TicketNumber = reader.GetInt32(1),
            CustomerName = reader.IsDBNull(2) ? null : reader.GetString(2),
            CustomerPhone = reader.IsDBNull(3) ? null : reader.GetString(3),
            Status = (TicketStatus)reader.GetInt32(4),
            CounterId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
            CreatedAt = reader.GetDateTime(6),
            CalledAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        };
    }
}

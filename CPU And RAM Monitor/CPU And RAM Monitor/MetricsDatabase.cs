using System;
using System.Collections.Generic;
using System.Data.SQLite;

public class MetricsDatabase
{
    private const string ConnectionString = "Data Source=MetricsDatabase.db;Version=3;";

    public MetricsDatabase()
    {
        try
        {
            InitializeDatabase();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing the database: {ex.Message}");
            
        }
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Metrics (Timestamp TEXT, CPUUsage REAL, RAMUsage REAL)";
                command.ExecuteNonQuery();
            }
        }
    }

    public void InsertMetrics(double cpuUsage, double ramUsage)
    {
        try
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "INSERT INTO Metrics (Timestamp, CPUUsage, RAMUsage) VALUES (@Timestamp, @CPUUsage, @RAMUsage)";
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@CPUUsage", cpuUsage);
                    command.Parameters.AddWithValue("@RAMUsage", ramUsage);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting metrics into the database: {ex.Message}");

        }
    }
    public List<string[]> GetMetrics()
    {
        List<string[]> metricsList = new List<string[]>();

        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT Timestamp, CPUUsage, RAMUsage FROM Metrics";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string timestamp = reader["Timestamp"].ToString();
                        string cpuUsage = reader["CPUUsage"].ToString();
                        string ramUsage = reader["RAMUsage"].ToString();

                        metricsList.Add(new string[] { timestamp, cpuUsage, ramUsage });
                    }
                }
            }
        }
        return metricsList;
    }
}

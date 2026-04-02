using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace eft_where_am_i.Classes
{
    public class QuestRepository
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public QuestRepository()
        {
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "quest_saves.db");
            _connectionString = $"Data Source={_dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS quests (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        map_name TEXT NOT NULL,
                        quest_name TEXT NOT NULL,
                        UNIQUE(map_name, quest_name)
                    )";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestRepository] Error initializing database: {ex.Message}");
            }
        }

        public void AddQuest(string mapName, string questName)
        {
            try
            {
                Console.WriteLine($"[QuestRepository] AddQuest: map={mapName}, quest={questName}");

                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT OR IGNORE INTO quests (map_name, quest_name)
                    VALUES ($mapName, $questName)";
                command.Parameters.AddWithValue("$mapName", mapName);
                command.Parameters.AddWithValue("$questName", questName);
                int rows = command.ExecuteNonQuery();
                Console.WriteLine($"[QuestRepository] AddQuest result: {rows} rows affected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestRepository] Error adding quest: {ex.Message}");
            }
        }

        public void RemoveQuest(string mapName, string questName)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    DELETE FROM quests
                    WHERE map_name = $mapName AND quest_name = $questName";
                command.Parameters.AddWithValue("$mapName", mapName);
                command.Parameters.AddWithValue("$questName", questName);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestRepository] Error removing quest: {ex.Message}");
            }
        }

        public List<string> GetQuests(string mapName)
        {
            var quests = new List<string>();
            try
            {
                Console.WriteLine($"[QuestRepository] GetQuests: map={mapName}");

                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT quest_name FROM quests
                    WHERE map_name = $mapName";
                command.Parameters.AddWithValue("$mapName", mapName);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    quests.Add(reader.GetString(0));
                }

                Console.WriteLine($"[QuestRepository] GetQuests result: {quests.Count} quests found");
                foreach (var q in quests)
                    Console.WriteLine($"[QuestRepository]   - {q}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestRepository] Error getting quests: {ex.Message}");
            }
            return quests;
        }
    }
}

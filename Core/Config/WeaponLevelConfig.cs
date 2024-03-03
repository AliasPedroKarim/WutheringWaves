using System.Data.SQLite;

namespace Core.Config;

public class WeaponLevelConfig
{
    private static SQLiteConnection GetConnection()
    {
        return new SQLiteConnection("Data Source=data/config/weapon/db_weapon.db");
    }


    public static byte[] WeaponLevelById(int levelId, int level)
    {
        byte[]? binData = null;
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT BinData FROM WeaponLevel WHERE LevelId = @LevelId AND Level = @Level", connection))
            {
                command.Parameters.AddWithValue("@LevelId", levelId);
                command.Parameters.AddWithValue("@Level", level);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        binData = (byte[])reader["BinData"];
                    }
                }
            }
        }
        return binData;
    }

    public static byte[] WeaponExpById(int id)
    {
        byte[]? binData = null;
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT BinData FROM WeaponExpItem WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        binData = (byte[])reader["BinData"];
                    }
                }
            }
        }
        return binData;
    }
}
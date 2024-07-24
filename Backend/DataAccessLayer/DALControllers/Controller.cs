using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DALControllers
{
    internal abstract class Controller
    {
        internal string _connectionString { get; set; }
        internal string _tableName { get; set; }

        internal Controller()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this._connectionString = $"Data Source={path}; Version=3;";
        }




        internal List<Object> SelectAll()
        {
            List<Object> list = new List<Object>();
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {_tableName};";
                SQLiteDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(ConvertReaderToObject(reader));
                    }
                }
                catch (Exception ex)
                {
                    Logger.GetLog().Error($"Select All of table {_tableName} in Controller failed");
                    throw new DataMisalignedException();
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            Logger.GetLog().Info($"SelectAll from table {_tableName} has been done successfully");
            return list;
        }




        internal abstract Object ConvertReaderToObject(SQLiteDataReader reader);

        internal abstract void Insert(Object dao);

        internal void DeleteAll()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {_tableName};"
                };
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Logger.GetLog().Info($"DeleteAll of table {_tableName} has been done successfully");
                }
                catch (Exception ex)
                {
                    Logger.GetLog().Error($"DeleteAll of table {_tableName} in Controller failed");
                    throw new DataMisalignedException();
                }

            }
        }









    }
}

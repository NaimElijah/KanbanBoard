using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DALControllers
{
    internal class ColumnController : Controller
    {
        private const string TableName = "Columns";

        private const string ColumnNameCol = "ColumnName";
        private const string SourceBoardIdCol = "SourceBoardId";
        private const string TasksLimitCol = "TasksLimit";

        internal ColumnController() : base()
        {
            _tableName = TableName;
        }




        internal List<ColumnDAO> SelectColumns(string BoardIdAttributeName, object BoardIdvalue)  //   this needs to return a list of the columns of a board.
        {
            List<ColumnDAO> list = new List<ColumnDAO>();
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {TableName} WHERE {BoardIdAttributeName} = @boardId;";

                command.Parameters.AddWithValue("@boardId", BoardIdvalue);

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
                    Logger.GetLog().Error($"Select Columns of board with id {BoardIdvalue} in the database failed");
                    throw new DataMisalignedException();
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            Logger.GetLog().Info($"SelectColumns of board with id {BoardIdvalue} was done successfully");
            return list;

        }


        internal override ColumnDAO ConvertReaderToObject(SQLiteDataReader reader)   // <<-------------  helper conversion function(for kinds of selects)
        {
            return new ColumnDAO(reader.GetString(0), (long)reader.GetValue(1), (long)reader.GetValue(2));
        }







        internal override void Insert(object dao)
        {
            if (dao is ColumnDAO columnDAO)
            {
                using (var connection = new SQLiteConnection(this._connectionString))
                {
                    try
                    {
                        SQLiteCommand command = new SQLiteCommand(null, connection);
                        string insert =
                            $"INSERT INTO {TableName} ({ColumnNameCol}, {SourceBoardIdCol}, {TasksLimitCol}) "
                            + $"VALUES (@columnName, @boardID, @tasksLimit);";

                        SQLiteParameter columnName = new SQLiteParameter("@columnName", columnDAO.Name);
                        SQLiteParameter boardID = new SQLiteParameter("@boardID", columnDAO.SourceBoardId);
                        SQLiteParameter tasksLimit = new SQLiteParameter("@tasksLimit", columnDAO.TasksLimit);


                        command.CommandText = insert;
                        command.Parameters.Add(columnName);
                        command.Parameters.Add(boardID);
                        command.Parameters.Add(tasksLimit);


                        connection.Open();
                        command.ExecuteNonQuery();
                        Logger.GetLog().Info("Insertion of column Successful in database");
                    }
                    catch (Exception ex)
                    {
                        Logger.GetLog().Error("Inserting column in database failed");
                        throw new DataMisalignedException();
                    }

                }
            }
        }









        internal void UpdateColumn(long boardId, string columnName, string attributeName, object value)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"UPDATE {TableName} SET {attributeName} = @val WHERE {SourceBoardIdCol} = @boardId AND {ColumnNameCol} = @columnName;"
                };
                try
                {
                    command.Parameters.AddWithValue("@boardId", boardId);
                    command.Parameters.AddWithValue("@columnName", columnName);
                    command.Parameters.AddWithValue("@val", value);
                    connection.Open();
                    command.ExecuteNonQuery();
                    Logger.GetLog().Info($"UpdateColumn of column {columnName} of board with id {boardId} was done successfully in database");
                }
                catch (Exception ex)
                {
                    Logger.GetLog().Error($"UpdateColumn of column {columnName} of board with id {boardId} in database failed");
                    throw new DataMisalignedException();
                }

            }
        }







        internal void DeleteColumn(long boardId, string columnName)
        {
            // deletes a specified column from the table
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {TableName} WHERE {SourceBoardIdCol} = @boardID AND {ColumnNameCol} = @columnName;"
                };
                try
                {
                    command.Parameters.AddWithValue("@boardID", boardId);
                    command.Parameters.AddWithValue("@columnName", columnName);
                    connection.Open();
                    command.ExecuteNonQuery();
                    Logger.GetLog().Info($"DeleteColumn of column {columnName} of board with id {boardId} Successful in database");
                }
                catch (Exception ex)
                {
                    Logger.GetLog().Error($"DeleteColumn of column {columnName} of board with id {boardId} in database failed");
                    throw new DataMisalignedException();
                }
                
            }

        }







    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DALControllers
{
    internal class BoardController : Controller
    {
        private const string TableName = "Boards";

        internal const string BoardIdCol = "BoardId";
        internal const string BoardOwnerEmailCol = "BoardOwnerEmail";
        internal const string BoardNameCol = "BoardName";
        internal const string BoardTaskCountCol = "CounterForTaskIdInBoard";

        internal BoardController() : base()
        {
            _tableName = TableName;
        }

        internal override BoardDAO ConvertReaderToObject(SQLiteDataReader reader)
        {
            return new BoardDAO((long)reader.GetValue(0), reader.GetString(1), reader.GetString(2), (long)reader.GetValue(3));
        }

        internal override void Insert(Object boardDao)
        {
            if (boardDao is BoardDAO board)
            {
                using (var connection = new SQLiteConnection(this._connectionString))
                {
                    try
                    {
                        SQLiteCommand command = new SQLiteCommand(null, connection);

                        string insert = $"INSERT INTO {TableName} ({BoardIdCol}, {BoardOwnerEmailCol}, {BoardNameCol}, {BoardTaskCountCol}) "
                                        + $"VALUES (@BoardId, @BoardOwnerEmail, @Name, @CounterForTaskIdInBoard);";

                        SQLiteParameter idPar = new SQLiteParameter("@BoardId", board.BoardId);
                        SQLiteParameter emailPar = new SQLiteParameter("@BoardOwnerEmail", board._BoardOwnerEmail);
                        SQLiteParameter namePar = new SQLiteParameter("@Name", board.Name);
                        SQLiteParameter counterPar = new SQLiteParameter("@CounterForTaskIdInBoard", board.CounterForTaskIdInBoard);

                        command.CommandText = insert;
                        command.Parameters.Add(idPar);
                        command.Parameters.Add(emailPar);
                        command.Parameters.Add(namePar);
                        command.Parameters.Add(counterPar);

                        connection.Open();

                        command.ExecuteNonQuery();
                        Logger.GetLog().Info("A new board insertion to table in database");
                    }
                    catch (Exception ex) 
                    { 
                        Logger.GetLog().Error("board insertion failed in database");
                        throw new DataMisalignedException();
                    }

                }
            }
        }




        internal void UpdateBoard(long boardId, string attributeName, object value)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"UPDATE {TableName} SET {attributeName} = @val WHERE {BoardIdCol} = {boardId};"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter("@val", value));
                    connection.Open();

                    command.ExecuteNonQuery();
                    Logger.GetLog().Info($"Board with id {boardId} has been updated in database");
                }
                catch (Exception ex) 
                {
                    Logger.GetLog().Error($"board update of board with id {boardId} failed in database");
                    throw new DataMisalignedException();
                }
            }
        }





        internal void DeleteBoard(long boardId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {TableName} WHERE {BoardIdCol} = @BoardID;" // should delete line
                };
                try
                {
                    command.Parameters.AddWithValue("@BoardID", boardId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    Logger.GetLog().Info($"a board with board id {boardId} has been deleted");
                }
                catch (Exception ex)
                {
                    Logger.GetLog().Error($"board deletion of board with id {boardId} failed in database");
                    throw new DataMisalignedException();
                }

            }
        }




    }
}

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
    internal class BoardsMembersController : Controller
    {
        private const string TableName = "BoardsMembers";

        private const string BoardIdCol = "BoardId";
        private const string MemberEmailCol = "MemberEmail";

        internal BoardsMembersController() : base()
        {
            _tableName = TableName;
        }


        // returns a list of the BoardsMembersDaos that are in that board with that board Id, for Load Data.
        internal List<BoardsMembersDAO> SelectBoardsMembers(string BoardIdAttributeName, long BoardIdvalue)
        {
            List<BoardsMembersDAO> list = new List<BoardsMembersDAO>();
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
                    Logger.GetLog().Error("SelectBoardsMembers failed in database");
                    throw new DataMisalignedException();
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            Logger.GetLog().Info("returning successfully with SelectBoardsMembers from the database");
            return list;

        }





        internal override BoardsMembersDAO ConvertReaderToObject(SQLiteDataReader reader)   // <<-------------  helper conversion function(for kinds of selects)
        {
            return new BoardsMembersDAO((long)reader.GetValue(0), reader.GetString(1));
        }





        internal override void Insert(object dao)
        {
            if (dao is BoardsMembersDAO boardMemberDao)
            {
                using (var connection = new SQLiteConnection(this._connectionString))
                {
                    try
                    {
                        SQLiteCommand command = new SQLiteCommand(null, connection);
                        string insert = $"INSERT INTO {TableName} ({BoardIdCol}, {MemberEmailCol}) " + $"VALUES (@boardID, @memberEmail);";

                        SQLiteParameter boardID = new SQLiteParameter("@boardID", boardMemberDao.BoardId);
                        SQLiteParameter memberEmail = new SQLiteParameter("@memberEmail", boardMemberDao.MemberEmail);
                        

                        command.CommandText = insert;
                        command.Parameters.Add(boardID);
                        command.Parameters.Add(memberEmail);
                       

                        connection.Open();
                        command.ExecuteNonQuery();
                        Logger.GetLog().Info("Inserting in BoardsMembers successful in the database");
                    }
                    catch (Exception ex)
                    {
                        Logger.GetLog().Error("Insertion of board member failed in database");
                        throw new DataMisalignedException();
                    }
                }
            }
        }



        internal void DeleteBoardMember(long boardId, string memberEmail)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {TableName} WHERE {BoardIdCol} = @boardID AND {MemberEmailCol} = @memberEmail;"
                };
                try
                {
                    command.Parameters.AddWithValue("@boardID", boardId);
                    command.Parameters.AddWithValue("@memberEmail", memberEmail);
                    connection.Open();
                    command.ExecuteNonQuery();
                    Logger.GetLog().Info($"DeleteBoardMember in the database with board id {boardId} and memberEmail {memberEmail} successful");
                }
                catch (Exception ex)
                {
                    Logger.GetLog().Error($"DeleteBoardMember in the database with board id {boardId} and memberEmail {memberEmail} failed");
                    throw new DataMisalignedException();
                }
            }
        }




        internal void DeleteBoardMembersByBoardId(long boardId)  //  delete all the board members with this boardId from the table
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {TableName} WHERE {BoardIdCol} = @boardID;"
                };
                try
                {
                    command.Parameters.AddWithValue("@boardID", boardId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    Logger.GetLog().Info($"DeleteBoardMembersByBoardId in database made with board id {boardId} successfully");
                }
                catch (Exception ex)
                {
                    Logger.GetLog().Error($"Delete Board Members by BoardId {boardId} failed in database");
                    throw new DataMisalignedException();
                }
                
            }






        }
    }
}

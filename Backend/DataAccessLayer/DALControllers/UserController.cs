using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;
using System.Data.SQLite;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DALControllers
{
    internal class UserController : Controller
    {
        private const string TableName = "Users";

        private const string UsersEmailCol = "Email";
        private const string UsersPasswordCol = "Password";

        internal UserController() : base()
        {
            _tableName = TableName;
        }

        internal override UserDAO ConvertReaderToObject(SQLiteDataReader reader)   // <<-------------  helper conversion function(for kinds of selects)
        {
            return new UserDAO(reader.GetString(0), reader.GetString(1));
        }



        /// <summary>
        /// this method inserts a user to the Users table in the database
        /// </summary>
        /// <returns>doesn't return anything, only makes changes</returns>
        internal override void Insert(object dao)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(null, connection);
                    string insertSQLCode = "INSERT INTO " + TableName + " (" + UsersEmailCol + ", " + UsersPasswordCol + ") VALUES (@UserEmail, @UserPassword);";

                    UserDAO UserDao = (UserDAO)dao;
                    SQLiteParameter emailParam = new SQLiteParameter("@UserEmail", UserDao.Email);
                    SQLiteParameter passwordParam = new SQLiteParameter("@UserPassword", UserDao.Password);
                    command.CommandText = insertSQLCode;
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(passwordParam);

                    connection.Open();
                    command.ExecuteNonQuery();
                    Logger.GetLog().Info($"Insert into table {TableName} has been done successfully");
                }
                catch (Exception e)
                {
                    Logger.GetLog().Error($"Insert into table {TableName} has failed");
                    throw new DataMisalignedException();
                }
            }
        }







        /// <summary>
        /// this method updates a user's certain data, based on the email provided. It updates in the database.
        /// </summary>
        /// <returns>doesn't return anything, only makes changes</returns>
        internal void UpdateUser(string email, string attributeName, object newValue)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(null, connection);
                    string updateSQLCode =
                        $"UPDATE {TableName} SET {attributeName} = @NewVal WHERE {UsersEmailCol} = {email};";

                    SQLiteParameter valParam = new SQLiteParameter("@NewVal", newValue);
                    command.CommandText = updateSQLCode;
                    command.Parameters.Add(valParam);

                    connection.Open();
                    command.ExecuteNonQuery();
                    Logger.GetLog().Info($"UpdateUser for user {email} for table {TableName} has been done successfully");
                }
                catch
                {
                    Logger.GetLog().Error($"UpdateUser for user {email} for table {TableName} has failed");
                    throw new DataMisalignedException();
                }
            }

        }








    }
}

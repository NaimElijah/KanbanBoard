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
    internal class TaskController : Controller
    {
        private const string TableName = "Tasks";

        private const string TaskIdInBoardCol = "TaskIdInBoard";
        private const string SourceBoardIdCol = "SourceBoardId";
        private const string SourceColumnNameCol = "SourceColumnName";
        private const string TaskAssigneeEmailCol = "TaskAssigneeEmail";
        private const string TitleCol = "Title";
        private const string DescriptionCol = "Description";
        private const string CreationTimeCol = "CreationTime";
        private const string DueDateCol = "DueDate";

        internal TaskController() : base()
        {
            _tableName = TableName;
        }




        internal List<TaskDAO> SelectTasks(string AttributeBoardIdName, long BoardIdvalue, string AttributeColumnName, string ColNameValue)
        {
            List<TaskDAO> list = new List<TaskDAO>();
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {TableName} WHERE {AttributeBoardIdName} = @boardId AND {AttributeColumnName} = @columnName;";
                
                command.Parameters.AddWithValue("@boardId", BoardIdvalue);
                command.Parameters.AddWithValue("@columnName", ColNameValue);

                SQLiteDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(ConvertReaderToObject(reader));
                    }

                    Logger.GetLog().Info("selection of tasks from database successful");
                }
                catch
                {
                    Logger.GetLog().Error("selection of tasks from database failed");
                    throw new DataMisalignedException();
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            return list;
        }


        internal override TaskDAO ConvertReaderToObject(SQLiteDataReader reader)  
        {
            return new TaskDAO((long)reader.GetValue(0), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetDateTime(6), reader.GetDateTime(7), (long)reader.GetValue(1));
        }


        internal override void Insert(Object taskDao)
        {
            if (taskDao is TaskDAO task)
            {
                using (var connection = new SQLiteConnection(this._connectionString))
                {
                    try
                    {
                        SQLiteCommand command = new SQLiteCommand(null, connection);
                        string insert =
                            $"INSERT INTO {TableName} ({TaskIdInBoardCol}, {SourceBoardIdCol},{SourceColumnNameCol}, {TaskAssigneeEmailCol}, {TitleCol}, {DescriptionCol}, {CreationTimeCol}, {DueDateCol}) "
                            + $"VALUES (@TaskId, @boardId, @ColumnName, @Assignee, @TaskName, @TaskDesc, @TaskCreDate, @TaskDueDate);";

                        SQLiteParameter idPar = new SQLiteParameter("@TaskId", task.Id);
                        SQLiteParameter BoardidPar = new SQLiteParameter("@boardId", task.SourceBoardId);
                        SQLiteParameter colPar = new SQLiteParameter("@ColumnName", task._SourceColumnName);
                        SQLiteParameter assigneePar = new SQLiteParameter("@Assignee", task._AssigneeEmail);
                        SQLiteParameter namePar = new SQLiteParameter("@TaskName", task._Title);
                        SQLiteParameter descPar = new SQLiteParameter("@TaskDesc", task._Description);
                        SQLiteParameter creDatecPar = new SQLiteParameter("@TaskCreDate", task._CreationTime);
                        SQLiteParameter dueDatePar = new SQLiteParameter("@TaskDueDate", task._DueDate);

                        command.CommandText = insert;
                        command.Parameters.Add(idPar);
                        command.Parameters.Add(BoardidPar);
                        command.Parameters.Add(colPar);
                        command.Parameters.Add(assigneePar);
                        command.Parameters.Add(namePar);
                        command.Parameters.Add(descPar);
                        command.Parameters.Add(creDatecPar);
                        command.Parameters.Add(dueDatePar);

                        connection.Open();
                        command.ExecuteNonQuery();
                        Logger.GetLog().Info("a task has been inserted to data base");
                    }
                    catch (Exception ex) 
                    {
                        Logger.GetLog().Error("task insertion failed in database");
                        throw new DataMisalignedException();
                    }
                }
            }
        }








        internal void UpdateTask(long taskId, long boardId, string attributeName, object value) // columnName not be needed because the task Id is unique within the Board, so we dont need the column name for the sql to find the row.
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"UPDATE {TableName} SET {attributeName} = @val WHERE {SourceBoardIdCol} = @boardId AND {TaskIdInBoardCol} = @taskId;"
                };
                try
                {
                    command.Parameters.AddWithValue("@boardId", boardId);
                    command.Parameters.AddWithValue("@taskId", taskId);

                    command.Parameters.Add(new SQLiteParameter("@val", value));
                    connection.Open();

                    command.ExecuteNonQuery();
                    Logger.GetLog().Info("task in table has been updated in database");
                }
                catch (Exception ex) 
                {
                    Logger.GetLog().Error("task update failed in database");
                    throw new DataMisalignedException();
                }
            }
        }






        internal void DeleteTask(long taskId, long boardId) 
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {TableName} WHERE {SourceBoardIdCol} = @boardID AND {TaskIdInBoardCol} = @taskID;"
                };
                try
                {
                    command.Parameters.AddWithValue("@boardID", boardId);
                    command.Parameters.AddWithValue("@taskID", taskId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    Logger.GetLog().Info("task has been deleted in database");
                }
                catch (Exception ex)
                {
                    Logger.GetLog().Error("task deletion failed in database");
                    throw new DataMisalignedException();
                }
                
            }
        }







    }
}

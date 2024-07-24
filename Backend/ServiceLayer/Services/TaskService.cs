using IntroSE.Kanban.Backend.BusinessLayer.BoardModule;
using IntroSE.Kanban.Backend.Exceptions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IntroSE.Kanban.Backend.ServiceLayer.Services
{
    public class TaskService
    {

        private BoardFacade Bf { get; set; }
        internal TaskService(BoardFacade bf)
        {
            Bf = bf;
        }





        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email is not associated with any existing account.
        /// An InvalidOperationException can be thrown if the given board name doesn't exist. 
        /// An InvalidOperationException can be thrown if the given title length exceeds 50 characters.
        /// An InvalidOperationException can be thrown if the given description length exceeds 300 characters.
        /// </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system.
        /// A board with the given name should exist.
        /// </pre-condition>
        /// <post-condition> A new task with the given details should be added to the backlog column of the given board. </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("AddTask: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Bf.AddTask(email.ToLower(), boardName, title, description, dueDate);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (InvalidConstraintException exception)
            {
                return new Response("Title has to be max. 50 characters, not empty. and for the description max. 300 characters, optional", null).GetSerializedVersion();
            }
            catch (InsufficientMemoryException exception)
            {
                return new Response("User is trying to create a task with a null description", null).GetSerializedVersion();
            }
            catch (InvalidTimeZoneException exception)
            {
                return new Response("due date is not legal", null).GetSerializedVersion();
            }
            catch (InvalidDataException exception)
            {
                return new Response("backlog column is full", null).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ExceptionHandler(exception).GetSerializedVersion();
            }
        }





        /// <summary>
        /// This method updates the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email is not associated with any existing account.
        /// An InvalidOperationException can be thrown if the given board name doesn't exist. 
        /// An InvalidOperationException can be thrown if the given columnOrdinal is out of bounds (less than 0 or greater than 2).
        /// An InvalidOperationException can be thrown if the given taskID does not exist. 
        /// </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system.
        /// A board with the given name should exist.
        /// The taskID should exist.
        /// </pre-condition>
        /// <post-condition> The due date of the task should be updated. </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("UpdateTaskDueDate: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Bf.UpdateTaskDueDate(email.ToLower(), boardName, columnOrdinal, taskId, dueDate);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (InvalidTimeZoneException exception)
            {
                return new Response("due date is not legal", null).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ExceptionHandler(exception).GetSerializedVersion();
            }
        }





        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email is not associated with any existing account.
        /// An InvalidOperationException can be thrown if the given board name doesn't exist. 
        /// An InvalidOperationException can be thrown if the given columnOrdinal is out of bounds (less than 0 or greater than 2).
        /// An InvalidOperationException can be thrown if the given taskID does not exist.
        /// An InvalidOperationException can be thrown if the given title length exceeds 50 characters.
        /// </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system.
        /// A board with the given name should exist.
        /// The taskID should exist.
        /// </pre-condition>
        /// <post-condition> The title of the task should be updated. </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("UpdateTaskTitle: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Bf.UpdateTaskTitle(email.ToLower(), boardName, columnOrdinal, taskId, title);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (InvalidConstraintException exception)
            {
                return new Response("Title has to be max. 50 characters, not empty. and for the description max. 300 characters, optional", null).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ExceptionHandler(exception).GetSerializedVersion();
            }

        }









        /// <summary>
        /// This method updates the description of a task.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email is not associated with any existing account.
        /// An InvalidOperationException can be thrown if the given board name doesn't exist. 
        /// An InvalidOperationException can be thrown if the given columnOrdinal is out of bounds (less than 0 or greater than 2).
        /// An InvalidOperationException can be thrown if the given taskID does not exist.
        /// An InvalidOperationException can be thrown if the given description length exceeds 300 characters.
        /// </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system.
        /// A board with the given name should exist.
        /// The taskID should exist.
        /// </pre-condition>
        /// <post-condition> The description of the task should be updated. </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("UpdateTaskDescription: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Bf.UpdateTaskDescription(email.ToLower(), boardName, columnOrdinal, taskId, description);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (InvalidConstraintException exception)
            {
                return new Response("Title has to be max. 50 characters, not empty. and for the description max. 300 characters, optional", null).GetSerializedVersion();
            }
            catch (InsufficientMemoryException exception)
            {
                return new Response("Description cannot be null", null).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ExceptionHandler(exception).GetSerializedVersion();
            }
        }








        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email is not associated with any existing account.
        /// An InvalidOperationException can be thrown if the given board name doesn't exist. 
        /// An InvalidOperationException can be thrown if the given columnOrdinal is out of bounds (less than 0 or greater than 2).
        /// An InvalidOperationException can be thrown if the given taskID does not exist or if the task with that ID is already in the 'Done' column. 
        /// </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system.
        /// A board with the given name should exist.
        /// The taskID should exist and that task should not be in the 'Done' column.
        /// </pre-condition>
        /// <post-condition> The chosen task should advance a column </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("AdvanceTask: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Bf.AdvanceTask(email.ToLower(), boardName, columnOrdinal, taskId);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (InvalidDataException exception)
            {
                return new Response("Cannot advance a task to an already full column", null).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ExceptionHandler(exception).GetSerializedVersion();
            }
        }






        /// <summary>
        /// This method returns all in-progress tasks of a user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email is not associated with any existing account. </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system. </pre-condition>
        /// <post-condition> No change in the state - in-progress tasks of the user are simply returned. </post-condition>
        /// <returns>A response with a list of the in-progress tasks of the user, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetInProgressTasks(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("GetInProgressTasks: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }

                List<TaskSL> res = new List<TaskSL>();
                foreach (TaskBL taskFromBL in Bf.InProgressTasks(email.ToLower()))
                {
                    res.Add(new TaskSL(taskFromBL));
                }

                return new Response(null, res.ToList()).GetSerializedVersion();
            }
            catch (InvalidDataException exception)
            {
                return new Response(null, new List<TaskSL>().ToList()).GetSerializedVersion();    //  in case he has no boards, so he won't have any in progress tasks.
            }
            catch (Exception exception)
            {
                return ExceptionHandler(exception).GetSerializedVersion();
            }
        }











        /// <summary>
        /// returns the proper response for most exceptions in Task Service
        /// </summary>
        private Response ExceptionHandler(Exception exception)
        {
            if (exception is InvalidOperationException)
                return new Response("the email given is not logged in the system", null);

            if (exception is EntryPointNotFoundException)
                return new Response("Board name given does not exist in that user's boards", null);

            if (exception is KeyNotFoundException)
                return new Response("Task Id given does not exist in the board of that user", null);

            if (exception is IndexOutOfRangeException)
                return new Response("the column number given is out of bounds, only columns 0 and 1 can be changed", null);

            if (exception is DirectoryNotFoundException)
                return new Response("the task Id given is not in the column given", null);

            if (exception is ArgumentNullException)
                return new Response("At least one of the given arguments is null or empty", null);

            if (exception is AccessViolationException)
                return new Response("Cannot change already done task", null);

            if (exception is InRowChangingEventException)
                return new Response("The user is trying to change the task even though he's not that task's assignee", null);

            if (exception is NotTaskAssigneeException)
                return new Response("The user trying to reassign the task is not the current task assignee", null);

            if (exception is DataMisalignedException) 
                return new Response("SQL Execution in database failed", null);

            return new Response(null, null);
        }











        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  DAL RELATED FROM HERE









        /// <summary>
        /// This method assigns a task to a user
        /// </summary>
        /// <exception> in this method an Exception can be thrown when the user isn't logged into the system </exception>
        /// <exception> in this method an Exception can be thrown when the user isn't the assignee of the task in case the task is already assigned to someone else </exception>
        /// <exception> in this method an Exception can be thrown when the user isn't a member of the board the task is in </exception>
        /// <exception> in this method an Exception can be thrown when the task doesn't exist, or doesnt exist in that board or in that column</exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system and be the assignee of the task if it's assigned, also the task must exist in that place. </pre-condition>
        /// <post-condition> No change in the state - the task will be assigned according to the emailAssignee argument </post-condition>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column number. The first column is 0, the number increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified a task ID</param>        
        /// <param name="emailAssignee">Email of the asignee user</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(emailAssignee))
                {
                    Logger.GetLog().Error("AssignTask: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Bf.AssignTask(email.ToLower(), boardName, columnOrdinal, taskID, emailAssignee.ToLower());
                return new Response(null, null).GetSerializedVersion();
            }
            catch (CheckoutException exception)
            {
                return new Response("Not a member of the same board", null).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ExceptionHandler(exception).GetSerializedVersion();
            }

        }





    }
}



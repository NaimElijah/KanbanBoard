using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using IntroSE.Kanban.Backend.BusinessLayer.AuthenticationModule;
using IntroSE.Kanban.Backend.BusinessLayer.UserModule;
using IntroSE.Kanban.Backend.DataAccessLayer.DALControllers;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;
using IntroSE.Kanban.Backend.Exceptions;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Services;
using Microsoft.VisualBasic;

namespace IntroSE.Kanban.Backend.BusinessLayer.BoardModule
{
    internal class BoardFacade
    {
        private Dictionary<string, Dictionary<string, BoardBL>> ExistingBoards { get; set; }

        private Dictionary<long, BoardBL> BoardIdToBoardBL { get; set; }
        // A Dictionary of <Id - BoardBL> THIS WON'T(!!) BE A DUPLICATE TO THE BOARDBLs in the previous Dict, they are all references to same BoardBLs.
        private Authentication Auth { get; set; }
        private int BoardIdCounter { get; set; }
        private BoardController BController { get; set; }
        private BoardsMembersController BMemController { get; set; }
        private ColumnController CController { get; set; }
        private TaskController TController { get; set; }

        internal BoardFacade(Authentication auth)
        {
            Auth = auth;
            ExistingBoards = new Dictionary<string, Dictionary<string, BoardBL>>();
            BoardIdToBoardBL = new Dictionary<long, BoardBL>();

            BController = new BoardController();
            BMemController = new BoardsMembersController();
            CController = new ColumnController();
            TController = new TaskController();

            BoardIdCounter = 0;
        }





        /// <summary>
        /// This method creates a board for the given user.
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="name">The name of the new board</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void CreateBoard(string email, string name)
        {

            // now checking the legitness of the input and handling it accordingly

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                Logger.GetLog().Error("one or two null or empty arguments were given for board creation");
                throw new ArgumentNullException();
            }
            else if (!Auth.IsLoggedIn(email))
            {
                Logger.GetLog().Error("A user that is not logged into the system tried to create a board");
                throw new InvalidOperationException();
            }                               // so now we know he's logged in and we'll try to create a board for him.
            else if (!ExistingBoards.ContainsKey(email))
            {
                // he currently does not have boards
                ExistingBoards.Add(email, new Dictionary<string, BoardBL>());
            }
            else if (ExistingBoards[email].ContainsKey(name))  // in the case he already has a list of boards in the system
            {
                Logger.GetLog().Error("User " + email + " tried to create a board with the same name as a board he already has");
                throw new AmbiguousMatchException();
            }

            // now for the case that the checks were positive and this is legit

            BoardBL addition = new BoardBL(name, BoardIdCounter, email);

            ExistingBoards[email].Add(name, addition);
            BoardIdToBoardBL.Add(BoardIdCounter, addition);

            BoardIdToBoardBL[BoardIdCounter].joinBoard(email, BoardIdCounter);

            BoardIdCounter++;

            Logger.GetLog().Info("User " + email + " now added a board named " + name);

        }






        /// <summary>
        /// This method deletes a board.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in and an owner of the board.</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void DeleteBoard(string email, string boardName)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(boardName))
            {
                Logger.GetLog().Error("DeleteBoard: Tried to delete a board but the email or board name is null or empty");
                throw new ArgumentNullException();
            }
            if (!ExistingBoards.ContainsKey(email))
            {
                Logger.GetLog().Error($"DeleteBoard: Tried to delete a board but the email: {email} does not have any existing boards");
                throw new KeyNotFoundException();
            }
            bool _isLoggedIn = Auth.IsLoggedIn(email);
            if (!_isLoggedIn)
            {
                Logger.GetLog().Error($"DeleteBoard: {email} Tried to delete a board but this user is not logged in");
                throw new InvalidOperationException();
            }
            if (!ExistingBoards[email].ContainsKey(boardName))
            {
                Logger.GetLog().Error($"DeleteBoard: {email} Tried to delete a board but this user doesn't have a board by this name");
                throw new KeyNotFoundException();
            }
            if (!ExistingBoards[email][boardName].BoardOwnerEmail.Equals(email))
            {
                Logger.GetLog().Error($"DeleteBoard: {email} Tried to delete a board but he is not the owner of this board");
                throw new UnauthorizedAccessException();
            }


            long boardIdToRemove = ExistingBoards[email][boardName].BoardId;


            //                                IT EVENTUALLY DELETES ALL OF THIS TABLE'S DATA(Cols,Members,Tasks) FROM THE DATABASE      <<------------------
            ExistingBoards[email][boardName].DeleteBoard();  //   <<-------------    this goes and deletes all of this board's contents(tasks, columns, members) from the database.


            foreach (Dictionary<string, BoardBL> boardsOfCertainUser in ExistingBoards.Values)  // removal from the dictionaries here
            {
                if (boardsOfCertainUser.ContainsKey(boardName))
                {
                    if (boardsOfCertainUser[boardName].BoardId == boardIdToRemove)
                    {
                        boardsOfCertainUser.Remove(boardName);
                    }
                }
            }

            BoardIdToBoardBL.Remove(boardIdToRemove);  // removal from the dictionaries here


            foreach (string userEmail in ExistingBoards.Keys)
            {
                if (ExistingBoards[userEmail].Count == 0)
                {
                    ExistingBoards.Remove(userEmail);
                    Logger.GetLog().Info($"DeleteBoard: {userEmail} was removed from the ExistingBoards because it has 0 boards after a board Deletion");
                }
            }

            Logger.GetLog().Info($"DeleteBoard: {email} successfully deleted the board named: {boardName}");

        }









        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  Task related down here









        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            CheckInputValidityForEmailAndBoardName("AddTask", email, boardName);  //  checks if he has that board, so if he doesn't have that board, he's not a member of that board.

            if (string.IsNullOrEmpty(title))
            {
                Logger.GetLog().Error("User is trying to create a task using null argument without a title");
                throw new ArgumentNullException();
            }
            else if (title.Length > 50)
            {
                Logger.GetLog().Error("User is trying to create a task an unvalid title");
                throw new InvalidConstraintException();
            }
            else if (description == null)
            {
                Logger.GetLog().Error("User is trying to create a task with a null description");
                throw new InsufficientMemoryException();
            }
            else if (description.Length > 300)
            {
                Logger.GetLog().Error("User is trying to create a task with an unvalid description");
                throw new InvalidConstraintException();
            }
            else if (dueDate.CompareTo(DateTime.Now) <= 0)
            {
                Logger.GetLog().Error("User " + email + " is trying to create a task with a dueDate in the past");
                throw new InvalidTimeZoneException();
            }
            else if (ExistingBoards[email][boardName].Columns[0].Tasks.Count >= ExistingBoards[email][boardName].Columns[0].TasksLimit && ExistingBoards[email][boardName].Columns[0].TasksLimit != -1)
            {
                Logger.GetLog().Error("User " + email + " is trying to add a task but the backlog column is full");
                throw new InvalidDataException();
            }

            long taskID = this.ExistingBoards[email][boardName].CounterForTaskIdInBoard;

            ExistingBoards[email][boardName].addTask(title, description, dueDate);   //  the process of adding a task

            Logger.GetLog().Info(email + " created a new task no' " + taskID + " in board " + boardName.ToString() + " successfully");
        }

        








        /// <summary>
        /// This method updates the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            IsTaskAssigneeAndTaskValidityChecks("UpdateTaskDueDate", email, boardName, taskId, columnOrdinal); // checks input validity

            if (dueDate.Equals(null))
            {
                Logger.GetLog().Error("User " + email + " is trying to update to a null due date");
                throw new ArgumentNullException();
            }
            else if (dueDate.CompareTo(DateTime.Now) < 0)
            {
                Logger.GetLog().Error("User " + email + " is trying to set a task's dueDate to a due date which is in the past");
                throw new InvalidTimeZoneException();
            }

            ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks[taskId].DueDate = dueDate;
            Logger.GetLog().Info($"UpdateTaskDueDate: Task {taskId} of column ordinal {columnOrdinal} from {boardName} successfully had its due date updated to {dueDate}");
        }







        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            IsTaskAssigneeAndTaskValidityChecks("UpdateTaskTitle", email, boardName, taskId, columnOrdinal); // a helper function that completes checks and throws exceptions where checks are negetive

            if (string.IsNullOrEmpty(title))
            {
                Logger.GetLog().Error("Title null or empty given for Updating Task Title");
                throw new ArgumentNullException();
            }
            else if (title.Length > 50)
            {
                Logger.GetLog().Error("Title given for Updating Task Title is longer than 50 characters");
                throw new InvalidConstraintException();
            }

            // now for the case that the checks were positive and this is legit

            ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks[taskId].Title = title;

            Logger.GetLog().Info("task " + taskId + " in board " + boardName + " in column " + columnOrdinal + ", of user " + email + " got its title updated to " + title);

        }








        /// <summary>
        /// This method updates the description of a task.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            IsTaskAssigneeAndTaskValidityChecks("UpdateTaskDescription", email, boardName, taskId, columnOrdinal);

            if (description == null)
            {
                Logger.GetLog().Error("User is trying to create a task with a null description");
                throw new InsufficientMemoryException();
            }

            if (description.Length > 300)
            {
                Logger.GetLog().Error("User is trying to create a task with an unvalid description");
                throw new InvalidConstraintException();
            }

            this.ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks[taskId].Description = description;

            Logger.GetLog().Info(email + " updated task no'" + taskId + "'s description in board " + boardName.ToString() + " successfully");
        }










        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            IsTaskAssigneeAndTaskValidityChecks("AdvanceTask", email, boardName, taskId, columnOrdinal); // checks input validity

            if (ExistingBoards[email][boardName].Columns[columnOrdinal+1].Tasks.Count >= ExistingBoards[email][boardName].Columns[columnOrdinal+1].TasksLimit && ExistingBoards[email][boardName].Columns[columnOrdinal + 1].TasksLimit != -1)
            {
                Logger.GetLog().Error("User " + email + " is trying to advance a task to the next column, but the next column is already full");
                throw new InvalidDataException();
            }

            ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks[taskId].TaskAdvance();

            ExistingBoards[email][boardName].Columns[columnOrdinal + 1].Tasks[taskId] = ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks[taskId];
            ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks.Remove(taskId);

            Logger.GetLog().Info($"AdvanceTask: Task {taskId} from column ordinal {columnOrdinal} from {boardName} successfully advanced to the next column");
        }








        /// <summary>
        /// This method returns all in-progress tasks of a user that he is their assignee.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response with a list of the in-progress tasks of the user that he is their assignee, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal List<TaskBL> InProgressTasks(string email)
        {

            if (string.IsNullOrEmpty(email))
            {
                Logger.GetLog().Error("got a null or empty email when asked for in progress tasks");
                throw new ArgumentNullException();
            }
            else if (!Auth.IsLoggedIn(email))
            {
                Logger.GetLog().Error("user " + email + " isn't logged to system");
                throw new InvalidOperationException();
            }
            else if (!ExistingBoards.ContainsKey(email))
            {
                Logger.GetLog().Info("user has no boards therefore no Inprogress tasks, so we're returning an empty list of tasks");  // board name doesn't exist
                throw new InvalidDataException();
            }

            List<TaskBL> res = new List<TaskBL>();

            foreach (string boardName in ExistingBoards[email].Keys)
            {
                foreach (TaskBL task in ExistingBoards[email][boardName].Columns[1].Tasks.Values)
                {
                    if (task.AssigneeEmail.Equals(email))  // only tasks that the user is assigned to
                    {
                        res.Add(task);
                    }
                }
            }

            Logger.GetLog().Info("Tasks of columns InProgressTasks of user " + email + " that he's their assignee are being returned");
            return res;
        }













        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  Column related down here
















        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            CheckInputValidityForColumn("LimitColumn", email, boardName, columnOrdinal);

            if (limit < -1 || (ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks.Count > limit && limit != -1))
            {
                Logger.GetLog().Error("LimitColumn: User " + email + "tried to change column limit to: " + limit + " in column " + columnOrdinal + " of board " + boardName + ", but value of limit can't be lower than -1 or lower than the number of tasks there and not -1.");
                throw new NotSupportedException();
            }

            ExistingBoards[email][boardName].Columns[columnOrdinal].TasksLimit = limit;

            Logger.GetLog().Info("column " + columnOrdinal + " of board " + boardName + " of user " + email + " got limited to " + limit);

        }








        /// <summary>
        /// This method gets the limit of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response with the column's limit, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal long GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            CheckInputValidityForColumn("GetColumnLimit", email, boardName, columnOrdinal);
            Logger.GetLog().Info("returning column limit of column " + columnOrdinal + " in board name " + boardName + " of user " + email);
            return ExistingBoards[email][boardName].Columns[columnOrdinal].TasksLimit;
        }








        /// <summary>
        /// This method gets the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response with the column's name, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            CheckInputValidityForColumn("GetColumnName", email, boardName, columnOrdinal);
            Logger.GetLog().Info("returning column name of column " + columnOrdinal + " in board name " + boardName + " of user " + email);
            return ExistingBoards[email][boardName].Columns[columnOrdinal].Name;
        }







        /// <summary>
        /// This method returns a column from a board given its ordinal.
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response with a list of the column's tasks, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal ColumnBL GetColumn(string email, string boardName, int columnOrdinal)
        {
            CheckInputValidityForColumn("GetColumn", email, boardName, columnOrdinal);

            Logger.GetLog().Info("Tasks of column " + columnOrdinal + " of board " + boardName + " of user " + email + " are being returned.");
            return ExistingBoards[email][boardName].Columns[columnOrdinal];

        }






        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  Helpers Down here

        /// <summary>
        /// A helper function for us to automatically check and throw exceptions of certain repetative checks
        /// </summary>
        /// <exception>InvalidOperationException when the user is not logged in the system</exception>
        /// <exception>EntryPointNotFoundException when the board does not exist for the user</exception>
        /// <exception>KeyNotFoundException when the task Id is not in the board of  that user</exception>
        /// <exception>IndexOutOfRangeException when the column number is not valid</exception>
        /// <exception>DirectoryNotFoundException when the task id is not in the column given</exception>
        /// <exception>ArgumentNullException when the email or boardName is null or empty</exception>
        internal void CheckInputValidityForColumn(string FuncName, string email, string boardName, int columnOrdinal)
        {
            CheckInputValidityForEmailAndBoardName(FuncName, email, boardName);
            
            if (columnOrdinal < 0 || columnOrdinal > 2)
            {
                Logger.GetLog().Error(FuncName + ": This method is trying to access an out of range column number");
                throw new IndexOutOfRangeException();
            }

        }





        /// <summary>
        /// A helper function for us to automatically check and throw exceptions of certain repetative checks
        /// </summary>
        /// <exception>InvalidOperationException when the user is not logged in the system</exception>
        /// <exception>EntryPointNotFoundException when the board does not exist for the user</exception>
        /// <exception>KeyNotFoundException when the task Id is not in the board of  that user</exception>
        /// <exception>IndexOutOfRangeException when the column number is not valid</exception>
        /// <exception>DirectoryNotFoundException when the task id is not in the column given</exception>
        /// <exception>ArgumentNullException when the email or boardName is null or empty</exception>
        internal void CheckInputValidityForEmailAndBoardName(string FuncName, string email, string boardName)
        {
            if (string.IsNullOrEmpty(email))
            {
                Logger.GetLog().Error(FuncName + ": email is null or empty");
                throw new ArgumentNullException();
            }
            if (string.IsNullOrEmpty(boardName))
            {
                Logger.GetLog().Error(FuncName + ": boardName is null or empty");
                throw new ArgumentNullException();
            }
            if (!Auth.IsLoggedIn(email))
            {
                Logger.GetLog().Error(FuncName + ": A user that is not logged in the system is trying to use this function");
                throw new InvalidOperationException();
            }                                                    // so now we know he's logged
            if (!ExistingBoards.ContainsKey(email))
            {
                // he does not have any boards
                Logger.GetLog().Error(FuncName + ": This method is being used with a board name but no board exists for the email: " + email);  // board name doesn't exist, he's not a member
                throw new EntryPointNotFoundException();
            }
            if (!ExistingBoards[email].ContainsKey(boardName))
            {
                // he does not have any boards
                Logger.GetLog().Error(FuncName + ": This method is being used with a board name that doesn't exist for the email: " + email);  // board name doesn't exist, he's not a member
                throw new EntryPointNotFoundException();
            }


        }




        /// <summary>
        /// A helper function for us to automatically check and throw exceptions of certain repetative checks
        /// </summary>
        /// <exception>InvalidOperationException when the user is not logged in the system</exception>
        /// <exception>EntryPointNotFoundException when the board does not exist for the user</exception>
        /// <exception>KeyNotFoundException when the task Id is not in the board of  that user</exception>
        /// <exception>IndexOutOfRangeException when the column number is not valid</exception>
        /// <exception>DirectoryNotFoundException when the task id is not in the column given</exception>
        /// <exception>ArgumentNullException when the email or boardName is null or empty</exception>
        internal void CheckInputValidityForTaskOperations(string FuncName, string email, string boardName, int taskId, int columnOrdinal)
        {
            CheckInputValidityForColumn(FuncName, email, boardName, columnOrdinal);

            if (ExistingBoards[email][boardName].CounterForTaskIdInBoard < taskId || taskId < 0)
            {
                Logger.GetLog().Error(FuncName + ": This method is being used with a task Id that does not exist in the board: " + boardName + ", of the user " + email);
                throw new KeyNotFoundException();
            }
            if (!ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks.ContainsKey(taskId))
            {
                Logger.GetLog().Error("User " + email + " is trying to change a task but the column given does not contain the task id given");
                throw new DirectoryNotFoundException();
            }
            if (columnOrdinal == 2)
            {
                Logger.GetLog().Error("User " + email + " is trying to change a task that is already done, which cannot happen by the rules");
                throw new AccessViolationException();
            }

        }



        /// <summary>
        /// A helper function for us to automatically check and throw exceptions of certain repetative checks
        /// </summary>
        /// <exception>AppDomainUnloadedException when the board Id given doesn't exist in the system</exception>
        internal void CheckBoardIdValidity(int boardId)
        { 
            if (!BoardIdToBoardBL.ContainsKey(boardId))
            {
                Logger.GetLog().Error($"The Board Id given: {boardId}, doesn't exist in the system");
                throw new AppDomainUnloadedException();
            }
        }




        /// <summary>
        /// A helper function for us to automatically check and throw exceptions of certain repetative checks
        /// </summary>
        /// <exception>UnauthorizedAccessException when the user is not the board owner</exception>
        /// <exception>The exceptions thrown in the CheckInputValidityForEmailAndBoardName() function</exception>
        internal void IsBoardOwnerAndBoardValidityChecks(string FuncName, string email, string boardName)              //  helper function
        {
            CheckInputValidityForEmailAndBoardName(FuncName, email, boardName);   // it will probably need a check first
            // this function will be used where we need that only the Owner will be able to do something so this is an extension to the other helper method which also checks If Owner
            if (!ExistingBoards[email][boardName].BoardOwnerEmail.Equals(email))
            {
                Logger.GetLog().Error($"user {email} can't do {FuncName} because he is not the owner of the board");
                throw new UnauthorizedAccessException();
            }
        }



        /// <summary>
        /// A helper function for us to automatically check and throw exceptions of certain repetative checks
        /// </summary>
        /// <exception>InRowChangingEventException when the user is not the task assignee</exception>
        /// <exception>The exceptions thrown in the CheckInputValidityForTaskOperations() function</exception>
        internal void IsTaskAssigneeAndTaskValidityChecks(string FuncName, string email, string boardName, int taskId, int columnOrdinal)         //  helper function
        {
            CheckInputValidityForTaskOperations(FuncName, email, boardName, taskId, columnOrdinal);   // it will probably need a check first
            // this function will be used where we need that only the assignee will be able to do something so this is an extension to the other helper method which also checks If Assignee
            if (!ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks[taskId].AssigneeEmail.Equals(email))
            {
                throw new InRowChangingEventException();
            }

        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  Helpers Up here


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  DAL RELATED FROM HERE










        /// <summary>
        /// This method returns a list of IDs of all user's boards.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response with a list of IDs of all user's boards, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal List<BoardBL> GetUserBoards(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                Logger.GetLog().Error("GetUserBoards: email is null or empty");
                throw new ArgumentNullException();
            }
            if (!Auth.IsLoggedIn(email))
            {
                Logger.GetLog().Error("GetUserBoards: A user that is not logged to the system is trying to use this function");
                throw new InvalidOperationException();
            }
            if (!ExistingBoards.ContainsKey(email))
            {
                Logger.GetLog().Info("GetUserBoards: Returning the the user's board IDs, in this case he doesn't have any boards so returning an empty list");
                return new List<BoardBL>();
            }

            Logger.GetLog().Info("GetUserBoards: Returning the the user's board IDs");
            return (ExistingBoards[email].Values).ToList();

        }









        /// <summary>
        /// This method adds a user as member to an existing board.
        /// </summary>
        /// <param name="email">The email of the user that joins the board. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void JoinBoard(string email, int boardID)
        {
            CheckBoardIdValidity(boardID);
            string boardName = GetBoardName(boardID);

            if (string.IsNullOrEmpty(email))
            {
                Logger.GetLog().Error("JoinBoard: email is null or empty");
                throw new ArgumentNullException();
            }
            if (!Auth.IsLoggedIn(email))
            {
                Logger.GetLog().Error("JoinBoard: A user that is not logged in the system is trying to use this function");
                throw new InvalidOperationException();
            }                                                    // so now we know he's logged

            if (!ExistingBoards.ContainsKey(email)) // if the member doesn't even have boards.
            {
                ExistingBoards.Add(email, new Dictionary<string, BoardBL>());  // to initialize his boards dictionary
                BoardIdToBoardBL[boardID].joinBoard(email,boardID);
                ExistingBoards[email].Add(boardName, BoardIdToBoardBL[boardID]);
            }
            else if (!ExistingBoards[email].ContainsKey(boardName)) // if the member doesn't have a board with the same name.
            {
                BoardIdToBoardBL[boardID].joinBoard(email, boardID);
                ExistingBoards[email].Add(boardName, BoardIdToBoardBL[boardID]);
            }
            else     // if he already has a board with the same name
            {
                if (ExistingBoards[email][boardName].BoardId == boardID)
                {
                    Logger.GetLog().Error($"JoinBoard: user is trying to join board with id {boardID} but is already a member of this board.");
                }
                else
                {
                    Logger.GetLog().Error($"JoinBoard: user is trying to join board with id {boardID} but already has a board with this name.");
                }
                throw new InvalidDataException();
            }

        }









        /// <summary>
        /// This method removes a user from the members list of a board.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void LeaveBoard(string email, int boardID)
        {
            CheckBoardIdValidity(boardID);
            string boardName = GetBoardName(boardID);

            CheckInputValidityForEmailAndBoardName("LeaveBoard", email, boardName);
            string doneColName = "done";

            if (!BoardIdToBoardBL[boardID].Members.Contains(email))
            {
                Logger.GetLog().Error($"LeaveBoard: user {email} is not a member of this board.");
                throw new InvalidProgramException();
            }

            if (BoardIdToBoardBL[boardID].BoardOwnerEmail.Equals(email))
            {
                Logger.GetLog().Error($"LeaveBoard: the owner {email} of board with id {boardID} is trying to leave the board, even though he can't because he is the owner of that board");
                throw new AbandonedMutexException();
            }

            
            // after the checks
            foreach (ColumnBL col in BoardIdToBoardBL[boardID].Columns)
            {
                if (!col.Name.Equals(doneColName))
                {
                    foreach (TaskBL task in col.Tasks.Values)
                    {
                        if (task.AssigneeEmail.Equals(email))
                        {
                            task.AssigneeEmail = "unassigned"; //defult
                        }
                    }
                }
            }

            BoardIdToBoardBL[boardID].leaveBoard(boardID, email);
            ExistingBoards[email].Remove(boardName);

            if (ExistingBoards[email].Count == 0)
            {
                ExistingBoards.Remove(email);
                Logger.GetLog().Info($"DeleteBoard: {email} was removed from the ExistingBoards because it has 0 boards after a board Deletion");
            }

            Logger.GetLog().Info($"User {email} left the board with id {boardID}");
            
        }







        /// <summary>
        /// This method returns a board's name
        /// </summary>
        /// <param name="boardId">The board's ID</param>
        /// <returns>A response with the board's name, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal string GetBoardName(int boardId)
        {
            CheckBoardIdValidity(boardId);

            Logger.GetLog().Info($"GetBoardName: returning board name of board with id {boardId}");
            return BoardIdToBoardBL[boardId].BoardName;
        }









        /// <summary>
        /// This method transfers a board ownership.
        /// </summary>
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>
        /// <param name="newOwnerEmail">Email of the new owner</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        /// 
        internal void TransferBoardOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        { 
            IsBoardOwnerAndBoardValidityChecks("TransferBoardOwnership", currentOwnerEmail, boardName);
            CheckBoardMembership("TransferBoardOwnership", currentOwnerEmail, newOwnerEmail, boardName);
            
            if (ExistingBoards[currentOwnerEmail][boardName].BoardOwnerEmail.Equals(newOwnerEmail))
            {
                Logger.GetLog().Error("TransferBoardOwnership: The user to whom you've attempted to transfer ownership is already the owner of this board.");
                throw new AlreadyIsBoardOwnerException();
            }

            ExistingBoards[currentOwnerEmail][boardName].BoardOwnerEmail = newOwnerEmail;
            Logger.GetLog().Info("TransferBoardOwnership: Transfering the ownership of board " + boardName + " from " + currentOwnerEmail + " to " + newOwnerEmail);
        }



        /// <summary>
        /// checks that the suspectedBoardMemberEmail is in that board that the boardMemberEmail is in.
        /// </summary>
        /// <param name="funcName"> the function name of the function we're checking this check in</param>
        /// <param name="boardMemberEmail">the board member we want to check if the suspectedBoardMemberEmail is in a board with him</param>
        /// <param name="suspectedBoardMemberEmail">the user we want to check if he is in a board with the boardMemberEmail</param>
        /// <param name="boardName">the board name</param>
        /// <exception cref="CheckoutException"> thrown when the suspectedBoardMemberEmail is not in that board that the boardMemberEmail is in</exception>
        private void CheckBoardMembership(string funcName, string boardMemberEmail, string suspectedBoardMemberEmail, string boardName) {

            if (!ExistingBoards[boardMemberEmail][boardName].Members.Contains(suspectedBoardMemberEmail))
            {
                Logger.GetLog().Error(funcName + ": This function requires users to be board members in order " +
                    "to take part in the execution of the function.");
                throw new CheckoutException();
            }
        }









        /// <summary>
        /// This method assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column number. The first column is 0, the number increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified a task ID</param>        
        /// <param name="emailAssignee">Email of the asignee user</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        /// 
        internal void AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {

            CheckInputValidityForTaskOperations("AssignTask", email, boardName, taskID, columnOrdinal);
            CheckBoardMembership("AssignTask", email, email, boardName);
            CheckBoardMembership("AssignTask", email, emailAssignee, boardName);

            if ((!ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks[taskID].AssigneeEmail.Equals("unassigned"))
                && (!ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks[taskID].AssigneeEmail.Equals(email)))
            {
                Logger.GetLog().Error("AssignTask" + ": The user trying to reassign the task is not the current task assignee." +
                                   "Because this task is already assigned it can only be reassigned by its current assignee.");
                throw new NotTaskAssigneeException();
            }

            ExistingBoards[email][boardName].Columns[columnOrdinal].Tasks[taskID].AssigneeEmail = emailAssignee;
            Logger.GetLog().Info("AssignTask: " + email + " is assigning task (id): " + taskID + " from board: " + boardName + " column ordinal: " + columnOrdinal + " to " + emailAssignee);

        }














        ///<summary>This method loads all persisted data.
        ///<para>
        ///<b>IMPORTANT:</b> When starting the system via the GradingService - do not load the data automatically, only through this method. 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
        ///</para>
        /// </summary>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void LoadData()  // for Boards, Columns and Tasks.
        {

            List<object> temp = BController.SelectAll();
            List<BoardDAO> boardDAOs = new List<BoardDAO>();

            foreach (object objBoarddao in temp)
            {
                boardDAOs.Add((BoardDAO)objBoarddao);
            }



            List<BoardBL> boardBLs = new List<BoardBL>();
            foreach (BoardDAO boarddao in boardDAOs)
            {
                boardBLs.Add(new BoardBL(boarddao));
            }

            // now we have a list of all of the boardBLs, now let's arrange them into the Dictionaries:
            // THIS DOES NOT CREATE DUPLICATES, ONLY REFERENCES !! SO IT'S OK.


            foreach (BoardBL listBoardBL in boardBLs)   //    Loading up the BoardIdToBoardBL Dictionary
            {
                BoardIdToBoardBL.Add(listBoardBL.BoardId, listBoardBL);
            }  // Loaded BoardIdToBoardBL Dictionary



            foreach (BoardBL listBoardBL in boardBLs)   //    Loading up the ExisitingBoards Dictionary
            {
                foreach (string memberemail in listBoardBL.Members)
                {
                    if (!ExistingBoards.ContainsKey(memberemail)) // if no boards he's in we're added yet, create his boards dict and add the current board there
                    {
                        Dictionary<string, BoardBL> tempForAdd = new Dictionary<string, BoardBL>();
                        tempForAdd.Add(listBoardBL.BoardName, listBoardBL);
                        ExistingBoards.Add(memberemail, tempForAdd);
                    }
                    else  // if (!ExisitingBoards[memberemail].ContainsKey(listBoardBL.BoardName)) // if the current board doesn't exist in his dict, add it to there
                    {
                        ExistingBoards[memberemail].Add(listBoardBL.BoardName, listBoardBL);
                    }

                }

            }  // Loaded ExisitingBoards Dictionary


            // Loading Done.

        }












        ///<summary>This method deletes all persisted data.
        ///<para>
        ///<b>IMPORTANT:</b> 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
        ///</para>
        /// </summary>
        ///<returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        internal void DeleteData()  // for Boards, Columns and Tasks.
        {
            TController.DeleteAll();
            CController.DeleteAll();
            BMemController.DeleteAll();
            BController.DeleteAll();

            ExistingBoards = new Dictionary<string, Dictionary<string, BoardBL>>();
            BoardIdToBoardBL = new Dictionary<long, BoardBL>();
            BoardIdCounter = 0;

            // Deleting Done.

        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        internal string GetBoardOwner(string email , string boardName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(boardName))
            {
                Logger.GetLog().Error("DeleteBoard: Tried to delete a board but the email or board name is null or empty");
                throw new ArgumentNullException();
            }
                Logger.GetLog().Info($"GetBoardOwner: returning board owner of board with  name {boardName} connect to email {email}");
            return ExistingBoards[email][boardName].BoardOwnerEmail;
        }
    }
}

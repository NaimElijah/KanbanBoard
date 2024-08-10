using System;
using IntroSE.Kanban.Backend.ServiceLayer.Services;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// A class for grading your work <b>ONLY</b>. The methods are not using good SE practices and you should <b>NOT</b> infer any insight on how to write the service layer/business layer. 
    /// <para>
    /// Each of the class' methods should return a JSON string with the following structure (see <see cref="System.Text.Json"/>):
    /// <code>
    /// {
    ///     "ErrorMessage": &lt;string&gt;,
    ///     "ReturnValue": &lt;object&gt;
    /// }
    /// </code>
    /// Where:
    /// <list type="bullet">
    ///     <item>
    ///         <term>ReturnValue</term>
    ///         <description>
    ///             The return value of the function.
    ///             <para>
    ///                 The value may be either a <paramref name="primitive"/>, a <paramref name="Task"/>, or an array of them. See below for the definition of <paramref name="Task"/>.
    ///             </para>
    ///             <para>If the function does not return a value or an exception has occorred, then the field should be either null or undefined.</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>ErrorMessage</term>
    ///         <description>If an exception has occorred, then this field will contain a string of the error message. Otherwise, the field will be null or undefined.</description>
    ///     </item>
    /// </list>
    /// </para>
    /// <para>
    /// An empty response is a response that both fields are either null or undefined.
    /// </para>
    /// <para>
    /// The structure of the JSON of a Task, is:
    /// <code>
    /// {
    ///     "Id": &lt;int&gt;,
    ///     "CreationTime": &lt;DateTime&gt;,
    ///     "Title": &lt;string&gt;,
    ///     "Description": &lt;string&gt;,
    ///     "DueDate": &lt;DateTime&gt;
    /// }
    /// </code>
    /// </para>
    /// </summary>
    public class GradingService
    {
        public ServiceFactory ServFact { get; set; }

        /// the constructor you gave us to implement here
        public GradingService()
        {
            ServFact = new ServiceFactory();
        }

        /// <summary>
        /// This method registers a new user to the system.
        /// </summary>
        /// <param name="email">The user email address, used as the username for logging into the system.</param>
        /// <param name="password">The user password.</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the password doesn't comply with the password rules or when an existing email is given </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the email must not be already registered and the password must comply with the password rules</pre-condition>
        /// <post-condition> the user will be registered and logged in </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Register(string email, string password)
        {
            return ServFact.Us.Register(email, password);
        }

        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the password isn't correct or when the email given is not registered </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user will be logged out </pre-condition>
        /// <post-condition> the user will be logged in </post-condition>
        /// <returns>A response with the user's email, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Login(string email, string password)
        {
            return ServFact.Us.Login(email, password);
        }

        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email given is not registered </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user will be logged in and the email given is already registered </pre-condition>
        /// <post-condition> the user will be logged out </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Logout(string email)
        {
            return ServFact.Us.Logout(email);
        }

        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email given is not registered or the boardName doesn't exist or the limit is lower than -1 or the column ordinal is not 0, 1 or 2 </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user will be logged in and the boardName exists for the user and the columnOrdinal is 0, 1 or 2 and the limit given is bigger than -2 </pre-condition>
        /// <post-condition> the column we wanted to limit will be limited according to the limit given </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            return ServFact.Cs.LimitColumn(email, boardName, columnOrdinal, limit);
        }

        /// <summary>
        /// This method gets the limit of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email given is not registered or the boardName doesn't exist for the user or the columnOrdinal is not 0, 1, or 2 </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user will be logged in and the boardName exists for the user and the columnOrdinal is 0, 1 or 2 </pre-condition>
        /// <post-condition> the state of things won't change, only the column limit is returned </post-condition>
        /// <returns>A response with the column's limit, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            return ServFact.Cs.GetColumnLimit(email, boardName, columnOrdinal);
        }

        /// <summary>
        /// This method gets the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email given is not registered or the boardName doesn't exist for the user or the columnOrdinal is not 0, 1, or 2 </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user will be logged in and the boardName exists for the user and the columnOrdinal is 0, 1 or 2 </pre-condition>
        /// <post-condition> the state of things won't change, only the column name is returned </post-condition>
        /// <returns>A response with the column's name, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            return ServFact.Cs.GetColumnName(email, boardName, columnOrdinal);
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
            return ServFact.Ts.AddTask(email, boardName, title, description, dueDate);
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
            return ServFact.Ts.UpdateTaskDueDate(email, boardName, columnOrdinal, taskId, dueDate);
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
        /// /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system.
        /// A board with the given name should exist.
        /// The taskID should exist.
        /// </pre-condition>
        /// <post-condition> The title of the task should be updated. </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            return ServFact.Ts.UpdateTaskTitle(email, boardName, columnOrdinal, taskId, title);
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
            return ServFact.Ts.UpdateTaskDescription(email, boardName, columnOrdinal, taskId, description);
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
            return ServFact.Ts.AdvanceTask(email, boardName, columnOrdinal, taskId);
        }

        /// <summary>
        /// This method returns a column from a board given its ordinal.
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email is not associated with any existing account.
        /// An InvalidOperationException can be thrown if the given board name doesn't exist. 
        /// An InvalidOperationException can be thrown if the given columnOrdinal is out of bounds (less than 0 or greater than 2). 
        /// </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system.
        /// A board with the given name should exist.
        /// </pre-condition>
        /// <post-condition> No change in the state - a column is simply returned. </post-condition>
        /// <returns>A response with a list of the column's tasks, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            return ServFact.Cs.GetColumn(email, boardName, columnOrdinal);
        }

        /// <summary>
        /// This method creates a board for the given user.
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="name">The name of the new board</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email is not associated with any existing account. </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system. </pre-condition>
        /// <post-condition> A board with the given name should be created. </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string CreateBoard(string email, string name)
        {
            return ServFact.Bs.CreateBoard(email, name);
        }

        /// <summary>
        /// This method deletes a board.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in and an owner of the board.</param>
        /// <param name="name">The name of the board</param>
        /// <exception> in this method an InvalidCredentialException can be thrown when the email is not associated with any existing account. 
        /// An InvalidOperationException can be thrown if the given board name (that should be deleted) doesn't exist.  </exception>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged in </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system.
        /// The user should have a board with the given name.
        /// </pre-condition>
        /// <post-condition> The board with the given name should be deleted. </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string DeleteBoard(string email, string name)
        {
            return ServFact.Bs.DeleteBoard(email, name);
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
        public string InProgressTasks(string email)
        {
            return ServFact.Ts.GetInProgressTasks(email);
        }










        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  FROM HERE, THE METHODS FOR MILESTONE 2:







        /* FROM HERE: NEW METHODS FOR MILESTONE 2-3 */




        /// <summary>
        /// This method returns a list of IDs of all user's boards.
        /// </summary>
        /// <exception> in this method an Exception can be thrown when the user isn't logged into the system </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system. </pre-condition>
        /// <post-condition> No change in the state - Board id's of the user are simply returned. </post-condition>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response with a list of IDs of all user's boards, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetUserBoards(string email)
        {
            return ServFact.Bs.GetUserBoards(email);
        }


      



        /// <summary>
        /// This method adds a user as member to an existing board.
        /// </summary>
        /// <exception> in this method an Exception can be thrown when the user isn't logged into the system </exception>
        /// <exception> in this method an Exception can be thrown when the user already has a board of that board he is trying to join to </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system and have no board with the board name of the board he is trying to join. </pre-condition>
        /// <post-condition> No change in the state - the user will join the board. </post-condition>
        /// <param name="email">The email of the user that joins the board. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string JoinBoard(string email, int boardID)
        {
            return ServFact.Bs.JoinBoard(email, boardID);
        }






        /// <summary>
        /// This method removes a user from the members list of a board.
        /// </summary>
        /// <exception> in this method an Exception can be thrown when the user isn't logged to the system </exception>
        /// <exception> in this method an Exception can be thrown when the user isn't a member of the board that has that boardId </exception>
        /// <exception> in this method an Exception can be thrown when the user is the owner of the board </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system and have a board with the board id of the board he is trying to leave and not be his owner. </pre-condition>
        /// <post-condition> No change in the state - the user will leave the board. </post-condition>
        /// <param name="email">The email of the user. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string LeaveBoard(string email, int boardID)
        {
            return ServFact.Bs.LeaveBoard(email, boardID);
        }






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
            return ServFact.Ts.AssignTask(email, boardName, columnOrdinal, taskID, emailAssignee);
        }






        /// <summary>
        /// This method returns a board's name
        /// </summary>
        /// <exception> in this method an Exception can be thrown when the boardId doesn't exist</exception>
        /// <pre-condition> A board with that boardId must exist. </pre-condition>
        /// <post-condition> No change in the state - the board name of the board with the boardId id will be returned </post-condition>
        /// <param name="boardId">The board's ID</param>
        /// <returns>A response with the board's name, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetBoardName(int boardId)
        {
            return ServFact.Bs.GetBoardName(boardId);
        }

        public string GetBoardOwner(string email , string boardName)
        {
            return ServFact.Bs.GetUserOwnerlBoards(boardId);
        }





        /// <summary>
        /// This method transfers a board ownership.
        /// </summary>
        /// <exception> in this method an Exception can be thrown when the user isn't logged into the system </exception>
        /// <exception> in this method an Exception can be thrown when the board name doesn't exist</exception>
        /// <exception> in this method an Exception can be thrown when the currentOwnerEmail isn't the current owner of the board </exception>
        /// <exception> in this method an Exception can be thrown if newOwnerEmail is not a member of the board</exception> 
        /// <pre-condition> the user must be logged to the system, the board must exist, the current owner must really be the owner of that board, and the new owner must already be a member of that board . </pre-condition>
        /// <post-condition> No change in the state - the board ownership will transfer to the new owner </post-condition>
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>
        /// <param name="newOwnerEmail">Email of the new owner</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            return ServFact.Bs.TransferBoardOwnership(currentOwnerEmail, newOwnerEmail, boardName);
        }






        ///<summary>This method loads all persisted data.
        ///<para>
        ///<b>IMPORTANT:</b> When starting the system via the GradingService - do not load the data automatically, only through this method. 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
        ///</para>
        /// </summary>
        /// <exception> in this method an Exception can be thrown when a connected database doesn't exist or isn't found</exception>
        /// <pre-condition> A connected database must exist </pre-condition>
        /// <post-condition> No change in the state - All the data from the database is loaded into the system </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string LoadData()
        {
            return ServFact.LoadAllData();
        }






        ///<summary>This method deletes all persisted data.
        ///<para>
        ///<b>IMPORTANT:</b> 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
        ///</para>
        /// </summary>
        /// <exception> in this method an Exception can be thrown when a connected database doesn't exist or isn't found</exception>
        /// <pre-condition> A connected database must exist </pre-condition>
        /// <post-condition> No change in the state - All the data in the database is deleted </post-condition>
        ///<returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string DeleteData()
        {
            return ServFact.DeleteAllData();
        }







    }
}

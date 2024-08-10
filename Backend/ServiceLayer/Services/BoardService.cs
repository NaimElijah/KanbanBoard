using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using IntroSE.Kanban.Backend.BusinessLayer.BoardModule;
using IntroSE.Kanban.Backend.Exceptions;

namespace IntroSE.Kanban.Backend.ServiceLayer.Services
{
    public class BoardService
    {

        private BoardFacade Bf { get; set; }
        internal BoardService(BoardFacade bf)
        {
            Bf = bf;
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
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("CreateBoard: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Bf.CreateBoard(email.ToLower(), name);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (AmbiguousMatchException e) // for already existing board name
            {
                return new Response("A user cannot create a board that has the same name as a board he already has", null).GetSerializedVersion();
            }
            catch (Exception e)
            {
                return BoardsExceptionHandler(e).GetSerializedVersion();
            }

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
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("DeleteBoard: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Bf.DeleteBoard(email.ToLower(), name);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (KeyNotFoundException e)
            {
                return new Response("User does not have any existing boards", null).GetSerializedVersion();
            }
            catch (Exception e)
            {
                return BoardsExceptionHandler(e).GetSerializedVersion();
            }
        }














        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////     DAL RELATED FROM HERE










        /// <summary>
        /// This method returns a list of IDs of all user's boards.
        /// </summary>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged to the system </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system. </pre-condition>
        /// <post-condition> No change in the state - Board id's of the user are simply returned. </post-condition>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response with a list of IDs of all user's boards, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetUserBoards(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("GetUserBoards: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }

                List<BoardBL> bls = Bf.GetUserBoards(email.ToLower());
                List<long> res = new List<long>();
                foreach (BoardBL boardbl in bls)
                {
                    res.Add(new BoardSL(boardbl).BoardId);
                }
                return new Response(null, res.ToList()).GetSerializedVersion();
            }
            catch (Exception e)
            {
                return BoardsExceptionHandler(e).GetSerializedVersion();
            }

        }












        /// <summary>
        /// This method adds a user as member to an existing board.
        /// </summary>
        /// <exception> in this method an InvalidOperationException can be thrown when the user isn't logged to the system </exception>
        /// <exception> in this method an AmbiguousException can be thrown when the user already has a board of that board he is trying to join to </exception>
        /// <pre-condition> the user associated with the given email must be already logged into the system and have no board with the board name of the board he is trying to join. </pre-condition>
        /// <post-condition> No change in the state - the user will join the board. </post-condition>
        /// <param name="email">The email of the user that joins the board. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string JoinBoard(string email, int boardID)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("JoinBoard: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }

                Bf.JoinBoard(email.ToLower(), boardID);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (InvalidDataException e)
            {
                return new Response("user is already a member of this board or has a board with identical name", null).GetSerializedVersion();
            }
            catch (Exception e)
            {
                return BoardsExceptionHandler(e).GetSerializedVersion();
            }
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
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("LeaveBoard: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }

                Bf.LeaveBoard(email.ToLower(), boardID);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (InvalidProgramException e)
            {
                return new Response("user is not a member of this board", null).GetSerializedVersion();
            }
            catch (AbandonedMutexException e)
            {
                return new Response("A board owner cannot leave a board he has ownership over", null).GetSerializedVersion();
            }
            catch (Exception e)
            {
                return BoardsExceptionHandler(e).GetSerializedVersion();
            }
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
            try
            {

                return new Response(null, Bf.GetBoardName(boardId)).GetSerializedVersion();
            }
            catch (Exception e)
            {
                return BoardsExceptionHandler(e).GetSerializedVersion();
            }
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
        public string TransferBoardOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            try
            {
                if (string.IsNullOrEmpty(currentOwnerEmail) || string.IsNullOrEmpty(newOwnerEmail))
                {
                    Logger.GetLog().Error("TransferBoardOwnership: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }

                Bf.TransferBoardOwnership(currentOwnerEmail.ToLower(), newOwnerEmail.ToLower(), boardName);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (AlreadyIsBoardOwnerException e)
            {
                return new Response("It is already the Board Owner", null).GetSerializedVersion();
            }
            catch (Exception e)
            {
                return BoardsExceptionHandler(e).GetSerializedVersion();
            }
        }












        ///<summary>This method loads all persisted data.
        ///<para>
        ///<b>IMPORTANT:</b> When starting the system via the GradingService - do not load the data automatically, only through this method. 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
        ///</para>
        /// </summary>
        /// <exception> in this method an Exception can be thrown when a connected database doesn't exist or not found</exception>
        /// <pre-condition> A connected database must exist </pre-condition>
        /// <post-condition> No change in the state - All the data from the database is loaded into the system </post-condition>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string LoadData()   //  of boards, columns and tasks
        {
            try
            {
                Bf.LoadData();
                return new Response(null, null).GetSerializedVersion();
            }
            catch (DataMisalignedException e) // for failed sql execution in database
            {
                Logger.GetLog().Error("Boards LoadData Failed");
                return new Response("SQL Execution in database failed", null).GetSerializedVersion();
            }
            catch (Exception e)
            {
                Logger.GetLog().Error("Boards LoadData Failed");
                return new Response("Boards LoadData Failed", null).GetSerializedVersion();
            }

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
            try
            {
                Bf.DeleteData();
                return new Response(null, null).GetSerializedVersion();
            }
            catch (DataMisalignedException e) // for failed sql execution in database
            {
                Logger.GetLog().Error("Boards DeleteData Failed");
                return new Response("SQL Execution in database failed", null).GetSerializedVersion();
            }
            catch (Exception e)
            {
                Logger.GetLog().Error("Boards DeleteData Failed");
                return new Response("Boards DeleteData Failed", null).GetSerializedVersion();
            }

        }





        /// <summary>
        /// a function that gets the exception thrown and returns the matching response.
        /// </summary>
        /// <returns>a function that gets the exception thrown and returns the matching response.</returns>
        private Response BoardsExceptionHandler(Exception exception)
        {
            if (exception is InvalidOperationException)
                return new Response("User has to be logged into the system", null);

            if (exception is ArgumentNullException)
                return new Response("One or two of the arguments given are null or empty", null);

            if (exception is UnauthorizedAccessException)
                return new Response("This action cannot be done by someone whos not the owner of this board", null);

            if (exception is AlreadyIsBoardOwnerException)
                return new Response("The user is already the owner of this board", null);

            if (exception is NotABoardMemberException)
                return new Response("One or more of the users is not a board member", null);

            if (exception is AppDomainUnloadedException) 
                return new Response("This board Id doesn't exist in the system", null);

            if (exception is EntryPointNotFoundException)
                return new Response("This board name doesn't exist for the user", null);

            if(exception is CheckoutException)
                return new Response("A user is not a board member in that board", null);

            if (exception is DataMisalignedException) // for failed sql execution in database
                return new Response("SQL Execution in database failed", null);

            // add more general ones if needed later

            return new Response(null, null);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        public string GetUserOwnerBoards(string email, string boardName)
        {
            {
                try
                {

                    if (string.IsNullOrEmpty(email))
                    {
                        Logger.GetLog().Error("LeaveBoard: Tried but one or more of the credentials given is null or empty");
                        throw new ArgumentNullException();
                    }

                    return new Response(null, Bf.GetBoardOwner(email,boardName)).GetSerializedVersion();
                }
                catch (Exception e)
                {
                    return BoardsExceptionHandler(e).GetSerializedVersion();
                }
            }

        }

        public string GetUserMemberslBoards(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("GetUserBoards: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }

                List<BoardBL> bls = Bf.GetUserBoards(email.ToLower());
                List<List<string>> res = new List<List<string>>();
                foreach (BoardBL boardbl in bls)
                {
                    res.Add(new BoardSL(boardbl).Members);
                }
                return new Response(null, res.ToList()).GetSerializedVersion();
            }
            catch (Exception e)
            {
                return BoardsExceptionHandler(e).GetSerializedVersion();
            }

        }



    }
}

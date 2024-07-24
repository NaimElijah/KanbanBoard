using IntroSE.Kanban.Backend.BusinessLayer.BoardModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer.Services
{
    public class ColumnService
    {

        private BoardFacade Bf { get; set; }
        internal ColumnService(BoardFacade bf)
        {
            Bf = bf;
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
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("LimitColumn: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                Bf.LimitColumn(email.ToLower(), boardName, columnOrdinal, limit);
                return new Response(null, null).GetSerializedVersion();
            }
            catch (NotSupportedException exception)
            {
                return new Response("limit number given cannot be below -1 or below the number of tasks already in the specified column", null).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ColExceptionHandler(exception).GetSerializedVersion();
            }


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
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("GetColumnLimit: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                return new Response(null, Bf.GetColumnLimit(email.ToLower(), boardName, columnOrdinal)).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ColExceptionHandler(exception).GetSerializedVersion();
            }
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
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("GetColumnName: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                return new Response(null, Bf.GetColumnName(email.ToLower(), boardName, columnOrdinal)).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ColExceptionHandler(exception).GetSerializedVersion();
            }
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
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Logger.GetLog().Error("GetColumn: Tried but one or more of the credentials given is null or empty");
                    throw new ArgumentNullException();
                }
                return new Response(null, new ColumnSL(Bf.GetColumn(email.ToLower(), boardName, columnOrdinal)).Tasks.Values.ToList()).GetSerializedVersion();
            }
            catch (Exception exception)
            {
                return ColExceptionHandler(exception).GetSerializedVersion();
            }
        }










        /// <summary>
        /// returns the proper response for most exceptions in Column Service.
        /// </summary>
        private Response ColExceptionHandler(Exception exception)
        {
            if (exception is InvalidOperationException)
                return new Response("the email given is not logged in the system", null);

            if (exception is EntryPointNotFoundException)
                return new Response("Board name given does not exist in that user's boards", null);

            if (exception is IndexOutOfRangeException)
                return new Response("the column number given is out of bounds, there are only columns 0, 1, 2", null);

            if (exception is ArgumentNullException)
                return new Response("At least one of the given arguments is null or empty", null);

            if (exception is DataMisalignedException) // for failed sql execution in database
                return new Response("SQL Execution in database failed", null);

            return new Response(null, null);
        }




    }
}





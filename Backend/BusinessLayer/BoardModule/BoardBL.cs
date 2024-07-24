using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;
using IntroSE.Kanban.Backend.ServiceLayer;
using Microsoft.VisualBasic;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace IntroSE.Kanban.Backend.BusinessLayer.BoardModule
{
    internal class BoardBL
    {
        internal BoardDAO BoardDao { get; set; }
        internal BoardsMembersDAO BoardsMembersDao { get; set; }
        internal string BoardName { get; set; }
        internal long BoardId { get; set; }

        private string boardowneremail;
        internal string BoardOwnerEmail
        {
            get => boardowneremail;
            set
            {
                BoardDao._BoardOwnerEmail = value;
                boardowneremail = value;
            }
        }

        internal List<ColumnBL> Columns { get; set; }
        internal List<string> Members { get; set; }

        private long counterfortaskIdInboard;
        internal long CounterForTaskIdInBoard
        {
            get => counterfortaskIdInboard;
            set
            {
                BoardDao.CounterForTaskIdInBoard = value;
                counterfortaskIdInboard = value;
            }
        }

        private string backColName = "backlog";
        private string inProgColName = "in progress";
        private string doneColName = "done";
        private long startingTasksLimit = -1;
        private long startingCounterForTaskIdInBoard = 0;


        internal BoardBL(string name, long boardId, string boardOwnerEmail)
        {
            BoardDao = new BoardDAO(boardId, boardOwnerEmail, name, startingCounterForTaskIdInBoard);
            BoardsMembersDao = new BoardsMembersDAO(boardId, boardOwnerEmail);
            BoardName = name;
            CounterForTaskIdInBoard = 0;
            Columns = new List<ColumnBL>();
            Columns.Add(new ColumnBL(boardId, backColName, startingTasksLimit));
            Columns.Add(new ColumnBL(boardId, inProgColName, startingTasksLimit));
            Columns.Add(new ColumnBL(boardId, doneColName, startingTasksLimit));

            Members = new List<string>();

            BoardId = boardId;
            BoardOwnerEmail = boardOwnerEmail;

            BoardDao.Persist();  //  insert to the database.
        }


        internal BoardBL(BoardDAO boardDao)    //    <<------------------------  constructor for loading data into it.
        {
            BoardDao = boardDao;
            BoardName = boardDao.Name;
            BoardId = boardDao.BoardId;
            BoardOwnerEmail = boardDao._BoardOwnerEmail;
            CounterForTaskIdInBoard = boardDao.CounterForTaskIdInBoard;
            BoardsMembersDao = new BoardsMembersDAO(BoardId, BoardOwnerEmail);

            Columns = new List<ColumnBL>();

            List<ColumnDAO> columnDaos = BoardDao.GetCols();

            foreach (ColumnDAO c in columnDaos)
            {
                Columns.Add(new ColumnBL(c));  // now the same like here, but for the tasks that should be inside column, so continue in ColumnBL --->
            }


            Members = new List<string>(); 

            List<BoardsMembersDAO> boardsMembersDaos = BoardsMembersDao.GetMembers();

            foreach (BoardsMembersDAO bm in boardsMembersDaos)
            {
                Members.Add(bm.MemberEmail);
            }

            BoardDao.IsPersisted = true;
        }


        /// <summary>
        ///  adds a task to the board, in the backlog column here and this uses another function that will eventually add the task in the database.
        /// </summary>
        internal void addTask(string title, string description, DateTime dueDate)
        {
            TaskBL theTaskAddition = new TaskBL(CounterForTaskIdInBoard, title, description, dueDate);

            BoardDao.AddTask(theTaskAddition.TaskDao);

            Columns[0].Tasks.Add(CounterForTaskIdInBoard, theTaskAddition);
            CounterForTaskIdInBoard++;

        }


        /// <summary>
        /// Deletes the current board and all of it's contents(members, tasks, columns).
        /// </summary>
        internal void DeleteBoard()
        {

            foreach (ColumnBL boardCol in Columns)  // deleting the columns from the database, each of them here will delete the tasks in him.
            {
                boardCol.DeleteCol();
            }

            BoardDao.DeleteBoard();  //  deletes the board from the Boards table
            BoardsMembersDao.DeleteMembers();  //  deletes the board members from the BoardsMembers Table

        }


        /// <summary>
        /// adds a joining member to this board.
        /// </summary>
        internal void joinBoard(string email, long boardID)
        {
            BoardsMembersDAO join = new BoardsMembersDAO(boardID, email);
            BoardsMembersDao.InsertBoardMember(join);
            Members.Add(email);
        }


        /// <summary>
        /// removes a leaving member from this board.
        /// </summary>
        internal void leaveBoard(long boardID, string email)
        {
            BoardsMembersDao.DeleteBoardMember(boardID, email);
            Members.Remove(email);
        }



    }
}

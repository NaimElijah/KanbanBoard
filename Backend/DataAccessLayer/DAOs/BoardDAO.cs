using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DALControllers;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DAOs
{
    internal class BoardDAO
    {
        internal BoardController BController { get; set; }
        internal bool IsPersisted { get; set; }
        internal long BoardId { get; set; }

        private string BoardOwnerEmail;
        internal string _BoardOwnerEmail {
            get => BoardOwnerEmail;
            set
            {
                if (IsPersisted)
                {
                    BController.UpdateBoard(this.BoardId, BoardOwnerEmailCol, value);
                }
                BoardOwnerEmail = value;
            }
        }
        internal string Name { get; set; }

        private long counterforTaskIdInboard;
        internal long CounterForTaskIdInBoard
        {
            get => counterforTaskIdInboard;
            set
            {
                if (IsPersisted)
                {
                    BController.UpdateBoard(this.BoardId, CounterForTaskIdInBoardCol, value);
                }
                counterforTaskIdInboard = value;
            }
        }

        internal ColumnController Ccontroller { get; set; }  

        internal const string BoardOwnerEmailCol = "BoardOwnerEmail";
        internal const string CounterForTaskIdInBoardCol = "CounterForTaskIdInBoard";

        internal const string BoardIdAttributeNameInColsTable = "SourceBoardId";


        internal BoardDAO(long boardId, string boardOwnerEmail, string name, long counterForTaskIdInBoard)
        {
            BController = new BoardController();
            Ccontroller = new ColumnController();  
            IsPersisted = false;
            BoardId = boardId;
            BoardOwnerEmail = boardOwnerEmail;
            Name = name;
            CounterForTaskIdInBoard = counterForTaskIdInBoard;
        }


        /// <summary>
        /// inserts the current board to the database.
        /// </summary>
        internal void Persist()
        {
            if (!IsPersisted)
            {
                BController.Insert(this);
                IsPersisted = true;
            }
        }


        /// <summary>
        /// deletes the current board from the database.
        /// </summary>
        internal void DeleteBoard()
        {
            BController.DeleteBoard(this.BoardId);
        }


        /// <summary>
        /// adding a task of this board to the database.
        /// </summary>
        /// <param name="taskDao"> the task we're adding</param>
        internal void AddTask(TaskDAO taskDao) 
        {
            taskDao.Persist(this.BoardId);
        }


        /// <summary>
        /// getting all the columns of this board from the database.
        /// </summary>
        /// <returns>getting all the columns of this board from the database.</returns>
        internal List<ColumnDAO> GetCols()   
        {
            return Ccontroller.SelectColumns(BoardIdAttributeNameInColsTable, this.BoardId);
        }




    }
}

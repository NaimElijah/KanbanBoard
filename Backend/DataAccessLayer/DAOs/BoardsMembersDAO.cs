using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DALControllers;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DAOs
{
    internal class BoardsMembersDAO
    {
        private const string BoardIdCol = "BoardId";
        internal BoardsMembersController BMController { get; set; }
        internal long BoardId { get; set; }
        internal string MemberEmail { get; set; }
        internal bool IsPersisted { get; set; }


        internal BoardsMembersDAO(long boardId, string memberEmail)
        {
            BMController = new BoardsMembersController();
            BoardId = boardId;
            MemberEmail = memberEmail;
            IsPersisted = false;
        }


        /// <summary>
        /// inserting a board member to the database.
        /// </summary>
        /// <param name="dao">the board member we're inserting</param>
        internal void InsertBoardMember(object dao)
        {
            BMController.Insert(dao);
        }

        /// <summary>
        /// deleting a board member from the database.
        /// </summary>
        /// <param name="boardId">the id of the board</param>
        /// <param name="memberEmail">the email of the member</param>
        internal void DeleteBoardMember(long boardId, string memberEmail)
        {
            BMController.DeleteBoardMember(boardId, memberEmail);
        }


        /// <summary>
        /// get a list of all the members of a boardId.
        /// </summary>
        /// <returns>get a list of all the members of a boardId.</returns>
        internal List<BoardsMembersDAO> GetMembers()
        {
            return BMController.SelectBoardsMembers(BoardIdCol, BoardId);
        }


        /// <summary>
        /// delete members of the board BoardId.
        /// </summary>
        internal void DeleteMembers()
        {
            BMController.DeleteBoardMembersByBoardId(BoardId);
        }



    }
}

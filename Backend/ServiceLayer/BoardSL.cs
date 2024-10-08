﻿using IntroSE.Kanban.Backend.BusinessLayer.BoardModule;
using IntroSE.Kanban.Backend.DataAccessLayer.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class BoardSL
    {
        public string BoardName { get; set; }
        public long BoardId { get; set; }
        public string BoardOwnerEmail { get; set; }
        internal List<ColumnSL> Columns { get; set; }
        public List<string> Members { get; set; }
        internal long CounterForTaskIdInBoard { get; set; }

        internal BoardSL(BoardBL boardbl)
        {
            BoardName = boardbl.BoardName;
            Columns = new List<ColumnSL>();
            Columns.Add(new ColumnSL(boardbl.Columns[0]));
            Columns.Add(new ColumnSL(boardbl.Columns[1]));
            Columns.Add(new ColumnSL(boardbl.Columns[2]));

            CounterForTaskIdInBoard = boardbl.CounterForTaskIdInBoard;
            BoardId = boardbl.BoardId;
            Members = boardbl.Members;
            BoardOwnerEmail = boardbl.BoardOwnerEmail;

        }
        public BoardSL()
        {
            // paramaterless constructor for deserialization
        }
    }
}

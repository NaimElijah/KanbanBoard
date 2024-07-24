using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Services;

namespace BackendTests.BoardTests
{
    internal class TasksTests
    {
        internal ServiceFactory SerFac { get; set; }

        private Response LegitRes = new Response(null, null);
        private Response NotLogged = new Response("the email given is not logged in the system", null);
        private Response BadBoardName = new Response("Board name given does not exist in that user's boards", null);
        private Response TaskIdNotExist = new Response("Task Id given does not exist in the board of that user", null);
        private Response NullRes = new Response("At least one of the given arguments is null or empty", null);
        private Response BadColumnNum = new Response("the column number given is out of bounds, only columns 0 and 1 can be changed", null);
        private Response IdNotInColumn = new Response("the task Id given is not in the column given", null);
        private Response TooLong = new Response("Title has to be max. 50 characters, not empty. and for the description max. 300 characters, optional", null);
        private Response wrongDate = new Response("due date is not legal", null);
        private Response NotABoardMember = new Response("The user is not a board member", null);
        private Response NotTaskAssignee = new Response("The user is trying to change the task even though he's not that task's assignee", null);
        private Response NotAssigneeTryingToAssign = new Response("The user trying to reassign the task is not the current task assignee", null);


        public TasksTests(ServiceFactory serfac)
        {
            SerFac = serfac;
        }




        /// <summary>
        /// This method tests the requirement: The board will support adding new tasks to its backlog column only by a board member.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestAddTask()
        {
            Console.WriteLine("Testing with TestAddTask:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass1234");
            SerFac.Bs.CreateBoard("n1@gmail.com", "board1");
            SerFac.Bs.CreateBoard("n1@gmail.com", "board2");
            SerFac.Us.Register("Taeer@walla.com", "Taeer123");
            SerFac.Bs.CreateBoard("Taeer@walla.com", "goals");
            SerFac.Us.Register("Taeery@walla.com", "Taeer123");
            SerFac.Bs.CreateBoard("Taeery@walla.com", "goalssss");
            SerFac.Us.Logout("Taeery@walla.com");

            if (!SerFac.Ts.AddTask("n1@gmail.com", "board1", "game of thrones", "Taeer is cool", new DateTime(2025, 7, 1, 13, 10, 00)).Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }
            if (!SerFac.Ts.AddTask("n1@gmail.com", "board2", "homework", "Taeer is cool", new DateTime(2025, 7, 1, 14, 10, 00)).Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }
            if (!SerFac.Ts.AddTask("n1@gmail.com", "board2","kanban project is so long i wanna die, is it gonna last foreverjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj? to be or not to be that is the question", "it is actually pretty cool cool", new DateTime(2024, 7, 1, 14, 10, 00)).Equals(TooLong.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 3" + "\n";
            }



            if (!SerFac.Ts.AddTask("Taeery@walla.com", "goalssss", "ghtdg", "Taeer is cool", new DateTime(2024, 7, 1, 14, 10, 00)).Equals(NotLogged.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 5" + "\n";
            }
            if (!SerFac.Ts.AddTask("Taeer@walla.com", "was ist das", "goals", "Taeer is cool", new DateTime(2024, 7, 1, 14, 10, 00)).Equals(BadBoardName.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 6" + "\n";
            }
            if (!SerFac.Ts.AddTask("n1@gmail.com", "", "goals", "Taeer is cool", new DateTime(2024, 7, 1, 14, 10, 00)).Equals(NullRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 7" + "\n";
            }
            if (!SerFac.Ts.AddTask("n1@gmail.com", "board1", "", "Taeer is cool", new DateTime(2024, 7, 1, 14, 10, 00)).Equals(NullRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 8" + "\n";
            }

            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            if (res == "")
            {
                res = "All Tests PASSED";
            }
            return res;

        }






        /// <summary>
        /// This method tests the requirement: A task that is not done can be changed by the user(only by the task assignee).
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestUpdateTaskDueDate()
        {
            Console.WriteLine("Testing with TestUpdateTaskDueDate:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("stav@gmail.com", "Pass123");
            SerFac.Us.Register("naim@gmail.com", "Pass123");

            SerFac.Bs.CreateBoard("stav@gmail.com", "board1");
            SerFac.Bs.JoinBoard("naim@gmail.com", 0);
            SerFac.Ts.AddTask("stav@gmail.com", "board1", "Title", "Description", new DateTime(2025, 7, 1, 13, 10, 00));
            SerFac.Ts.AssignTask("naim@gmail.com", "board1", 0, 0, "stav@gmail.com");

            if (!SerFac.Ts.UpdateTaskDueDate("stav@gmail.com", "board1", 0, 0, new DateTime(2025, 9, 11, 13, 10, 00)).Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit board
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskDueDate("naim@gmail.com", "board1", 0, 0, new DateTime(2026, 8, 11, 13, 10, 00)).Equals(NotTaskAssignee.GetSerializedVersion()))   //  TEST Legit board
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskDueDate("n2@gmail.com", "board1", 0, 0, new DateTime(2025, 9, 11, 13, 10, 00)).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 3" + "\n";
            }

            SerFac.Us.Logout("stav@gmail.com");
            if (!SerFac.Ts.UpdateTaskDueDate("stav@gmail.com", "board1", 0, 0, new DateTime(2025, 9, 11, 13, 10, 00)).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 4" + "\n";
            }


            SerFac.Us.Login("stav@gmail.com", "Pass123");
            if (!SerFac.Ts.UpdateTaskDueDate("stav@gmail.com", "board2", 0, 0, new DateTime(2025, 9, 11, 13, 10, 00)).Equals(BadBoardName.GetSerializedVersion()))   //  TEST bad board name
            {
                res += "Test FAILED - Test 5" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskDueDate("stav@gmail.com", "board1", 0, 2, new DateTime(2025, 9, 11, 13, 10, 00)).Equals(TaskIdNotExist.GetSerializedVersion()))   //  TEST task id non existent
            {
                res += "Test FAILED - Test 6" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskDueDate(null, "board1", 0, 0, new DateTime(2025, 9, 11, 13, 10, 00)).Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 7" + "\n";
            }
            if (!SerFac.Ts.UpdateTaskDueDate("stav@gmail.com", null, 0, 0, new DateTime(2025, 9, 11, 13, 10, 00)).Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 8" + "\n";
            }

            
            if (!SerFac.Ts.UpdateTaskDueDate("stav@gmail.com", "board1", -3, 0, new DateTime(2025, 9, 11, 13, 10, 00)).Equals(BadColumnNum.GetSerializedVersion()))   //  TEST bad col num
            {
                res += "Test FAILED - Test 9" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskDueDate("stav@gmail.com", "board1", 1, 0, new DateTime(2025, 9, 11, 13, 10, 00)).Equals(IdNotInColumn.GetSerializedVersion()))   //  TEST id not in column
            {
                res += "Test FAILED - Test 10" + "\n";
            }



            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            if (res == "")
            {
                res = "All Tests PASSED";
            }
            return res;

        }









        /// <summary>
        /// This method tests the requirement: A task that is not done can be changed by the user(only by the task assignee).
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestUpdateTaskTitle()
        {
            Console.WriteLine("Testing with TestUpdateTaskTitle:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass123");
            SerFac.Us.Register("n2@gmail.com", "Pass123");

            SerFac.Bs.CreateBoard("n1@gmail.com", "board1");
            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title", "Description", new DateTime(2025, 7, 1, 13, 10, 00));
            SerFac.Ts.AssignTask("n1@gmail.com", "board1", 0, 0, "n1@gmail.com");

            if (!SerFac.Ts.UpdateTaskTitle("n2@gmail.com", "board1", 0, 0, "New Title 1").Equals(BadBoardName.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            SerFac.Bs.JoinBoard("n2@gmail.com", 0);
            if (!SerFac.Ts.UpdateTaskTitle("n2@gmail.com", "board1", 0, 0, "New Title 1").Equals(NotTaskAssignee.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }


            if (!SerFac.Ts.UpdateTaskTitle("n1@gmail.com", "board1", 0, 0, "New Title 1").Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit board
            {
                res += "Test FAILED - Test 3" + "\n";
            }



            if (!SerFac.Ts.UpdateTaskTitle("n1@gmail.com", "board1", 0, 0, "Neeeeeeew Titlerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrsssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssddddddddddddddddddddddddddddddddddddddddddddddddddddd").Equals(TooLong.GetSerializedVersion()))   //  TEST too long
            {
                res += "Test FAILED - Test 4" + "\n";
            }


            if (!SerFac.Ts.UpdateTaskTitle("n3@gmail.com", "board1", 0, 0, "New Title 2").Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 5" + "\n";
            }
            SerFac.Us.Logout("n1@gmail.com");
            if (!SerFac.Ts.UpdateTaskTitle("n1@gmail.com", "board1", 0, 0, "New Title 3").Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 6" + "\n";
            }


            SerFac.Us.Login("n1@gmail.com", "Pass123");
            if (!SerFac.Ts.UpdateTaskTitle("n1@gmail.com", "board2", 0, 0, "New Title 4").Equals(BadBoardName.GetSerializedVersion()))   //  TEST bad board name
            {
                res += "Test FAILED - Test 7" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskTitle("n1@gmail.com", "board1", 0, 2, "New Title 5").Equals(TaskIdNotExist.GetSerializedVersion()))   //  TEST task id non existent
            {
                res += "Test FAILED - Test 8" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskTitle(null, "board1", 0, 0, "New Title 6").Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 9" + "\n";
            }
            if (!SerFac.Ts.UpdateTaskTitle("n1@gmail.com", null, 0, 0, "New Title 7").Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 10" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskTitle("n1@gmail.com", "board1", -3, 0, "New Title 8").Equals(BadColumnNum.GetSerializedVersion()))   //  TEST bad col num
            {
                res += "Test FAILED - Test 11" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskTitle("n1@gmail.com", "board1", 1, 0, "New Title 9").Equals(IdNotInColumn.GetSerializedVersion()))   //  TEST id not in column
            {
                res += "Test FAILED - Test 12" + "\n";
            }


            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            if (res == "")
            {
                res = "All Tests PASSED";
            }
            return res;

        }













        /// <summary>
        /// This method tests the requirement: A task that is not done can be changed by the user(only by the task assignee).
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestUpdateTaskDescription()
        {
            Console.WriteLine("Testing with TestUpdateTaskDescription:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass1235");
            SerFac.Bs.CreateBoard("n1@gmail.com", "board1");
            SerFac.Bs.CreateBoard("n1@gmail.com", "board2");
            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title1", "Description1", new DateTime(2025, 7, 1, 13, 10, 00));
            SerFac.Ts.AddTask("n1@gmail.com", "board2", "Title2", "Description2", new DateTime(2025, 7, 1, 13, 15, 00));
            SerFac.Ts.AssignTask("n1@gmail.com", "board1", 0, 0, "n1@gmail.com");
            SerFac.Ts.AssignTask("n1@gmail.com", "board2", 0, 0, "n1@gmail.com");

            SerFac.Us.Register("Taeer@walla.com", "Taeer123");
            SerFac.Bs.CreateBoard("Taeer@walla.com", "goals");
            SerFac.Us.Register("Taeery@walla.com", "Taeer123");
            SerFac.Bs.CreateBoard("Taeery@walla.com", "goalssss");
            SerFac.Ts.AddTask("Taeery@walla.com", "goalssss","Bday goals", "27 years old better have kids", new DateTime(2025, 6, 15, 12, 15, 00));
            SerFac.Ts.AssignTask("Taeery@walla.com", "goalssss", 0, 0, "Taeery@walla.com");
            SerFac.Bs.JoinBoard("n1@gmail.com", 3);
            SerFac.Us.Logout("Taeery@walla.com");
            
            if (!SerFac.Ts.UpdateTaskDescription("n1@gmail.com", "board1", 0, 0, "Taeer is cool").Equals(LegitRes.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskDescription("n1@gmail.com", "goalssss", 0, 0, "ccccoooooooollllll").Equals(NotTaskAssignee.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            if (!SerFac.Ts.UpdateTaskDescription("n1@gmail.com", "board2", 0, 0, "Taeer is cool").Equals(LegitRes.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 3" + "\n";
            }
            if (!SerFac.Ts.UpdateTaskDescription("n1@gmail.com", "board1", 0, 10, "Taeer is cool").Equals(TaskIdNotExist.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 4" + "\n";
            }
            if (!SerFac.Ts.UpdateTaskDescription("n123@gmail.com", "board1", 0, 0, "Taeer is cool").Equals(NotLogged.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 5" + "\n";
            } 
            SerFac.Us.Logout("Taeery@walla.com");
            if (!SerFac.Ts.UpdateTaskDescription("Taeery@walla.com", "goalssss", 0, 0, "Taeer is cool").Equals(NotLogged.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 6" + "\n";
            }
            if (!SerFac.Ts.UpdateTaskDescription("Taeer@walla.com", "goals", 0, 0, "Taeer is cool").Equals(IdNotInColumn.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 7" + "\n";
            }
            if (!SerFac.Ts.UpdateTaskDescription("n1@gmail.com", "board1", 12, 0, "Taeer is cool").Equals(BadColumnNum.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 8" + "\n";
            }
            if (!SerFac.Ts.UpdateTaskDescription("n1@gmail.com", "billa", 0, 1, "Taeer is cool").Equals(BadBoardName.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 9" + "\n";
            } 
            if (!SerFac.Ts.UpdateTaskDescription("n1@gmail.com", "board1", 0, 0, "").Equals(LegitRes.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 10" + "\n";
            }
            if (!SerFac.Ts.UpdateTaskDescription("n1@gmail.com", "board1", 0, 0, "Taeer is coolllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll").Equals(TooLong.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 11" + "\n";
            }

            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            if (res == "")
            {
                res = "All Tests PASSED";
            }
            return res;

        }







        /// <summary>
        /// This method tests the requirement: Tasks can be moved from ‘backlog’ to ‘in progress’ or from ‘in progress’ to ‘done’ columns(only by the task assignee). No other movements are allowed.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestAdvanceTask()
        {
            Console.WriteLine("Testing with TestAdvanceTask:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("stav@gmail.com", "Pass123");

            SerFac.Bs.CreateBoard("stav@gmail.com", "board1");
            SerFac.Ts.AddTask("stav@gmail.com", "board1", "Title", "Description", new DateTime(2025, 7, 1, 13, 10, 00));
            SerFac.Ts.AssignTask("stav@gmail.com", "board1", 0, 0, "stav@gmail.com");

            if (!SerFac.Ts.AdvanceTask("stav@gmail.com", "board1", 0, 0).Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit board
            {
                res += "Test FAILED - Test 1" + "\n";
            }


            if (!SerFac.Ts.AdvanceTask("n2@gmail.com", "board1", 1, 0).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            SerFac.Us.Logout("stav@gmail.com");
            if (!SerFac.Ts.AdvanceTask("stav@gmail.com", "board1", 1, 0).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 3" + "\n";
            }


            SerFac.Us.Login("stav@gmail.com", "Pass123");
            if (!SerFac.Ts.AdvanceTask("stav@gmail.com", "board2", 1, 0).Equals(BadBoardName.GetSerializedVersion()))   //  TEST bad board name
            {
                res += "Test FAILED - Test 4" + "\n";
            }

            if (!SerFac.Ts.AdvanceTask("stav@gmail.com", "board1", 1, 2).Equals(TaskIdNotExist.GetSerializedVersion()))   //  TEST task id non existent
            {
                res += "Test FAILED - Test 5" + "\n";
            }

            if (!SerFac.Ts.AdvanceTask(null, "board1", 1, 0).Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 6" + "\n";
            }
            if (!SerFac.Ts.AdvanceTask("stav@gmail.com", null, 1, 0).Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 7" + "\n";
            }

            if (!SerFac.Ts.AdvanceTask("stav@gmail.com", "board1", -3, 0).Equals(BadColumnNum.GetSerializedVersion()))   //  TEST bad col num
            {
                res += "Test FAILED - Test 8" + "\n";
            }

            if (!SerFac.Ts.AdvanceTask("stav@gmail.com", "board1", 0, 0).Equals(IdNotInColumn.GetSerializedVersion()))   //  TEST id not in column
            {
                res += "Test FAILED - Test 9" + "\n";
            }



            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            if (res == "")
            {
                res = "All Tests PASSED";
            }
            return res;

        }







        /// <summary>
        /// This method tests the requirement: Users will be able to list their 'in progress’ tasks that they are assigned to from all of their boards.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestGetInProgressTasks()
        {
            Console.WriteLine("Testing with TestGetInProgressTasks:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass123");
            SerFac.Bs.CreateBoard("n1@gmail.com", "board1");
            SerFac.Us.Register("taeer@gmail.com", "Pass123");
            SerFac.Bs.CreateBoard("taeer@gmail.com", "cool");
            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title1", "Description1", new DateTime(2025, 9, 1, 13, 10, 00));
            SerFac.Ts.AssignTask("n1@gmail.com", "board1", 0, 0, "n1@gmail.com");

            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title2", "Description2", new DateTime(2025, 10, 9, 10, 11, 13));
            SerFac.Ts.AdvanceTask("n1@gmail.com", "board1", 0, 0);
            

            SerFac.Us.Logout("taeer@gmail.com");
            if(!SerFac.Ts.GetInProgressTasks("taeer@gmail.com").Equals(NotLogged.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            SerFac.Ts.AdvanceTask("n1@gmail.com", "board1", 1, 0);
            

            if(!SerFac.Ts.GetInProgressTasks("").Equals(NullRes.GetSerializedVersion()))  
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            List<TaskSL> ls = new List<TaskSL>();
            Response NoinProgTasksNoBoards = new Response(null, ls.ToList());

            SerFac.Us.Login("taeer@gmail.com", "Pass123");
            if (!SerFac.Ts.GetInProgressTasks("taeer@gmail.com").Equals(NoinProgTasksNoBoards.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 3" + "\n";
            }

            SerFac.Bs.DeleteBoard("taeer@gmail.com", "cool");
            if (!SerFac.Ts.GetInProgressTasks("taeer@gmail.com").Equals(NoinProgTasksNoBoards.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 4" + "\n";
            }

            if (!SerFac.Ts.GetInProgressTasks("n1@gmail.com").Equals(NoinProgTasksNoBoards.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 5" + "\n";
            }

            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            if (res == "")
            {
                res = "All Tests PASSED";
            }
            return res;

        }






        /// <summary>
        /// An unassigned task can be assigned by any board member to any board member. By
        /// default, a task is unassigned. The assignment of an assigned task can be changed only by
        /// its assignee.
        /// </summary>
        /// <returns>The function will return a string describing the amount of tests executed and will say which ones were successful.</returns>
        internal string TestAssignTask()
        {
            Console.WriteLine("Testing with TestAssignTask:");

            string res = "There are 5 tests: \n"; 

            SerFac.Us.Register("naim@gmail.com", "Pass123"); //  create a user
            SerFac.Us.Register("taeer@post.bgu.ac.il", "Pass123"); //  create a user
            SerFac.Us.Register("stav@gmail.com", "Pass123"); //  create a user 
            SerFac.Us.Register("david@gmail.com", "Pass123"); //  create a user

            SerFac.Bs.CreateBoard("naim@gmail.com", "board1"); // create a board
            SerFac.Bs.JoinBoard("taeer@post.bgu.ac.il", 0); // join board
            SerFac.Bs.JoinBoard("stav@gmail.com", 0); // join board

            SerFac.Ts.AddTask("taeer@post.bgu.ac.il", "board1", "Work for Stav", "Enjoy", new DateTime(2025, 7, 1, 13, 10, 00));

            // someone who isn't a board member trying to assign an unassigned task
            if (SerFac.Ts.AssignTask("david@gmail.com", "board1", 0, 0, "stav@gmail.com").Equals((BadBoardName.GetSerializedVersion())))
            {
                res += "Test PASSED - Test 1" + "\n";
            }

            // someone who is a board member trying to assign an unassigned task
            if (SerFac.Ts.AssignTask("taeer@post.bgu.ac.il", "board1", 0, 0, "stav@gmail.com").Equals((LegitRes.GetSerializedVersion())))
            {
                res += "Test PASSED - Test 2" + "\n";
            }

            // someone who isn't the assignee of a task trying to assign an assigned task
            if (SerFac.Ts.AssignTask("naim@gmail.com", "board1", 0, 0, "taeer@post.bgu.ac.il").Equals((NotAssigneeTryingToAssign.GetSerializedVersion())))
            {
                res += "Test PASSED - Test 3" + "\n";
            }

            // someone who is the assignee of a task trying to assign an assigned task
            if (SerFac.Ts.AssignTask("stav@gmail.com", "board1", 0, 0, "taeer@post.bgu.ac.il").Equals((LegitRes.GetSerializedVersion())))
            {
                res += "Test PASSED - Test 4" + "\n";
            }

            // someone who isn't a board member trying to assign an assigned task
            if (SerFac.Ts.AssignTask("david@gmail.com", "board1", 0, 0, "stav@gmail.com").Equals((BadBoardName.GetSerializedVersion())))
            {
                res += "Test PASSED - Test 5" + "\n";
            }


            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            return res;

        }






        /// <summary>
        /// This method executes all tests in this class.
        /// </summary>
        /// <returns>Successful tests will return "Test PASSED" and unsuccessful tests will return "Test FAILED"</returns>
        internal void TestAll()
        {
            Console.WriteLine(TestAddTask());

            Console.WriteLine(TestUpdateTaskDueDate());

            Console.WriteLine(TestUpdateTaskTitle());

            Console.WriteLine(TestUpdateTaskDescription());

            Console.WriteLine(TestAdvanceTask());

            Console.WriteLine(TestGetInProgressTasks());

            Console.WriteLine(TestAssignTask());
        }

    }
}

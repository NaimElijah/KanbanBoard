using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Services;

namespace BackendTests.BoardTests
{
    internal class BoardTests
    {
        internal ServiceFactory SerFac { get; set; }

        private Response NullRes = new Response("One or two of the arguments given are null or empty", null); 
        private Response LegitRes = new Response(null, null);
        private Response NotLoggedRes = new Response("User has to be logged into the system", null);
        private Response NotFoundBoardId = new Response("This board Id doesn't exist in the system", null);
        private Response NotFoundBoardName = new Response("This board name doesn't exist for the user", null);
        private Response SameNameOrMember = new Response("user is already a member of this board or has a board with identical name", null);
        private Response NotOwner = new Response("This action cannot be done by someone whos not the owner of this board", null);
        private Response AlreadyIsBoardOwner = new Response("It is already the Board Owner", null);
        private Response NotABoardMember = new Response("A user is not a board member in that board", null);
        private Response ThisUserNotBoardMember = new Response("user is not a member of this board", null);
        private Response OwnerCantLeave = new Response("A board owner cannot leave a board he has ownership over", null);




        public BoardTests(ServiceFactory serfac)
        {
            SerFac = serfac;
        }




        /// <summary>
        /// This method tests the requirement: Users will be able to create and delete boards. The user who creates the board will be its
        /// owner (unless transferred)
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestCreateBoard()
        {
            Console.WriteLine("Testing with TestCreateBoard:");
            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.


            SerFac.Us.Register("n1@gmail.com", "Pass123");

            Response BadNameRes = new Response("A user cannot create a board that has the same name as a board he already has", null);

            if (!SerFac.Bs.CreateBoard("n1@gmail.com", "Bname1").Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit board
            {
                res += "Test FAILED - Test 1" + "\n";
            }




            if (!SerFac.Bs.CreateBoard("n1@gmail.com", "Bname1").Equals(BadNameRes.GetSerializedVersion()))   //  TEST board name
            {
                res += "Test FAILED - Test 2" + "\n";
            }




            if (!SerFac.Bs.CreateBoard("n2@gmail.com", "BsomeName").Equals(NotLoggedRes.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 3" + "\n";
            }

            SerFac.Us.Logout("n1@gmail.com");
            if (!SerFac.Bs.CreateBoard("n1@gmail.com", "Bname2").Equals(NotLoggedRes.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 4" + "\n";
            }






            if (!SerFac.Bs.CreateBoard("", "Bname3").Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test FAILED - Test 5" + "\n";
            }

            if (!SerFac.Bs.CreateBoard("n3@gmail.com", "").Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test FAILED - Test 6" + "\n";
            }

            if (!SerFac.Bs.CreateBoard("", "").Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test FAILED - Test 7" + "\n";
            }

            if (!SerFac.Bs.CreateBoard("n4@gmail.com", null).Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test FAILED - Test 8" + "\n";
            }

            if (!SerFac.Bs.CreateBoard(null, "Bname4").Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test FAILED - Test 9" + "\n";
            }

            if (!SerFac.Bs.CreateBoard(null, null).Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test FAILED - Test 10" + "\n";
            }




            SerFac.Us.Login("n1@gmail.com", "Pass123");
            if (!SerFac.Bs.CreateBoard("n1@gmail.com", "Bname2").Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit board
            {
                res += "Test FAILED - Test 11" + "\n";
            }


            if (!SerFac.Bs.CreateBoard("n1@.gmail.com", "Bname3").Equals(NotLoggedRes.GetSerializedVersion()))   //  TEST of not logged and invalid email even though it won't even be added 
            {                                                                                                                                            //  in the register function
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
        /// This method tests the requirement: Users will be able to create and delete boards. A board owner can delete a board (all of its tasks are deleted).
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestDeleteBoard()
        {
            Console.WriteLine("Testing with TestDeleteBoard:");

            string res = "There are 9 tests. If a test passes it says so. If it isn't mentioned then it failed:\n";

            Response BoardDoesntExistRes = new Response("User does not have any existing boards", null);

            SerFac.Us.Register("n1@gmail.com", "Pass123"); //  create a user
            SerFac.Us.Register("n2@gmail.com", "Pass123"); //  create a user
            SerFac.Bs.CreateBoard("n1@gmail.com", "Bname1"); // create a board


            // Tests 1-4: null inputs:
            if (SerFac.Bs.DeleteBoard("", "Bname3").Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test passed - Test 1" + "\n";
            }

            if (SerFac.Bs.DeleteBoard("n1@gmail.com", "").Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test passed - Test 2" + "\n";
            }

            if (SerFac.Bs.DeleteBoard("n1@gmail.com", null).Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test passed - Test 3" + "\n";
            }

            if (SerFac.Bs.DeleteBoard(null, "Bname4").Equals(NullRes.GetSerializedVersion()))   //  TEST null argument\s
            {
                res += "Test passed - Test 4" + "\n";
            }

            // Test 5 - deleting a board that doesn't exist
            if (SerFac.Bs.DeleteBoard("n1@gmail.com", "I don't exist").Equals(BoardDoesntExistRes.GetSerializedVersion()))
            {
                res += "Test passed - Test 5" + "\n";
            }


            SerFac.Bs.JoinBoard("n2@gmail.com", 0);
            // Test 6 - deleting a board while not owner of that board
            if (SerFac.Bs.DeleteBoard("n2@gmail.com", "Bname1").Equals(NotOwner.GetSerializedVersion()))
            {
                res += "Test passed - Test 6" + "\n";
            }

            // Test 7 - deleting a board 
            if (SerFac.Bs.DeleteBoard("n1@gmail.com", "Bname1").Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test passed - Test 7" + "\n";
            }

            // Test 8 - deleting a board from a user that has no boards 
            if (SerFac.Bs.DeleteBoard("n1@gmail.com", "whatever").Equals(BoardDoesntExistRes.GetSerializedVersion()))
            {
                res += "Test passed - Test 8" + "\n";
            }

            SerFac.Bs.CreateBoard("n1@gmail.com", "Bname1"); // recreate the deleted board

            // Test 9 - deleting a board while not being logged in
            SerFac.Us.Logout("n1@gmail.com");
            if (SerFac.Bs.DeleteBoard("n1@gmail.com", "Bname1").Equals(NotLoggedRes.GetSerializedVersion()))
            {
                res += "Test passed - Test 9" + "\n";
            }

            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            return res;

        }





        /// <summary>
        /// This method tests the TestGetUserBoards given to us in the GradingService.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestGetUserBoards()
        {
            Console.WriteLine("Testing with TestGetUserBoards:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass123");
            SerFac.Us.Register("n2@gmail.com", "Pass1234");

            SerFac.Us.Register("n3@gmail.com", "Pass1234");

            SerFac.Bs.CreateBoard("n1@gmail.com", "b1");
            SerFac.Bs.JoinBoard("n2@gmail.com", 0);

            List<long> ids = new List<long>();
            ids.Add(0);
            Response idsRes = new Response(null, ids);

            if (!SerFac.Bs.GetUserBoards("n1@gmail.com").Equals(idsRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }
            if (!SerFac.Bs.GetUserBoards("n2@gmail.com").Equals(idsRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            Response empLsRes = new Response(null, new List<long>());
            if (!SerFac.Bs.GetUserBoards("n3@gmail.com").Equals(empLsRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 3" + "\n";
            }

            SerFac.Bs.CreateBoard("n2@gmail.com", "b2");
            ids.Add(1);
            idsRes = new Response(null, ids);
            if (!SerFac.Bs.GetUserBoards("n2@gmail.com").Equals(idsRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 4" + "\n";
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
        ///  Users will be able to join and leave existing boards created by someone else. There is no need to get permission from the board owner.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestJoinBoard()
        {
            Console.WriteLine("Testing with TestJoinBoard:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass123"); //  create a user
            SerFac.Us.Register("taeery@post.bgu.ac.il", "Pass123"); //  create a user
            SerFac.Us.Register("satvos@gmail.com", "Pass123"); //  create a user

            SerFac.Bs.CreateBoard("n1@gmail.com", "Bname1"); // create a board
            SerFac.Bs.JoinBoard("taeery@post.bgu.ac.il", 0);

            if (!SerFac.Bs.JoinBoard("satvos@gmail.com", 0).Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit input
            {
                res += "Test FAILED - Test 1" + "\n";
            }
          
            if (!SerFac.Bs.JoinBoard("n1@gmail.com", 0).Equals(SameNameOrMember.GetSerializedVersion()))   // TEST Illegitimate password
            {
                res += "Test FAILED - Test 2" + "\n";
            }
            if (!SerFac.Bs.JoinBoard("taeery@post.bgu.ac.il", 0).Equals(SameNameOrMember.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 3" + "\n";
            }
            if (!SerFac.Bs.JoinBoard("taeery@post.bgu.ac.il", 1).Equals(NotFoundBoardId.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 4" + "\n";
            }
            if (!SerFac.Bs.JoinBoard("n1@gmail.com", 7).Equals(NotFoundBoardId.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 5" + "\n";
            }

            SerFac.Bs.LeaveBoard("taeery@post.bgu.ac.il", 0);

            if (!SerFac.Bs.JoinBoard("taeery@post.bgu.ac.il", 0).Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit
            {
                res += "Test FAILED - Test 6" + "\n";
            }
            SerFac.Bs.LeaveBoard("taeery@post.bgu.ac.il", 0);
            SerFac.Bs.CreateBoard("taeery@post.bgu.ac.il", "Bname1");

            if (!SerFac.Bs.JoinBoard("taeery@post.bgu.ac.il", 0).Equals(SameNameOrMember.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 7" + "\n";
            }

            if (!SerFac.Bs.JoinBoard("satvos@gmail.com", 1).Equals(SameNameOrMember.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 8" + "\n";
            }

            if (!SerFac.Bs.JoinBoard("taeery@post.bgu.ac.il", 1).Equals(SameNameOrMember.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 9" + "\n";
            }

            SerFac.Bs.LeaveBoard("satvos@gmail.com", 0);

            if (!SerFac.Bs.JoinBoard("satvos@gmail.com", 0).Equals(LegitRes.GetSerializedVersion()))   //  TEST Illegitimate password
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
        ///  Users will be able to join and leave existing boards created by someone else. There is no need to get permission from the board owner.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestLeaveBoard()
        {
            Console.WriteLine("Testing with TestLeaveBoard:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass123"); //  create a user
            SerFac.Us.Register("taeery@post.bgu.ac.il", "Pass123"); //  create a user
            SerFac.Us.Register("satvos@gmail.com", "Pass123"); //  create a user 

            SerFac.Bs.CreateBoard("n1@gmail.com", "Bname1"); // create a board

            SerFac.Bs.JoinBoard("taeery@post.bgu.ac.il", 0);
            SerFac.Bs.JoinBoard("satvos@gmail.com", 0);

            SerFac.Ts.AddTask("taeery@post.bgu.ac.il", "Bname1", "mesimot le Stav", "she lo tishta'amem", new DateTime(2025, 7, 1, 13, 10, 00));
            SerFac.Ts.AssignTask("taeery@post.bgu.ac.il", "Bname1", 0, 0, "satvos@gmail.com");

            if (!SerFac.Bs.LeaveBoard("satvos@gmail.com", 0).Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit input
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            if (!SerFac.Bs.LeaveBoard("satvos@gmail.com", 0).Equals(NotFoundBoardName.GetSerializedVersion()))   // TEST Illegitimate password
            {
                res += "Test FAILED - Test 2" + "\n";
            }
            if (!SerFac.Bs.LeaveBoard("n1@gmail.com", 0).Equals(OwnerCantLeave.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 3" + "\n";
            }
            if (!SerFac.Bs.LeaveBoard("taeery@post.bgu.ac.il", 0).Equals(LegitRes.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 4" + "\n";
            }

            if (!SerFac.Bs.LeaveBoard("n1@gmail.com", 7).Equals(NotFoundBoardId.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 5" + "\n";
            }

            SerFac.Bs.JoinBoard("taeery@post.bgu.ac.il", 0);

            if (!SerFac.Bs.LeaveBoard("taeery@post.bgu.ac.il", 2).Equals(NotFoundBoardId.GetSerializedVersion()))   //  TEST Legit
            {
                res += "Test FAILED - Test 6" + "\n";
            }

            SerFac.Bs.LeaveBoard("taeery@post.bgu.ac.il", 0);
            SerFac.Bs.CreateBoard("taeery@post.bgu.ac.il", "Bname1");

            if (!SerFac.Bs.LeaveBoard("taeery@post.bgu.ac.il", 0).Equals(ThisUserNotBoardMember.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 7" + "\n";
            }

            if (!SerFac.Bs.LeaveBoard("satvos@gmail.com", 1).Equals(NotFoundBoardName.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 8" + "\n";
            }

            if (!SerFac.Bs.LeaveBoard("taeery@post.bgu.ac.il", 1).Equals(OwnerCantLeave.GetSerializedVersion()))
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
        /// This method tests the TestGetBoardName given to us in the GradingService.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestGetBoardName()
        {
            Console.WriteLine("Testing with TestGetBoardName:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass123");
            SerFac.Us.Register("n2@gmail.com", "Pass1234");

            string name1 = "b1";
            Response res1 = new Response(null, name1);
            string name2 = "b2";
            Response res2 = new Response(null, name2);

            SerFac.Bs.CreateBoard("n1@gmail.com", name1);
            SerFac.Bs.CreateBoard("n2@gmail.com", name2);


            if (!SerFac.Bs.GetBoardName(0).Equals(res1.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            if (!SerFac.Bs.GetBoardName(1).Equals(res2.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            if (!SerFac.Bs.GetBoardName(2).Equals(NotFoundBoardId.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 3" + "\n";
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
        /// A board owner can transfer the board ownership to another board member (a user who joined the board).
        /// </summary>
        /// <returns>The function will return a string describing the amount of tests executed and will say which ones were successful.</returns>
        internal string TestTransferBoardOwnership()
        {
            Console.WriteLine("Testing with TestTransferBoardOwnership:");

            string res = "There are 6 tests: \n";   

            SerFac.Us.Register("naim@gmail.com", "Pass123"); //  create a user
            SerFac.Us.Register("taeer@post.bgu.ac.il", "Pass123"); //  create a user
            SerFac.Us.Register("stav@gmail.com", "Pass123"); //  create a user
            SerFac.Us.Register("david@gmail.com", "Pass123"); //  create a user 

            SerFac.Bs.CreateBoard("naim@gmail.com", "board1"); // create a board
            SerFac.Bs.JoinBoard("taeer@post.bgu.ac.il", 0);
            SerFac.Bs.JoinBoard("stav@gmail.com", 0);


            // someone who isn't the owner trying to transfer ownership
            if (SerFac.Bs.TransferBoardOwnership("stav@gmail.com", "taeer@post.bgu.ac.il", "board1").Equals(NotOwner.GetSerializedVersion()))
            {
                res += "Test PASSED - Test 1" + "\n";
            }

            // owner trying to transfer ownership to himself
            if (SerFac.Bs.TransferBoardOwnership("naim@gmail.com", "naim@gmail.com", "board1").Equals(AlreadyIsBoardOwner.GetSerializedVersion()))
            {
                res += "Test PASSED - Test 2" + "\n";
            } 

            // owner trying to transfer ownership to someone who isn't a member
            if (SerFac.Bs.TransferBoardOwnership("naim@gmail.com", "david@gmail.com", "board1").Equals(NotABoardMember.GetSerializedVersion()))
            {
                res += "Test PASSED - Test 3" + "\n";
            } 
            
            // legit transfer from naim to taeer
            if (SerFac.Bs.TransferBoardOwnership("naim@gmail.com", "taeer@post.bgu.ac.il", "board1").Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test PASSED - Test 4" + "\n";
            }

            // Now naim trying to transfer ownership even though he isn't the owner anymore
            if (SerFac.Bs.TransferBoardOwnership("naim@gmail.com", "stav@gmail.com", "board1").Equals(NotOwner.GetSerializedVersion()))
            {
                res += "Test PASSED - Test 5" + "\n";
            }

            // Now taeer (the new owner) trying to transfer ownership to stav
            if (SerFac.Bs.TransferBoardOwnership("taeer@post.bgu.ac.il", "stav@gmail.com", "board1").Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test PASSED - Test 6" + "\n";
            }

            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            return res;
        }






        /// <summary>
        /// This method tests the requirement: In this assignment, persistent data is stored in an SQLite database. The following data should be persisted: users and boards (including
        /// tasks/columns/etc.) Persisted data should be restored once the program starts.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestLoadData()
        {
            Console.WriteLine("Testing with TestLoadData:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            if (!SerFac.Bs.LoadData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }
            if (!SerFac.Bs.LoadData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            SerFac.Bs.DeleteData();
            if (!SerFac.Bs.LoadData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 3" + "\n";
            }
            if (!SerFac.Bs.LoadData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 4" + "\n";
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
        /// This method tests the requirement: In this assignment, persistent data is stored in an SQLite database. The following data should be persisted: users and boards (including
        /// tasks/columns/etc.)
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestDeleteData()
        {
            Console.WriteLine("Testing with TestDeleteData:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.


            if (!SerFac.Bs.DeleteData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }
            if (!SerFac.Bs.DeleteData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            SerFac.Bs.LoadData();
            if (!SerFac.Bs.DeleteData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 3" + "\n";
            }
            if (!SerFac.Bs.DeleteData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 4" + "\n";
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
        /// This method executes all tests in this class.
        /// </summary>
        /// <returns>Successful tests will return "Test PASSED" and unsuccessful tests will return "Test FAILED"</returns>
        internal void TestAll()
        {
            Console.WriteLine(TestCreateBoard());
            
            Console.WriteLine(TestDeleteBoard());
            
            Console.WriteLine(TestGetUserBoards());
            
            Console.WriteLine(TestJoinBoard());
            
            Console.WriteLine(TestLeaveBoard());
            
            Console.WriteLine(TestGetBoardName());
            
            Console.WriteLine(TestTransferBoardOwnership());
            
            Console.WriteLine(TestLoadData());
            
            Console.WriteLine(TestDeleteData());
        }


    }
}

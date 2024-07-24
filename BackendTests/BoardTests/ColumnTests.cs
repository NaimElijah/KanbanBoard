using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Services;

namespace BackendTests.BoardTests
{
    internal class ColumnTests
    {
        internal ServiceFactory SerFac { get; set; }

        private Response LegitRes = new Response(null, null);
        private Response NotLogged = new Response("the email given is not logged in the system", null);
        private Response BadBoardName = new Response("Board name given does not exist in that user's boards", null);
        private Response NullRes = new Response("At least one of the given arguments is null or empty", null);
        private Response BadColumnNum = new Response("the column number given is out of bounds, there are only columns 0, 1, 2", null);

        private Response BadLimitNum = new Response("limit number given cannot be below -1 or below the number of tasks already in the specified column", null);

        public ColumnTests(ServiceFactory serfac)
        {
            SerFac = serfac;
        }


        



        /// <summary>
        /// This method tests the requirement: Each column should support limiting the maximum number of its tasks.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestLimitColumn()
        {
            Console.WriteLine("Testing with TestLimitColumn:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass123");
            SerFac.Bs.CreateBoard("n1@gmail.com", "board1");
            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title", "Description", new DateTime(2025, 9, 1, 13, 10, 00));

            if (!SerFac.Cs.LimitColumn("n1@gmail.com", "board1", 0, 5).Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            if (!SerFac.Cs.LimitColumn("n1@gmail.com", "board1", 0, -5).Equals(BadLimitNum.GetSerializedVersion()))   //  TEST Bad limit
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            if (!SerFac.Cs.LimitColumn("n1@gmail.com", "board1", 0, 0).Equals(BadLimitNum.GetSerializedVersion()))   //  TEST Bad limit
            {
                res += "Test FAILED - Test 3" + "\n";
            }



            if (!SerFac.Cs.LimitColumn("n2@gmail.com", "board1", 0, 5).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 4" + "\n";
            }

            SerFac.Us.Logout("n1@gmail.com");
            if (!SerFac.Cs.LimitColumn("n1@gmail.com", "board1", 0, 4).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 5" + "\n";
            }

            SerFac.Us.Login("n1@gmail.com", "Pass123");
            
            if (!SerFac.Cs.LimitColumn("n1@gmail.com", "board1", 4, 4).Equals(BadColumnNum.GetSerializedVersion()))   //  TEST bad column num
            {
                res += "Test FAILED - Test 7" + "\n";
            }

            if (!SerFac.Cs.LimitColumn(null, "board1", 0, 4).Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 8" + "\n";
            }
            if (!SerFac.Cs.LimitColumn("n1@gmail.com", "", 0, 4).Equals(NullRes.GetSerializedVersion()))   //  TEST null
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
        /// This method tests the requirement: this method returns a column's limit
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestGetColumnLimit()
        {
            Console.WriteLine("Testing with TestGetColumnLimit:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("stav@gmail.com", "Pass123");


            SerFac.Bs.CreateBoard("stav@gmail.com", "board1");
            SerFac.Ts.AddTask("stav@gmail.com", "board1", "Title", "Description", new DateTime(2024, 7, 1, 13, 10, 00));

            if (!SerFac.Cs.GetColumnLimit("stav@gmail.com", "board1", 0).Equals(new Response(null, -1).GetSerializedVersion()))   //  TEST Legit board
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            


            if (!SerFac.Cs.GetColumnLimit("n2@gmail.com", "board1", 0).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            SerFac.Us.Logout("stav@gmail.com");
            if (!SerFac.Cs.GetColumnLimit("stav@gmail.com", "board1", 0).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 3" + "\n";
            }


            SerFac.Us.Login("stav@gmail.com", "Pass123");
            if (!SerFac.Cs.GetColumnLimit("stav@gmail.com", "board2", 0).Equals(BadBoardName.GetSerializedVersion()))   //  TEST bad board name
            {
                res += "Test FAILED - Test 4" + "\n";
            }

       

            if (!SerFac.Cs.GetColumnLimit(null, "board1", 0).Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 5" + "\n";
            }
            if (!SerFac.Cs.GetColumnLimit("stav@gmail.com", null, 0).Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 6" + "\n";
            }

            if (!SerFac.Cs.GetColumnLimit("stav@gmail.com", "board1", 3).Equals(BadColumnNum.GetSerializedVersion()))   //  TEST bad col num
            {
                res += "Test FAILED - Test 7" + "\n";
            }
            if (!SerFac.Cs.GetColumnLimit("stav@gmail.com", "board1", -3).Equals(BadColumnNum.GetSerializedVersion()))   //  TEST bad col num
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
        /// This method tests the requirement: this method returns a column's name
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestGetColumnName()
        {
            Console.WriteLine("Testing with TestGetColumnName:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("taeer@gmail.com", "Pass123");
            SerFac.Us.Register("n1@gmail.com", "Pass123");
            SerFac.Bs.CreateBoard("n1@gmail.com", "board1");
            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title1", "Description1", new DateTime(2024, 9, 1, 13, 10, 00));
            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title2", "Description2", new DateTime(2024, 10, 9, 10, 11, 13));
            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title3", "Description2", new DateTime(2024, 10, 9, 10, 11, 13));
            SerFac.Ts.AdvanceTask("n1@gmail.com", "board1", 0, 2);
            SerFac.Ts.AdvanceTask("n1@gmail.com", "board1", 0, 3);
            SerFac.Ts.AdvanceTask("n1@gmail.com", "board1", 1, 3);


            if (!SerFac.Cs.GetColumnName("n2@gmail.com", "board1", 0).Equals(NotLogged.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 1" + "\n";
            }
            if (!SerFac.Cs.GetColumnName("taeer@gmail.com", "board1", 0).Equals(BadBoardName.GetSerializedVersion()))   
            {
                res += "Test FAILED - Test 2" + "\n";
            }
            if (!SerFac.Cs.GetColumnName("n1@gmail.com", "board1", 0).Equals(new Response(null, "backlog").GetSerializedVersion()))  
            {
                res += "Test FAILED - Test 3" + "\n";
            }
            if (!SerFac.Cs.GetColumnName("n1@gmail.com", "board1", 3).Equals(BadColumnNum.GetSerializedVersion()))  
            {
                res += "Test FAILED - Test 4" + "\n";
            }
            if (!SerFac.Cs.GetColumnName(null, "board1", 0).Equals(NullRes.GetSerializedVersion())) 
            {
                res += "Test FAILED - Test 5" + "\n";
            }
            if (!SerFac.Cs.GetColumnName("n1@gmail.com", "", 0).Equals(NullRes.GetSerializedVersion())) 
            {
                res += "Test FAILED - Test 6" + "\n";
            } 
            if (!SerFac.Cs.GetColumnName("n1@gmail.com", "board1", 17).Equals(BadColumnNum.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 7" + "\n";
            }
            if (!SerFac.Cs.GetColumnName("n1@gmail.com", "taeer", 17).Equals(BadBoardName.GetSerializedVersion())) 
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
        /// This method tests the requirement: this method returns a column
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestGetColumn()
        {
            Console.WriteLine("Testing with TestGetColumn:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("n1@gmail.com", "Pass123");
            SerFac.Bs.CreateBoard("n1@gmail.com", "board1");
            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title1", "Description1", new DateTime(2024, 9, 1, 13, 10, 00));
            SerFac.Ts.AddTask("n1@gmail.com", "board1", "Title2", "Description2", new DateTime(2024, 10, 9, 10, 11, 13));

            if (!SerFac.Cs.GetColumn("n2@gmail.com", "board1", 0).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            SerFac.Us.Logout("n1@gmail.com");
            if (!SerFac.Cs.GetColumn("n1@gmail.com", "board1", 0).Equals(NotLogged.GetSerializedVersion()))   //  TEST not logged
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            SerFac.Us.Login("n1@gmail.com", "Pass123");
            if (!SerFac.Cs.GetColumn("n1@gmail.com", "board2", 0).Equals(BadBoardName.GetSerializedVersion()))   //  TEST bad board name
            {
                res += "Test FAILED - Test 3" + "\n";
            }

            if (!SerFac.Cs.GetColumn("n1@gmail.com", "board1", 3).Equals(BadColumnNum.GetSerializedVersion()))   //  TEST bad column num
            {
                res += "Test FAILED - Test 4" + "\n";
            }

            if (!SerFac.Cs.GetColumn(null, "board1", 0).Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 5" + "\n";
            }
            if (!SerFac.Cs.GetColumn("n1@gmail.com", "", 0).Equals(NullRes.GetSerializedVersion()))   //  TEST null
            {
                res += "Test FAILED - Test 6" + "\n";
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
            Console.WriteLine(TestLimitColumn());

            Console.WriteLine(TestGetColumnLimit());

            Console.WriteLine(TestGetColumnName());

            Console.WriteLine(TestGetColumn());
        }

    }
}

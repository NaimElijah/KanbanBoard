using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Services;

namespace BackendTests.UserTests
{
    internal class UserTests
    {
        internal ServiceFactory SerFac { get; set; }

        private Response LegitRes = new Response(null, null);
        private Response BadPassReg = new Response("The password provided during registration does not comply with the password rules", null);

        private Response NullRes = new Response("One or more of the credentials given to register are null or empty", null);
        private Response InvalidEmail = new Response("The email provided during registration isn't valid", null);

        public UserTests(ServiceFactory serfac)
        {
            SerFac = serfac;
        }




        /// <summary>
        /// This method tests the requirement: The program will allow the registration of new users. Upon successful registration, the registered user is also logged in (no need to login again).
        ///                                 A valid password must be between 6 to 20 characters and must include at least one uppercase letter, one lowercase character, and a number.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestRegister()
        {
            Console.WriteLine("Testing with TestRegister:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.



            if (!SerFac.Us.Register("n1@gmail.com", "Pass123").Equals(LegitRes.GetSerializedVersion()))   //  TEST Legit input
            {
                res += "Test FAILED - Test 1" + "\n";
            }

            if (!SerFac.Us.Register("n2@gmail.com", "Pass").Equals(BadPassReg.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            if (!SerFac.Us.Register("n3@gmail.com", "pass123").Equals(BadPassReg.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 3" + "\n";
            }

            if (!SerFac.Us.Register("n4@gmail.com", "password").Equals(BadPassReg.GetSerializedVersion()))   //  TEST Illegitimate password
            {
                res += "Test FAILED - Test 4" + "\n";
            }






            if (!SerFac.Us.Register("", "Pass123").Equals(NullRes.GetSerializedVersion()))   //  TEST empty
            {
                res += "Test FAILED - Test 5" + "\n";
            }

            if (!SerFac.Us.Register("nnnn@gmail.com", "").Equals(NullRes.GetSerializedVersion()))   //  TEST empty
            {
                res += "Test FAILED - Test 6" + "\n";
            }

            if (!SerFac.Us.Register("", "").Equals(NullRes.GetSerializedVersion()))   //  TEST empty
            {
                res += "Test FAILED - Test 7" + "\n";
            }






            if (!SerFac.Us.Register(null, "Pass123").Equals(NullRes.GetSerializedVersion()))   //  TEST Null
            {
                res += "Test FAILED - Test 8" + "\n";
            }

            if (!SerFac.Us.Register("nnnn@gmail.com", null).Equals(NullRes.GetSerializedVersion()))   //  TEST Null
            {
                res += "Test FAILED - Test 9" + "\n";
            }

            if (!SerFac.Us.Register(null, null).Equals(NullRes.GetSerializedVersion()))   //  TEST Null
            {
                res += "Test FAILED - Test 10" + "\n";
            }


            //////////////////////////

            if (!SerFac.Us.Register("nnn@fd@gmail.com", "Pass123").Equals(InvalidEmail.GetSerializedVersion()))   //  TEST invalid email
            {
                res += "Test FAILED - Test 11" + "\n";
            }
            if (!SerFac.Us.Register("TRds184@.walla.com", "Pass123").Equals(InvalidEmail.GetSerializedVersion()))   //  TEST invalid email
            {
                res += "Test FAILED - Test 12" + "\n";
            }
            if (!SerFac.Us.Register("@sdad.com", "Pass123").Equals(InvalidEmail.GetSerializedVersion()))   //  TEST invalid email
            {
                res += "Test FAILED - Test 13" + "\n";
            }
            if (!SerFac.Us.Register("dwad@outlook.co.il", "Pass123").Equals(LegitRes.GetSerializedVersion()))   //  TEST invalid email
            {
                res += "Test FAILED - Test 14" + "\n";
            }



            if (!SerFac.Us.Register("n10@gmail.com", "pass123").Equals(BadPassReg.GetSerializedVersion()))   //  TEST invalid password
            {
                res += "Test FAILED - Test 11" + "\n";
            }
            if (!SerFac.Us.Register("n11@gmail.com", "Pass123456789123456789").Equals(BadPassReg.GetSerializedVersion()))   //  TEST invalid password
            {
                res += "Test FAILED - Test 12" + "\n";
            }
            if (!SerFac.Us.Register("n12@gmail.com", "Password").Equals(BadPassReg.GetSerializedVersion()))   //  TEST invalid password
            {
                res += "Test FAILED - Test 13" + "\n";
            }
            if (!SerFac.Us.Register("n13@gmail.com", "123456789").Equals(BadPassReg.GetSerializedVersion()))   //  TEST invalid password
            {
                res += "Test FAILED - Test 14" + "\n";
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
        /// This method tests the requirement: Users will be able to login and logout using their credentials.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestLogin()
        {
            Console.WriteLine("Testing with TestLogin:");
            string res = "There are 8 tests. If a test passes it says so. If it isn't mentioned then it failed:\n";   

            SerFac.Us.Register("stav@gmail.com", "Pass123"); // Create the user that will be used for the tests

            SerFac.Us.Logout("stav@gmail.com");

            //  TEST 1 for logging in a valid user
            if (SerFac.Us.Login("stav@gmail.com", "Pass123").Equals(new Response(null, "stav@gmail.com").GetSerializedVersion()))
            {
                res += "Test passed - Test 1" + "\n";
            }

            // TEST 2 for logging in a user that is already logged in
            if (SerFac.Us.Login("stav@gmail.com", "Pass123").Equals(new Response("User is already logged in", null).GetSerializedVersion()))
            {
                res += "Test passed - Test 2" + "\n";
            }

            // TEST 3 for checking null email input 
            if (SerFac.Us.Login("", "Pass123").Equals(new Response("One or more of the credentials given is null or empty", null).GetSerializedVersion()))
            {
                res += "Test passed - Test 3" + "\n";
            }

            // TEST 4 for checking null password input 
            if (SerFac.Us.Login("stav@gmail.com", "").Equals(new Response("One or more of the credentials given is null or empty", null).GetSerializedVersion()))
            {
                res += "Test passed - Test 4" + "\n";
            }

            SerFac.Us.Logout("stav@gmail.com");
            // TEST 5 for checking wrong password input 
            if (SerFac.Us.Login("stav@gmail.com", "Hello").Equals(new Response("Incorrect Credentials", null).GetSerializedVersion()))
            {
                res += "Test passed - Test 5" + "\n";
            }

            // TEST 6 for checking unrecognized email
            if (SerFac.Us.Login("stav2@gmail.com", "Pass123").Equals(new Response("email is unrecognized", null).GetSerializedVersion()))
            {
                res += "Test passed - Test 6" + "\n";
            }

            // TEST 7 for null email
            if (SerFac.Us.Login(null, "pass").Equals(new Response("One or more of the credentials given is null or empty", null).GetSerializedVersion()))
            {
                res += "Test passed - Test 7" + "\n";
            }

            // TEST 8 for null password
            if (SerFac.Us.Login("stav@gmail.com", null).Equals(new Response("One or more of the credentials given is null or empty", null).GetSerializedVersion()))
            {
                res += "Test passed - Test 8" + "\n";
            }

            SerFac.Bs.DeleteData();  // reseting for the next test
            SerFac.Us.DeleteData();  // reseting for the next test

            return res;
        }










        /// <summary>
        /// This method tests the requirement: Users will be able to login and logout using their credentials.
        /// </summary>
        /// <returns>If the test is successful it will return "Test PASSED" otherwise it will return "Test FAILED"</returns>
        internal string TestLogout()
        {
            Console.WriteLine("Testing with TestLogout:");

            string res = "";   // to be changed later if a test fails along the upcoming tests in this method.

            SerFac.Us.Register("taeery@walla.co.il", "Pass12345");
            SerFac.Us.Register("stav@zahav.net.il", "Pass12345");
            SerFac.Us.Register("taeerc@post.bgu.ac.il", "Pass12345");
            SerFac.Us.Register("taeeroosh@gmail.com", "Pass12345");
            SerFac.Us.Register("naimA@gmx.com", "Pass12345");

            SerFac.Us.Logout("taeery@walla.co.il");
            SerFac.Us.Logout("taeeroosh@gmail.com");

            Response testEmail = new Response("email doesn't exist in the system", null);
            Response testLogin = new Response("user isn't logged in to system", null);
            Response succes = new Response(null, null);
            Response testNull = new Response("given null argument", null);

            if (!SerFac.Us.Logout("taeery@walla.co.il").Equals(testLogin.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }
            if (!SerFac.Us.Logout("").Equals(testNull.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }
            if (!SerFac.Us.Logout(null).Equals(testNull.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 3" + "\n";
            }
            if (!SerFac.Us.Logout("naim@gmx.com").Equals(testEmail.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 4" + "\n";
            }
            if (!SerFac.Us.Logout("stav@zahav.net.il").Equals(succes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 5" + "\n";
            }
            if (!SerFac.Us.Logout("taeerc@post.bgu.ac.il").Equals(succes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 6" + "\n";
            }
            if (!SerFac.Us.Logout("taeeroosh@gmail.com").Equals(testLogin.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 7" + "\n";
            }
            if (!SerFac.Us.Logout("mich@walla.com").Equals(testEmail.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 8" + "\n";
            }
            if (!SerFac.Us.Logout("naimA@gmx.com").Equals(succes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 9" + "\n";
            }

            if (res == "")
            {
                res = "All Tests PASSED";
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



            if (!SerFac.Us.LoadData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }
            if (!SerFac.Us.LoadData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            SerFac.Us.DeleteData();
            if (!SerFac.Us.LoadData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 3" + "\n";
            }
            if (!SerFac.Us.LoadData().Equals(LegitRes.GetSerializedVersion()))
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



            if (!SerFac.Us.DeleteData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 1" + "\n";
            }
            if (!SerFac.Us.DeleteData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 2" + "\n";
            }

            SerFac.Us.LoadData();
            if (!SerFac.Us.DeleteData().Equals(LegitRes.GetSerializedVersion()))
            {
                res += "Test FAILED - Test 3" + "\n";
            }
            if (!SerFac.Us.DeleteData().Equals(LegitRes.GetSerializedVersion()))
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
        Console.WriteLine(TestRegister());

        Console.WriteLine(TestLogin());

        Console.WriteLine(TestLogout());

        Console.WriteLine(TestLoadData());

        Console.WriteLine(TestDeleteData());
        }

}
}

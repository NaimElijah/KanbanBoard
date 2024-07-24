using BackendTests.BoardTests;
using IntroSE.Kanban.Backend.ServiceLayer.Services;

namespace BackendTests;

public class TestsMain
{
    internal UserTests.UserTests UT { get; set; }
    internal BoardTests.BoardTests BT { get; set; } 
    internal TasksTests TT { get; set; }
    internal ColumnTests CT { get; set; }

    public TestsMain()
    {
        ServiceFactory SF = new ServiceFactory();
        UT = new UserTests.UserTests(SF);
        BT = new BoardTests.BoardTests(SF);
        TT = new TasksTests(SF);
        CT = new ColumnTests(SF);
    }

    public static void Main(string[] args)
    {
        TestsMain Tests = new TestsMain();
        Console.WriteLine();

        Tests.UT.TestAll();
        Console.WriteLine();
        
        Tests.BT.TestAll();
        Console.WriteLine();

        Tests.TT.TestAll();
        Console.WriteLine();

        Tests.CT.TestAll();
        Console.WriteLine();

    }
}
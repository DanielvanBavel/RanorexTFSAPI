using RanorexTFSAPI.Scripts;

namespace RanorexTFSAPI
{
    public class Program
    {
        static void Main(string[] args)
        {
            TestExecution TestExe = new TestExecution();
            TestExe.RunTestCase();
        }
    }
}
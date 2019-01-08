using RanorexTFSAPI.API;
using System;

namespace RanorexTFSAPI.Scripts
{
    public class TestExecution
    {
        public void RunTestCase()
        {
            TFSApi.GetTestCasesById();

            foreach (var item in TFSApi.ListItems)
            {
                Console.WriteLine("item received" + item.WorkItemId);
            }
        }
    }
}

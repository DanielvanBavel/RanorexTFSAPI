using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RanorexTFSAPI.API
{
    public static class TFSApi
    {
        private static string _ResponseBody { get; set; }

        public static List<ListItem> ListItems;

        static TFSApi()
        {
            ListItems = new List<ListItem>();
        }

        /// <summary>
        /// Recieves the data from GetTestCasesByTestPlanAndTestSuite
		/// Deserialize Json and to a .Net Object (TestCase) and give responsebody as parameter 
		/// Loop through data and get the id value to pass in to the next method
        /// </summary>
        /// <returns></returns>
        public static async Task GetTestCasesById()
        {
            string projectname = ConfigurationManager.AppSettings.Get("projectname");
            string planId = ConfigurationManager.AppSettings.Get("planId");
            string suiteId = ConfigurationManager.AppSettings.Get("suiteId");

            Task<string> json = GetTestCasesByTestPlanAndTestSuite(projectname, planId, suiteId);

            TestCase testcase = JsonConvert.DeserializeObject<TestCase>(_ResponseBody);

            foreach (JObject item in testcase.value)
            {
                var stringId = item["testCase"]["id"].ToString();

                var IntId = int.Parse(stringId);

                await GetWorkItem(IntId);
            }
        }

        /// <summary>
        /// makes an api Call with the given parameters and returns an json string
        /// </summary>
        /// <param name="projectname"></param>
        /// <param name="planId"></param>
        /// <param name="suiteId"></param>
        /// <returns>responseBody as a json string</returns>        
        public static async Task<string> GetTestCasesByTestPlanAndTestSuite(string projectname, string planId, string suiteId)
        {
            try
            {
                string username = ConfigurationManager.AppSettings.Get("username");
                string password = ConfigurationManager.AppSettings.Get("password");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                           Encoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));

                    using (HttpResponseMessage response = client.GetAsync(
                        "https://tfs.delta.nl/tfs/Retail/" + projectname + "/_apis/test/plans/" + planId + "/suites/" + suiteId + "/testcases").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        _ResponseBody = await response.Content.ReadAsStringAsync();

                        return _ResponseBody;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// gets the data through api call of a workitem
        /// </summary>
        /// <param name="id"></param>
        /// <returns>null</returns>        
        public static async Task<List<ListItem>> GetWorkItem(int id)
        {
            try
            {
                string username = ConfigurationManager.AppSettings.Get("username");
                string password = ConfigurationManager.AppSettings.Get("password");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                           Encoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));

                    using (HttpResponseMessage response = client.GetAsync("https://tfs.delta.nl/tfs/Retail/_apis/wit/workItems/" + id + "").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string data = await response.Content.ReadAsStringAsync();

                        WorkItem workitem = JsonConvert.DeserializeObject<WorkItem>(data);

                        foreach (KeyValuePair<string, string> fields in workitem.fields)
                        {
                            if (fields.Key == "System.Tags")
                            {
                                //Console.WriteLine("workitem id: = " + workitem.id + ":\t" + fields.Key + ":\t" + fields.Value);
                                ListItems.Add(new ListItem(workitem.id, fields.Key, fields.Value));
                            }
                        }
                        
                    }                    
                }
                return ListItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return null;
            }
        }
    }
}

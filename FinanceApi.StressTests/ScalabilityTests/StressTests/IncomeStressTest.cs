using NBench;
using System.Text;
using System.Net.Http.Headers;
using FinanceApi.Test.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FinanceApi.Data.Dtos;

namespace FinanceApi.Test.ScalabilityTests
{
    public class IncomeStressTest
    {

        private CustomWebApplicationFactory factory;
        private HttpClient client;
        private Counter counter;
        private StringContent jsonGoalManageDto;

        [PerfSetup]
        public async void Setup(BenchmarkContext context)
        {
            factory = new CustomWebApplicationFactory();
            client = factory.CreateClient();
            counter = context.GetCounter("GetCounter");
            var goalManageDto = new GoalManageDto() { Id = 0, Title = "Title", Description = "Description", Amount = 10000, Currency = "eur", StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2030, 1, 1) };
            var t = new StringContent(JsonConvert.SerializeObject(goalManageDto), Encoding.UTF8, "application/json");
            var authToken = await GetAuthenticationTokenAsync("user1@example.com", "Password!2");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        }

        [PerfBenchmark(NumberOfIterations = 5,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion("GetCounter", MustBe.GreaterThan, 1000)]
        public async void GetIncomesTest()
        {
            client.GetAsync("https://localhost:7190/api/Income/current");
            counter.Increment();
        }

        [PerfBenchmark(NumberOfIterations = 5,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion("GetCounter", MustBe.GreaterThan, 2000)]
        public async void CreateIncomeTest()
        {
            client.PostAsync("https://localhost:7190/api/Income/post", jsonGoalManageDto);
            counter.Increment();
        }



        [PerfCleanup]
        public void Dispose()
        {

        }

        public async Task<string> GetAuthenticationTokenAsync(string email, string password)
        {
            var requestBody = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    email,
                    password
                }),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/login", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<object>(await response.Content.ReadAsStringAsync());
                string accesToken = JObject.FromObject(data)["accessToken"]?.ToString();
                return accesToken;
            }


            return null;
        }
    }
}

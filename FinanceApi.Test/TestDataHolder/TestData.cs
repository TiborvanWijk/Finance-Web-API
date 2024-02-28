using FinanceApi.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApi.Test.TestDataHolder
{
    public abstract class TestData
    {
        public static IEnumerable<object[]> CreateGoalValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 1, Title = "Test-Title-1", Description = "Description-1", Amount = 2000, Currency = "eur", StartDate = DateTime.Now, EndDate = new DateTime(2026, 1, 1) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 2, Title = "Test-Title-1", Description = "Description for a valid goal with a different currency", Amount = 1500, Currency = "usd", StartDate = DateTime.Now.AddDays(5), EndDate = new DateTime(2026, 1, 1) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 3, Title = "Test-Title-1", Description = "Description for a valid goal with a longer time period", Amount = 3000, Currency = "eur", StartDate = DateTime.Now.AddDays(10), EndDate = new DateTime(2026, 2, 15) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 4, Title = "Test-Title-1", Description = "Description for a valid goal with a larger amount", Amount = 5000, Currency = "php", StartDate = DateTime.Now.AddDays(3), EndDate = new DateTime(2026, 1, 1) }, null };
        }
        public static IEnumerable<object[]> GetGoalValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new DateTime(2020, 3, 15), new DateTime(2022, 7, 10), "progress", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2021, 6, 5), new DateTime(2023, 4, 25), "amount", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2019, 2, 10), new DateTime(2024, 11, 30), "title", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2020, 9, 20), new DateTime(2023, 1, 5), null, null, null };

            yield return new object[] { "user2@example.com", new DateTime(2020, 8, 12), new DateTime(2023, 6, 18), "progress", "desc", null };
            yield return new object[] { "user2@example.com", new DateTime(2022, 4, 30), new DateTime(2023, 2, 8), null, null, null };
            yield return new object[] { "user2@example.com", new DateTime(2019, 11, 8), new DateTime(2024, 9, 22), "amount", "desc", null };
            yield return new object[] { "user2@example.com", new DateTime(2021, 7, 3), new DateTime(2023, 4, 2), "title", "desc", null };

            yield return new object[] { "user3@example.com", new DateTime(2020, 5, 22), new DateTime(2023, 10, 15), null, null, null };
            yield return new object[] { "user3@example.com", new DateTime(2022, 1, 17), new DateTime(2024, 8, 7), "progress", "desc", null };
            yield return new object[] { "user3@example.com", new DateTime(2019, 10, 3), new DateTime(2024, 3, 12), "amount", "desc", null };
            yield return new object[] { "user3@example.com", new DateTime(2020, 6, 28), new DateTime(2023, 12, 5), "title", "desc", null };

            yield return new object[] { "user4@example.com", new DateTime(2021, 12, 7), new DateTime(2023, 5, 28), "progress", "desc", null };
            yield return new object[] { "user4@example.com", new DateTime(2019, 2, 25), new DateTime(2024, 1, 20), "amount", "desc", null };
            yield return new object[] { "user4@example.com", new DateTime(2020, 7, 14), new DateTime(2023, 9, 10), "title", "desc", null };
            yield return new object[] { "user4@example.com", new DateTime(2022, 4, 18), new DateTime(2024, 3, 30), null, null, null };

            yield return new object[] { "user1@example.com", null, null, "progress", "desc", null };
            yield return new object[] { "user1@example.com", null, null, "amount", "desc", null };
            yield return new object[] { "user1@example.com", null, null, "title", "desc", null };
            yield return new object[] { "user1@example.com", null, null, null, null, null };

            yield return new object[] { "user2@example.com", null, null, "progress", "desc", null };
            yield return new object[] { "user2@example.com", null, null, null, null, null };
            yield return new object[] { "user2@example.com", null, null, "amount", "desc", null };
            yield return new object[] { "user2@example.com", null, null, "title", "desc", null };

            yield return new object[] { "user3@example.com", null, null, null, null, null };
            yield return new object[] { "user3@example.com", null, null, "progress", "desc", null };
            yield return new object[] { "user3@example.com", null, null, "amount", "desc", null };
            yield return new object[] { "user3@example.com", null, null, "title", "desc", null };

            yield return new object[] { "user4@example.com", null, null, "progress", "desc", null };
            yield return new object[] { "user4@example.com", null, null, "amount", "desc", null };
            yield return new object[] { "user4@example.com", null, null, "title", "desc", null };
            yield return new object[] { "user4@example.com", null, null, null, null, null };

            yield return new object[] { "user2@example.com", null, null, null, null, "user1@example.com" };
            yield return new object[] { "user3@example.com", null, null, null, null, "user1@example.com" };

        }
        public static IEnumerable<object[]> CreateGoalInvalidtestTestData()
        {
            // Invalid amount (less than or equal to 0)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 0, Currency = "USD", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid currency ISO code
            yield return new object[] { "user2@example.com", new GoalManageDto { Amount = 1000, Currency = "InvalidCurrency", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid time period (e.g., end date earlier than start date)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 2000, Currency = "EUR", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-7) }, null };


            // Combining multiple invalid conditions
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 0, Currency = "InvalidCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };

            // Invalid amount (zero or negative)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 0, Currency = "USD", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = -500, Currency = "EUR", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid currency ISO code
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 1000, Currency = "", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 1500, Currency = "InvalidCurrencyCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) }, null };

            // Invalid time period (end date earlier than start date)
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 2000, Currency = "GBP", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-7) }, null };

            // Combining multiple invalid conditions
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 0, Currency = "InvalidCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = -500, Currency = "", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 1000, Currency = "InvalidCurrency", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 2000, Currency = "USD", StartDate = DateTime.Now.AddDays(15), EndDate = DateTime.Now }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto { Amount = 3000, Currency = "GBP", StartDate = DateTime.Now.AddDays(15), EndDate = DateTime.Now.AddDays(10) }, null };
        }
        public static IEnumerable<object[]> DeleteGoalValidTestData()
        {
            yield return new object[] { "user1@example.com", 1, null };
            yield return new object[] { "user1@example.com", 2, null };
            yield return new object[] { "user1@example.com", 3, null };
            yield return new object[] { "user3@example.com", 4, "user1@example.com" };
            yield return new object[] { "user3@example.com", 5, "user1@example.com" };
            yield return new object[] { "user2@example.com", 6, null };
            yield return new object[] { "user2@example.com", 7, null };
            yield return new object[] { "user2@example.com", 8, null };
            yield return new object[] { "user2@example.com", 9, null };
            yield return new object[] { "user2@example.com", 10, null };
        }
        public static IEnumerable<object[]> DeleteGoalNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", 6, null };
            yield return new object[] { "user1@example.com", 7, null };
            yield return new object[] { "user1@example.com", 8, null };
            yield return new object[] { "user1@example.com", 9, null };
            yield return new object[] { "user1@example.com", 10, null };
        }

        public static IEnumerable<object[]> GetUsersExpensesValidInputsTestData()
        {
            yield return new object[] { new DateTime(2020, 1, 1), new DateTime(2024, 1, 1), "amount", null, "user123" };
            yield return new object[] { new DateTime(2021, 1, 1), new DateTime(2023, 1, 1), "title", "desc", "user123" };
            yield return new object[] { null, null, null, "desc", null };
            yield return new object[] { null, null, "amount", null, "user123" };
            yield return new object[] { null, null, "title", null, null };
            yield return new object[] { null, null, "urgency", null, null };
            yield return new object[] { null, null, "urgency", "desc", null };
        }
        public static IEnumerable<object[]> GetUsersExpensesInvalidInputsTestData()
        {
            yield return new object[] { new DateTime(2020, 1, 1), null, null, null, null };
            yield return new object[] { null, new DateTime(2020, 1, 1), null, null, null };
        }
        public static IEnumerable<object[]> ExpenseDtoInvalidInputTestData()
        {
            yield return new object[] { new ExpenseDto() { Id = 1, Title = "Title-1", Description = "Description-1", Amount = decimal.MinValue, Currency = "USD", Date = DateTime.Now, DocumentUrl = "URL-1" }, null };
            yield return new object[] { new ExpenseDto() { Id = 2, Title = "Title-2", Description = "Description-2", Amount = decimal.MaxValue, Currency = "USD", Date = DateTime.Now, DocumentUrl = "URL-2" }, null };
            yield return new object[] { new ExpenseDto() { Id = 3, Title = "Title-3", Description = "Description-3", Amount = -1, Currency = "INVALID", Date = DateTime.Now, DocumentUrl = "URL-3" }, null };
            yield return new object[] { new ExpenseDto() { Id = 4, Title = "Title-4", Description = "Description-4", Amount = 9, Currency = "INVALID", Date = DateTime.Now, DocumentUrl = "URL-4" }, null };
        }
        public static IEnumerable<object[]> AddCategoryToExpenseValidInputTestData()
        {
            yield return new object[] { 1,
                new List<int>(){
                    1,2,3,4,5,6,7,8,9,10
                },
                null
            };
        }
        public static IEnumerable<object[]> ExpenseDtoValidInputsTestData()
        {
            yield return new object[] {
                new ExpenseDto() {
                    Id = 1, Title = "Title1", Description = "Desctiption1", Amount = 18, Currency = "EUR", Date = DateTime.Now, DocumentUrl = "URL"
                }, null
                };
        }

        public static IEnumerable<object[]> GetUsersIncomeValidInputsTestData()
        {
            yield return new object[] { new DateTime(2020, 1, 1), new DateTime(2024, 1, 1), "amount", null, "user123" };
            yield return new object[] { new DateTime(2021, 1, 1), new DateTime(2023, 1, 1), "title", "desc", "user123" };
            yield return new object[] { null, null, null, "desc", null };
            yield return new object[] { null, null, "amount", null, "user123" };
            yield return new object[] { null, null, "title", null, null };
        }
        public static IEnumerable<object[]> GetUsersIncomeInvalidInputsTestData()
        {
            yield return new object[] { new DateTime(2020, 1, 1), null, null, null, null };
            yield return new object[] { null, new DateTime(2020, 1, 1), null, null, null };
        }
        public static IEnumerable<object[]> IncomeDtoInvalidInputTestData()
        {
            yield return new object[] { new IncomeDto() { Id = 1, Title = "Title-1", Description = "Description-1", Amount = decimal.MinValue, Currency = "USD", Date = DateTime.Now, DocumentUrl = "URL-1" }, null };
            yield return new object[] { new IncomeDto() { Id = 2, Title = "Title-2", Description = "Description-2", Amount = decimal.MaxValue, Currency = "USD", Date = DateTime.Now, DocumentUrl = "URL-2" }, null };
            yield return new object[] { new IncomeDto() { Id = 3, Title = "Title-3", Description = "Description-3", Amount = -1, Currency = "INVALID", Date = DateTime.Now, DocumentUrl = "URL-3" }, null };
            yield return new object[] { new IncomeDto() { Id = 4, Title = "Title-4", Description = "Description-4", Amount = 9, Currency = "INVALID", Date = DateTime.Now, DocumentUrl = "URL-4" }, null };
        }
        public static IEnumerable<object[]> AddCategoryToIncomeValidInputTestData()
        {
            yield return new object[] { 1,
                new List<int>(){
                    1,2,3,4,5,6,7,8,9,10
                },
                null
            };
        }
        public static IEnumerable<object[]> IncomeDtoValidInputsTestData()
        {
            yield return new object[] {
                new IncomeDto() {
                    Id = 1, Title = "Title1", Description = "Desctiption1", Amount = 18, Currency = "EUR", Date = DateTime.Now, DocumentUrl = "URL"
                }, null
                };
        }

        public static IEnumerable<object[]> AddCategoryToGoalValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, new List<int>() { 2, 3, 4 }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { 3, 4, 1 }, null };
            yield return new object[] { "user1@example.com", 3, new List<int>() { 4, 1, 2 }, null };
            yield return new object[] { "user1@example.com", 4, new List<int>() { 1, 2, 3 }, null };
            yield return new object[] { "user1@example.com", 5, new List<int>() { 2, 3, 4 }, null };
            yield return new object[] { "user3@example.com", 1, new List<int>() { 2, 3, 4 }, "user1@example.com" };

        }

        public static IEnumerable<object[]> UpdateGoalValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: hdawudb", Id = 1, Amount = 9100, Currency = "usd", Description = "description: dwj4", StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2030, 2, 13) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: dnwaujdnaw", Id = 2, Amount = 375400, Currency = "eur", Description = "description: diw038", StartDate = new DateTime(2022, 5, 3), EndDate = new DateTime(2025, 8, 17) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: opjkyuypmm", Id = 3, Amount = 13820, Currency = "fjd", Description = "description: hduwe92", StartDate = new DateTime(2019, 12, 21), EndDate = new DateTime(2023, 6, 21) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: jdiwjdw", Id = 4, Amount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11) }, null };
            yield return new object[] { "user3@example.com", new GoalManageDto() { Title = "title: hduwaw", Id = 5, Amount = 101100, Currency = "fkp", Description = "description: djwirb", StartDate = new DateTime(2023, 2, 25), EndDate = new DateTime(2027, 3, 12) }, "user1@example.com" };

        }

        public static IEnumerable<object[]> UpdateGoalBadRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: hdawudb", Id = 1, Amount = 9100, Currency = "usd", Description = "description: dwj4", StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2019, 2, 13) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: dnwaujdnaw", Id = 2, Amount = 375400, Currency = "NOT VALID", Description = "description: diw038", StartDate = new DateTime(2022, 5, 3), EndDate = new DateTime(2025, 8, 17) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: opjkyuypmm", Id = 3, Amount = 0, Currency = "fjd", Description = "description: hduwe92", StartDate = new DateTime(2019, 12, 21), EndDate = new DateTime(2023, 6, 21) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: jdiwjdw", Id = 4, Amount = -100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11) }, null };
            yield return new object[] { "user3@example.com", new GoalManageDto() { Title = "title: hduwaw", Id = 5, Amount = -100, Currency = "NOT VALID", Description = "description: djwirb", StartDate = new DateTime(2030, 2, 25), EndDate = new DateTime(2027, 3, 12) }, "user1@example.com" };

        }

        public static IEnumerable<object[]> UpdateGoalNotFoundRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: hdawudb", Id = 20, Amount = 9100, Currency = "usd", Description = "description: dwj4", StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2030, 2, 13) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: dnwaujdnaw", Id = 22, Amount = 375400, Currency = "eur", Description = "description: diw038", StartDate = new DateTime(2022, 5, 3), EndDate = new DateTime(2025, 8, 17) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: opjkyuypmm", Id = 23, Amount = 13820, Currency = "fjd", Description = "description: hduwe92", StartDate = new DateTime(2019, 12, 21), EndDate = new DateTime(2023, 6, 21) }, null };
            yield return new object[] { "user1@example.com", new GoalManageDto() { Title = "title: jdiwjdw", Id = 24, Amount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11) }, null };
            yield return new object[] { "user2@example.com", new GoalManageDto() { Title = "title: jdiwjdw", Id = 94, Amount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11) }, null };
            yield return new object[] { "user2@example.com", new GoalManageDto() { Title = "title: jdiwjdw", Id = 124, Amount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11) }, null };
            yield return new object[] { "user2@example.com", new GoalManageDto() { Title = "title: jdiwjdw", Id = 345, Amount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11) }, null };
            yield return new object[] { "user3@example.com", new GoalManageDto() { Title = "title: hduwaw", Id = -1, Amount = 101100, Currency = "fkp", Description = "description: djwirb", StartDate = new DateTime(2023, 2, 25), EndDate = new DateTime(2027, 3, 12) }, null };
            yield return new object[] { "user3@example.com", new GoalManageDto() { Title = "title: hduwaw", Id = 2134, Amount = 101100, Currency = "fkp", Description = "description: djwirb", StartDate = new DateTime(2023, 2, 25), EndDate = new DateTime(2027, 3, 12) }, "user1@example.com" };
            yield return new object[] { "user3@example.com", new GoalManageDto() { Title = "title: hduwaw", Id = 456, Amount = 101100, Currency = "fkp", Description = "description: djwirb", StartDate = new DateTime(2023, 2, 25), EndDate = new DateTime(2027, 3, 12) }, "user1@example.com" };

        }

        public static IEnumerable<object[]> AddCategoryToGoalBadRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, new List<int>() { }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { }, null };
            yield return new object[] { "user1@example.com", 3, new List<int>() { }, null };
            yield return new object[] { "user1@example.com", 4, new List<int>() { }, null };
            yield return new object[] { "user1@example.com", 1, new List<int>() { 1 }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { 2 }, null };
            yield return new object[] { "user1@example.com", 3, new List<int>() { 3 }, null };
            yield return new object[] { "user1@example.com", 4, new List<int>() { 4 }, null };
            yield return new object[] { "user1@example.com", 5, new List<int>() { 1 }, null };
        }

        public static IEnumerable<object[]> AddCategoryToGoalNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", 10, new List<int>() { 2 }, null };
            yield return new object[] { "user1@example.com", 13, new List<int>() { 3 }, null };
            yield return new object[] { "user1@example.com", 19, new List<int>() { 4 }, null };
            yield return new object[] { "user1@example.com", 1, new List<int>() { 10 }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { 13 }, null };
            yield return new object[] { "user1@example.com", 3, new List<int>() { 19 }, null };
            yield return new object[] { "user1@example.com", 4, new List<int>() { -1 }, null };
            yield return new object[] { "user1@example.com", -1, new List<int>() { -1 }, null };
        }

        public static IEnumerable<object[]> RemoveCategoriesValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, 1, null };
            yield return new object[] { "user1@example.com", 2, 2, null };
            yield return new object[] { "user1@example.com", 3, 3, null };
            yield return new object[] { "user1@example.com", 4, 4, null };
            yield return new object[] { "user3@example.com", 5, 1, "user1@example.com" };
        }

        public static IEnumerable<object[]> RemoveCategoriesNotFoundInputsTestData()
        {
            yield return new object[] { "user1@example.com", 11, 2, null };
            yield return new object[] { "user1@example.com", 1, 19, null };
            yield return new object[] { "user1@example.com", 13, 132, null };
            yield return new object[] { "user1@example.com", 3, 9, null };
            yield return new object[] { "user3@example.com", 11, 11, "user1@example.com" };
        }

        public static IEnumerable<object[]> RemoveCategoriesBadRequestInputsTestData()
        {
            yield return new object[] { "user1@example.com", 1, 2, null };
            yield return new object[] { "user1@example.com", 2, 3, null};
            yield return new object[] { "user1@example.com", 3, 4, null};
            yield return new object[] { "user1@example.com", 4, 1, null};
            yield return new object[] { "user3@example.com", 5, 2, "user1@example.com" };
        }

        public static IEnumerable<object[]> GetIncomesValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new DateTime(2022, 1, 1), new DateTime(2024, 1, 1), "title", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2019, 1, 1), new DateTime(2023, 1, 1), "amount", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2017, 1, 1), new DateTime(2022, 1, 1), "title", null, null };
            yield return new object[] { "user1@example.com", new DateTime(2023, 1, 1), new DateTime(2025, 1, 1), "amount", null, null };
            yield return new object[] { "user1@example.com", new DateTime(2020, 1, 1), new DateTime(2022, 1, 1), null, "desc", null };
            yield return new object[] { "user2@example.com", new DateTime(2021, 1, 1), new DateTime(2023, 1, 1), null, null, "user1@example.com" };
            yield return new object[] { "user3@example.com", new DateTime(2021, 1, 1), new DateTime(2023, 1, 1), null, null, "user1@example.com" };
        }

        public static IEnumerable<object[]> CreateIncomeValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "eur", Amount = 23, Date = DateTime.Now, DocumentUrl = "receipt" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "php", Amount = 2353, Date = DateTime.Now, DocumentUrl = "receipt" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "usd", Amount = 24, Date = DateTime.Now.AddDays(-8), DocumentUrl = "receipt" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "cyp", Amount = 45, Date = DateTime.Now.AddDays(-6), DocumentUrl = "receipt" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "chk", Amount = 92, Date = DateTime.Now.AddDays(-2), DocumentUrl = "receipt" }, null };
            yield return new object[] { "user3@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "jpg", Amount = 167, Date = DateTime.Now.AddMonths(-1), DocumentUrl = "receipt" }, "user1@example.com" };
        }
    }
}

using FinanceApi.Data.Dtos;
using FinanceApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApi.Test.TestDataHolder
{
    public abstract class TestData
    {

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
        public static IEnumerable<object[]> AddCategoryToExpenseValidInputTestDataUnitTest()
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
        public static IEnumerable<object[]> AddCategoryToIncomeValidInputTestDataUnitTest()
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

        public static IEnumerable<object[]> RemoveCategoriesFromGoalValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, 1, null };
            yield return new object[] { "user1@example.com", 2, 2, null };
            yield return new object[] { "user1@example.com", 3, 3, null };
            yield return new object[] { "user1@example.com", 4, 4, null };
            yield return new object[] { "user3@example.com", 5, 1, "user1@example.com" };
        }

        public static IEnumerable<object[]> RemoveCategoriesFromGoalNotFoundInputsTestData()
        {
            yield return new object[] { "user1@example.com", 11, 2, null };
            yield return new object[] { "user1@example.com", 1, 19, null };
            yield return new object[] { "user1@example.com", 13, 132, null };
            yield return new object[] { "user1@example.com", 3, 9, null };
            yield return new object[] { "user3@example.com", 11, 11, "user1@example.com" };
        }

        public static IEnumerable<object[]> RemoveCategoriesFromGoalBadRequestInputsTestData()
        {
            yield return new object[] { "user1@example.com", 1, 2, null };
            yield return new object[] { "user1@example.com", 2, 3, null };
            yield return new object[] { "user1@example.com", 3, 4, null };
            yield return new object[] { "user1@example.com", 4, 1, null };
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
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "fjd", Amount = 45, Date = DateTime.Now.AddDays(-6), DocumentUrl = "receipt" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "gbp", Amount = 92, Date = DateTime.Now.AddDays(-2), DocumentUrl = "receipt" }, null };
            yield return new object[] { "user3@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "fkp", Amount = 167, Date = DateTime.Now.AddMonths(-1), DocumentUrl = "receipt" }, "user1@example.com" };
        }

        public static IEnumerable<object[]> CreateIncomeBadRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "work", Description = "40 week", Currency = "THIS IS WRONG", Amount = 23, Date = DateTime.Now, DocumentUrl = "receipt" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "work", Description = "40 week", Currency = "usd", Amount = -100, Date = DateTime.Now, DocumentUrl = "receipt" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "work", Description = "40 week", Currency = "usd", Amount = 8932785975270953757, Date = DateTime.Now, DocumentUrl = "receipt" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "work", Description = "40 week", Currency = "THIS IS WRONG", Amount = -100, Date = DateTime.Now, DocumentUrl = "receipt" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "work", Description = "40 week", Currency = "THIS IS WRONG", Amount = 8932785975270953757, Date = DateTime.Now, DocumentUrl = "receipt" }, null };
        }

        public static IEnumerable<object[]> UpdateIncomeValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 1, Title = "new Title", Description = "new Description", Currency = "php", Amount = 999999, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 3, Title = "Random title-dwddwwa214", Description = "new Description", Currency = "all", Amount = 123, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 4, Title = "A cool heading", Description = "new Description", Currency = "dzd", Amount = 438, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 6, Title = "TITLE/TITLE", Description = "new Description", Currency = "aoa", Amount = 12, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 8, Title = "FREELANCE", Description = "new Description", Currency = "xcd", Amount = 5953, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user3@example.com", new IncomeDto() { Id = 9, Title = "COMPANY-123", Description = "new Description", Currency = "ars", Amount = 198, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, "user1@example.com" };
            yield return new object[] { "user3@example.com", new IncomeDto() { Id = 13, Title = "BET nr-1", Description = "new Description", Currency = "amd", Amount = 32, Date = DateTime.Now.AddDays(1), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, "user1@example.com" };
            yield return new object[] { "user3@example.com", new IncomeDto() { Id = 17, Title = "casino", Description = "new Description", Currency = "azn", Amount = 123, Date = DateTime.Now.AddDays(-2), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new IncomeDto() { Id = 62, Title = "new Title", Description = "new Description", Currency = "bbd", Amount = 5638, Date = DateTime.Now.AddDays(2), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user4@example.com", new IncomeDto() { Id = 78, Title = "new Title", Description = "new Description", Currency = "bzd", Amount = 783, Date = DateTime.Now.AddDays(1), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
        }

        public static IEnumerable<object[]> UpdateIncomeBadrequestInputTestData()
        {
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 1, Title = null, Description = "new Description", Currency = "WRONG", Amount = 999999, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 3, Title = "Random title-dwddwwa214", Description = "new Description", Currency = "all", Amount = -1000, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 4, Title = "A cool heading", Description = "new Description", Currency = "dzd", Amount = 99999999999999999, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user3@example.com", new IncomeDto() { Id = 13, Title = "BET nr-1", Description = "new Description", Currency = "amd", Amount = -1000, Date = DateTime.Now.AddDays(1), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, "user1@example.com" };
            yield return new object[] { "user3@example.com", new IncomeDto() { Id = 17, Title = "casino", Description = "new Description", Currency = "WRONG", Amount = 000000000000000000, Date = DateTime.Now.AddDays(-2), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new IncomeDto() { Id = 62, Title = "new Title", Description = "new Description", Currency = "WRONG", Amount = 100, Date = DateTime.Now.AddDays(2), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user4@example.com", new IncomeDto() { Id = 78, Title = "new Title", Description = "new Description", Currency = "WRONG", Amount = 783, Date = DateTime.Now.AddDays(1), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
        }

        public static IEnumerable<object[]> UpdateIncomeNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 69, Title = "TITLE/TITLE", Description = "new Description", Currency = "aoa", Amount = 12, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 83, Title = "FREELANCE", Description = "new Description", Currency = "xcd", Amount = 5953, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user3@example.com", new IncomeDto() { Id = 92, Title = "COMPANY-123", Description = "new Description", Currency = "ars", Amount = 198, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user3@example.com", new IncomeDto() { Id = 132, Title = "BET nr-1", Description = "new Description", Currency = "amd", Amount = 32, Date = DateTime.Now.AddDays(1), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user4@example.com", new IncomeDto() { Id = 622, Title = "new Title", Description = "new Description", Currency = "bbd", Amount = 5638, Date = DateTime.Now.AddDays(2), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };
            yield return new object[] { "user4@example.com", new IncomeDto() { Id = 782, Title = "new Title", Description = "new Description", Currency = "bzd", Amount = 783, Date = DateTime.Now.AddDays(1), DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, null };

        }

        public static IEnumerable<object[]> DeleteIncomeValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, null };
            yield return new object[] { "user1@example.com", 3, null };
            yield return new object[] { "user1@example.com", 4, null };
            yield return new object[] { "user1@example.com", 6, null };
            yield return new object[] { "user1@example.com", 8, null };
            yield return new object[] { "user2@example.com", 21, null };
            yield return new object[] { "user2@example.com", 27, null };
            yield return new object[] { "user2@example.com", 32, null };
            yield return new object[] { "user3@example.com", 9, "user1@example.com" };
            yield return new object[] { "user3@example.com", 13, "user1@example.com" };
            yield return new object[] { "user3@example.com", 17, "user1@example.com" };
            yield return new object[] { "user4@example.com", 62, null };
            yield return new object[] { "user4@example.com", 78, null };

        }

        public static IEnumerable<object[]> DeleteIncomeNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", 69, null };
            yield return new object[] { "user1@example.com", 83, null };
            yield return new object[] { "user2@example.com", 93, null };
            yield return new object[] { "user2@example.com", 102, null };
            yield return new object[] { "user3@example.com", 111, null };
            yield return new object[] { "user3@example.com", 132, null };
            yield return new object[] { "user4@example.com", 622, null };
            yield return new object[] { "user4@example.com", 782, null };

        }

        public static IEnumerable<object[]> AddCategoryToIncomeValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, new List<int>() { 2, 3, 4 }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { 1, 3, 4 }, null };
            yield return new object[] { "user1@example.com", 3, new List<int>() { 2, 4, 1 }, null };
            yield return new object[] { "user1@example.com", 4, new List<int>() { 1, 2, 3 }, null };
        }

        public static IEnumerable<object[]> AddCategoryToIncomeBadRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, new List<int>() { 1 }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { 2 }, null };
            yield return new object[] { "user1@example.com", 3, new List<int>() { 3 }, null };
            yield return new object[] { "user1@example.com", 4, new List<int>() { 4 }, null };
            yield return new object[] { "user1@example.com", 5, new List<int>() { 1 }, null };

        }

        public static IEnumerable<object[]> AddCategoryToIncomeNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", 21, new List<int>() { 2 }, null };
            yield return new object[] { "user1@example.com", 22, new List<int>() { 3 }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { 8 }, null };
            yield return new object[] { "user1@example.com", 3, new List<int>() { 25 }, null };
            yield return new object[] { "user1@example.com", 25, new List<int>() { 25 }, null };
        }

        public static IEnumerable<object[]> RemoveCategoriesFromIncomeValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, 1, null };
            yield return new object[] { "user1@example.com", 2, 2, null };
            yield return new object[] { "user1@example.com", 3, 3, null };
            yield return new object[] { "user1@example.com", 4, 4, null };
            yield return new object[] { "user1@example.com", 5, 1, null };
        }
        public static IEnumerable<object[]> RemoveCategoriesFromIncomeNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", 21, 1, null };
            yield return new object[] { "user1@example.com", 22, 22, null };
            yield return new object[] { "user1@example.com", 1, 8, null };
            yield return new object[] { "user1@example.com", 19, 7, null };
            yield return new object[] { "user1@example.com", 25, 2, null };
        }

        public static IEnumerable<object[]> RemoveCategoriesIncomeNotFoundInputsTestData()
        {
            yield return new object[] { "user1@example.com", 21, 1, null };
            yield return new object[] { "user1@example.com", 22, 2, null };
            yield return new object[] { "user3@example.com", 23, 3, "user1@example.com" };
            yield return new object[] { "user2@example.com", 42, 20, null };
            yield return new object[] { "user2@example.com", 43, 3, null };
            yield return new object[] { "user3@example.com", 62, 3, null };
            yield return new object[] { "user3@example.com", 464, 3, null };
            yield return new object[] { "user4@example.com", 81, 1, null };
            yield return new object[] { "user4@example.com", 123, 56, null };

        }
        // More test data needed (REMOVE THIS COMMAND WHEN DONE) \\
        public static IEnumerable<object[]> RemoveCategoriesFromIncomeBadRequestInputsTestData()
        {
            yield return new object[] { "user1@example.com", 1, 2, null };
            yield return new object[] { "user1@example.com", 2, 3, null };
            yield return new object[] { "user1@example.com", 3, 4, null };
            yield return new object[] { "user1@example.com", 4, 1, null };
            yield return new object[] { "user1@example.com", 5, 2, null };
        }

        public static IEnumerable<object[]> GetExpenseValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new DateTime(2022, 4, 8), new DateTime(2025, 5, 8), "title", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2023, 3, 8), new DateTime(2024, 1, 1), "amount", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2019, 2, 8), new DateTime(2020, 1, 1), "urgency", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2020, 6, 9), new DateTime(2023, 1, 1), "title", null, null };
            yield return new object[] { "user1@example.com", new DateTime(2021, 6, 2), new DateTime(2024, 1, 1), "amount", null, null };
            yield return new object[] { "user3@example.com", new DateTime(2022, 4, 5), new DateTime(2023, 6, 8), "urgency", null, "user1@example.com" };
            yield return new object[] { "user2@example.com", new DateTime(2022, 4, 8), new DateTime(2025, 5, 8), "title", "desc", null };
            yield return new object[] { "user2@example.com", new DateTime(2023, 3, 8), new DateTime(2024, 1, 1), "amount", "desc", null };
            yield return new object[] { "user3@example.com", new DateTime(2019, 2, 8), new DateTime(2020, 1, 1), "urgency", "desc", null };
            yield return new object[] { "user3@example.com", new DateTime(2020, 6, 9), new DateTime(2023, 1, 1), "title", null, null };
            yield return new object[] { "user4@example.com", new DateTime(2021, 6, 2), new DateTime(2024, 1, 1), "amount", null, null };
        }

        public static IEnumerable<object[]> CreateExpenseValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "Food", Description = "Ate at a restaurant", Amount = 54, Currency = "eur", Date = DateTime.Now, DocumentUrl = "www.restaurant.com/receipt", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "shopping", Description = "shoes", Amount = 460, Currency = "php", Date = DateTime.Now.AddDays(3), DocumentUrl = "shoes", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "insurance", Description = "insurance", Amount = 395, Currency = "usd", Date = DateTime.Now.AddDays(2), DocumentUrl = "receipt", Urgency = Urgency.High }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "gym", Description = "", Amount = 323, Currency = "eur", Date = DateTime.Now.AddDays(7), DocumentUrl = "receipt", Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "gym", Description = "", Amount = 100000, Currency = "eur", Date = DateTime.Now.AddDays(7), DocumentUrl = "receipt", Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "gym", Description = "", Amount = 1, Currency = "eur", Date = DateTime.Now.AddDays(7), DocumentUrl = "receipt", Urgency = Urgency.Medium }, null };
        }
        // datetime null should return bad request. Does not in current state\\
        public static IEnumerable<object[]> CreateExpenseBadRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "Food", Description = "Ate at a restaurant", Amount = -1, Currency = "WRONG", Date = DateTime.Now, DocumentUrl = "www.restaurant.com/receipt", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "shopping", Description = "shoes", Amount = 460, Currency = "WRONG", Date = DateTime.Now.AddDays(3), DocumentUrl = "shoes", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "insurance", Description = "insurance", Amount = 10000000000000000000, Currency = "usd", Date = DateTime.Now.AddDays(2), DocumentUrl = "receipt", Urgency = Urgency.High }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "gym", Description = "", Amount = 10000000000000000000, Currency = "WRONG", Date = DateTime.Now.AddDays(7), DocumentUrl = "receipt", Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "gym", Description = "", Amount = 1, Date = DateTime.Now.AddDays(7), DocumentUrl = "receipt" }, null };
        }

        public static IEnumerable<object[]> UpdateExpenseValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 1, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 32, Currency = "eur", Date = DateTime.Now.AddDays(-100), DocumentUrl = "new receipt", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 2, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 290, Currency = "php", Date = DateTime.Now.AddDays(-165), DocumentUrl = "new receipt", Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 3, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 213, Currency = "eur", Date = DateTime.Now.AddDays(-45), DocumentUrl = "new receipt", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 4, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 376, Currency = "php", Date = DateTime.Now.AddDays(234), DocumentUrl = "new receipt", Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 5, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 9023, Currency = "usd", Date = DateTime.Now.AddDays(442), DocumentUrl = "new receipt", Urgency = Urgency.High }, null };
        }
        // datetime null should return bad request. Does not in current state\\
        public static IEnumerable<object[]> UpdateExpenseBadrequestInputTestData()
        {
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 1, Title = "Food", Description = "Ate at a restaurant", Amount = -1, Currency = "WRONG", Date = DateTime.Now, DocumentUrl = "www.restaurant.com/receipt", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 2, Title = "shopping", Description = "shoes", Amount = 460, Currency = "WRONG", Date = DateTime.Now.AddDays(3), DocumentUrl = "shoes", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 3, Title = "insurance", Description = "insurance", Amount = 10000000000000000000, Currency = "usd", Date = DateTime.Now.AddDays(2), DocumentUrl = "receipt", Urgency = Urgency.High }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 4, Title = "gym", Description = "", Amount = 10000000000000000000, Currency = "WRONG", Date = DateTime.Now.AddDays(7), DocumentUrl = "receipt", Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 6, Title = "gym", Description = "", Amount = 1, Date = DateTime.Now.AddDays(7), DocumentUrl = "receipt" }, null };
        }

        public static IEnumerable<object[]> UpdateExpenseNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 21, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 32, Currency = "eur", Date = DateTime.Now.AddDays(-100), DocumentUrl = "new receipt", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 62, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 290, Currency = "php", Date = DateTime.Now.AddDays(-165), DocumentUrl = "new receipt", Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 83, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 213, Currency = "eur", Date = DateTime.Now.AddDays(-45), DocumentUrl = "new receipt", Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 34, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 376, Currency = "php", Date = DateTime.Now.AddDays(234), DocumentUrl = "new receipt", Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 45, Title = "new title", Description = "The desciption-2223355aa-23bcis45bsBD", Amount = 9023, Currency = "usd", Date = DateTime.Now.AddDays(442), DocumentUrl = "new receipt", Urgency = Urgency.High }, null };
        }

        public static IEnumerable<object[]> DeleteExpenseValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, null };
            yield return new object[] { "user1@example.com", 5, null };
            yield return new object[] { "user3@example.com", 16, "user1@example.com" };
            yield return new object[] { "user2@example.com", 25, null };
            yield return new object[] { "user2@example.com", 35, null };
            yield return new object[] { "user3@example.com", 46, null };
            yield return new object[] { "user3@example.com", 57, null };
            yield return new object[] { "user4@example.com", 63, null };
            yield return new object[] { "user4@example.com", 76, null };
        }
        public static IEnumerable<object[]> DeleteExpenseNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", 21, null };
            yield return new object[] { "user1@example.com", 25, null };
            yield return new object[] { "user3@example.com", 116, "user1@example.com" };
            yield return new object[] { "user2@example.com", 125, null };
            yield return new object[] { "user2@example.com", 135, null };
            yield return new object[] { "user3@example.com", 146, null };
            yield return new object[] { "user3@example.com", 157, null };
            yield return new object[] { "user4@example.com", 163, null };
            yield return new object[] { "user4@example.com", 176, null };
        }
        public static IEnumerable<object[]> AddCategoryToExpenseValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, new List<int>() { 2, 3, 4 }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { 3, 4 }, null };
            yield return new object[] { "user3@example.com", 3, new List<int>() { 1, 2, 4 }, "user1@example.com" };
            yield return new object[] { "user2@example.com", 21, new List<int>() { 6 }, null };
            yield return new object[] { "user2@example.com", 22, new List<int>() { 7 }, null };
            yield return new object[] { "user3@example.com", 41, new List<int>() { 11 }, null };
            yield return new object[] { "user4@example.com", 61, new List<int>() { 14 }, null };
            yield return new object[] { "user4@example.com", 62, new List<int>() { 15 }, null };
        }

        public static IEnumerable<object[]> AddCategoryToExpenseNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, new List<int>() { 5 }, null };
            yield return new object[] { "user1@example.com", 324, new List<int>() { 3 }, null };
            yield return new object[] { "user3@example.com", 3, new List<int>() { 5 }, null };
            yield return new object[] { "user2@example.com", 432, new List<int>() { 2 }, null };
            yield return new object[] { "user2@example.com", 21, new List<int>() { 9 }, null };
            yield return new object[] { "user3@example.com", 123, new List<int>() { 2 }, null };
            yield return new object[] { "user3@example.com", 43, new List<int>() { 5 }, null };
            yield return new object[] { "user4@example.com", 1, new List<int>() { 2 }, null };
            yield return new object[] { "user4@example.com", 62, new List<int>() { 5 }, null };
        }

        public static IEnumerable<object[]> AddCategoryToExpenseBadRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, new List<int>() { 1 }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { 2 }, null };
            yield return new object[] { "user3@example.com", 3, new List<int>() { 3 }, "user1@example.com" };
            yield return new object[] { "user2@example.com", 21, new List<int>() { }, null };
            yield return new object[] { "user2@example.com", 22, new List<int>() { }, null };
            yield return new object[] { "user3@example.com", 41, new List<int>() { }, null };
            yield return new object[] { "user3@example.com", 43, new List<int>() { }, null };
            yield return new object[] { "user4@example.com", 61, new List<int>() { 13 }, null };
            yield return new object[] { "user4@example.com", 62, new List<int>() { 14 }, null };

        }

        public static IEnumerable<object[]> RemoveCategoriesFromExpenseValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, 1, null };
            yield return new object[] { "user1@example.com", 2, 2, null };
            yield return new object[] { "user3@example.com", 3, 3, "user1@example.com" };
            yield return new object[] { "user2@example.com", 21, 5, null };
            yield return new object[] { "user2@example.com", 22, 6, null };
            yield return new object[] { "user3@example.com", 41, 9, null };
            yield return new object[] { "user3@example.com", 43, 11, null };
            yield return new object[] { "user4@example.com", 61, 13, null };
            yield return new object[] { "user4@example.com", 62, 14, null };
        }

        public static IEnumerable<object[]> RemoveCategoriesFromExpenseNotFoundInputsTestData()
        {
            yield return new object[] { "user1@example.com", 123, 1, null };
            yield return new object[] { "user1@example.com", 2123, 2, null };
            yield return new object[] { "user3@example.com", 53, 3, null };
            yield return new object[] { "user2@example.com", 221, 23, null };
            yield return new object[] { "user2@example.com", 221, 2, null };
            yield return new object[] { "user3@example.com", 42, 6, null };
            yield return new object[] { "user3@example.com", 443, 3, null };
            yield return new object[] { "user4@example.com", 62, 76, null };
            yield return new object[] { "user4@example.com", 662, 2, null };

        }

        public static IEnumerable<object[]> RemoveCategoriesFromExpenseBadRequestInputsTestData()
        {
            yield return new object[] { "user1@example.com", 1, 2, null };
            yield return new object[] { "user1@example.com", 2, 3, null };
            yield return new object[] { "user3@example.com", 3, 4, "user1@example.com" };
            yield return new object[] { "user2@example.com", 21, 8, null };
            yield return new object[] { "user2@example.com", 22, 8, null };
            yield return new object[] { "user3@example.com", 41, 12, null };
            yield return new object[] { "user3@example.com", 43, 12, null };
            yield return new object[] { "user4@example.com", 61, 14, null };
            yield return new object[] { "user4@example.com", 62, 15, null };

        }

        public static IEnumerable<object[]> GetBudgetValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new DateTime(2020, 3, 15), new DateTime(2022, 7, 10), "spending", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2021, 6, 5), new DateTime(2023, 4, 25), "limitAmount", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2019, 2, 10), new DateTime(2024, 11, 30), "title", "desc", null };
            yield return new object[] { "user1@example.com", new DateTime(2020, 9, 20), new DateTime(2023, 1, 5), null, null, null };

            yield return new object[] { "user2@example.com", new DateTime(2020, 8, 12), new DateTime(2023, 6, 18), "spending", "desc", null };
            yield return new object[] { "user2@example.com", new DateTime(2022, 4, 30), new DateTime(2023, 2, 8), null, null, null };
            yield return new object[] { "user2@example.com", new DateTime(2019, 11, 8), new DateTime(2024, 9, 22), "limitAmoun", "desc", null };
            yield return new object[] { "user2@example.com", new DateTime(2021, 7, 3), new DateTime(2023, 4, 2), "title", "desc", null };

            yield return new object[] { "user3@example.com", new DateTime(2020, 5, 22), new DateTime(2023, 10, 15), null, null, null };
            yield return new object[] { "user3@example.com", new DateTime(2022, 1, 17), new DateTime(2024, 8, 7), "spending", "desc", null };
            yield return new object[] { "user3@example.com", new DateTime(2019, 10, 3), new DateTime(2024, 3, 12), "limitAmoun", "desc", null };
            yield return new object[] { "user3@example.com", new DateTime(2020, 6, 28), new DateTime(2023, 12, 5), "title", "desc", null };

            yield return new object[] { "user4@example.com", new DateTime(2021, 12, 7), new DateTime(2023, 5, 28), "progress", "desc", null };
            yield return new object[] { "user4@example.com", new DateTime(2019, 2, 25), new DateTime(2024, 1, 20), "limitAmoun", "desc", null };
            yield return new object[] { "user4@example.com", new DateTime(2020, 7, 14), new DateTime(2023, 9, 10), "title", "desc", null };
            yield return new object[] { "user4@example.com", new DateTime(2022, 4, 18), new DateTime(2024, 3, 30), null, null, null };

            yield return new object[] { "user1@example.com", null, null, "spending", "desc", null };
            yield return new object[] { "user1@example.com", null, null, "limitAmoun", "desc", null };
            yield return new object[] { "user1@example.com", null, null, "title", "desc", null };
            yield return new object[] { "user1@example.com", null, null, null, null, null };

            yield return new object[] { "user2@example.com", null, null, "spending", "desc", null };
            yield return new object[] { "user2@example.com", null, null, null, null, null };
            yield return new object[] { "user2@example.com", null, null, "limitAmoun", "desc", null };
            yield return new object[] { "user2@example.com", null, null, "title", "desc", null };

            yield return new object[] { "user3@example.com", null, null, null, null, null };
            yield return new object[] { "user3@example.com", null, null, "spending", "desc", null };
            yield return new object[] { "user3@example.com", null, null, "limitAmoun", "desc", null };
            yield return new object[] { "user3@example.com", null, null, "title", "desc", null };

            yield return new object[] { "user4@example.com", null, null, "spending", "desc", null };
            yield return new object[] { "user4@example.com", null, null, "limitAmoun", "desc", null };
            yield return new object[] { "user4@example.com", null, null, "title", "desc", null };
            yield return new object[] { "user4@example.com", null, null, null, null, null };

            yield return new object[] { "user2@example.com", null, null, null, null, "user1@example.com" };
            yield return new object[] { "user3@example.com", null, null, null, null, "user1@example.com" };

        }

        public static IEnumerable<object[]> CreateBudgetValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "Test-Title-1", Description = "Description-1", LimitAmount = 2000, Currency = "eur", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(2), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "Test-Title-2", Description = "Description for a valid goal with a different currency", LimitAmount = 1500, Currency = "usd", StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddYears(3), Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "Test-Title-3", Description = "Description for a valid goal with a longer time period", LimitAmount = 3000, Currency = "eur", StartDate = DateTime.Now.AddDays(10), EndDate = DateTime.Now.AddYears(3).AddMonths(2), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "Test-Title-4", Description = "Description for a valid goal with a larger amount", LimitAmount = 5000, Currency = "php", StartDate = DateTime.Now.AddDays(3), EndDate = DateTime.Now.AddYears(2), Urgency = Urgency.High }, null };

        }

        public static IEnumerable<object[]> CreateBudgetBadRequestTestTestData()
        {
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 0, Currency = "USD", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Urgency = Urgency.Low }, null };
            yield return new object[] { "user2@example.com", new BudgetManageDto { LimitAmount = 1000, Currency = "InvalidCurrency", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 2000, Currency = "EUR", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-7), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 0, Currency = "InvalidCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 0, Currency = "USD", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = -500, Currency = "EUR", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 1000, Currency = "", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 1500, Currency = "InvalidCurrencyCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 2000, Currency = "GBP", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-7), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 0, Currency = "InvalidCode", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = -500, Currency = "", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 1000, Currency = "InvalidCurrency", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-15), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 2000, Currency = "USD", StartDate = DateTime.Now.AddDays(15), EndDate = DateTime.Now, Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto { LimitAmount = 3000, Currency = "GBP", StartDate = DateTime.Now.AddDays(15), EndDate = DateTime.Now.AddDays(10), Urgency = Urgency.Low }, null };

        }

        public static IEnumerable<object[]> UpdateBudgetValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: hdawudb", Id = 1, LimitAmount = 9100, Currency = "usd", Description = "description: dwj4", StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2030, 2, 13), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: dnwaujdnaw", Id = 2, LimitAmount = 375400, Currency = "eur", Description = "description: diw038", StartDate = new DateTime(2022, 5, 3), EndDate = new DateTime(2025, 8, 17), Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: opjkyuypmm", Id = 3, LimitAmount = 13820, Currency = "fjd", Description = "description: hduwe92", StartDate = new DateTime(2019, 12, 21), EndDate = new DateTime(2023, 6, 21), Urgency = Urgency.High }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: jdiwjdw", Id = 4, LimitAmount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11), Urgency = Urgency.Low }, null };
            yield return new object[] { "user3@example.com", new BudgetManageDto() { Title = "title: hduwaw", Id = 4, LimitAmount = 101100, Currency = "fkp", Description = "description: djwirb", StartDate = new DateTime(2023, 2, 25), EndDate = new DateTime(2027, 3, 12) }, "user1@example.com" };

        }

        public static IEnumerable<object[]> UpdateBudgetBadRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: hdawudb", Id = 1, LimitAmount = 9100, Currency = "usd", Description = "description: dwj4", StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2019, 2, 13), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: dnwaujdnaw", Id = 2, LimitAmount = 375400, Currency = "NOT VALID", Description = "description: diw038", StartDate = new DateTime(2022, 5, 3), EndDate = new DateTime(2025, 8, 17), Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: opjkyuypmm", Id = 3, LimitAmount = 0, Currency = "fjd", Description = "description: hduwe92", StartDate = new DateTime(2019, 12, 21), EndDate = new DateTime(2023, 6, 21), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: jdiwjdw", Id = 4, LimitAmount = -100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11), Urgency = Urgency.High }, null };
            yield return new object[] { "user3@example.com", new BudgetManageDto() { Title = "title: hduwaw", Id = 4, LimitAmount = -100, Currency = "NOT VALID", Description = "description: djwirb", StartDate = new DateTime(2030, 2, 25), EndDate = new DateTime(2027, 3, 12) }, "user1@example.com" };

        }

        public static IEnumerable<object[]> DeleteBudgetNotFoundInputTestData()
        {
            yield return new object[] { "user1@example.com", 6, null };
            yield return new object[] { "user1@example.com", 7, null };
            yield return new object[] { "user1@example.com", 8, null };
            yield return new object[] { "user1@example.com", 9, null };
            yield return new object[] { "user1@example.com", 10, null };
        }

        public static IEnumerable<object[]> AddCategoryToBudgetValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, new List<int>() { 2, 3, 4 }, null };
            yield return new object[] { "user1@example.com", 2, new List<int>() { 3, 4, 1 }, null };
            yield return new object[] { "user1@example.com", 3, new List<int>() { 4, 1, 2 }, null };
            yield return new object[] { "user1@example.com", 4, new List<int>() { 1, 2, 3 }, null };
            yield return new object[] { "user2@example.com", 5, new List<int>() { 6, 7, 8 }, null };
        }

        public static IEnumerable<object[]> AddCategoryToBudgetBadRequestInputTestData()
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

        public static IEnumerable<object[]> AddCategoryToBudgetNotFoundInputTestData()
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

        public static IEnumerable<object[]> RemoveCategoriesFromBudgetValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, 1, null };
            yield return new object[] { "user1@example.com", 2, 2, null };
            yield return new object[] { "user1@example.com", 3, 3, null };
            yield return new object[] { "user1@example.com", 4, 4, null };
            yield return new object[] { "user2@example.com", 5, 5, null };

        }

        public static IEnumerable<object[]> RemoveCategoriesFromBudgetNotFoundInputsTestData()
        {
            yield return new object[] { "user1@example.com", 11, 2, null };
            yield return new object[] { "user1@example.com", 1, 19, null };
            yield return new object[] { "user1@example.com", 13, 132, null };
            yield return new object[] { "user1@example.com", 3, 9, null };
            yield return new object[] { "user3@example.com", 11, 11, "user1@example.com" };
        }

        public static IEnumerable<object[]> RemoveCategoriesFromBudgetBadRequestInputsTestData()
        {
            yield return new object[] { "user1@example.com", 1, 2, null };
            yield return new object[] { "user1@example.com", 2, 3, null };
            yield return new object[] { "user1@example.com", 3, 4, null };
            yield return new object[] { "user1@example.com", 4, 1, null };
            yield return new object[] { "user2@example.com", 5, 6, null };
        }

        public static IEnumerable<object[]> UpdateBudgetNotFoundRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: hdawudb", Id = 20, LimitAmount = 9100, Currency = "usd", Description = "description: dwj4", StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2030, 2, 13), Urgency = Urgency.High }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: dnwaujdnaw", Id = 22, LimitAmount = 375400, Currency = "eur", Description = "description: diw038", StartDate = new DateTime(2022, 5, 3), EndDate = new DateTime(2025, 8, 17), Urgency = Urgency.Medium }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: opjkyuypmm", Id = 23, LimitAmount = 13820, Currency = "fjd", Description = "description: hduwe92", StartDate = new DateTime(2019, 12, 21), EndDate = new DateTime(2023, 6, 21), Urgency = Urgency.Low }, null };
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "title: jdiwjdw", Id = 24, LimitAmount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11), Urgency = Urgency.Medium }, null };
            yield return new object[] { "user2@example.com", new BudgetManageDto() { Title = "title: jdiwjdw", Id = 94, LimitAmount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11), Urgency = Urgency.Low }, null };
            yield return new object[] { "user2@example.com", new BudgetManageDto() { Title = "title: jdiwjdw", Id = 124, LimitAmount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11), Urgency = Urgency.High }, null };
            yield return new object[] { "user2@example.com", new BudgetManageDto() { Title = "title: jdiwjdw", Id = 345, LimitAmount = 29100, Currency = "gbp", Description = "description: 82639", StartDate = new DateTime(2015, 9, 12), EndDate = new DateTime(2017, 5, 11), Urgency = Urgency.Medium }, null };
            yield return new object[] { "user3@example.com", new BudgetManageDto() { Title = "title: hduwaw", Id = -1, LimitAmount = 101100, Currency = "fkp", Description = "description: djwirb", StartDate = new DateTime(2023, 2, 25), EndDate = new DateTime(2027, 3, 12), Urgency = Urgency.High }, null };
            yield return new object[] { "user3@example.com", new BudgetManageDto() { Title = "title: hduwaw", Id = 2134, LimitAmount = 101100, Currency = "fkp", Description = "description: djwirb", StartDate = new DateTime(2023, 2, 25), EndDate = new DateTime(2027, 3, 12) }, "user1@example.com" };
            yield return new object[] { "user3@example.com", new BudgetManageDto() { Title = "title: hduwaw", Id = 456, LimitAmount = 101100, Currency = "fkp", Description = "description: djwirb", StartDate = new DateTime(2023, 2, 25), EndDate = new DateTime(2027, 3, 12) }, "user1@example.com" };

        }

        public static IEnumerable<object[]> DeleteBudgetValidTestData()
        {
            yield return new object[] { "user1@example.com", 1, null };
            yield return new object[] { "user1@example.com", 2, null };
            yield return new object[] { "user1@example.com", 3, null };
            yield return new object[] { "user3@example.com", 4, "user1@example.com" };
            yield return new object[] { "user2@example.com", 6, null };
            yield return new object[] { "user2@example.com", 7, null };
            yield return new object[] { "user2@example.com", 8, null };
            yield return new object[] { "user3@example.com", 9, null };
            yield return new object[] { "user3@example.com", 10, null };

        }
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

        public static IEnumerable<object[]> GetUserValidInputTestData()
        {
            yield return new object[] { "user1@example.com" };
            yield return new object[] { "user2@example.com" };
            yield return new object[] { "user3@example.com" };
            yield return new object[] { "user4@example.com" };
        }

        public static IEnumerable<object[]> UpdateUsersCurrencyValidInputTestData()
        {
            yield return new object[] { "user1@example.com", "usd" };
            yield return new object[] { "user2@example.com", "eur" };
            yield return new object[] { "user3@example.com", "php" };
            yield return new object[] { "user4@example.com", "usd" };
        }

        public static IEnumerable<object[]> UpdateUsersCurrencyBadRequestInputTestData()
        {
            yield return new object[] { "user1@example.com", "THIS IS WRONG" };
            yield return new object[] { "user2@example.com", "WRONG" };
            yield return new object[] { "user3@example.com", "123" };
            yield return new object[] { "user4@example.com", "512" };
        }

        public static IEnumerable<object[]> GetCategoryValidInputTestData()
        {
            yield return new object[] { "user1@example.com", "title", "desc", null };
            yield return new object[] { "user1@example.com", "income", "desc", null };
            yield return new object[] { "user1@example.com", "expense", "desc", null };
            yield return new object[] { "user1@example.com", null, "desc", null };
            yield return new object[] { "user1@example.com", "title", null, null };
            yield return new object[] { "user1@example.com", "income", null, null };
            yield return new object[] { "user1@example.com", "expense", null, null };
            yield return new object[] { "user1@example.com", null, null, null };
        }
        public static IEnumerable<object[]> CreateCategoryValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Title = "Food", Description = "Anything that has to do with food" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Title = "Drinks", Description = "Anything that has to do with drinks" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Title = "Shopping", Description = "Anything that has to do with shopping" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Title = "Party", Description = "Anything that has to do with parties" }, null };

        }

        public static IEnumerable<object[]> CreateCategoryBadRequestTestData()
        {
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Title = "title-1", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Title = "title-2", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Title = "title-3", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Title = "title-4", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Title = "title-5", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Title = "title-6", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Title = "title-7", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Title = "title-8", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Title = "title-9", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Title = "title-10", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Title = "title-11", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Title = "title-12", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Title = "title-13", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Title = "title-14", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Title = "title-15", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Title = "title-16", Description = "DOES NOT MATTER" }, null };
        }

        public static IEnumerable<object[]> UpdateCategoryValidInputTestData()
        {
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 1, Title = "new-title-1", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 2, Title = "new-title-2", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 3, Title = "new-title-3", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 4, Title = "new-title-4", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 5, Title = "new-title-5", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 6, Title = "new-title-6", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 7, Title = "new-title-7", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 8, Title = "new-title-8", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 9, Title = "new-title-9", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 10, Title = "new-title-10", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 11, Title = "new-title-11", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 12, Title = "new-title-12", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 13, Title = "new-title-13", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 14, Title = "new-title-14", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 15, Title = "new-title-15", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 16, Title = "new-title-16", Description = "DOES NOT MATTER" }, null };
        }

        public static IEnumerable<object[]> UpdateCategoryBadRequestTestData()
        {
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 1, Title = "title-2", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 2, Title = "title-3", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 3, Title = "title-4", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 4, Title = "title-1", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 5, Title = "title-6", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 6, Title = "title-7", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 7, Title = "title-8", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 8, Title = "title-5", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 9, Title = "title-10", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 10, Title = "title-11", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 11, Title = "title-12", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 12, Title = "title-9", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 13, Title = "title-14", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 14, Title = "title-15", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 15, Title = "title-16", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 16, Title = "title-13", Description = "DOES NOT MATTER" }, null };

        }

        public static IEnumerable<object[]> UpdateCategoryNotFoundTestData()
        {
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 5, Title = "new-title-1", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 6, Title = "new-title-2", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 7, Title = "new-title-3", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 8, Title = "new-title-4", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 9, Title = "new-title-5", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 10, Title = "new-title-6", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 11, Title = "new-title-7", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user2@example.com", new CategoryManageDto() { Id = 12, Title = "new-title-8", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 13, Title = "new-title-9", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 14, Title = "new-title-10", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 15, Title = "new-title-11", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user3@example.com", new CategoryManageDto() { Id = 1, Title = "new-title-12", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 2, Title = "new-title-13", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 3, Title = "new-title-14", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 4, Title = "new-title-15", Description = "DOES NOT MATTER" }, null };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 5, Title = "new-title-16", Description = "DOES NOT MATTER" }, null };

        }

        public static IEnumerable<object[]> DeleteCategoryValidInputTestData()
        {
            yield return new object[] { "user1@example.com", 1, null };
            yield return new object[] { "user1@example.com", 2, null };
            yield return new object[] { "user1@example.com", 3, null };
            yield return new object[] { "user1@example.com", 4, null };
            yield return new object[] { "user2@example.com", 5, null };
            yield return new object[] { "user2@example.com", 6, null };
            yield return new object[] { "user2@example.com", 7, null };
            yield return new object[] { "user2@example.com", 8, null };
            yield return new object[] { "user3@example.com", 9, null };
            yield return new object[] { "user3@example.com", 10, null };
            yield return new object[] { "user3@example.com", 11, null };
            yield return new object[] { "user3@example.com", 12, null };
            yield return new object[] { "user4@example.com", 13, null };
            yield return new object[] { "user4@example.com", 14, null };
            yield return new object[] { "user4@example.com", 15, null };
            yield return new object[] { "user4@example.com", 16, null };
        }

        public static IEnumerable<object[]> DeleteCategoryNotFoundTestData()
        {
            yield return new object[] { "user1@example.com", 16, null };
            yield return new object[] { "user1@example.com", 15, null };
            yield return new object[] { "user1@example.com", 14, null };
            yield return new object[] { "user1@example.com", 13, null };
            yield return new object[] { "user2@example.com", 12, null };
            yield return new object[] { "user2@example.com", 11, null };
            yield return new object[] { "user2@example.com", 10, null };
            yield return new object[] { "user2@example.com", 9, null };
            yield return new object[] { "user3@example.com", 8, null };
            yield return new object[] { "user3@example.com", 7, null };
            yield return new object[] { "user3@example.com", 6, null };
            yield return new object[] { "user3@example.com", 5, null };
            yield return new object[] { "user4@example.com", 4, null };
            yield return new object[] { "user4@example.com", 3, null };
            yield return new object[] { "user4@example.com", 2, null };
            yield return new object[] { "user4@example.com", 1, null };

        }

        public static IEnumerable<object[]> GetAllAuthorizationInvitesValidTestData()
        {
            yield return new object[] { "user1@example.com" };
            yield return new object[] { "user2@example.com" };
            yield return new object[] { "user3@example.com" };
            yield return new object[] { "user4@example.com" };
        }

        public static IEnumerable<object[]> GetOutgoingAuthorizationInvitesValidTestData()
        {
            yield return new object[] { "user1@example.com" };
            yield return new object[] { "user2@example.com" };
            yield return new object[] { "user3@example.com" };
            yield return new object[] { "user4@example.com" };
        }

        public static IEnumerable<object[]> GetAllAuthorizedUsersValidTestData()
        {
            yield return new object[] { "user1@example.com" };
            yield return new object[] { "user2@example.com" };
            yield return new object[] { "user3@example.com" };
            yield return new object[] { "user4@example.com" };
        }

        public static IEnumerable<object[]> AuthorizeUserValidTestData()
        {
            yield return new object[] { "user2@example.com", "user3@example.com", "Valid title", "Valid description" };
            yield return new object[] { "user3@example.com", "user4@example.com", "Valid title", "Valid description" };
            yield return new object[] { "user4@example.com", "user1@example.com", "Valid title", "Valid description" };

        }

        public static IEnumerable<object[]> GiveEditPermissionValidInputTestData()
        {
            yield return new object[] { "user1@example.com", "user2@example.com", true };
            yield return new object[] { "user1@example.com", "user3@example.com", false };
        }

        public static IEnumerable<object[]> GiveEditPermissionNotFoundTestData()
        {
            yield return new object[] { };
        }

        public static IEnumerable<object[]> GiveEditPermissionBadRequestTestData()
        {
            yield return new object[] { "user1@example.com", "user4@example.com", true };
            yield return new object[] { "user1@example.com", "user1@example.com", true };
            yield return new object[] { "user2@example.com", "user1@example.com", true };
            yield return new object[] { "user2@example.com", "user2@example.com", true };
            yield return new object[] { "user2@example.com", "user3@example.com", true };
            yield return new object[] { "user2@example.com", "user4@example.com", true };
            yield return new object[] { "user3@example.com", "user1@example.com", true };
            yield return new object[] { "user3@example.com", "user2@example.com", true };
            yield return new object[] { "user3@example.com", "user3@example.com", true };
            yield return new object[] { "user3@example.com", "user4@example.com", true };
            yield return new object[] { "user4@example.com", "user1@example.com", true };
            yield return new object[] { "user4@example.com", "user2@example.com", true };
            yield return new object[] { "user4@example.com", "user3@example.com", true };
            yield return new object[] { "user4@example.com", "user4@example.com", true };
        }

        public static IEnumerable<object[]> DeleteAuthorizationValidTestData()
        {
            yield return new object[] { "user1@example.com", "user2@example.com" };
            yield return new object[] { "user1@example.com", "user3@example.com" };
        }

        public static IEnumerable<object[]> AuthorizeUserBadRequestTestData()
        {
            yield return new object[] { "user2@example.com", "user3@example.com", "This is way to big of a title to be called valid", "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium q" };
            yield return new object[] { "user3@example.com", "user4@example.com", "This is way to big of a title to be called valid", "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium q" };
            yield return new object[] { "user4@example.com", "user1@example.com", "This is way to big of a title to be called valid", "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium q" };
            yield return new object[] { "user1@example.com", "user1@example.com", "Valid", "Valid" };
        }
        public static IEnumerable<object[]> GetBudgetForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", "user4@example.com" };
            yield return new object[] { "user4@example.com", "user1@example.com" };
            yield return new object[] { "user4@example.com", "user2@example.com" };
            yield return new object[] { "user4@example.com", "user3@example.com" };
        }

        public static IEnumerable<object[]> CreateBudgetForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Title = "Test-Title-1", Description = "Description-1", LimitAmount = 2000, Currency = "eur", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(2), Urgency = Urgency.Low }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new BudgetManageDto() { Title = "Test-Title-2", Description = "Description for a valid goal with a different currency", LimitAmount = 1500, Currency = "usd", StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddYears(5), Urgency = Urgency.Medium }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new BudgetManageDto() { Title = "Test-Title-3", Description = "Description for a valid goal with a longer time period", LimitAmount = 3000, Currency = "eur", StartDate = DateTime.Now.AddDays(10), EndDate = DateTime.Now.AddYears(4), Urgency = Urgency.High }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new BudgetManageDto() { Title = "Test-Title-4", Description = "Description for a valid goal with a larger amount", LimitAmount = 5000, Currency = "php", StartDate = DateTime.Now.AddDays(3), EndDate = DateTime.Now.AddYears(2).AddMonths(6), Urgency = Urgency.Low }, "user3@example.com" };
        }

        public static IEnumerable<object[]> UpdateBudgetForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", new BudgetManageDto() { Id = 1, Title = "Test-Title-1", Description = "Description-1", LimitAmount = 2000, Currency = "eur", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(2), Urgency = Urgency.Low }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new BudgetManageDto() { Id = 13, Title = "Test-Title-2", Description = "Description for a valid goal with a different currency", LimitAmount = 1500, Currency = "usd", StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddYears(5), Urgency = Urgency.Medium }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new BudgetManageDto() { Id = 14, Title = "Test-Title-3", Description = "Description for a valid goal with a longer time period", LimitAmount = 3000, Currency = "eur", StartDate = DateTime.Now.AddDays(10), EndDate = DateTime.Now.AddYears(4), Urgency = Urgency.High }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new BudgetManageDto() { Id = 15, Title = "Test-Title-4", Description = "Description for a valid goal with a larger amount", LimitAmount = 5000, Currency = "php", StartDate = DateTime.Now.AddDays(3), EndDate = DateTime.Now.AddYears(2).AddMonths(6), Urgency = Urgency.Low }, "user3@example.com" };

        }

        public static IEnumerable<object[]> DeleteBudgetForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 16, "user4@example.com" };
            yield return new object[] { "user4@example.com", 1, "user1@example.com" };
            yield return new object[] { "user4@example.com", 6, "user2@example.com" };
            yield return new object[] { "user4@example.com", 9, "user3@example.com" };
        }

        public static IEnumerable<object[]> AddCategoryToBudgetForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 16, new List<int>() { 13, 14 }, "user4@example.com" };
            yield return new object[] { "user4@example.com", 1, new List<int>() { 2, 3, 4 }, "user1@example.com" };
            yield return new object[] { "user4@example.com", 6, new List<int>() { 5, 7, 8 }, "user2@example.com" };
            yield return new object[] { "user4@example.com", 9, new List<int>() { 10, 11, 12 }, "user3@example.com" };

        }

        public static IEnumerable<object[]> RemoveCategoryFromBudgetForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 16, 14, "user4@example.com" };
            yield return new object[] { "user4@example.com", 1, 4, "user1@example.com" };
            yield return new object[] { "user4@example.com", 6, 8, "user2@example.com" };
            yield return new object[] { "user4@example.com", 9, 12, "user3@example.com" };

        }

        public static IEnumerable<object[]> GetCategoriesForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", "user4@example.com" };
            yield return new object[] { "user4@example.com", "user1@example.com" };
            yield return new object[] { "user4@example.com", "user2@example.com" };
            yield return new object[] { "user4@example.com", "user3@example.com" };
        }

        public static IEnumerable<object[]> CreateCategoryForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Title = "Food", Description = "Anything that has to do with food" }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Title = "Drinks", Description = "Anything that has to do with drinks" }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Title = "Shopping", Description = "Anything that has to do with shopping" }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Title = "Party", Description = "Anything that has to do with parties" }, "user3@example.com" };
        }

        public static IEnumerable<object[]> UpdateCategoryForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", new CategoryManageDto() { Id = 14, Title = "Food", Description = "Anything that has to do with food" }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 2, Title = "Drinks", Description = "Anything that has to do with drinks" }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 6, Title = "Shopping", Description = "Anything that has to do with shopping" }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new CategoryManageDto() { Id = 11, Title = "Party", Description = "Anything that has to do with parties" }, "user3@example.com" };
        }

        public static IEnumerable<object[]> DeleteCategoryForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 13, "user4@example.com" };
            yield return new object[] { "user4@example.com", 2, "user1@example.com" };
            yield return new object[] { "user4@example.com", 6, "user2@example.com" };
            yield return new object[] { "user4@example.com", 9, "user3@example.com" };
        }

        public static IEnumerable<object[]> GetExpenseForbidenTestData()
        {
            yield return new object[] { "user1@example.com", "user4@example.com" };
            yield return new object[] { "user4@example.com", "user1@example.com" };
            yield return new object[] { "user4@example.com", "user2@example.com" };
            yield return new object[] { "user4@example.com", "user3@example.com" };
        }

        public static IEnumerable<object[]> CreateExpenseForbidenTestData()
        {
            yield return new object[] { "user1@example.com", new ExpenseDto() { Title = "Food", Description = "Ate at a restaurant", Amount = 54, Currency = "eur", Date = DateTime.Now, DocumentUrl = "www.restaurant.com/receipt", Urgency = Urgency.Low }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new ExpenseDto() { Title = "shopping", Description = "shoes", Amount = 460, Currency = "php", Date = DateTime.Now.AddDays(3), DocumentUrl = "shoes", Urgency = Urgency.Low }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new ExpenseDto() { Title = "insurance", Description = "insurance", Amount = 395, Currency = "usd", Date = DateTime.Now.AddDays(2), DocumentUrl = "receipt", Urgency = Urgency.High }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new ExpenseDto() { Title = "gym", Description = "", Amount = 323, Currency = "eur", Date = DateTime.Now.AddDays(7), DocumentUrl = "receipt", Urgency = Urgency.Medium }, "user3@example.com" };

        }

        public static IEnumerable<object[]> UpdateExpenseForbidenTestData()
        {
            yield return new object[] { "user1@example.com", new ExpenseDto() { Id = 63, Title = "Food", Description = "Ate at a restaurant", Amount = 54, Currency = "eur", Date = DateTime.Now, DocumentUrl = "www.restaurant.com/receipt", Urgency = Urgency.Low }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new ExpenseDto() { Id = 15, Title = "shopping", Description = "shoes", Amount = 460, Currency = "php", Date = DateTime.Now.AddDays(3), DocumentUrl = "shoes", Urgency = Urgency.Low }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new ExpenseDto() { Id = 25, Title = "insurance", Description = "insurance", Amount = 395, Currency = "usd", Date = DateTime.Now.AddDays(2), DocumentUrl = "receipt", Urgency = Urgency.High }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new ExpenseDto() { Id = 50, Title = "gym", Description = "", Amount = 323, Currency = "eur", Date = DateTime.Now.AddDays(7), DocumentUrl = "receipt", Urgency = Urgency.Medium }, "user3@example.com" };
        }

        public static IEnumerable<object[]> DeleteExpenseForbidenTestData()
        {
            yield return new object[] { "user1@example.com", 64, "user4@example.com" };
            yield return new object[] { "user4@example.com", 18, "user1@example.com" };
            yield return new object[] { "user4@example.com", 37, "user2@example.com" };
            yield return new object[] { "user4@example.com", 46, "user3@example.com" };
        }

        public static IEnumerable<object[]> AddCategoryToExpenseForbidenTestData()
        {
            yield return new object[] { "user1@example.com", 64, new List<int>() { 13, 14 }, "user4@example.com" };
            yield return new object[] { "user4@example.com", 18, new List<int>() { 1, 2, 3 }, "user1@example.com" };
            yield return new object[] { "user4@example.com", 37, new List<int>() { 5, 6, 7 }, "user2@example.com" };
            yield return new object[] { "user4@example.com", 46, new List<int>() { 9, 10, 11 }, "user3@example.com" };

        }

        public static IEnumerable<object[]> RemoveCategoriesFromExpenseForbidenTestData()
        {
            yield return new object[] { "user1@example.com", 64, 16, "user4@example.com" };
            yield return new object[] { "user4@example.com", 18, 2, "user1@example.com" };
            yield return new object[] { "user4@example.com", 37, 6, "user2@example.com" };
            yield return new object[] { "user4@example.com", 46, 15, "user3@example.com" };
        }

        public static IEnumerable<object[]> GetGoalsForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", "user4@example.com" };
            yield return new object[] { "user4@example.com", "user1@example.com" };
            yield return new object[] { "user4@example.com", "user2@example.com" };
            yield return new object[] { "user4@example.com", "user3@example.com" };
        }

        public static IEnumerable<object[]> CreateGoalsForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 1, Title = "Test-Title-1", Description = "Description-1", Amount = 2000, Currency = "eur", StartDate = DateTime.Now, EndDate = new DateTime(2026, 1, 1) }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new GoalManageDto() { Id = 2, Title = "Test-Title-1", Description = "Description for a valid goal with a different currency", Amount = 1500, Currency = "usd", StartDate = DateTime.Now.AddDays(5), EndDate = new DateTime(2026, 1, 1) }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new GoalManageDto() { Id = 3, Title = "Test-Title-1", Description = "Description for a valid goal with a longer time period", Amount = 3000, Currency = "eur", StartDate = DateTime.Now.AddDays(10), EndDate = new DateTime(2026, 2, 15) }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new GoalManageDto() { Id = 4, Title = "Test-Title-1", Description = "Description for a valid goal with a larger amount", Amount = 5000, Currency = "php", StartDate = DateTime.Now.AddDays(3), EndDate = new DateTime(2026, 1, 1) }, "user3@example.com" };

        }

        public static IEnumerable<object[]> UpdateGoalsForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", new GoalManageDto() { Id = 1, Title = "Test-Title-1", Description = "Description-1", Amount = 2000, Currency = "eur", StartDate = DateTime.Now, EndDate = new DateTime(2026, 1, 1) }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new GoalManageDto() { Id = 2, Title = "Test-Title-1", Description = "Description for a valid goal with a different currency", Amount = 1500, Currency = "usd", StartDate = DateTime.Now.AddDays(5), EndDate = new DateTime(2026, 1, 1) }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new GoalManageDto() { Id = 3, Title = "Test-Title-1", Description = "Description for a valid goal with a longer time period", Amount = 3000, Currency = "eur", StartDate = DateTime.Now.AddDays(10), EndDate = new DateTime(2026, 2, 15) }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new GoalManageDto() { Id = 4, Title = "Test-Title-1", Description = "Description for a valid goal with a larger amount", Amount = 5000, Currency = "php", StartDate = DateTime.Now.AddDays(3), EndDate = new DateTime(2026, 1, 1) }, "user3@example.com" };

        }

        public static IEnumerable<object[]> DeleteGoalsForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 16, "user4@example.com" };
            yield return new object[] { "user4@example.com", 4, "user1@example.com" };
            yield return new object[] { "user4@example.com", 7, "user2@example.com" };
            yield return new object[] { "user4@example.com", 12, "user3@example.com" };
        }

        public static IEnumerable<object[]> AddCategoryToGoalsForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 16, new List<int>() { 13, 15, 16 }, "user4@example.com" };
            yield return new object[] { "user4@example.com", 4, new List<int>() { 1, 2, 3 }, "user1@example.com" };
            yield return new object[] { "user4@example.com", 7, new List<int>() { 5, 6 }, "user2@example.com" };
            yield return new object[] { "user4@example.com", 12, new List<int>() { 9, 11, 12 }, "user3@example.com" };
        }

        public static IEnumerable<object[]> RemoveCategoryFromGoalForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 16, 13, "user4@example.com" };
            yield return new object[] { "user4@example.com", 4, 4, "user1@example.com" };
            yield return new object[] { "user4@example.com", 7, 6, "user2@example.com" };
            yield return new object[] { "user4@example.com", 12, 10, "user3@example.com" };
        }

        public static IEnumerable<object[]> GetIncomesForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", "user4@example.com" };
            yield return new object[] { "user4@example.com", "user1@example.com" };
            yield return new object[] { "user4@example.com", "user2@example.com" };
            yield return new object[] { "user4@example.com", "user3@example.com" };
        }

        public static IEnumerable<object[]> CreateIncomesForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "eur", Amount = 23, Date = DateTime.Now, DocumentUrl = "receipt" }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "php", Amount = 2353, Date = DateTime.Now, DocumentUrl = "receipt" }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "usd", Amount = 24, Date = DateTime.Now.AddDays(-8), DocumentUrl = "receipt" }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new IncomeDto() { Title = "food", Description = "Cheese, tomato, burgers", Currency = "fjd", Amount = 45, Date = DateTime.Now.AddDays(-6), DocumentUrl = "receipt" }, "user3@example.com" };

        }

        public static IEnumerable<object[]> UpdateIncomesForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", new IncomeDto() { Id = 1, Title = "new Title", Description = "new Description", Currency = "php", Amount = 999999, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, "user4@example.com" };
            yield return new object[] { "user4@example.com", new IncomeDto() { Id = 3, Title = "Random title-dwddwwa214", Description = "new Description", Currency = "all", Amount = 123, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, "user1@example.com" };
            yield return new object[] { "user4@example.com", new IncomeDto() { Id = 4, Title = "A cool heading", Description = "new Description", Currency = "dzd", Amount = 438, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, "user2@example.com" };
            yield return new object[] { "user4@example.com", new IncomeDto() { Id = 6, Title = "TITLE/TITLE", Description = "new Description", Currency = "aoa", Amount = 12, Date = DateTime.Now, DocumentUrl = "WWW.SOMETHINGFOREXAMPLE.com" }, "user3@example.com" };
        }

        public static IEnumerable<object[]> DeleteIncomeForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 61, "user4@example.com" };
            yield return new object[] { "user4@example.com", 12, "user1@example.com" };
            yield return new object[] { "user4@example.com", 36, "user2@example.com" };
            yield return new object[] { "user4@example.com", 53, "user3@example.com" };
        }

        public static IEnumerable<object[]> AddCategoryToIncomeForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 61, new List<int>() { 16 }, "user4@example.com" };
            yield return new object[] { "user4@example.com", 12, new List<int>() { 3 }, "user1@example.com" };
            yield return new object[] { "user4@example.com", 36, new List<int>() { 5 }, "user2@example.com" };
            yield return new object[] { "user4@example.com", 53, new List<int>() { 12 }, "user3@example.com" };
        }

        public static IEnumerable<object[]> RemoveCategoriesFromIncomeForbiddenTestData()
        {
            yield return new object[] { "user1@example.com", 61, 13, "user4@example.com" };
            yield return new object[] { "user4@example.com", 12, 2, "user1@example.com" };
            yield return new object[] { "user4@example.com", 36, 6, "user2@example.com" };
            yield return new object[] { "user4@example.com", 53, 11, "user3@example.com" };
        }
    }
}
namespace FinanceApi.Data.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Currency { get; set; }
    }
}

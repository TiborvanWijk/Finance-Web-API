﻿namespace FinanceApi.Data.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}

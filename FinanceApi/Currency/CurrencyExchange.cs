namespace FinanceApi.Currency
{
    public abstract class CurrencyExchange
    {


        public static decimal GetExchangeRate(string fromCurrency, string toCurrency, DateTime date)
        {
            // External api will be needed for this
            return 1;
        }
    }
}

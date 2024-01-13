using System.Globalization;

namespace FinanceApi.Validators
{
    public abstract class Validator
    {

        public static bool IsValidCurrencyCode(string currencyCode)
        {

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures)
            {
                RegionInfo regionInfo = new RegionInfo(culture.Name);
                if(regionInfo.ISOCurrencySymbol.ToLower().Equals(currencyCode.ToLower()))
                    return true;
            }

            return false;
        }

        public static bool ValidateTimePeriod(DateTime? startDate, DateTime? endDate, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (!startDate.HasValue || !endDate.HasValue)
            {
                errorCode = 400;
                errorMessage = "Invalid date format.";
                return false;
            }

            if (startDate >= endDate)
            {
                errorCode = 400;
                errorMessage = "EndDate must be later then startDate.";
                return false;
            }

            if (endDate.Value - startDate.Value > TimeSpan.FromDays(365 * 30))
            {
                errorCode = 400;
                errorMessage = "The time period should be within 30 years.";
                return false;
            }

            return true;
        }

    }
}

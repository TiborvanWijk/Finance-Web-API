using System.Globalization;

namespace FinanceApi.Validators
{
    public abstract class Validator
    {

        internal static bool IsValidCurrencyCode(string currencyCode)
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

    }
}

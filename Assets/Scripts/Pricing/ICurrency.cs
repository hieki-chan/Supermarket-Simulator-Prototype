namespace Supermarket.Pricing
{
    public interface ICurrency
    {
        StandardCurrency ToStandard();
    }

    public static class CurrencyConverter
    {
        /*public static C2 Convert<C1, C2>(C1 from) where C1 : struct, ICurrency where C2 : struct, ICurrency
        {
            from.ToStandard();
        }*/
    }
}

namespace Food.Domain.Common;

internal static class Guard
{
    public static decimal Positive(decimal value, string paramName) =>
        value > 0 ? value : throw new ArgumentOutOfRangeException(paramName, value, "Value must be positive.");

    public static decimal NonNegative(decimal value, string paramName) =>
        value >= 0 ? value : throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be negative.");

    public static int Positive(int value, string paramName) =>
        value > 0 ? value : throw new ArgumentOutOfRangeException(paramName, value, "Value must be positive.");

    public static int NonNegative(int value, string paramName) =>
        value >= 0 ? value : throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be negative.");

    public static string NotEmpty(string value, string paramName) =>
        !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("Value cannot be empty.", paramName);
}

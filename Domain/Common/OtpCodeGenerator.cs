namespace Domain.Common;

public static class OtpCodeGenerator
{
    private static Random _random = new Random();

    public static string GenerateOtp(int length = 6)
    {
        return _random.Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length)).ToString("D" + length);
    }
}

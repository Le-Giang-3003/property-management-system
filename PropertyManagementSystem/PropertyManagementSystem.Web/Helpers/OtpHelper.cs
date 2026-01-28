namespace PropertyManagementSystem.Web.Helpers
{
    public static class OtpHelper
    {
        public static string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6 digits
        }

        public static bool ValidateOtp(string? sessionOtp, string? userInputOtp)
        {
            return !string.IsNullOrEmpty(sessionOtp) &&
                   !string.IsNullOrEmpty(userInputOtp) &&
                   sessionOtp.Trim() == userInputOtp.Trim();
        }
    }

}

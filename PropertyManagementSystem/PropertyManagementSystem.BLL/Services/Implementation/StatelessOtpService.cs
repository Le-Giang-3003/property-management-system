using PropertyManagementSystem.BLL.Services.Interface;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Stateless OTP service implementation.
/// </summary>
public class StatelessOtpService : IStatelessOtpService
{
    /// <summary>
    /// The secret key
    /// </summary>
    private readonly string _secretKey = "PropertyManagementSecretKey2026!@#";
    /// <summary>
    /// The email service
    /// </summary>
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of the <see cref=".StatelessOtpService"/> class.
    /// </summary>
    /// <param name="emailService">The email service.</param>
    public StatelessOtpService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    /// <summary>
    /// Generates the otp hash.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <returns></returns>
    public async Task<string> GenerateOtpHashAsync(string email)  
    {
        var otp = Random.Shared.Next(100000, 999999).ToString("D6");
        var expiresTicks = DateTime.UtcNow.AddMinutes(5).Ticks.ToString();  

        var data = $"{email}:{otp}:{expiresTicks}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var hash = Convert.ToBase64String(hashBytes);

        await _emailService.SendEmailAsync(email, "Xác thực email", $"Mã OTP: {otp}");

        return $"{hash}:{expiresTicks}";
    }

    /// <summary>
    /// Verifies the otp asynchronous.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="userOtp">The user otp.</param>
    /// <param name="receivedHash">The received hash.</param>
    /// <returns></returns>
    public bool VerifyOtpAsync(string email, string userOtp, string receivedHash)  
    {
        var parts = receivedHash.Split(':');
        if (parts.Length != 2) return false;

        var hash = parts[0];
        var expiresTicksStr = parts[1];

        if (!long.TryParse(expiresTicksStr, out long expiresTicks)) return false;
        if (DateTime.UtcNow.Ticks > expiresTicks) return false;  

        var data = $"{email}:{userOtp}:{expiresTicksStr}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var expectedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var expectedHash = Convert.ToBase64String(expectedBytes);

        return hash == expectedHash;
    }
}

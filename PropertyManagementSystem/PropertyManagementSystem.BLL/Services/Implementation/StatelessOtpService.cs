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
        var expiresTicks = DateTime.UtcNow.AddMinutes(1).Ticks.ToString();  

        var data = $"{email}:{otp}:{expiresTicks}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var hash = Convert.ToBase64String(hashBytes);

        var emailContent = CreateRegistrationOtpEmail(email, otp);
        await _emailService.SendEmailAsync(email, "Xác thực email", emailContent);

        return $"{hash}:{expiresTicks}";
    }   

    /// <summary>
    /// Verifies the otp asynchronous.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="userOtp">The user otp.</param>
    /// <param name="receivedHash">The received hash.</param>
    /// <returns></returns>
    public (bool IsValid, string ErrorMessage) VerifyOtpAsync(string email, string userOtp, string receivedHash)  
    {
        var parts = receivedHash.Split(':');
        if (parts.Length != 2) return (false, "Invalid OTP format");

        var hash = parts[0];
        var expiresTicksStr = parts[1];

        if (!long.TryParse(expiresTicksStr, out long expiresTicks)) return (false, "Invalid expiration time"); 
        if (DateTime.UtcNow.Ticks > expiresTicks) return (false, "OTP has expired");

        var data = $"{email}:{userOtp}:{expiresTicksStr}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var expectedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var expectedHash = Convert.ToBase64String(expectedBytes);

        if (hash != expectedHash)
            return (false, "Invalid OTP code");

        return (true, "OTP verified successfully");
    }

    /// <summary>
    /// ACCOUNT REGISTRATION OTP VERIFICATION
    /// </summary>
    private static string CreateRegistrationOtpEmail(string userName, string otp)
    {
        return $@"
        <html>
        <head>
            <style>
                body {{ font-family: 'Segoe UI', Arial, sans-serif; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
                .header {{ background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
                .content {{ padding: 30px; background: #f9f9f9; }}
                .otp-box {{ background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%); padding: 30px; text-align: center; margin: 30px 0; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
                .otp-code {{ color: white; letter-spacing: 15px; margin: 0; font-size: 48px; font-weight: bold; text-shadow: 2px 2px 4px rgba(0,0,0,0.3); }}
                .info-box {{ background: #e3f2fd; border-left: 4px solid #2196F3; padding: 15px; margin: 20px 0; }}
                .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                .footer {{ background: #f1f1f1; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1 style='margin: 0;'>Account Registration</h1>
                </div>
                
                <div class='content'>
                    <p>Hello <strong>{userName}</strong>,</p>
                    
                    <p>Thank you for registering with <strong>Property Management System</strong>!</p>
                    
                    <p>To complete your registration, please enter the following verification code:</p>
                    
                    <div class='otp-box'>
                        <h1 class='otp-code'>{otp}</h1>
                    </div>
                    
                    <div class='info-box'>
                        <p style='margin: 0; font-size: 14px; color: #1976D2;'>
                            <strong>Why do I need this?</strong><br>
                            This verification code ensures that you are the owner of this email address and helps us protect your account from unauthorized access.
                        </p>
                    </div>
                    
                    <div class='warning'>
                        <p style='margin: 0; font-weight: bold; color: #856404;'>Important Security Notes:</p>
                        <ul style='margin: 10px 0; color: #856404;'>
                            <li>This OTP is valid for <strong>5 minutes</strong> only</li>
                            <li><strong>DO NOT</strong> share this code with anyone</li>
                            <li>Our staff will never ask for your OTP</li>
                            <li>If you did not request this code, please ignore this email</li>
                        </ul>
                    </div>
                    
                    <p style='color: #666; font-size: 14px; margin-top: 30px;'>
                        <strong>Email:</strong> {userName}<br>
                        <strong>Sent at:</strong> {DateTime.Now:MM/dd/yyyy HH:mm:ss}
                    </p>
                </div>
                
                <div class='footer'>
                    This email was sent automatically from <strong>Property Management System</strong>.<br>
                    Please do not reply to this email.<br>
                    Need help? Contact us at: <a href='mailto:support@propertymanagement.com'>support@propertymanagement.com</a>
                </div>
            </div>
        </body>
        </html>
            ";
    }
}

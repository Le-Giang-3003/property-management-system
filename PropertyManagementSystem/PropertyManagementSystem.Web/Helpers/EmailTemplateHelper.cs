namespace PropertyManagementSystem.Web.Helpers
{
    public static class EmailTemplateHelper
    {
        /// <summary>
        /// EMAIL 1: ACCOUNT REGISTRATION CONFIRMATION
        /// </summary>
        public static string CreateAccountConfirmationEmail(string userName, string confirmationLink)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Segoe UI', Arial, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
                        .header {{ background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background: #f9f9f9; }}
                        .button {{ display: inline-block; padding: 15px 30px; background: #4CAF50; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; margin: 20px 0; }}
                        .footer {{ background: #f1f1f1; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; }}
                        .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>Confirm Your Account</h1>
                        </div>
                        
                        <div class='content'>
                            <p>Hello <strong>{userName}</strong>,</p>
                            
                            <p>Thank you for registering an account with <strong>Property Management System</strong>!</p>
                            
                            <p>To complete your registration, please click the button below to confirm your email address:</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{confirmationLink}' class='button'>
                                    Confirm Account
                                </a>
                            </div>
                            
                            <p style='color: #666; font-size: 14px;'>
                                Or copy this link into your browser:<br>
                                <a href='{confirmationLink}' style='color: #4CAF50; word-break: break-all;'>{confirmationLink}</a>
                            </p>
                            
                            <div class='warning'>
                                <p style='margin: 0; font-weight: bold; color: #856404;'>Important Notes:</p>
                                <ul style='margin: 10px 0; color: #856404;'>
                                    <li>This confirmation link is valid for <strong>24 hours</strong></li>
                                    <li>If you did not register this account, please ignore this email</li>
                                </ul>
                            </div>
                        </div>
                        
                        <div class='footer'>
                            This email was sent automatically from <strong>Property Management System</strong>.<br>
                            Please do not reply to this email.
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        /// <summary>
        /// EMAIL 2: LEASE SIGNING OTP
        /// </summary>
        public static string CreateLeaseSigningOtpEmail(string userName, string leaseNumber, string otp)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Segoe UI', Arial, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background: #f9f9f9; }}
                        .otp-box {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; margin: 30px 0; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
                        .otp-code {{ color: white; letter-spacing: 15px; margin: 0; font-size: 48px; font-weight: bold; text-shadow: 2px 2px 4px rgba(0,0,0,0.3); }}
                        .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                        .footer {{ background: #f1f1f1; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>Lease Signing Confirmation</h1>
                        </div>
                        
                        <div class='content'>
                            <p>Hello <strong>{userName}</strong>,</p>
                            
                            <p>You are signing lease <strong>{leaseNumber}</strong> on the Property Management System.</p>
                            
                            <p>Your OTP confirmation code is:</p>
                            
                            <div class='otp-box'>
                                <h1 class='otp-code'>{otp}</h1>
                            </div>
                            
                            <div class='warning'>
                                <p style='margin: 0; font-weight: bold; color: #856404;'>Important Notes:</p>
                                <ul style='margin: 10px 0; color: #856404;'>
                                    <li>This OTP is valid for <strong>5 minutes</strong></li>
                                    <li><strong>DO NOT</strong> share this code with anyone</li>
                                    <li>If you did not request this, please ignore this email</li>
                                    <li>Once signed, the lease will be legally binding</li>
                                </ul>
                            </div>
                            
                            <p style='color: #666; font-size: 14px; margin-top: 30px;'>
                                <strong>Lease:</strong> {leaseNumber}<br>
                                <strong>Sent at:</strong> {DateTime.Now:MM/dd/yyyy HH:mm:ss}
                            </p>
                        </div>
                        
                        <div class='footer'>
                            This email was sent automatically from <strong>Property Management System</strong>.<br>
                            Please do not reply to this email.
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        /// <summary>
        /// EMAIL 3: LEASE FULLY SIGNED NOTIFICATION
        /// </summary>
        public static string CreateLeaseFullySignedEmail(string userName, string leaseNumber, string leaseDetailsUrl)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Segoe UI', Arial, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
                        .header {{ background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background: #f9f9f9; }}
                        .success-box {{ background: #d4edda; border: 2px solid #28a745; padding: 20px; border-radius: 8px; margin: 20px 0; text-align: center; }}
                        .button {{ display: inline-block; padding: 15px 30px; background: #28a745; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; margin: 20px 0; }}
                        .footer {{ background: #f1f1f1; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>Lease is Now Active</h1>
                        </div>
                        
                        <div class='content'>
                            <p>Hello <strong>{userName}</strong>,</p>
                            
                            <div class='success-box'>
                                <h2 style='color: #28a745; margin: 0;'>Congratulations!</h2>
                                <p style='margin: 10px 0 0 0; font-size: 16px;'>
                                    Lease <strong>{leaseNumber}</strong> has been fully signed by both parties and is now legally active.
                                </p>
                            </div>
                            
                            <p>You can view the lease details and download the PDF at:</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{leaseDetailsUrl}' class='button'>
                                    View Lease Details
                                </a>
                            </div>
                            
                            <p style='color: #666; font-size: 14px;'>
                                Thank you for using our service!
                            </p>
                        </div>
                        
                        <div class='footer'>
                            This email was sent automatically from <strong>Property Management System</strong>.<br>
                            If you need support, please contact: support@propertymanagement.com
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}

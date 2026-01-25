namespace PropertyManagementSystem.Web.Helpers
{
    public static class EmailTemplateHelper
    {
        /// <summary>
        /// EMAIL 1: XÁC NHẬN ĐĂNG KÝ TÀI KHOẢN
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
                            <h1 style='margin: 0;'>✅ Xác nhận tài khoản</h1>
                        </div>
                        
                        <div class='content'>
                            <p>Xin chào <strong>{userName}</strong>,</p>
                            
                            <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>Property Management System</strong>!</p>
                            
                            <p>Để hoàn tất đăng ký, vui lòng nhấn vào nút bên dưới để xác nhận địa chỉ email của bạn:</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{confirmationLink}' class='button'>
                                    🔓 Xác nhận tài khoản
                                </a>
                            </div>
                            
                            <p style='color: #666; font-size: 14px;'>
                                Hoặc copy đường link sau vào trình duyệt:<br>
                                <a href='{confirmationLink}' style='color: #4CAF50; word-break: break-all;'>{confirmationLink}</a>
                            </p>
                            
                            <div class='warning'>
                                <p style='margin: 0; font-weight: bold; color: #856404;'>⚠️ Lưu ý:</p>
                                <ul style='margin: 10px 0; color: #856404;'>
                                    <li>Link xác nhận có hiệu lực trong <strong>24 giờ</strong></li>
                                    <li>Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email</li>
                                </ul>
                            </div>
                        </div>
                        
                        <div class='footer'>
                            Email này được gửi tự động từ <strong>Property Management System</strong>.<br>
                            Vui lòng không trả lời email này.
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        /// <summary>
        /// EMAIL 2: GỬI OTP KÝ HỢP ĐỒNG
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
                            <h1 style='margin: 0;'>🔐 Xác nhận ký hợp đồng</h1>
                        </div>
                        
                        <div class='content'>
                            <p>Xin chào <strong>{userName}</strong>,</p>
                            
                            <p>Bạn đang thực hiện ký hợp đồng <strong>{leaseNumber}</strong> trên hệ thống Property Management System.</p>
                            
                            <p>Mã OTP xác nhận của bạn là:</p>
                            
                            <div class='otp-box'>
                                <h1 class='otp-code'>{otp}</h1>
                            </div>
                            
                            <div class='warning'>
                                <p style='margin: 0; font-weight: bold; color: #856404;'>⚠️ Lưu ý quan trọng:</p>
                                <ul style='margin: 10px 0; color: #856404;'>
                                    <li>Mã OTP có hiệu lực trong <strong>5 phút</strong></li>
                                    <li><strong>KHÔNG</strong> chia sẻ mã này với bất kỳ ai</li>
                                    <li>Nếu bạn không thực hiện thao tác này, vui lòng bỏ qua email</li>
                                    <li>Sau khi ký, hợp đồng sẽ có hiệu lực pháp lý</li>
                                </ul>
                            </div>
                            
                            <p style='color: #666; font-size: 14px; margin-top: 30px;'>
                                <strong>Hợp đồng:</strong> {leaseNumber}<br>
                                <strong>Thời gian gửi:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                            </p>
                        </div>
                        
                        <div class='footer'>
                            Email này được gửi tự động từ <strong>Property Management System</strong>.<br>
                            Vui lòng không trả lời email này.
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        /// <summary>
        /// EMAIL 3: THÔNG BÁO HỢP ĐỒNG ĐÃ KÝ ĐẦY ĐỦ
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
                            <h1 style='margin: 0;'>🎉 Hợp đồng đã có hiệu lực</h1>
                        </div>
                        
                        <div class='content'>
                            <p>Xin chào <strong>{userName}</strong>,</p>
                            
                            <div class='success-box'>
                                <h2 style='color: #28a745; margin: 0;'>✅ Chúc mừng!</h2>
                                <p style='margin: 10px 0 0 0; font-size: 16px;'>
                                    Hợp đồng <strong>{leaseNumber}</strong> đã được ký đầy đủ bởi cả 2 bên và chính thức có hiệu lực pháp lý.
                                </p>
                            </div>
                            
                            <p>Bạn có thể xem chi tiết hợp đồng và tải xuống bản PDF tại:</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{leaseDetailsUrl}' class='button'>
                                    📄 Xem chi tiết hợp đồng
                                </a>
                            </div>
                            
                            <p style='color: #666; font-size: 14px;'>
                                Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!
                            </p>
                        </div>
                        
                        <div class='footer'>
                            Email này được gửi tự động từ <strong>Property Management System</strong>.<br>
                            Nếu cần hỗ trợ, vui lòng liên hệ: support@propertymanagement.com
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}

using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;

namespace EventPlanning.Security
{
    public class MyLibrary
    {
        public static async Task SendMail(string emailAddress, string resetCode)
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUser = "alganemre.8@gmail.com";
            string smtpPass = "";

            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress), "Email address cannot be null or empty.");
            }

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = "Password Reset Code",
                    Body = $"Your reset code is: {resetCode}",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(emailAddress);
                await client.SendMailAsync(mailMessage);
            }
        }
        public static string GenerateResetCode()
        {
            // 6 haneli rastgele bir kod oluştur
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }



        public static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password), "Password cannot be null.");
            }

            // Hash the password using SHA256 (single line)
            return Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (password == null || storedHash == null)
            {
                throw new ArgumentNullException(password == null ? nameof(password) : nameof(storedHash));
            }

            // Re-hash the password and compare (single line)
            return Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password))) == storedHash;
        }
    }
}

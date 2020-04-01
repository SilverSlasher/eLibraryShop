using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text;

namespace eLibraryShop.Services
{
    public class EmailService
    {
        public static void SendEmailConfirmation(string username, string to, string confirmationLink)
        {
            MailAddress fromMailAddress = new MailAddress("elibraryhost@gmail.com", "Krzysztof Aniskiewicz");

            MailMessage mail = new MailMessage("elibraryhost@gmail.com",to);

            mail.From = fromMailAddress;

            mail.Subject = $"{ username } właśnie utworzyłeś konto w Twojej aplikacji!";

            StringBuilder body = new StringBuilder();

            body.AppendLine("<h1> eLibraryShop - Księgarnia edukacyjna </h1>");
            body.AppendLine("<p>Twoje konto zostało pomyślnie utworzone w serwisie!</p>");
            body.AppendLine("<p>Abyś potwierdzić założenie konta oraz odblokować możliwość korzystania z serwisu kliknij w link poniżej:</p>");
            body.AppendLine($"<a href=\"{confirmationLink}\" target=\"_blank\">aktywuj konto</a>");
            body.AppendLine("<br />");
            body.AppendLine("<p> ---- </p>");

            mail.Body = body.ToString();
            mail.IsBodyHtml = true;

            mail.To.Add(to);

            SmtpClient client = new SmtpClient("smtp.gmail.com",587);

            client.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "elibraryhost@gmail.com",
                Password = "Csharpeducation2020"
            };

            client.EnableSsl = true;
            client.Send(mail);
        }

        public static void SendPasswordResetRequest(string username, string to, string passwordResetLink)
        {
            MailAddress fromMailAddress = new MailAddress("elibraryhost@gmail.com", "Krzysztof Aniskiewicz");

            MailMessage mail = new MailMessage("elibraryhost@gmail.com",to);

            mail.From = fromMailAddress;

            mail.Subject = $"{ username } właśnie otrzymaliśmy żądanie zresetowania hasła!";

            StringBuilder body = new StringBuilder();

            body.AppendLine("<h1> eLibraryShop - Księgarnia edukacyjna </h1>");
            body.AppendLine($"<p>{username}, otrzymaliśmy żądanie zresetowania Twojego hasła w serwisie!</p>");
            body.AppendLine("<p>Jeśli to Ty dokonałeś zgłoszenia kliknij w link poniżej:</p>");
            body.AppendLine($"<a href=\"{passwordResetLink}\" target=\"_blank\">zresetuj hasło</a>");
            body.AppendLine("<br />");
            body.AppendLine("<p> ---- </p>");

            mail.Body = body.ToString();
            mail.IsBodyHtml = true;

            mail.To.Add(to);

            SmtpClient client = new SmtpClient("smtp.gmail.com",587);

            client.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "elibraryhost@gmail.com",
                Password = "Csharpeducation2020"
            };

            client.EnableSsl = true;
            client.Send(mail);
        }

    }
}

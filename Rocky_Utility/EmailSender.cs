
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System.Drawing;
using System.Net.Mail;

namespace Rocky_Utility
{
    public class EmailSender : IEmailSender
    {      
        public Task SendEmailAsync(string email,string subject, string htmlMessage)
        {
            return Execute(email,subject,htmlMessage);
        }

        public async Task Execute(string email, string subject, string htmlMessage)
        {
            try
            {              
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("Моя компания", "bananjekrd@gmail.com")); //отправитель сообщения
                message.To.Add(new MailboxAddress("Client", email)); //адресат сообщения
                message.Subject = subject; //тема сообщения
                message.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 465, true); //либо использум порт 465
                    client.Authenticate("bananjekrd@gmail.com", "bbnheiwizzuamfad"); //логин-пароль от аккаунта
                    client.Send(message);

                    client.Disconnect(true);
                    
                }
            }
            catch (Exception e)
            {
               
            }
        }
    }
}

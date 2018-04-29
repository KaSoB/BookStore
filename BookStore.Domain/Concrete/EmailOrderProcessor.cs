using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BookStore.Domain.Concrete {
    public class EmailSettings {
        public string MailToAddress { get; set; } = "a@p.pl";
        public string MailFromAddress { get; set; } = "a@p.pl";
        public bool UseSSL { get; set; } = true;
        public string Username { get; set; } = "abc";
        public string Password { get; set; } = "abc";
        public string ServerName { get; set; } = "abc";
        public int ServerPort { get; set; } = 587;
        public bool WriteAsFile { get; set; } = true;
        public string FileLocation { get; set; } = @"c:\store_emails";
    }

    public class EmailOrderProcessor : IOrderProcessor {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings) {
            emailSettings = settings;
        }
        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo) {
            using (var smtpClient = new SmtpClient()) {

                smtpClient.EnableSsl = emailSettings.UseSSL;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile) {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = CreateBody(cart, shippingInfo);

                MailMessage mailMessage = new MailMessage(
                                       emailSettings.MailFromAddress,
                                       emailSettings.MailToAddress,
                                       "Otrzymano nowe zamówienie!",
                                       body.ToString());

                if (emailSettings.WriteAsFile) {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                // smtpClient.Send(mailMessage); todo:
            }
        }
        private StringBuilder CreateBody(Cart cart, ShippingDetails shippingInfo) {
            StringBuilder body = new StringBuilder()
                .AppendLine("Nowe zamówienie")
                .AppendLine("---")
                .AppendLine("Produkty:");

            foreach (var line in cart.Lines) {
                var subtotal = line.Product.Price * line.Quantity;
                body.AppendFormat("{0} x {1} (wartość: {2:c}", line.Quantity, line.Product.Name, subtotal);
            }

            body.AppendFormat("Wartość całkowita: {0:c}", cart.ComputeTotalValue())
                .AppendLine("---")
                .AppendLine("Wysyłka dla:")
                .AppendLine(shippingInfo.Name)
                .AppendLine(shippingInfo.Line1)
                .AppendLine(shippingInfo.Line2 ?? "")
                .AppendLine(shippingInfo.Line3 ?? "")
                .AppendLine(shippingInfo.City)
                .AppendLine(shippingInfo.State ?? "")
                .AppendLine(shippingInfo.Country)
                .AppendLine(shippingInfo.Zip)
                .AppendLine("---")
                .AppendFormat("Pakowanie prezentu: {0}",
                    shippingInfo.GiftWrap ? "Tak" : "Nie");
            return body;
        }
    }

}

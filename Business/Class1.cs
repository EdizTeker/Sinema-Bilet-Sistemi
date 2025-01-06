namespace BusinessLayer
{
    using DataAccessLayer;
    using System.Collections.Generic;
    using Entities;
    using iText.Kernel.Pdf;
    using iText.Layout;
    using iText.Layout.Element;
    using iText.Layout.Properties;
    using iText.IO.Font;
    using iText.Kernel.Font;
    using System.IO;
    using MailKit.Net.Smtp;
    using MimeKit;
   



    public class FilmService
    {
        private DataAccess dataAccess = new DataAccess();

        public void DeleteFilm(string filmName)
        {
            dataAccess.DeleteFilm(filmName);
        }

        public void UpdateFilm(string oldFilmName, string newFilmName)
        {
            dataAccess.UpdateFilm(oldFilmName, newFilmName);
        }

        public List<Film> GetFilmler()
        {
            return dataAccess.GetFilmler();
        }

        public List<DateOnly> GetGrafikDetay(Film film)
        {
            return dataAccess.GetGrafikDetay(film);
        }

        public bool CheckIfMusteriExists(string tcKimlik)
    {
        return dataAccess.CheckIfMusteriExists(tcKimlik);
    }
        public bool CheckTcKimlik(string tcKimlik)
        {
            return (tcKimlik.Length == 11 && tcKimlik.All(char.IsDigit)) ? true : false; //Burada tc kimliğin 11 haneli ve sadece sayılardan oluşup oluşmadığı bool şeklinde döndürülüyor.
        }
        public void AddMusteri(string tcKimlik, string musteriAdi, string musteriEposta)
        {
            dataAccess.AddMusteri(tcKimlik, musteriAdi, musteriEposta);
        }
        public void AddKoltukDurumu(int filmID, DateOnly tarih, int koltukNumarasi, string musteriID)
        {
            dataAccess.AddKoltukDurumu(filmID, tarih, koltukNumarasi, musteriID);
        }


        public int[] GetKoltukDurumlari(Film film, DateOnly tarih)
        {
            List<int> doluKoltuklar = new List<int>();
            List<KoltukDurumu> tumKoltukDurumlari = dataAccess.GetKoltuklar();

            //Filtreleme: Sadece belirtilen film ve tarih için.
            var filtrelenmisKoltuklar = tumKoltukDurumlari
                .Where(k => k.Film.FilmID == film.FilmID && k.Tarih == tarih)
                .ToList();

            foreach (var koltuk in filtrelenmisKoltuklar)
            {
                doluKoltuklar.Add(koltuk.KoltukNumarasi); //Koltuk numaralarını listeye ekliyoruz.
            }

            return doluKoltuklar.ToArray(); //Listeyi int dizisine çevirerek döndürüyoruz.
        }

    }

    public class PdfService
    {
        public static string CreatePdf(string tcKimlik, string musteriAdi, string musteriEposta, string filmAdi, string koltukNumarasi, DateOnly tarih, string zaman)
        {
            //PDF dosyasının kaydedileceği yol.
            string pdfFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Bilet.pdf");
             
            

            //PDF oluşturma işlemi.
            using (PdfWriter writer = new PdfWriter(pdfFilePath))
            {
                using (PdfDocument pdfDoc = new PdfDocument(writer))
                {
                    Document document = new Document(pdfDoc);
                    
                    string fontPath = "C:\\Windows\\Fonts\\arial.ttf"; 
                    PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                    document.SetFont(font);
                    
                    //Başlık ekler.
                    document.Add(new Paragraph("Sinema Biletiniz").SetFontSize(22));

                    //Müşteri bilgilerini ekler.
                    document.Add(new Paragraph($"Film Adı: {filmAdi}").SetFontSize(18));
                    document.Add(new Paragraph($"Koltuk Numarası: {koltukNumarasi}").SetFontSize(18));
                    document.Add(new Paragraph($"Seans Tarihi: {tarih}").SetFontSize(18));
                    document.Add(new Paragraph($"Seans Zamanı: {zaman}").SetFontSize(18));
                    document.Add(new Paragraph($"TC Kimlik: {tcKimlik}").SetFontSize(12));
                    document.Add(new Paragraph($"Müşteri Adı: {musteriAdi}").SetFontSize(12));
                    document.Add(new Paragraph($"E-posta: {musteriEposta}").SetFontSize(12));
                    

                    document.Add(new Paragraph("\n").SetFontSize(12)); //Boşluk ekler.
                    document.Add(new Paragraph("İyi seyirler dileriz!"));
                }
            }

            return pdfFilePath;
        }
    }

    public class EmailService
    {
        public void SendEmailWithAttachment(string toEmail, string subject, string bodyText, string attachmentPath)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Bilet Sistemi", "gonderen@example.com"));
            message.To.Add(new MailboxAddress("Alıcı", toEmail));
            message.Subject = subject;

            //Mesajın gövdesi.
            var body = new TextPart("plain")
            {
                Text = bodyText
            };

            //PDF eki.
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(File.OpenRead(attachmentPath)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(attachmentPath)
            };

            //Gövde ve eki birleştirir.
            var multipart = new Multipart("mixed") { body, attachment };
            message.Body = multipart;

            //SMTP üzerinden gönderir.
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("", ""); // E-posta ve şifre
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
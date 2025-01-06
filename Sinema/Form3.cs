using BusinessLayer;
using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sinema
{
    public partial class Form3 : Form
    {
        private FilmService filmService = new FilmService();
        private EmailService emailService;
        private int _koltukNo;
        private Film _film;
        public Form3(int koltukNo, Film film)
        {
            InitializeComponent();
            emailService = new EmailService();
            _koltukNo = koltukNo;
            _film = film;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //Spesifik filmin bilgilerini alır.
            label1.Text = $"Seçilen Koltuk: {_koltukNo}";
            label2.Text = $"Film Adı: {_film.FilmAdi}";
            label3.Text = $"Seans Zamanı: {_film.Zaman}";
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            //Girilen verileri alır.
            string tcKimlik = textBox3.Text;
            string musteriAdi = textBox1.Text;
            string musteriEposta = textBox2.Text;

            if (filmService.CheckTcKimlik(tcKimlik))
            {
                Form previousForm = Application.OpenForms["Form2"]; //Önceki formu kaydeder.

                //Müşteriyi kaydeder.
                filmService.AddMusteri(tcKimlik, musteriAdi, musteriEposta);

                //Film ve koltuk bilgilerini alıp kaydeder.
                int filmID = _film.FilmID;
                DateOnly tarih = DateOnly.FromDateTime(DateTime.Now); 
                int koltukNumarasi = _koltukNo; 
                string musteriID = tcKimlik;   
                filmService.AddKoltukDurumu(filmID, tarih, koltukNumarasi, musteriID);

                //PDF dosyasını kaydeder.
                string pdfPath = PdfService.CreatePdf(tcKimlik, musteriAdi, musteriEposta, _film.FilmAdi, koltukNumarasi.ToString(), tarih, _film.Zaman);

                //E-posta için bilgilerini ve PDF yolunu kaydeder.
                string aliciEmail = musteriEposta;
                string konu = "Biletiniz";
                string mesaj = "Satın aldığınız bilet ektedir. İyi günler.";
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string pdfYolu = Path.Combine(desktopPath, "Bilet.pdf");

                //E-posta adresi doğru girilmişse gönderir. Doğru girilmediyse hata verir.
                try
                {
                    
                    emailService.SendEmailWithAttachment(aliciEmail, konu, mesaj, pdfYolu);
                    DialogResult result = MessageBox.Show("Biletiniz E-posta şeklinde gönderildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK && previousForm != null) { this.Close(); previousForm.Close(); }//Açık olan iki formu kapatır.
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show($"Bilet satın alındı fakat e-posta gönderilemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (result == DialogResult.OK && previousForm != null) { this.Close(); previousForm.Close(); }//Açık olan iki formu kapatır.
                }

                // PDF'yi açmak için kullanıcıya seçenek sunun
                //if (MessageBox.Show("PDF'yi açmak ister misiniz?", "PDF Aç", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{
                //    System.Diagnostics.Process.Start("explorer.exe", pdfPath);
                //}
            }
            else MessageBox.Show("TC Kimlik numaranızı lütfen doğru giriniz"); //TC Kimlik numarası yanlış ise direkt hata verir.

        }
    }
}

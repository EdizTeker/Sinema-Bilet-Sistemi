using BusinessLayer;
using Entities;
using System;
using System.Collections.Generic;


namespace Sinema
{
    public partial class Form2 : Form
    {
        private Film _film;
        private FilmService filmService = new FilmService();
        public Form2(Film film)
        {
            InitializeComponent();
            _film = film;

        }
        int x = 10; //İlk butonun yatay konumu.
        int y = 10; //İlk butonun dikey konumu.
        private void Form2_Load(object sender, EventArgs e)
        {

            int[] bugununDoluKoltuklari = filmService.GetKoltukDurumlari(_film, DateOnly.FromDateTime(DateTime.Now));

            label1.Text = $"Film Adı: {_film.FilmAdi}";
            label2.Text = $"Film Süresi: {_film.Sure}";
            label3.Text = $"Seans Zamanı: {_film.Zaman}";


            for (int i = 1; i < 16; i++)
            {
                Button filmButton = new Button();
                filmButton.Tag = i; //Her butona bir numara ekliyoruz.

                //Eğer bugununDoluKoltuklari dizisinde o anki koltuk numarası varsa, butonu "Dolu" yapıyoruz.
                if (bugununDoluKoltuklari.Contains(i))
                {
                    filmButton.Enabled = false;
                    filmButton.Text = "Dolu";
                }
                else
                {
                    filmButton.Text = "Boş";
                }

                filmButton.Width = 120;
                filmButton.Height = 100;
                filmButton.Location = new Point(x, y);

                filmButton.Click += FilmButton_Click;

                //Panel'e butonu ekler.
                panel1.Controls.Add(filmButton);

                //Yatayda yer kalmazsa alta geçer.
                x += filmButton.Width + 10;
                if (x + filmButton.Width > panel1.Width)
                {
                    x = 10;
                    y += filmButton.Height + 10;
                }
            }

        }

        private void FilmButton_Click(object sender, EventArgs e)
        {
            //Tıklanan butonu alır.
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                //Koltuk numarasını veya sıra bilgisini alır.
                int koltukNo = (int)clickedButton.Tag;

                //Yeni bir form oluşturur ve bilgileri aktarır.
                Form3 form3 = new Form3(koltukNo, _film);
                form3.FormClosed += (s, args) => this.Enabled = true;
                form3.ShowDialog(); //Bu, yeni form açıkken diğer forma işlem yapılmasını engeller.
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.ShowDialog();
        }
    }
}

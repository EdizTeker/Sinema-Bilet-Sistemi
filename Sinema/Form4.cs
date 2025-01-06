using BusinessLayer;
using DataAccessLayer;
using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;


namespace Sinema
{
    public partial class Form4 : Form
    {


        private FilmService filmService = new FilmService();

        public Form4()
        {

            InitializeComponent();
        }



        private void Form4_Load(object sender, EventArgs e)
        {
            FilmService filmService = new FilmService();
            listBox1.Items.Clear();
            var filmler = filmService.GetFilmler();
            foreach (var film in filmler)
            {
                listBox1.Items.Add(film.FilmAdi);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedFilm = listBox1.SelectedItem.ToString();
                filmService.DeleteFilm(selectedFilm);
                MessageBox.Show("Film silindi.");

            }
            else
            {
                MessageBox.Show("Lütfen silinecek bir film seçin.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedFilm = listBox1.SelectedItem.ToString();
                string newFilmName = textBox1.Text;

                if (!string.IsNullOrEmpty(newFilmName))
                {
                    filmService.UpdateFilm(selectedFilm, newFilmName);
                    MessageBox.Show("Film güncellendi.");
                    
                }
                else
                {
                    MessageBox.Show("Yeni film adını lütfen giriniz.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellenecek bir film seçin.");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

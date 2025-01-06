namespace Sinema
{
    using BusinessLayer;
    


    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }

        private FilmService filmService = new FilmService();
        private void Form1_Load(object sender, EventArgs e)
        {
            FilmService filmService = new FilmService();
            var filmler = filmService.GetFilmler();

            int x = 10; //Ýlk butonun yatay konumu.
            int y = 10; //Ýlk butonun dikey konumu.

            foreach (var film in filmler)
            {
                // Yeni bir buton oluþturur.
                Button filmButton = new Button();
                filmButton.Text = film.FilmAdi + "\n" + film.Zaman;
                filmButton.Width = 120;
                filmButton.Height = 100;
                filmButton.Location = new Point(x, y);

                //Filmin tüm bilgilerini Tag içine ekler.
                filmButton.Tag = film;

                //Click olayýný atar.
                filmButton.Click += button1_Click_1;

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

      
       

        private void button1_Click_1(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                //Film bilgilerini alýr.
                Entities.Film film = clickedButton.Tag as Entities.Film;

                this.Enabled = false;

                //Koltuk seçimi için formu açar.
                Form2 koltukSecimForm = new Form2(film);
                koltukSecimForm.FormClosed += (s, args) => this.Enabled = true;
                koltukSecimForm.ShowDialog();
                
            }
        }
    }
}

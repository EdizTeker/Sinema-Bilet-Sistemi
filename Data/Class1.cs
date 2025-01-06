namespace DataAccessLayer
{
    using Entities;
    using System;
    using System.Data;
    using System.Windows.Forms;
    using System.Data.OleDb;
    


    public class DataAccess
    {

        //private static string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sinema.accdb");
        //private static string connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + databasePath;
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\edizt.LAPTOP-N4TQ5B3T\Desktop\Sinema\Sinema.accdb;";

        //Filmleri veritabanından çeken bir fonksiyon.
        public List<Film> GetFilmler()
        {
            List<Film> filmler = new List<Film>();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Filmler";
                    OleDbCommand command = new OleDbCommand(query, connection);
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Film film = new Film()
                        {
                            FilmID = Convert.ToInt32(reader["Kimlik"]),
                            FilmAdi = reader["FilmAdi"].ToString(),
                            Sure = reader["FilmSuresi"].ToString(),
                            Zaman = reader["SeansZamani"].ToString(),

                        };


                    filmler.Add(film);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Veri çekme hatası: " + ex.Message);
                }
            }
            return filmler;
        }

        //Bütün koltuk durumlarını veritabanından çeken bir fonksiyon.
        public List<KoltukDurumu> GetKoltuklar()
        {
            List<KoltukDurumu> durumlar = new List<KoltukDurumu>();
            List<Film> filmler = GetFilmler();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM KoltukDurumlari";
                    OleDbCommand command = new OleDbCommand(query, connection);
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int filmID = Convert.ToInt32(reader["FilmID"]); //FilmID'yi alıyoruz.

                        //FilmID ile ilgili Film nesnesini buluyoruz.
                        Film ilgiliFilm = filmler.FirstOrDefault(f => f.FilmID == filmID);                        

                        KoltukDurumu durum = new KoltukDurumu()
                        {
                            KoltukID = Convert.ToInt32(reader["Kimlik"]),
                            Film = ilgiliFilm, //Film nesnesini buraya bağlıyoruz.
                            Tarih = DateOnly.FromDateTime((DateTime)reader["Tarih"]),
                            KoltukNumarasi = Convert.ToInt32(reader["KoltukNumarasi"]),
                        };

                        durumlar.Add(durum);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Veri çekme hatası: " + ex.Message);
                }
            }
            return durumlar;
        }

        //TC kimlik numarasına göre müşterinin olup olmadığını döndüren bir fonksiyon.
        public bool CheckIfMusteriExists(string tcKimlik)
        {
            bool exists = false; //Varsayılan olarak müşteri bulunmaz kabul ediyoruz.

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    //TC Kimliği'ne göre müşteri sorgusu.
                    string query = "SELECT COUNT(*) FROM Musteri WHERE TcKimlik = @TcKimlik";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@TcKimlik", tcKimlik);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        //Eğer TC Kimliği'ne sahip bir müşteri varsa.
                        if (count > 0)
                        {
                            exists = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Veri kontrol hatası: " + ex.Message);
                }
            }

            return exists;
        }

        //Müşteri yoksa müşteriyi veritabanına ekleyen bir fonksiyon.
        public void AddMusteri(string tcKimlik, string musteriAdi, string musteriEposta)
        {
            //TC kimliği veritabanında var mı diye kontrol ediyoruz.
            if (CheckIfMusteriExists(tcKimlik))
            {
                
                return; //Eğer varsa veri eklemeyi engelliyoruz.
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    //Müşteri eklemek için SQL sorgusu.
                    string query = "INSERT INTO Musteri (TcKimlik, MusteriAdiSoyadi, MusteriEmail) VALUES (@TcKimlik, @MusteriAdiSoyadi, @MusteriEmail)";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@TcKimlik", tcKimlik);
                        command.Parameters.AddWithValue("@MusteriAdiSoyadi", musteriAdi);
                        command.Parameters.AddWithValue("@MusteriEmail", musteriEposta);

                        command.ExecuteNonQuery();
                    }

                    
                }
                catch (Exception ex)
                {
                    throw new Exception("Veri ekleme hatası: " + ex.Message);
                }
            }
        }

        //Bilet alınınca veritabanına alınan koltuk numarasını ekleyen bir fonksiyon.
        public void AddKoltukDurumu(int filmID, DateOnly tarih, int koltukNumarasi, string musteriID)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                  
                    string query = "INSERT INTO KoltukDurumlari (FilmID, Tarih, KoltukNumarasi, MusteriID) VALUES (@FilmID, @Tarih, @KoltukNumarasi, @MusteriID)";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@FilmID", filmID);
                        command.Parameters.AddWithValue("@Tarih", tarih.ToString()); 
                        command.Parameters.AddWithValue("@KoltukNumarasi", koltukNumarasi);
                        command.Parameters.AddWithValue("@MusteriID", musteriID);
                        
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Veri ekleme hatası: " + ex.Message);
                }
            }
        }

        //Çalışmayan grafik döndürme fonksiyonu.
        public List<DateOnly> GetGrafikDetay(Film film)
        {
            int filmid = film.FilmID;

            List<DateOnly> Tarihler = new List<DateOnly>();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Tarih FROM KoltukDurumlari WHERE FilmID = ?";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", filmid); 

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateOnly tarih = DateOnly.FromDateTime(reader.GetDateTime(0));
                                Tarihler.Add(tarih);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                }
            }
            return Tarihler;
        }

        public void DeleteFilm(string filmName)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("DELETE FROM Filmler WHERE FilmAdi = @FilmAdi", connection);
                command.Parameters.AddWithValue("@FilmAdi", filmName);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateFilm(string oldFilmName, string newFilmName)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("UPDATE Filmler SET FilmAdi = @NewFilmAdi WHERE FilmAdi = @OldFilmAdi", connection);
                command.Parameters.AddWithValue("@NewFilmAdi", newFilmName);
                command.Parameters.AddWithValue("@OldFilmAdi", oldFilmName);
                command.ExecuteNonQuery();
            }


        }


    }


}



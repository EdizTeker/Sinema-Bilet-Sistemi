namespace Entities
{
    public class Film
    {
        public int FilmID { get; set; }
        public string FilmAdi { get; set; }
        public string Sure { get; set; }
        public string Zaman { get; set; }
    }
    public class Musteri
    {
        public string Tcno { get; set; }
        public string MusteriAdi { get; set; }
        public string MusteriEposta { get; set; }
    }
    public class KoltukDurumu
    {
        public int KoltukID { get; set; }
        public Film Film { get; set; }
        public Musteri Musteri { get; set; }
        public DateOnly Tarih { get; set; }
        public int KoltukNumarasi { get; set; }
    }
    

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prodavnica
{
    public class Racun
    {
    
        private List<Artikal> artikli;
        private double cena;
        private DateTime datum;
        private TimeSpan vreme;
        private int id;

        public Racun() {
            artikli = new List<Artikal>();
            cena = 0.0;
            datum = new DateTime();
            vreme = datum.TimeOfDay;
            id = 0;
        }

        public Racun(int id, double cena, DateTime datum, TimeSpan vreme)
        {
            this.id = id;
            this.cena = cena;
            this.datum = datum;
            this.vreme = vreme;
        }

       

        public void izracunajCenu() {
            Cena = 0.0;
            foreach (Artikal artikal in artikli)
            {
                double kolCena = artikal.Cena * artikal.Kolicina;
                Cena += (kolCena - kolCena * artikal.Popust / 100);
            }
        }


        public DateTime Datum
        {
            get
            {
                return datum;
            }
            set
            {
                datum = value;
            }
        }


        public TimeSpan Vreme
        {
            get
            {
                return vreme;
            }
            set
            {
                vreme = value;
            }
        }

        public List<Artikal> Artikli
        {
            get
            {
                return artikli;
            }
            set
            {
                artikli = value;
            }
        }

        public double Cena
        {
            get
            {
                return cena;
            }
            set
            {
                cena = value;
            }
        }
    }
}

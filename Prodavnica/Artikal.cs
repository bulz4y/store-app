using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prodavnica
{
    public class Artikal
    {

        private string naziv;
        private double cena, popust;
        private int id;
        private int kolicina;
        private int idGrupe;
        private string slika;

        public Artikal(string naziv, double cena, double popust, int id, int idGrupe, string slika) {
            this.naziv = naziv;
            this.cena = cena;
            this.popust = popust;
            this.id = id;
            this.idGrupe = idGrupe;
            this.slika = slika;
            kolicina = 0;
        }

        public string Slika
        {
            get
            {
                return slika;
            }
            set
            {
                slika = value;
            }
        }

        public int IdGrupe
        {
            get
            {
                return idGrupe;
            }

            set
            {
                IdGrupe = value;
            }
        }

        public int Kolicina
        {
            get
            {
                return kolicina;
            }

            set
            {
                kolicina = value;
            }
        }

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public string Naziv
        {
            get
            {
                return naziv;
            }
            set
            {
                naziv = value;
            }
        }

        public double Popust
        {
            get
            {
                return popust;
            }
            set
            {
                popust = value;
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

        public override string ToString() {
            double kolCena = kolicina * cena;
            return naziv + " " + kolicina + " x " + cena.ToString("N", System.Globalization.CultureInfo.CurrentCulture) + "  " + "-" + popust + "%  " + (kolCena - kolCena * popust / 100).ToString("N", System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}

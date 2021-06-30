using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prodavnica
{
    public class Grupa
    {
        private string naziv;
        private int id;

        public Grupa(string naziv, int id) {
            this.naziv = naziv;
            this.id = id;
        }

        public int ID
        {
            get
            {
                return id;
            }
        }

        public string Naziv {
            get
            {
                return naziv;
            }
            set
            {
                naziv = value;
            }
        }

        public override string ToString()
        {
            return naziv;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Threading;

namespace Prodavnica
{
    public partial class frmPregledRacuna : Form
    {

        int rowGap = 20;
        Baza baza;
        Racun racun;
        Thread t;
        Grupa grupa;
        List<Racun> racuni;
        List<Racun> izabraniRacuni;

        public frmPregledRacuna()
        {
            InitializeComponent();
            baza = new Baza();
            racuni = new List<Racun>();
            izabraniRacuni = new List<Racun>();
            
        }

        public frmPregledRacuna(Racun racun, Grupa grupa) : this() {
            this.racun = racun;
            this.grupa = grupa;
        }

        private string  duzinaCrtica(int size) {

            int temp = size;
            Label label1 = new Label();
            label1.Font = new Font(new FontFamily("Arial Narrow"), 14.00F, FontStyle.Bold);

            label1.Text = "- ";
            temp = label1.PreferredWidth;

            while (temp < size)
            {
                label1.Text += "- ";
                temp = label1.PreferredWidth;
            }


            return label1.Text;
        }

        private void citanjeRacuna() {
            try
            {
                baza.openConnection();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Conn;
                cmd.CommandText = "Select * from Racun";

                OleDbDataReader reader = cmd.ExecuteReader();

                

                while (reader.Read())
                {
                    int id = int.Parse(reader["ID_Racuna"].ToString());
                    double cena = double.Parse(reader["Cena"].ToString());
                    DateTime datum = (DateTime)(reader["Datum"]);
                    DateTime dt = (DateTime)(reader["Vreme"]);

                    TimeSpan vreme = dt.TimeOfDay;
                    Racun racun = new Racun(id, cena, datum, vreme);

                    racuni.Add(racun);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška: " + ex.Message);
            }
            finally
            {
                baza.closeConnection();
            }
        }

        private void crtanjeRacuna(List<Racun> racuni) {

            panelRacuni.Controls.Clear();
            int gap = 15;
            int size = (panelRacuni.Width - SystemInformation.VerticalScrollBarWidth - gap * 4) / 3;
            int c = 0;
            int k = 0;


            if(racuni.Count == 0)
            {
                
                Label lbl = new Label();
                lbl.Font = new Font(new FontFamily("Microsoft Sans Serif"), 11.00F, FontStyle.Bold);
                lbl.BackColor = Color.White;
                lbl.ForeColor = Color.Black;
                lbl.Text = "Nijedan rezultat nije pronadjen";
                lbl.Width = lbl.PreferredWidth;
                lbl.Location = new Point(panelRacuni.Width / 2 - lbl.PreferredWidth / 2 , panelRacuni.Height / 2);
              
               
                panelRacuni.Controls.Add(lbl);
                
            }

            foreach (Racun racun in racuni)
            {
                // promenljive za responsive grupe


                ListBox lb = new ListBox();
                lb.SelectionMode = SelectionMode.None;

                lb.BackgroundImageLayout = ImageLayout.Stretch;
                lb.Width = size;
                lb.Height = size + 20;
                lb.Location = new Point(c * (size + gap) + gap, k * size + gap + k * rowGap);
                lb.Tag = grupa.ID;
                lb.Font = new Font(new FontFamily("Arial Narrow"), 14.00F, FontStyle.Bold);

                lb.Items.Add("Prodavnica Gomex d.o.o");
                lb.Items.Add("Starčevo 26232");

                lb.Items.Add(duzinaCrtica(size));
                lb.Items.Add(duzinaCrtica(size));

                lb.Items.Add("Cena: " + racun.Cena.ToString("N", System.Globalization.CultureInfo.CurrentCulture));

                lb.Items.Add(duzinaCrtica(size));
                lb.Items.Add(duzinaCrtica(size));

                lb.Items.Add("Datum: " + racun.Datum.ToShortDateString());
                string minuti = racun.Vreme.Minutes < 10 ? "0" + racun.Vreme.Minutes.ToString() : racun.Vreme.Minutes.ToString();
                string sati = racun.Vreme.Hours < 10 ? "0" + racun.Vreme.Hours.ToString() : racun.Vreme.Hours.ToString();
                lb.Items.Add("Vreme: " + sati + ":" + minuti);


                // predji u novi red posle 3 grupe;
                c++;
                if (c == 3)
                {
                    c = 0;
                    k++;
                }

                // dodaj grupu u panel
                panelRacuni.Controls.Add(lb);

            }
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            dtVremeDo.Format = DateTimePickerFormat.Custom;
            dtVremeDo.CustomFormat = "HH:mm";
            dtVremeDo.ShowUpDown = true;
            dtVremeOd.Format = DateTimePickerFormat.Custom;
            dtVremeOd.CustomFormat = "HH:mm";
            dtVremeOd.ShowUpDown = true;

            panelRacuni.BackColor = Color.FromArgb(20, Color.Chocolate);

            citanjeRacuna();

            crtanjeRacuna(racuni);

        }

        private void pretraga() {

        }

        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmProizvodi frm2 = new frmProizvodi(grupa, racun);
            frmPocetna frmPocetna = new frmPocetna(racun, frm2);
            frmPocetna.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if(dtDatumOd.Value.Date > dtDatumDo.Value.Date)
            {
                MessageBox.Show("Datum od mora biti manji od Datum do.");
                return;
            }

            izabraniRacuni = new List<Racun>();

          
          

            foreach (Racun racun in racuni)
            {
                TimeSpan tsDo = new TimeSpan(dtVremeDo.Value.TimeOfDay.Hours, dtVremeDo.Value.TimeOfDay.Minutes, 0);
                TimeSpan tsOd = new TimeSpan(dtVremeOd.Value.TimeOfDay.Hours, dtVremeOd.Value.TimeOfDay.Minutes, 0);



                if (tsDo >= tsOd) {
                    if ((racun.Datum.Date >= dtDatumOd.Value.Date && racun.Datum.Date <= dtDatumDo.Value.Date) && racun.Vreme >= tsOd && racun.Vreme <= tsDo)
                    {
                        // racun se poklapa sa vremenom
                        izabraniRacuni.Add(racun);
                    }
                } else
                {
                    if ((racun.Datum.Date >= dtDatumOd.Value.Date && racun.Datum.Date <= dtDatumDo.Value.Date) && (racun.Vreme >= tsOd || racun.Vreme <= tsDo))
                    {
                        // racun se poklapa sa vremenom
                        izabraniRacuni.Add(racun);
                    }
                }

               
               /* if (racun.Datum.Equals(dtDatumOd.Value.Date) && (racun.Vreme.Hours == dtVremeDo.Value.TimeOfDay.Hours) && (racun.Vreme.Minutes == dtVremeDo.Value.TimeOfDay.Minutes)) {
                    // racun se poklapa sa vremenom
                    izabraniRacuni.Add(racun);
                }*/
            }

            crtanjeRacuna(izabraniRacuni);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (izabraniRacuni.Count > 0)
            {
                sortirajRastuce(izabraniRacuni);
                crtanjeRacuna(izabraniRacuni);
            }
            else
            {
                sortirajRastuce(racuni);
                crtanjeRacuna(racuni);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            crtanjeRacuna(racuni);
        }

        private void zameni(List<Racun> racuni, int i, int j) {
            Racun temp = racuni[i];
            racuni[i] = racuni[j];
            racuni[j] = temp;
        }

        private void sortirajRastuce(List<Racun> racuni) {
            for (int i = 0; i < racuni.Count - 1; i++) {
                for (int j = i + 1; j < racuni.Count; j++) {
                    if(racuni[i].Datum > racuni[j].Datum)
                    {
                        zameni(racuni, i, j);
                    } else if(racuni[i].Datum == racuni[j].Datum)
                    {
                        if (racuni[i].Vreme.Hours > racuni[j].Vreme.Hours)
                        {
                            zameni(racuni, i, j);
                        }
                        else if (racuni[i].Vreme.Hours == racuni[j].Vreme.Hours) {

                            if (racuni[i].Vreme.Minutes > racuni[j].Vreme.Minutes)
                            {
                                zameni(racuni, i, j);
                            }
                        }
                    }
                }
            }
        }

        private void sortirajOpadajuce(List<Racun> racuni)
        {
            for (int i = 0; i < racuni.Count - 1; i++)
            {
                for (int j = i + 1; j < racuni.Count; j++)
                {
                    if (racuni[i].Datum < racuni[j].Datum)
                    {
                        zameni(racuni, i, j);
                    }
                    else if (racuni[i].Datum == racuni[j].Datum)
                    {
                        if (racuni[i].Vreme.Hours < racuni[j].Vreme.Hours)
                        {
                            zameni(racuni, i, j);
                        }
                        else if (racuni[i].Vreme.Hours == racuni[j].Vreme.Hours)
                        {

                            if (racuni[i].Vreme.Minutes < racuni[j].Vreme.Minutes)
                            {
                                zameni(racuni, i, j);
                            }
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (izabraniRacuni.Count > 0)
            {
                sortirajOpadajuce(izabraniRacuni);
                crtanjeRacuna(izabraniRacuni);
            }
            else {
                sortirajOpadajuce(racuni);
                crtanjeRacuna(racuni);
            }

        }
    }
}

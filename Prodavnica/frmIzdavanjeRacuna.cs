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

namespace Prodavnica
{
    public partial class frmIzdavanjeRacuna : Form
    {
        Baza baza;
        Racun racun;
        Grupa grupa;
        string stranica;

        public frmIzdavanjeRacuna()
        {
            InitializeComponent();
            baza = new Baza();
        }

        public frmIzdavanjeRacuna(Grupa grupa, Racun racun, string stranica) : this()
        {
            this.racun = racun;
            this.grupa = grupa;
            this.stranica = stranica;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (stranica == "pocetna")
            {
                frmProizvodi frmProizvod = new frmProizvodi(grupa, racun);
                frmPocetna frmPocetna = new frmPocetna(racun, frmProizvod);
                frmPocetna.Show();
                this.Hide();
            }
            else {
                frmProizvodi frmProizvod = new frmProizvodi(grupa, racun);
                frmProizvod.Show();
                this.Hide();
            }
          
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        int listBoxSirina()
        {

            int maxWidth = 0;
            int temp = 0;
            Label label1 = new Label();
            label1.Font = new Font(new FontFamily("Arial Narrow"), 14.00F, FontStyle.Bold);

            foreach (Artikal artikal in racun.Artikli)
            {
                label1.Text = artikal.ToString();
                temp = label1.PreferredWidth;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            label1.Dispose();
            return maxWidth + 20;
        }

        private string duzinaCrtica()
        {

            int maxWidth = listBoxSirina() - 20;
            int temp = 0;
            Label label1 = new Label();
            label1.Font = new Font(new FontFamily("Arial Narrow"), 14.00F, FontStyle.Bold);

            label1.Text = "- ";
            temp = label1.PreferredWidth;

            while (temp < maxWidth) {
                label1.Text += "- ";
                temp = label1.PreferredWidth;
            }


            return label1.Text;

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            lbRacun.Width = listBoxSirina();

            lbRacun.Items.Add("Prodavnica Gomex d.o.o");
            lbRacun.Items.Add("Starčevo 26232");

            lbRacun.Items.Add(duzinaCrtica());

            foreach (Artikal artikal in racun.Artikli) {
                lbRacun.Items.Add(artikal.ToString());
            }


            lbRacun.Items.Add(duzinaCrtica());

            lbRacun.Items.Add("Cena: " + racun.Cena);

            lbRacun.Items.Add(duzinaCrtica());

            lbRacun.SelectionMode = SelectionMode.None;
        }

        private void button2_Click(object sender, EventArgs e)
        {
                frmProizvodi frm2 = new frmProizvodi(grupa, racun);
                frmPocetna frmPocetna = new frmPocetna(racun, frm2);
                frmPocetna.Show();
                this.Hide();

        }

        private void pocetna() {
            frmPocetna frmPocetna = new frmPocetna();
            frmPocetna.Show();
            this.Hide();
        }

        private void izdajRacun() {
            try
            {
                baza.openConnection();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Conn;
                cmd.CommandText = @"insert into Racun(Cena, Datum, Vreme)
                                        values(@cena, @datum, @vreme)";

                DateTime dt = DateTime.Now;

                cmd.Parameters.AddWithValue("cena", racun.Cena);
                cmd.Parameters.AddWithValue("datum", dt.ToShortDateString());
                cmd.Parameters.AddWithValue("vreme", dt.ToShortTimeString());

                int upisano = cmd.ExecuteNonQuery();

                if (upisano > 0)
                {
                    MessageBox.Show("Račun uspešno izdat.");
                    pocetna();
                }
                else
                {
                    MessageBox.Show("Račun nije uspešno izdat.");
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message);
            }
            finally
            {
                baza.closeConnection();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Da li ste sigurni da želite da izdate racun ?", "Brisanje", MessageBoxButtons.YesNo);
            if(dr == DialogResult.Yes)
            {

                izdajRacun();
              

            
            }
        }
    }
}

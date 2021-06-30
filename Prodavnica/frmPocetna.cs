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
using System.IO;

namespace Prodavnica
{
    public partial class frmPocetna : Form
    {

        Baza baza;
        List<Grupa> grupe;
        private Racun racun;
        private Grupa izabranaGrupa;
        private DialogResult dr;
        private frmProizvodi form2;

        public frmPocetna()
        {
            InitializeComponent();

            baza = new Baza();
            grupe = new List<Grupa>();
            izabranaGrupa = new Grupa("", -1);
            this.racun = new Racun();
          
        }

        public frmPocetna(Racun racun, frmProizvodi frm) : this() {
            this.racun = racun;
            this.form2 = frm;
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


        public void dodajArtikal() {
            if (lbRacun.SelectedItem == null) { MessageBox.Show("Morate izabrati artikal sa računa."); return; }

            Artikal artikal = lbRacun.SelectedItem as Artikal;
                form2.azurirajRacun(artikal.ID, artikal.IdGrupe, artikal.Kolicina + 1, false);

                lbRacun.DataSource = null;
                lbRacun.DataSource = racun.Artikli;
                // Ispisi cenu racuna
                lblCena.Text = racun.Cena.ToString("N", System.Globalization.CultureInfo.CurrentCulture) + " RSD";
              
        }

        public void oduzmiArtikal()
        {
            if (lbRacun.SelectedItem == null) { MessageBox.Show("Morate izabrati artikal sa računa."); return; }

            Artikal artikal = lbRacun.SelectedItem as Artikal;

            if (artikal.Kolicina == 1)
            {
                dr = MessageBox.Show("Da li ste sigurni da želite da izbrisisete artikal sa računa ?", "Brisanje", MessageBoxButtons.YesNo);
                if (dr == DialogResult.No) return;
    
            }

            form2.azurirajRacun(artikal.ID, artikal.IdGrupe, artikal.Kolicina - 1, false);

            lbRacun.DataSource = null;
            lbRacun.DataSource = racun.Artikli;
            // Ispisi cenu racuna
            lblCena.Text = racun.Cena.ToString("N", System.Globalization.CultureInfo.CurrentCulture) + " RSD";

            if (lbRacun.Items.Count == 0)
            {
                btnUkloni.Enabled = false;
                btnPlus.Enabled = false;
                btnMinus.Enabled = false;
                btnIzdavanje.Enabled = false;
            }
            else
            {
                btnUkloni.Enabled = true;
                btnPlus.Enabled = true;
                btnMinus.Enabled = true;
                btnIzdavanje.Enabled = true;
            }

            
           

        }

        public void ukloniArtikalSaRacuna()
        {
            if (lbRacun.SelectedItem == null) { MessageBox.Show("Morate izabrati artikal sa računa."); return; }

            dr = MessageBox.Show("Da li ste sigurni da želite da izbrisisete artikal sa računa ?", "Brisanje", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                Artikal artikal = lbRacun.SelectedItem as Artikal;
                form2.azurirajRacun(artikal.ID, artikal.IdGrupe, 0, false);
                
                lbRacun.DataSource = null;
                lbRacun.DataSource = racun.Artikli;
                // Ispisi cenu racuna
                lblCena.Text = racun.Cena.ToString("N", System.Globalization.CultureInfo.CurrentCulture) + " RSD";

                if (lbRacun.Items.Count == 0)
                {
                    btnUkloni.Enabled = false;
                    btnPlus.Enabled = false;
                    btnMinus.Enabled = false;
                    btnIzdavanje.Enabled = false;
                }
                else
                {
                    btnUkloni.Enabled = true;
                    btnPlus.Enabled = true;
                    btnMinus.Enabled = true;
                    btnIzdavanje.Enabled = true;
                }
            }

        }

        private void citajGrupe() {
            try
            {

                baza.openConnection();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Conn;
                cmd.CommandText = "Select * From Grupa";

                OleDbDataReader reader = cmd.ExecuteReader();

                // uzmi podatke iz baze
                while (reader.Read())
                {

                    int id = int.Parse(reader["ID_Grupe"].ToString());
                    string naziv = reader["NazivGrupe"].ToString();
                    Grupa grupa = new Grupa(naziv, id);
                    grupe.Add(grupa);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                baza.closeConnection();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
    
            // promenljive za responsive grupe
            int gap = 10;
            int size = (panelGroup.Width - SystemInformation.VerticalScrollBarWidth) / 3 - gap;
            int c = 0;
            int k = 0;


            


            // ispisi racun u listu
            if (racun.Artikli.Count == 0)
            {
                lblRacun.Visible = true;
                btnUkloni.Enabled = false;
                btnPlus.Enabled = false;
                btnMinus.Enabled = false;
                btnIzdavanje.Enabled = false;
            }
            else {
                lblRacun.Visible = false;
                btnUkloni.Enabled = true;
                btnPlus.Enabled = true;
                btnMinus.Enabled = true;
                btnIzdavanje.Enabled = true;

                lbRacun.DataSource = racun.Artikli;
                lbRacun.Width = listBoxSirina();
            }

            // Ispisi cenu racuna
            lblCena.Text = racun.Cena.ToString("N", System.Globalization.CultureInfo.CurrentCulture) + " RSD";


            citajGrupe();
           



            foreach (Grupa grupa in grupe)
            {
                
                Button btn = new Button();
                Label lbl = new Label();

                // Button Grupe
                if (File.Exists("./slike/" + grupa.Naziv.ToLower() + ".jpg"))
                {
                    btn.BackgroundImage = new Bitmap("./slike/" + grupa.Naziv.ToLower() + ".jpg");
                }
                else
                {
                    btn.BackgroundImage = new Bitmap("./slike/no-image.jpg");
                }

                btn.BackgroundImageLayout = ImageLayout.Stretch;
                btn.Width = size;
                btn.Height = size;
                btn.Location = new Point(c * size + c * gap, k * size + k * gap);
                btn.Cursor = Cursors.Hand;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Tag = grupa.ID;
                btn.Click += kupovina;


                // labela na dugmetu
                string[] reci = grupa.Naziv.Split(' ');

                if (reci.Length == 1)
                {
                    lbl.Text = reci[0];
                }
                else {

                    foreach (string rec in reci)
                    {
                        lbl.Text += rec + Environment.NewLine;
                    }
                }  

                lbl.Parent = btn;
                lbl.Tag = grupa.ID;
                lbl.AutoSize = true;
                lbl.BackColor = Color.Linen;
                lbl.Font = new Font(new FontFamily("Georgia"), 16, FontStyle.Regular);
                lbl.Click += kupovina;
                lbl.Padding = new Padding(5);
                lbl.Location = new Point(btn.Width / 2 - lbl.Width / 2, btn.Height / 2 - lbl.Height / 2);
                lbl.Cursor = Cursors.Hand;

                // predji u novi red posle 3 grupe;
                c++;
                if (c == 3)
                {
                    c = 0;
                    k++;
                }

                // dodaj grupu u panel
                panelGroup.Controls.Add(btn);

            }
        }


        private void kupovina(object sender, EventArgs e) {
           
       
            if (sender is Button)
            {
                Button btn = sender as Button;
                foreach (Grupa grupa in grupe)
                {
                    if (grupa.ID == int.Parse(btn.Tag.ToString())) {
                        izabranaGrupa = grupa;
                        break;
                    }
                }
            }
            else if (sender is Label) {
                Label lbl = sender as Label;
                foreach (Grupa grupa in grupe)
                {
                    if (grupa.ID == int.Parse(lbl.Tag.ToString()))
                    {
                        izabranaGrupa = grupa;
                        break;
                    }
                }
            }
            frmProizvodi frmKupovina = new frmProizvodi(izabranaGrupa, racun);
            frmKupovina.Show();
            this.Hide();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmNoviProizvod frmProizvod = new frmNoviProizvod(racun, izabranaGrupa);
            frmProizvod.Show();
            this.Hide();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ukloniArtikalSaRacuna();
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            dodajArtikal();
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            oduzmiArtikal();
        }

        private void btnIzdavanje_Click(object sender, EventArgs e)
        {
            frmIzdavanjeRacuna frmIzdavanje = new frmIzdavanjeRacuna(izabranaGrupa, racun, "pocetna");
            frmIzdavanje.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmPregledRacuna frmRacuni = new frmPregledRacuna(racun, izabranaGrupa);
            frmRacuni.Show();
            this.Hide();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Threading;

namespace Prodavnica
{
    public partial class frmProizvodi : Form
    {
        bool update = false;
        private string search;
        private DialogResult dr;
        bool numericObradjen = false;
        Baza baza;
        Thread t;
        delegate void proizvod();
        int gap = 30;
        int rowGap = 180;
        Grupa izabranaGrupa;
        List<Artikal> artikli;
        private Racun racun;

        public frmProizvodi()
        {
            InitializeComponent();
            baza = new Baza();
            izabranaGrupa = null;
            artikli = new List<Artikal>();
            racun = new Racun();
            search = "";
            t = new Thread(proizvodUpdate);
        }

        public frmProizvodi(Grupa grupa, Racun racun) : this() {
            this.izabranaGrupa = grupa;
            this.racun = racun;
        }

        private void nadjiProizvode(int grupa) {
            try
            {

                baza.openConnection();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Conn;
                cmd.CommandText = "Select * From Artikal Where ID_Grupe = " + grupa;

                OleDbDataReader reader = cmd.ExecuteReader();
                artikli.Clear();

                // uzmi podatke iz baze
                while (reader.Read())
                {
                    int id = int.Parse(reader["ID_Artikla"].ToString());
                    string naziv = reader["NazivArtikla"].ToString();
                    double cena = double.Parse(reader["CenaArtikla"].ToString());
                    double popust = double.Parse(reader["Popust"].ToString());
                    int idGrupe = int.Parse(reader["ID_Grupe"].ToString());
                    string slika = reader["Slika"].ToString();

                    Artikal artikal = new Artikal(naziv, cena, popust, id, idGrupe, slika);
                    artikli.Add(artikal);

                }

                if(racun.Artikli.Count > 0)
                {
                    foreach(Artikal artikal in racun.Artikli)
                    {
                        foreach(Artikal ar in artikli)
                        {
                            if(ar.ID == artikal.ID)
                            {
                                ar.Kolicina = artikal.Kolicina;
                                break;
                            }
                        }
                    }
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

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }


        public void ukloniArtikalSaRacuna() {
            if (lbRacun.SelectedItem == null) { MessageBox.Show("Morate izabrati artikal sa računa."); return; }

            dr = MessageBox.Show("Da li ste sigurni da želite da izbrisisete artikal sa računa ?", "Brisanje", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes) {
                Artikal artikal = lbRacun.SelectedItem as Artikal;
                azurirajRacun(artikal.ID, artikal.IdGrupe, 0, false);
                update = true;
                azurirajPanelArtikle();
               
            }

            update = false;
        }

        public void dodajArtikal()
        {
            if (lbRacun.SelectedItem == null) { MessageBox.Show("Morate izabrati artikal sa računa."); return; }

                Artikal artikal = lbRacun.SelectedItem as Artikal;
                azurirajRacun(artikal.ID, artikal.IdGrupe, artikal.Kolicina + 1, false);
                update = true;
                azurirajPanelArtikle();

                update = false;
        }

        public void oduzmiArtikal()
        {
            if (lbRacun.SelectedItem == null) { MessageBox.Show("Morate izabrati artikal sa računa."); return; }

            Artikal artikal = lbRacun.SelectedItem as Artikal;
            if (artikal.Kolicina == 1) {
                dr = MessageBox.Show("Da li ste sigurni da želite da izbrisisete artikal sa računa ?", "Brisanje", MessageBoxButtons.YesNo);
                if (dr == DialogResult.No) return;
            }
           
               
                azurirajRacun(artikal.ID, artikal.IdGrupe, artikal.Kolicina - 1, false);
                update = true;
                azurirajPanelArtikle();
                update = false;
        }

        public void azurirajRacun(int ID,int idGrupe, int value, bool update) {

            if (update) return;

            
            nadjiProizvode(idGrupe);
            if (value > 0)
            {
                for (int i = 0; i < artikli.Count; i++)
                {
                    if (artikli[i].ID == ID)
                    {                     
                        int z = -1;
                        bool postoji = false;

                        for (int k = 0; k < racun.Artikli.Count; k++)
                        {
                            if (racun.Artikli[k].ID == ID)
                            {
                                postoji = true;
                                z = k;
                                break;
                            }
                        }

                        if (postoji && z > -1) {
                            racun.Artikli[z].Kolicina = value;
                            artikli[i].Kolicina = value;
                        } else
                        {
                            artikli[i].Kolicina = value;
                            racun.Artikli.Add(artikli[i]);
                        }

                        
                    }
                }
            }
            else
            {
                for(int i = 0; i < artikli.Count; i++)
                {
                    if(ID == artikli[i].ID)
                    {
                        for (int j = 0; j < racun.Artikli.Count; j++)
                        {
                            if (racun.Artikli[j].ID == ID)
                            {
                                artikli[i].Kolicina = value;
                                racun.Artikli.RemoveAt(j);
                                break;
                            }

                        }
                    }
                
                }
                
                      
                      
                

            }

            // Izracunaj cenu racuna
            racun.izracunajCenu();

            // Ispisi cenu racuna
            lblCena.Text = racun.Cena.ToString("N", System.Globalization.CultureInfo.CurrentCulture) + " RSD";

            // Dodaj racun u listu
            lbRacun.DataSource = null;           
            lbRacun.DataSource = racun.Artikli;

            // Proveri da li je racun prazan
            if(lbRacun.Items.Count == 0)
            {
                lbRacun.Width = 248;
                lblRacun.Visible = true;
                btnUkloni.Enabled = false;
                btnPlus.Enabled = false;
                btnMinus.Enabled = false;
                btnIzdavanje.Enabled = false;
            } else
            {
                lblRacun.Visible = false;
                btnUkloni.Enabled = true;
                btnPlus.Enabled = true;
                btnMinus.Enabled = true;
                btnIzdavanje.Enabled = true;

                lbRacun.Width = listBoxSirina();
            }
          


        }

        int listBoxSirina()
        {
            int maxWidth = 0;
            int temp = 0;
            Label label1 = new Label();
            label1.Font = new Font(new FontFamily("Arial Narrow"), 14.00F ,FontStyle.Bold);

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

        private void numericProvera(object sender, EventArgs e) {
            NumericUpDown num = sender as NumericUpDown;
            if (num.Text == "")
            {
                numericObradjen = true;
                num.Value = 0;
                num.Text = "0";
               
            }

            numericObradjen = false;
            azurirajRacun((int)num.Tag, izabranaGrupa.ID, (int)num.Value, update);
               
        }

        private void obradiProizvod(object sender, EventArgs e) {

            if (numericObradjen) return;

            NumericUpDown num = sender as NumericUpDown;

            azurirajRacun((int)num.Tag, izabranaGrupa.ID, (int)num.Value, update);

        }

        private void zaustaviWheel(object sender, MouseEventArgs e) {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void azurirajPanelArtikle() {
            foreach (Artikal artikal in artikli)
            {
                foreach (Control ctrl in panel1.Controls)

                    if (ctrl is NumericUpDown) {
                        NumericUpDown nud = ctrl as NumericUpDown;
                        if ((int)nud.Tag == artikal.ID)
                        {
                            nud.Value = artikal.Kolicina;
                            break;
                        }
                    }

               
            }
        }

        private void crtanjePanela() {
            int size = (panel1.Width - SystemInformation.VerticalScrollBarWidth - gap * 4) / 3;

            int c = 0;
            int k = 0;

            panel1.Controls.Clear();

            

            // Popuni panel sa artiklima
            for (int i = 0; i < artikli.Count; i++)
            {

              

                if (artikli[i].Naziv.ToLower().Contains(search.ToLower())) {
                    Button btn = new Button();

                    // Artikal Button
                    if (File.Exists("./slike/" + artikli[i].Slika))
                    {
                        btn.BackgroundImage = new Bitmap("./slike/" + artikli[i].Slika);
                    }
                    else
                    {
                        btn.BackgroundImage = new Bitmap("./slike/no-image.jpg");
                    }

                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.Width = size;
                    btn.Height = size;
                    //btn.Location = new Point(c * size + c * gap, k * size + k * rowGap);
                    btn.Location = new Point(c * (size + gap) + gap, k * size + gap + k * rowGap);
                    btn.Cursor = Cursors.Hand;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = new Font(new FontFamily("Georgia"), 10, FontStyle.Bold);
                    btn.Tag = artikli[i].ID;


                    // Naziv proizvoda
                    Label naziv = new Label();
                    naziv.Text = artikli[i].Naziv;
                    naziv.Width = size;
                    naziv.TextAlign = ContentAlignment.MiddleCenter;
                    naziv.Font = new Font(new FontFamily("Georgia"), 10, FontStyle.Bold);
                    naziv.BackColor = Color.Transparent;
                    naziv.Location = new Point(c * (size + gap) + gap + size / 2 / naziv.Width / 2, k * size + gap + k * rowGap + size + 10);

                    // Cena proizvoda
                    Label cena = new Label();
                    cena.Text = artikli[i].Cena.ToString("N", System.Globalization.CultureInfo.CurrentCulture) + " RSD";
                    cena.Width = size;
                    cena.TextAlign = ContentAlignment.MiddleCenter;
                    cena.Font = new Font(new FontFamily("Georgia"), 10, FontStyle.Bold);
                    cena.BackColor = Color.Transparent;
                    cena.Location = new Point(c * (size + gap) + gap + size / 2 / cena.Width / 2, k * size + gap + k * rowGap + size + 50);

                    // Popust
                    if (artikli[i].Popust > 0)
                    {
                        Label popust = new Label();
                        popust.Width = size;
                        popust.TextAlign = ContentAlignment.MiddleCenter;
                        popust.Text = "Popust " + artikli[i].Popust + "%";
                        popust.Font = new Font(new FontFamily("Georgia"), 10, FontStyle.Bold);
                        popust.BackColor = Color.Transparent;
                        popust.Location = new Point(c * (size + gap) + gap + size / 2 / popust.Width / 2, k * size + gap + k * rowGap + size + 90);
                        panel1.Controls.Add(popust);
                    }

                    //  Box za dodavanje i oduzimanje artikla
                    NumericUpDown nud = new NumericUpDown();
                    ((TextBox)nud.Controls[1]).MaxLength = 4;
                    nud.MinimumSize = new Size(size, rowGap / 2);
                    nud.Width = size;
                    nud.Height = rowGap / 2;
                    nud.Location = new Point(btn.Location.X, k * size + gap + k * rowGap + size + 130);
                    nud.Tag = artikli[i].ID;
                    nud.Maximum = 9999;
                    nud.ForeColor = Color.Black;
                    nud.TextAlign = HorizontalAlignment.Center;
                    nud.BorderStyle = BorderStyle.None;
                    nud.Font = new Font(new FontFamily("Georgia"), 11, FontStyle.Bold);
                    nud.KeyUp += numericProvera;
                    nud.MouseWheel += zaustaviWheel;
                    nud.ValueChanged += obradiProizvod;



                    // provera za novi red
                    c++;
                    if (c == 3)
                    {
                        c = 0;
                        k++;
                    }

                    panel1.Controls.Add(btn);
                    panel1.Controls.Add(nud);
                    panel1.Controls.Add(naziv);
                    panel1.Controls.Add(cena);

                }
            }

            if(panel1.Controls.Count == 0)
            {
                Label lbl = new Label();
                lbl.Font = new Font(new FontFamily("Microsoft Sans Serif"), 11.00F, FontStyle.Bold);
                lbl.BackColor = Color.White;
                lbl.ForeColor = Color.Black;
                lbl.Text = "Nijedan rezultat nije pronadjen";
                lbl.Width = lbl.PreferredWidth;
                lbl.Location = new Point(panel1.Width / 2 - lbl.PreferredWidth / 2, panel1.Height / 2);


                panel1.Controls.Add(lbl);
            } else
            {
                update = true;
                azurirajPanelArtikle();
                update = false;
            }
               

        }

        private void sortirajRastuce()
        {
            for (int i = 0; i < artikli.Count - 1; i++)
            {
                if (artikli[i].Naziv.ToLower().Contains(search.ToLower())) {
                    for (int j = i + 1; j < artikli.Count; j++)
                    {
                        if (artikli[j].Naziv.ToLower().Contains(search.ToLower())) {
                            if (artikli[i].Cena > artikli[j].Cena)
                            {
                                zameni(artikli, i, j);
                            }
                        }
                       

                    }
                }
               
            }
        }

        private void sortirajOpadajuce()
        {
            for (int i = 0; i < artikli.Count - 1; i++)
            {
                if (artikli[i].Naziv.ToLower().Contains(search.ToLower()))
                {
                    for (int j = i + 1; j < artikli.Count; j++)
                    {
                        if (artikli[j].Naziv.ToLower().Contains(search.ToLower()))
                        {
                            if (artikli[i].Cena < artikli[j].Cena)
                            {
                                zameni(artikli, i, j);
                            }
                        }


                    }
                }
            }
        }

        private void zameni(List<Artikal> artikal, int i, int j) 
        {
            Artikal temp = artikal[i];
            artikal[i] = artikal[j];
            artikal[j] = temp;
        }

        private void proizvodUpdate() {
            if(lblProizvod.InvokeRequired)
            {
                var func = new proizvod(proizvodUpdate);
                lblProizvod.Invoke(func);
            } else
            {
                lblProizvod.Text = izabranaGrupa.Naziv;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            t.Start();

            lblProizvod.Text = izabranaGrupa.Naziv;

            panel1.BackColor = Color.FromArgb(20, Color.Chocolate);

            // uzmi sve proizvode iz baze
            nadjiProizvode(izabranaGrupa.ID);

            // crtaj Panel
            crtanjePanela();
         

           

            // Ispisi trenutni racun u listbox
            if (racun.Artikli.Count == 0)
            {
                lblRacun.Visible = true;
                btnUkloni.Enabled = false;
                btnPlus.Enabled = false;
                btnMinus.Enabled = false;
                btnIzdavanje.Enabled = false;
            }
            else
            {
                // ima artikala na racunu
                update = true;

                lblRacun.Visible = false;
                btnUkloni.Enabled = true;
                btnPlus.Enabled = true;
                btnMinus.Enabled = true;
                btnIzdavanje.Enabled = true;

                azurirajPanelArtikle();
                lbRacun.DataSource = racun.Artikli;
                lbRacun.Width = listBoxSirina();
            }

            // Ispisi cenu racuna
            lblCena.Text = racun.Cena.ToString("N", System.Globalization.CultureInfo.CurrentCulture) + " RSD";

          

            update = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmPocetna frmPocetna = new frmPocetna(racun, this);
            frmPocetna.Show();
            this.Hide();
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
            frmIzdavanjeRacuna frmIzdavanje = new frmIzdavanjeRacuna(izabranaGrupa, racun, "proizvodi");
            frmIzdavanje.Show();
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            search = textBox1.Text.Trim();
            crtanjePanela();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            sortirajOpadajuce();
            crtanjePanela();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sortirajRastuce();
            crtanjePanela();
        }
    }
}

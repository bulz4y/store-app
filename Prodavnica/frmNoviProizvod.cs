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
    public partial class frmNoviProizvod : Form
    {
        Racun racun;
        Grupa izabranaGrupa;
        List<Grupa> grupe;
        Baza baza;
         double popust;
        private OpenFileDialog ofd;
        private string slika;
        public frmNoviProizvod()
        {
            InitializeComponent();
            baza = new Baza();
            grupe = new List<Grupa>();
            slika = "";
            popust = 0;
        }

        public frmNoviProizvod(Racun racun, Grupa grupa) : this() {
            this.racun = racun;
            this.izabranaGrupa = grupa;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmProizvodi frm2 = new frmProizvodi(izabranaGrupa, racun);
            frmPocetna frmPocetna = new frmPocetna(racun, frm2);
            frmPocetna.Show();
            this.Hide();
        }

        private void citajGrupe() {
            try {
                baza.openConnection();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Conn;
                cmd.CommandText = "Select * From Grupa";

                OleDbDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    string naziv = reader["NazivGrupe"].ToString();
                    int id = int.Parse(reader["ID_Grupe"].ToString());

                    Grupa grupa = new Grupa(naziv, id);
                    grupe.Add(grupa);
                }



            } catch(Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message);
            } finally
            {
                baza.closeConnection();
            }
        }

        private void popuniGrupe()
        {
            foreach (Grupa grupa in grupe)
            {
                cbGrupe.Items.Add(grupa);
            }
            cbGrupe.Text = "Izaberite Grupu: ";
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            citajGrupe();
           
            popuniGrupe();
        }


        private void upisiArtikal() {
            try
            {
                baza.openConnection();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Conn;
                cmd.CommandText = @"Insert into Artikal(NazivArtikla, CenaArtikla, Popust, ID_Grupe, Slika)
                                        Values(@naziv, @cena, @popust, @idGrupa, @slika)
                                        ";
                Grupa izabranaGrupa = cbGrupe.SelectedItem as Grupa;
                cmd.Parameters.AddWithValue("naziv", txtNaziv.Text.Trim());
                cmd.Parameters.AddWithValue("cena", double.Parse(txtCena.Text.Trim()));
                cmd.Parameters.AddWithValue("popust", popust);
                cmd.Parameters.AddWithValue("idGrupe", izabranaGrupa.ID);
                cmd.Parameters.AddWithValue("slika", slika);

                // upisi u bazu i proveri da li je baza promenjena
                int upisano = cmd.ExecuteNonQuery();

                if (upisano > 0)
                {
                    MessageBox.Show("Artikal uspešno dodat.");

                }
                else
                {
                    MessageBox.Show("Upis u bazu neuspešan.");
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

        private void ocistiPolja()
        {
            cbGrupe.SelectedIndex = -1;
            cbGrupe.Text = "Izaberite Grupu: ";

            txtCena.Text = "";
            txtNaziv.Text = "";
            txtPopust.Text = "";

            bgSlika.BackgroundImage = null;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string output = "";
            bool validno = true;
            double cena = 0;

            if(cbGrupe.SelectedItem == null) { output += "Morate izabrati grupu." + Environment.NewLine; validno = false; }
            if(txtNaziv.Text.Trim() == "") { output += "Naziv ne sme biti prazan." + Environment.NewLine; validno = false; }

            if(txtCena.Text.Trim() == "") { output += "Cena ne sme biti prazna." + Environment.NewLine; validno = false; }
            else
            {
                bool valid = double.TryParse(txtCena.Text, out cena);
                if(!valid) { output += "Cena mora biti broj" + Environment.NewLine; validno = false; }
                else if(cena <= 0) { output += "Cena mora biti veća od 0." + Environment.NewLine; validno = false; }
            }

            if(txtPopust.Text.Trim() != "")
            {
                bool popustValid = double.TryParse(txtPopust.Text, out popust);
                if (!popustValid) { output += "Popust mora biti broj." + Environment.NewLine; validno = false; }
                else if (popust >= 100) { output += "Popust mora biti manji od 100." + Environment.NewLine; validno = false; }
                else if (popust < 0) popust = 0;
            }
           


            if(validno)
            {
                //upisi u bazu
                DialogResult dr = MessageBox.Show("Da li ste sigurni da želite da dodate novi proizvod ?", "Dodavanje", MessageBoxButtons.YesNo);
                if(dr == DialogResult.Yes)
                {
                    upisiArtikal();
                    ocistiPolja();
                }
              

            } else
            {
                // validacija neuspesna
                MessageBox.Show(output);
            }

        }

        private void btnUkloni_Click(object sender, EventArgs e)
        {
            string combinedPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "./slike");

             ofd = new OpenFileDialog();
             ofd.Title = "Izaberite Sliku";
             ofd.InitialDirectory = System.IO.Path.GetFullPath(combinedPath);

            DialogResult dr =  ofd.ShowDialog();

            if (dr == DialogResult.OK) {
                // iseci samo ime fajla iz stringa
                slika = ofd.FileName.Substring(ofd.FileName.LastIndexOf("\\") + 1);

                // postavi sliku kao background
                bgSlika.BackgroundImage = new Bitmap("./slike/" + slika);
            }

            
            
        }
    }
}

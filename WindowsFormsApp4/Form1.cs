using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        private TextBox txtNotaExamenAles;
        private TextBox txtNumeExamenAles;
        public Form1()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase(); // Inițializare bază de date
            
            PopuleazaComboFacultati(); // Populăm ComboBox-ul cu facultăți
            this.txtNotaExamenAles = new TextBox();
            this.txtNumeExamenAles = new TextBox();

        }

        private void buttonAfiseazaTot_Click(object sender, EventArgs e)
        {
            AfiseazaCandidati(); // Afișează toți candidații
        }

       

        private void buttonAdaugaCandidat_Click(object sender, EventArgs e)
        {
            // Verificăm dacă toate câmpurile sunt completate corect
            if (string.IsNullOrWhiteSpace(txtNume.Text) || string.IsNullOrWhiteSpace(txtPrenume.Text) || string.IsNullOrWhiteSpace(txtDomiciliu.Text))
            {
                MessageBox.Show("Completează toate câmpurile.");
                return;
            }

            // Preluăm datele despre candidat
            string nume = txtNume.Text;
            string prenume = txtPrenume.Text;
            string domiciliu = txtDomiciliu.Text;
            DateTime dataNasterii = dateTimePicker1.Value; // Preluăm data nașterii
            

            // Verificăm dacă există o facultate selectată
            if (comboBoxFacultati.SelectedItem == null)
            {
                MessageBox.Show("Selectează o facultate.");
                return;
            }

            // Obținem facultatea selectată din ComboBox
            var facultateSelectata = comboBoxFacultati.SelectedItem as Facultate;

            if (facultateSelectata != null)
            {
                // Adăugăm candidatul și facultatea selectată în baza de date
                int candidatId = DatabaseHelper.AdaugaCandidat(nume, prenume, domiciliu, facultateSelectata.ID, dataNasterii);

                // După ce candidatul a fost adăugat, deschidem formularul pentru completarea notelor
                FormNota formNota = new FormNota(candidatId);
                formNota.ShowDialog();

                // Afișăm toți candidații după adăugare
                AfiseazaCandidati();
            }
            else
            {
                MessageBox.Show("Selectează o facultate validă.");
            }
        }

        private void AfiseazaCandidati()
        {
            // Asigurăm că DataGridView-ul este curățat de vechile date
            dataGridCandidati.Rows.Clear();
            //dataGridCandidati.DataSource = null;

            // Verificăm dacă coloanele există deja
            if (dataGridCandidati.Columns.Count == 0)
            {
                dataGridCandidati.Columns.Add("ID", "ID");
                dataGridCandidati.Columns["ID"].Width = 40;
                dataGridCandidati.Columns.Add("Nume", "Nume");
                dataGridCandidati.Columns.Add("Prenume", "Prenume");
                dataGridCandidati.Columns.Add("Domiciliu", "Domiciliu");
                dataGridCandidati.Columns.Add("Facultate", "Facultate");
                dataGridCandidati.Columns.Add("DataNasterii", "Data Nasterii"); // Coloana pentru Data Nasterii
                dataGridCandidati.Columns.Add("NotaMatematica", "Matematica");
                dataGridCandidati.Columns["NotaMatematica"].Width = 65;
                dataGridCandidati.Columns.Add("NotaRomana", "Romana");
                dataGridCandidati.Columns["NotaRomana"].Width = 54;
                dataGridCandidati.Columns.Add("NotaIstorie", "Istorie");
                dataGridCandidati.Columns["NotaIstorie"].Width = 45;
                dataGridCandidati.Columns.Add("NotaExamenAles", "Nota Examen Alegere");
                dataGridCandidati.Columns["NotaExamenAles"].Width = 70;
                dataGridCandidati.Columns.Add("NumeExamenAles", "Examen Alegere");
                dataGridCandidati.Columns.Add("MedieAdmitere", "Medie Admitere");
                dataGridCandidati.Columns["MedieAdmitere"].Width = 70;

                dataGridCandidati.Columns["NumeExamenAles"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridCandidati.Columns["Domiciliu"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            // Obținem toți candidații din baza de date
            var candidati = DatabaseHelper.GetCandidati();
            foreach (var c in candidati)
            {
                // Afișăm datele candidatului
                dataGridCandidati.Rows.Add(c.ID, c.Nume, c.Prenume, c.Domiciliu, c.Facultate, c.DataNasterii.ToString("dd/MM/yyyy"), c.NotaMatematica, c.NotaRomana, c.NotaIstorie, c.NotaExamenAles, c.NumeExamenAles, c.MedieAdmitere);
            }
        }

       
        private void PopuleazaComboFacultati()
        {
            // Populăm ComboBox-ul cu facultăți din baza de date
            comboBoxFacultati.Items.Clear();
            var facultati = DatabaseHelper.GetFacultati();
            foreach (var f in facultati)
            {
                comboBoxFacultati.Items.Add(f);
            }

            // Setează primul element ca selecționat dacă există facultăți
            if (comboBoxFacultati.Items.Count > 0) 
            {
                comboBoxFacultati.SelectedIndex = 0;
            }
        }

    }
}

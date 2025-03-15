using System;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class FormNota : Form
    {
        private int _candidatId;

        public FormNota(int candidatId)
        {
            InitializeComponent();
            this.Load += new EventHandler(FormNota_Load);
            _candidatId = candidatId;
        }
        private void FormNota_Load(object sender, EventArgs e)
        {
            // Inițializarea câmpurilor la încărcarea formei
            txtNota1.Text = "";
            txtNota2.Text = "";
            txtNota3.Text = "";

            txtNotaExamenAles.Text = "";
            txtNumeExamenAles.Text = "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Preluăm notele
            double notaMatematica = Convert.ToDouble(txtNota1.Text);
            double notaRomana = Convert.ToDouble(txtNota2.Text);
            double notaIstorie = Convert.ToDouble(txtNota3.Text);
            double notaExamenAles = Convert.ToDouble(txtNotaExamenAles.Text);
            string numeExamenAles = txtNumeExamenAles.Text;

            // Salvează notele în baza de date
            DatabaseHelper.SalveazaNote(_candidatId, notaMatematica, notaRomana, notaIstorie, notaExamenAles, numeExamenAles);

            // Închidem formularul
            this.Close();
        }

       
    }
}

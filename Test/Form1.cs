using System;
using System.IO;
using System.Windows.Forms;
using Question;
namespace Test
{
    public partial class Form1 : Form
    {
        public string FileName
        {
            get; private set;

        }
        public Form1()
        {
            InitializeComponent();
        }
        private void btnTest_Click(object sender, EventArgs e)
        {

            if (!File.Exists(txtBoxFileName.Text))
            {
                MessageBox.Show("File không tồn tại, vui lòng thử lại!");
                return;
            }
            FileName = txtBoxFileName.Text;
            Data read = new Data();
            string[] questions = read.readFile(FileName);
            string s = questions[0];
            MessageBox.Show(s);
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            if (openFileDialogQuestion.ShowDialog() == DialogResult.OK)
            {
                txtBoxFileName.Text = openFileDialogQuestion.FileName;
            }
        }

        private void txtBoxFileName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

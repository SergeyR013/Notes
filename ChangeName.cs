using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Notes
{
    public partial class ChangeName : Form
    {
        Form1 form;
        String oldName;
        OleDbConnection oleDbConnection;
        OleDbCommand command;

        public ChangeName(Form1 form, String oldName, OleDbConnection oleDbConnection, OleDbCommand command)
        {
            InitializeComponent();
            this.form = form;
            this.oldName = oldName;
            this.oleDbConnection = oleDbConnection;
            this.command = command;
        }

        private void Change_Click(object sender, EventArgs e)
        {
            try
            {
                oleDbConnection.Open();

                String noteName = sender.ToString();

                String query = "UPDATE Notes.Note SET NoteName = '" + textBox1.Text + "' WHERE NoteName = '" + oldName + "'";
                command = new OleDbCommand(query, oleDbConnection);
                command.ExecuteReader();

                oleDbConnection.Close();
                form.LoadNotes();
                Cancel_Click(sender, e);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
            finally
            {
                if (oleDbConnection.State == ConnectionState.Open) oleDbConnection.Close();
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChangeName_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.Select();
            form.Enabled = true;
        }
    }
}

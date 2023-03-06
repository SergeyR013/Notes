using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;

namespace Notes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            richTextBox1.Enabled = false;
            LoadNotes();
        }
        OleDbConnection oleDbConnection;
        string stringConnection = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=Notes.mdb";
        OleDbCommand command;
        OleDbDataReader reader;
        List<int> arrayList = new List<int>();
        

        public void LoadNotes()
        {
            arrayList.Clear();
            listBox1.Items.Clear();
            oleDbConnection = new OleDbConnection(stringConnection);

            try
            {
                oleDbConnection.Open();

                String query = "SELECT id, NoteName FROM Notes.Note";
                command = new OleDbCommand(query, oleDbConnection);
                reader = command.ExecuteReader();

                if (reader.HasRows != false)
                {

                    while (reader.Read())
                    {
                        listBox1.Items.Add(reader["NoteName"]);
                        arrayList.Add(Convert.ToInt32(reader["id"]));
                    }
                }
                oleDbConnection.Close();
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

        

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                richTextBox1.ContextMenuStrip = contextMenuStrip1;
            }
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.TextLength > 0)
            {
                richTextBox1.Copy();
            }
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.TextLength > 0)
            {
                richTextBox1.Cut();
            }
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.TextLength > 0)
            {
                richTextBox1.SelectAll();
            }
        }


        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            try
            {
                richTextBox1.Enabled = true;
                oleDbConnection.Open();

                String noteName = listBox1.SelectedItem.ToString();

                String query = "SELECT Source FROM Notes.Note WHERE NoteName = '" + noteName + "'";
                command = new OleDbCommand(query, oleDbConnection);
                reader = command.ExecuteReader();

                if (reader.HasRows != false)
                {
                    
                    reader.Read();

                    if (reader["Source"].ToString() == "")
                    {
                        richTextBox1.Text = "";
                        return;
                    } 
                    WebBrowser wb = new WebBrowser();
                    wb.Navigate("about:blank");
                    wb.Document.Write(reader["Source"].ToString());
                    wb.Document.ExecCommand("SelectAll", false, null);
                    wb.Document.ExecCommand("Copy", false, null);
                    
                    
                    richTextBox1.SelectAll();
                    richTextBox1.Paste();
                    wb.Dispose();
                }
                oleDbConnection.Close();
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

        private void изменитьИмяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите заметку!");
            }
            else
            {
                ChangeName cn = new ChangeName(this, listBox1.SelectedItem.ToString(), oleDbConnection, command);
                cn.Show();
                this.Enabled = false;
            }
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String newNote = "Новая Заметка" + (arrayList.Count + 1);
            try
            {
                oleDbConnection.Open();


                String query = "INSERT INTO Notes.Note (NoteName) values ('"+newNote+"')";
                command = new OleDbCommand(query, oleDbConnection);
                command.ExecuteReader();
                oleDbConnection.Close();
                LoadNotes();
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                richTextBox1.Text = "";

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

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите заметку!");
                return;
            }
            try
            {
                DialogResult res = MessageBox.Show("Вы точно хотите удалить заметку?", "Внимание!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Cancel) return;
                richTextBox1.Enabled = false;
                oleDbConnection.Open();

                String noteName = listBox1.SelectedItem.ToString();

                String query = "DELETE FROM Notes.Note WHERE NoteName = '"+noteName+"'";
                command = new OleDbCommand(query, oleDbConnection);
                command.ExecuteReader();
                oleDbConnection.Close();

                LoadNotes();
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

        private void сохранитьЗаметкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите заметку!");
                return;
            }
            try
            {
                oleDbConnection.Open();

                String noteName = listBox1.SelectedItem.ToString();

                String query = "UPDATE Notes.Note SET Source = '" + richTextBox1.Text + "' WHERE NoteName = '" + noteName + "'";
                command = new OleDbCommand(query, oleDbConnection);
                command.ExecuteReader();

                oleDbConnection.Close();
                MessageBox.Show("Заметка успешно сохранена!");

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
    }
}

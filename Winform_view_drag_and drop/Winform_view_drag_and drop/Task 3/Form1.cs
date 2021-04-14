using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_3
{
    public partial class Form1 : Form
    {
        string namefile = "file";
        string changefile;
        string pathfile;
        public Form1()
        {
            InitializeComponent();
            richTextBox.AllowDrop = true;
            richTextBox.DragDrop += RichTextBox_DragDrop;
            richTextBox.DragEnter += RichTextBox_DragEnter;
        }

        private void RichTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void RichTextBox_DragDrop(object sender, DragEventArgs e)
        {
            var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            changefile = paths[0];
            string text = File.ReadAllText(paths[0]);
            richTextBox.Text = text;

        }

        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "(*.txt)|*.txt"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pathfile = dialog.FileName;
                string text = File.ReadAllText(pathfile);
                changefile = dialog.FileName;
                richTextBox.Text = text;
                Text = pathfile;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (File.Exists(changefile))
            {
                using (var fs = new StreamWriter(changefile))
                {
                    fs.Write(richTextBox.Text);
                };
                MessageBox.Show("Success saved");
            }
            else
            {
                NewDoc_Click(sender, e);
            }
        }

        private void NewDoc_Click(object sender, EventArgs e)
        {
            richTextBox.Text = "";
            richTextBox.Enabled = false;

            TextBox box = new TextBox
            {
                Location = new Point(150, 25)
            };
            Label l = new Label
            {
                Location = new Point(100, 25),
                Text = "Name file"
            };
            Controls.Add(box);
            Controls.Add(l);
            box.TextChanged += (s, args) =>
            {
                namefile = box.Text;
            };
            box.KeyUp += (s, args) =>
            {
                if (!(string.IsNullOrWhiteSpace(box.Text)))
                {
                    if (args.KeyCode == Keys.Enter)
                    {
                        File.Create($"{namefile}.txt");

                        Controls.Remove(box);
                        Controls.Remove(l);
                        MessageBox.Show("Success create");

                        richTextBox.Enabled = true;
                        changefile = $"{namefile}.txt";
                    }
                }
            };

        }
        private void BtnCopy_Click(object sender, EventArgs e)
        {
            richTextBox.Copy();
        }

        private void BtnPaste_Click(object sender, EventArgs e)
        {

            richTextBox.Paste();
        }

        private void BtnCut_Click(object sender, EventArgs e)
        {
            richTextBox.Cut();
        }
        private void BtnRevoke_Click(object sender, EventArgs e)
        {
            if (richTextBox.CanUndo)
            {
                richTextBox.Undo();
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {

            FontDialog fontDialog = new FontDialog
            {
                ShowColor = true
            };
            fontDialog.Font = richTextBox.SelectionFont;
            fontDialog.Color = richTextBox.SelectionColor;

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox.SelectionFont = fontDialog.Font;
                richTextBox.SelectionColor = fontDialog.Color;
            }
        }

        private void Save_As_Click(object sender, EventArgs e)
        {
            richTextBox.Enabled = false;

            //выбор места сохранения
            string path = "";
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.SelectedPath;
            }


            TextBox box = new TextBox
            {
                Location = new Point(150, 25)
            };
            Label l = new Label
            {
                Location = new Point(100, 25),
                Text = "Name file"
            };

            Button btnOk = new Button
            {
                Location = new Point(250, 25),
                Text = "OK",
                Size = new Size(30, 20)
            };

            Controls.Add(box);
            Controls.Add(btnOk);
            Controls.Add(l);

            box.TextChanged += (s, args) =>
            {
                namefile = box.Text;
            };
            box.KeyUp += (s, args) =>
            {
                if (!(string.IsNullOrWhiteSpace(box.Text)))
                {
                    btnOk.Click += (send, arg) =>
                    {
                        string file = $"{ namefile }.txt";
                        string newfile = path + "\\" + file;


                        using (var fs = new StreamWriter(newfile))
                        {
                            fs.Write(richTextBox.Text);
                        };

                        richTextBox.Enabled = true;
                        changefile = $"{namefile}.txt";
                        Controls.Remove(box);
                        Controls.Remove(l);
                        Controls.Remove(btnOk);
                    };

                }
            };

        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();

        }
    }
}

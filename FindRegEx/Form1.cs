using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FindRegEx
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Properties.Settings sets = Properties.Settings.Default;
            textBox1.Text = sets.Path;
            textBox2.Text = sets.TextRegex;
            textBox3.Text = sets.FileRegex;
        }

        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = textBox1.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = dlg.SelectedPath;
            }
        }

        private void buttonProcess_Click(object sender, EventArgs e)
        {
            Properties.Settings sets = Properties.Settings.Default;
            sets.Path = textBox1.Text;
            sets.TextRegex = textBox2.Text;
            sets.FileRegex = textBox3.Text;
            sets.Save();

            richTextBox1.Text = "";
            ScanDirs(textBox1.Text, textBox3.Text, textBox2.Text);
        }

        private void ScanDirs(string dir, string fileFilter, string grep)
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                Match m = Regex.Match(file, fileFilter);
                if (m.Value.Length != 0)
                {
                    //Debugger.Log(0, "", "File " + file + "\n");
                    ScanFile(file, grep);
                }
            }
            string [] dirs = Directory.GetDirectories(dir);
            foreach (string dr in dirs)
            {
                //Debugger.Log(0, "", "Directory " + dr + "\n");
                ScanDirs(Path.Combine(dir, dr), fileFilter, grep);
            }
        }

        private void ScanFile(string fileName, string grep)
        {
            string[] lines = File.ReadAllLines(fileName);
            foreach (string line in lines)
            {
                Match m = Regex.Match(line, grep);
                if (m.Value.Length > 0)
                {
                    //Debugger.Log(0, "", "Line: " + m.Value + " -- ");
                    if (m.Groups.Count > 0)
                    {
                        Group g = m.Groups[1];
                        if (g.Success)
                        {
                            //                    Debugger.Log(0, "", g.ToString() + "\n");
                            richTextBox1.AppendText(g.ToString() + "\n");
                        }
                        else
                        {
                            richTextBox1.AppendText(m.Value + "\n");
                        }
                    }
                    else
                    {
                        richTextBox1.AppendText(m.Value + "\n");
                    }
                    //richTextBox1.AppendText(line + "\n");
                }
            }
        }
    }
}

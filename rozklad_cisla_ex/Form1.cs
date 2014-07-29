using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rozklad_cisla_ex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            List<int> arr = new List<int>();

            int cur;

            if (int.TryParse(textBox1.Text, out cur))
            {
                bool b = true;
                while (b)
                {
                    b = false;
                    for (int a = 2; a < cur; a++)
                    {
                        if (cur % a == 0)
                        {
                            arr.Add(a);
                            cur = cur / a;
                            b = true;
                            break;
                        }
                    }
                }
                arr.Add(cur);
                if (arr.Count <= 1)
                {
                    label1.Text = cur.ToString() + " je prvocislo";
                }
                else
                {
                    string str = "";
                    foreach (int i in arr)
                    {
                        if (str.Length > 0)
                            str += " * ";
                        str += i.ToString();
                    }
                    label1.Text = str;
                }
            }
            else
            {
                label1.Text = "Input is not a number.";
            }
        }
    }
}

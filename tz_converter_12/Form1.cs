using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using GCAL.Base;

namespace tz_converter_12
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //XmlDocument doc = new XmlDocument();
                //doc.Load(openFileDialog1.SafeFileName);

                GPTimeZoneListOld oldList = new GPTimeZoneListOld();
                oldList.getTimeZones();
                GPTimeZoneList newList = oldList.convertTimezones();

                listBox1.BeginUpdate();
                listBox1.Items.Clear();
                foreach (string s in oldList.log)
                {
                    listBox1.Items.Add(s);
                }
                listBox1.EndUpdate();

                oldList.saveXml("d:\\Temp\\Timezones-q.xml");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

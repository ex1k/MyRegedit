using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace my_regedit
{
    public partial class Form1 : Form
    {     
        RegistryKey key = Registry.CurrentUser;
        List<String> list = new List<string>();
        object temp;
        int type_status = 0; //1 - byte
        
            public Form1()
        {
            InitializeComponent();
        }
        private void init_root()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            textBox1.Clear();
            textBox2.Clear();
            listBox1.Items.Add("HKEY_CLASSES_ROOT");
            listBox1.Items.Add("HKEY_CURRENT_USER");
            listBox1.Items.Add("HKEY_LOCAL_MACHINE");
            listBox1.Items.Add("HKEY_USERS");
            listBox1.Items.Add("HKEY_CURRENT_CONFIG");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            init_root();
        }

        public void get_part(RegistryKey key)
        {
            type_status = 0;
            list.Clear();
            listBox1.Items.Clear();
            foreach (String s in key.GetSubKeyNames())
            {
                list.Add(s);
                listBox1.Items.Add(s);
            }
            listBox2.Items.Clear();
            foreach (String s in key.GetValueNames())
            {
                listBox2.Items.Add(s);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            type_status = 0;
            switch (listBox1.Text)
            {
                case "HKEY_CLASSES_ROOT":
                    key = Registry.ClassesRoot; break;
                case "HKEY_CURRENT_USER":
                    key = Registry.CurrentUser; break;
                case "HKEY_LOCAL_MACHINE":
                    key = Registry.LocalMachine; break;
                case "HKEY_USERS":
                    key = Registry.Users; break;
                case "HKEY_CURRENT_CONFIG":
                    key = Registry.CurrentConfig; break;
                default:
                    try
                    {
                        get_part(key = key.OpenSubKey(listBox1.Text, true));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    return;
            }
            get_part(key);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            temp = Registry.GetValue(key.Name, listBox2.Text, -1);
            if (temp is byte[])
            {
                textBox1.Text = BitConverter.ToString((byte[])temp);
                type_status = 1;
            }
            else if (temp is string[])
            {
                textBox1.Text = string.Join(" ", (string[])temp);
                type_status = 2;
            }
            else 
            {
                textBox1.Text = temp.ToString();
                type_status = 0;
            } 
            textBox2.Text = key.GetValueKind(listBox2.Text).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (type_status == 1)
                {
                    string[] hexSplit = textBox1.Text.Split('-');
                    byte[] input = new byte[hexSplit.Length];
                    int i = 0;
                    foreach (String hex in hexSplit)
                    {
                        input[i] = (Byte)Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                        i++;
                    }
                    key.SetValue(listBox2.Text, input, key.GetValueKind(listBox2.Text));
                }
                else if (type_status == 2)
                {
                    string[] strSplit = textBox1.Text.Split(' ');
                    key.SetValue(listBox2.Text, strSplit, key.GetValueKind(listBox2.Text));
                }
                else
                    key.SetValue(listBox2.Text, textBox1.Text, key.GetValueKind(listBox2.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

using MechanicARM.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MechanicARM
{
    public partial class ConnectionSettings : Form
    {
        public ConnectionSettings()
        {
            InitializeComponent();
        }

        private void ConnectionSettings_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.Server;
            textBox2.Text = Properties.Settings.Default.DBName;
            textBox4.Text = Properties.Settings.Default.Login;
            textBox5.Text = Properties.Settings.Default.Password;
            if (Properties.Settings.Default.Authentication)
                comboBox1.SelectedIndex = 0;
            textBox1.Select(0, 0);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 1)
            {
                textBox4.Enabled = true;
                textBox5.Enabled = true;
            }
            else
            {
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                textBox4.Text = "";
                textBox5.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Server = textBox1.Text;
            Properties.Settings.Default.Login = textBox4.Text;
            Properties.Settings.Default.DBName = textBox2.Text;
            Properties.Settings.Default.Password = textBox5.Text;
            if (comboBox1.SelectedIndex == 0)
                Properties.Settings.Default.Authentication = true;
            else
                Properties.Settings.Default.Authentication = false;

            if (BaseWorker.CheckBaseConnection())
            {
                MessageBox.Show("Подключение установлено", "Сообщение");
                Properties.Settings.Default.Save();
                this.DialogResult = DialogResult.OK;
            }
            else
                MessageBox.Show("Подключение не установлено", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            
        }
    }
}

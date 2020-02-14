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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();           
        }

        private void TryToLogin()
        {
            string login = textBox1.Text;
            string pas = textBox2.Text;
            try
            {
                if (login.Length == 0 || pas.Length == 0)
                    throw new Exception("Введите имя пользователя и пароль");
                if (BaseWorker.CheckBaseConnection())
                {
                    string[] logres = BaseWorker.TryLogin(login, pas);
                    if (Convert.ToInt32(logres[3]) > 0)
                    {
                        MessageBox.Show(logres[4], "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        this.Visible = false;
                        GlobalParam.pb_USER_ID = Convert.ToInt32(logres[0]);
                        GlobalParam.pb_USER_Name = logres[1];
                        GlobalParam.pb_USER_TYPE = Convert.ToInt32(logres[2]);
                        Main main = new Main();
                        main.ShowDialog();
                        this.Visible = true;
                    }
                }
                else
                {
                    if (MessageBox.Show("Отсутствует подключение к базе данных! Проверить параметры подключения?", "Соединение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        new ConnectionSettings().ShowDialog();
                }              
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TryToLogin();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TryToLogin();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TryToLogin();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Form Cs = new ConnectionSettings();
            Cs.Show();
        }
    }
}

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

namespace MechanicARM.Forms
{
    public partial class RegistrationUsersUpdate : Form
    {
        int id;
        bool edit = false;
        bool pswchange = false;

        public RegistrationUsersUpdate()
        {
            InitializeComponent();
            id = 0;
        }

        public RegistrationUsersUpdate(DataGridViewRow row, bool passw)
        {
            InitializeComponent();
            edit = true;
            if (passw)
            {
                edit = false;
                pswchange = true;
            }

            id = (int)row.Cells["U_ID"].Value;
            textBox1.Text = row.Cells["U_LOGIN"].Value.ToString();
            if (row.Cells["U_FAMALY"].Value != null)
                textBox2.Text = row.Cells["U_FAMALY"].Value.ToString();
            if (row.Cells["U_NAME"].Value != null)
                textBox3.Text = row.Cells["U_NAME"].Value.ToString();
            if (row.Cells["U_ACCESS_ID"].Value != null)
            {
                int access = (int)row.Cells["U_ACCESS_ID"].Value;
                if (access < 2)
                    comboBox1.SelectedIndex = 0;
                if (access == 2)
                    comboBox1.SelectedIndex = 1;
                if (access == 3)
                    comboBox1.SelectedIndex = 2;
            }
            if (row.Cells["U_KEY_ACTIVE"].Value != null)
            {
                bool act = (bool)row.Cells["U_KEY_ACTIVE"].Value;
                if (!act)
                    checkBox1.Checked = true;
            }
            textBox2.Select(0, 0);
        }

        private void RegistrationUsersUpdate_Load(object sender, EventArgs e)
        {
            this.Text = "Создание пользователя";
            if (edit)
            {
                this.Text = "Редактирование пользователя";
                textBox1.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                label6.Visible = false;
                label5.Visible = false;
                this.Size = new Size(this.Size.Width, 283);
            }
            if (pswchange)
            {
                this.Text = "Смена пароля";
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                comboBox1.Enabled = false;
                checkBox1.Enabled = false;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string lc_U_Login = textBox1.Text.Trim();
            string lc_U_FAMALY = textBox2.Text.Trim();
            string lc_U_NAME = textBox3.Text.Trim();
            int lc_U_ACCESS_ID = 1;
            if (comboBox1.Text == "Администратор")
                lc_U_ACCESS_ID = 3;
            else if (comboBox1.Text == "Оператор")
                lc_U_ACCESS_ID = 2;
            else if (comboBox1.Text == "Гость" || comboBox1.Text == "")
                lc_U_ACCESS_ID = 1;
            int lc_U_KEY_Active = 1;
            if (checkBox1.Checked)
                lc_U_KEY_Active = 0;

            string psw1 = textBox4.Text.Trim();
            string psw2 = textBox5.Text.Trim();

            try
            {
                if (lc_U_Login.Length == 0 || lc_U_FAMALY.Length == 0 || lc_U_NAME.Length == 0)
                    throw new Exception("Не заполнены основные поля");
                if (!edit)
                {
                    if (psw1.Length == 0 || psw2.Length == 0)
                        throw new Exception("Не указан пароль");
                    if (psw1 != psw2)
                        throw new Exception("Пароль не совпадает");
                }
                string[] res = null;
                if (id == 0)
                    res = BaseWorker.InsUpd_User(lc_U_Login, lc_U_FAMALY, lc_U_NAME, lc_U_ACCESS_ID, lc_U_KEY_Active, psw1, id, 1);
                if (edit)
                    res = BaseWorker.InsUpd_User(lc_U_Login, lc_U_FAMALY, lc_U_NAME, lc_U_ACCESS_ID, lc_U_KEY_Active, psw1, id, 2);
                if (pswchange)
                    res = BaseWorker.InsUpd_User(lc_U_Login, lc_U_FAMALY, lc_U_NAME, lc_U_ACCESS_ID, lc_U_KEY_Active, psw1, id, 3);

                if (res != null)
                {
                    if (Convert.ToInt32(res[1]) > 0)
                        throw new Exception(res[2]);
                    Tag = res[0];
                    this.DialogResult = DialogResult.OK;
                }
                else
                    throw new Exception("Не удалось выполнить операцию");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

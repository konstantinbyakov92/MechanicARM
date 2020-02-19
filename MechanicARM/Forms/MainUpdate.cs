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
    public partial class MainUpdate : Form
    {
        int tzaid;
        int type;

        public MainUpdate()
        {
            InitializeComponent();
            type = 1;
        }

        public MainUpdate(DataGridViewRow row)
        {
            InitializeComponent();
            Text = "Редактирование данных";
            tzaid = (int)row.Cells["Id"].Value;
            if (row.Cells["StatusTZA"].Value != null)
                comboBox1.Text = row.Cells["StatusTZA"].Value.ToString();
            if (row.Cells["TZAName"].Value != null)
                textBox2.Text = row.Cells["TZAName"].Value.ToString();
            type = 0;
        }

        private void MainUpdate_Load(object sender, EventArgs e)
        {
            if (type == 1)
            {
                this.Text = "Добавление данных";
            }
            else
                this.Text = "Редактирование данных";
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string lc_status_tza = comboBox1.Text.Trim();
            try
            {
                string[] res = BaseWorker.UpdateTZA(lc_status_tza, tzaid, type);
                if (res != null)
                {
                    if (Convert.ToInt32(res[1]) > 0)
                        throw new Exception(res[2]);
                    Tag = res[0];
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

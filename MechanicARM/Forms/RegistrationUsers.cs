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
    public partial class RegistrationUsers : Form
    {
        public RegistrationUsers()
        {
            InitializeComponent();
        }

        private void RegistrationUsers_Load(object sender, EventArgs e)
        {
            if (GlobalParam.pb_USER_TYPE == 0 || GlobalParam.pb_USER_TYPE == 1)
            {
                добавитьToolStripMenuItem.Enabled = false;
                редактироватьToolStripMenuItem.Enabled = false;
                удалитьToolStripMenuItem.Enabled = false;
                изменитьПарольToolStripMenuItem.Enabled = false;
            }

            BaseWorker.LoadUsers(dataGridView1);
        }

        private void RegistrationUsers_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Tag != null)
                Main.forms[(int)Tag] = null;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedCells.Count > 0)
                {
                    int currentRow = dataGridView1.SelectedCells[0].RowIndex;
                    object pb_U_KEY_ACTIVE = dataGridView1.Rows[currentRow].Cells[7].Value;
                    object pb_U_ID = dataGridView1.Rows[currentRow].Cells[5].Value;

                    if (pb_U_KEY_ACTIVE != null && (Convert.ToBoolean(pb_U_KEY_ACTIVE) == false))
                        удалитьToolStripMenuItem.Enabled = true;
                    else
                        удалитьToolStripMenuItem.Enabled = false;
                    if (pb_U_ID != null && (int)pb_U_ID > 1)
                        редактироватьToolStripMenuItem.Enabled = true;
                    else if (pb_U_ID != null && (int)pb_U_ID <= 1)
                        редактироватьToolStripMenuItem.Enabled = false;
                    if (pb_U_ID != null && (int)pb_U_ID > 0)
                        изменитьПарольToolStripMenuItem.Enabled = true;
                    else if (pb_U_ID != null && (int)pb_U_ID <= 0)
                        изменитьПарольToolStripMenuItem.Enabled = false;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void ДобавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrationUsersUpdate f = new RegistrationUsersUpdate();
            //Если новая запись была успешно добавлена
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //перезагружаем грид
                BaseWorker.LoadUsers(dataGridView1);
            }
        }

        private void РедактироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Если выделена строка
            if (dataGridView1.SelectedCells.Count > 0)
            {
                //Получаем значения в текущей строке
                int currentRow = dataGridView1.SelectedCells[0].RowIndex;
                RegistrationUsersUpdate f = new RegistrationUsersUpdate(dataGridView1.Rows[currentRow], false);
                //Если запись была успешно отредактирована
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //перезагружаем грид
                    BaseWorker.LoadUsers(dataGridView1);
                    //встаем на эту запись
                    dataGridView1.ClearSelection();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["U_ID"].Value.ToString() == dataGridView1.Rows[currentRow].Cells["U_ID"].Value.ToString())
                        {
                            dataGridView1.Rows[row.Index].Selected = true;
                            dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                        }
                    }
                }
            }
            else
                MessageBox.Show("Вы не выделили строку!");
        }

        private void ИзменитьПарольToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                //Получаем значения в текущей строке
                int currentRow = dataGridView1.SelectedCells[0].RowIndex;
                RegistrationUsersUpdate f = new RegistrationUsersUpdate(dataGridView1.Rows[currentRow], true);
                //Если запись была успешно отредактирована
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //перезагружаем грид
                    BaseWorker.LoadUsers(dataGridView1);
                    //встаем на эту запись
                    dataGridView1.ClearSelection();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["U_ID"].Value.ToString() == dataGridView1.Rows[currentRow].Cells["U_ID"].Value.ToString())
                        {
                            dataGridView1.Rows[row.Index].Selected = true;
                            dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                        }
                    }
                }
            }
            else
                MessageBox.Show("Вы не выделили строку!");
        }

        private void УдалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                try
                {
                    //получаем ид выделенного пользователя
                    int currentRow = dataGridView1.SelectedCells[0].RowIndex;
                    int id = Convert.ToInt32(dataGridView1.Rows[currentRow].Cells["U_ID"].Value);
                    if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Предупреждение", MessageBoxButtons.OKCancel) != DialogResult.Cancel)
                    {
                        string[] res = BaseWorker.Delete_User(id);
                        if (res != null)
                        {
                            if (Convert.ToInt32(res[1]) > 0)
                                throw new Exception(res[2]);
                            //удаляем выделенную строку из таблицы на форме
                            dataGridView1.Rows.Remove(dataGridView1.Rows[currentRow]);
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

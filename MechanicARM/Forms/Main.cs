using MechanicARM.Forms;
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
    public partial class Main : Form
    {
        private bool needSelect;
        public DataGridViewRow selectedRow;
        public static Form[] forms;
        int i;

        public Main()
        {
            InitializeComponent();
            forms = new Form[5];
            op();
        }

        public Main(bool p)
        {
            InitializeComponent();
            needSelect = p;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            label1.Text = "Текущий пользователь: " + GlobalParam.pb_USER_Name;
            if (GlobalParam.pb_USER_TYPE == 0 || GlobalParam.pb_USER_TYPE == 1 || GlobalParam.pb_USER_TYPE == 2)

            {
                //регистрация пользователей доступна только создателю и администратору
                регистрацияToolStripMenuItem1.Visible = false;         
            }
            BaseWorker.LoadTZA(dataGridView1);
        }

        private void StartForm(int i, string formName)
        {
            //проверяем подключение к БД
            if (BaseWorker.CheckBaseConnection())
            {
                //Если нужная форма еще не открыта
                if (forms[i] == null)
                {
                    Type t = Type.GetType(formName, false, true);
                    forms[i] = (Form)Activator.CreateInstance(t);
                    forms[i].Show();
                    forms[i].Tag = i;
                }
                else
                {
                    //если форма уже открыта - то делаем ее активной
                    forms[i].Activate();
                }
            }
            else
                MessageBox.Show("Отсутствует подключение к БД");
        }

        private void ВыходИзПрограммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void регистрацияToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StartForm(1, "MechanicARM.Forms.RegistrationUsers");
        }

        private void настройкаПодключенияToolStripMenuItem_Click(object sender, EventArgs e)
        {          
            new ConnectionSettings().ShowDialog();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //если форма была открыта для выбора
            if (needSelect)
            {
                if (e.RowIndex != -1)
                {
                    if (dataGridView1.SelectedCells.Count > 0)
                    {
                        //Получаем значения в текущей строке
                        int currentRow = dataGridView1.SelectedCells[0].RowIndex;
                        selectedRow = dataGridView1.Rows[currentRow];
                        //Закрываем форму
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    else
                        MessageBox.Show("Вы не выделили строку!");
                }
            }
        }

        //private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    MainUpdate f = new MainUpdate();
        //    f.Tag = 1;
        //    //если новая запись была успешно добавлена
        //    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        //перезагружаем грид
        //        BaseWorker.LoadTZA(dataGridView1);
        //        //встаем на новую запись
        //        dataGridView1.ClearSelection();
        //        foreach (DataGridViewRow row in dataGridView1.Rows)
        //        {
        //            if (row.Cells["Id"].Value.ToString() == f.Tag.ToString())
        //            {
        //                dataGridView1.Rows[row.Index].Selected = true;
        //                dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
        //            }
        //        }
        //    }
        //}


        private void редактироватьtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Если выделена строка
            if (dataGridView1.SelectedCells.Count > 0)
            {
                //Получаем значения в текущей строке
                int currentRow = dataGridView1.SelectedCells[0].RowIndex;
                MainUpdate f = new MainUpdate(dataGridView1.Rows[currentRow]);
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    BaseWorker.LoadTZA(dataGridView1);
                    //пробуем выделить отредактированную
                    dataGridView1.ClearSelection();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["Id1C"].Value.ToString() == f.Tag.ToString())
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

        private void удалитьtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                try
                {
                    //получаем ид выделенного продукта
                    int currentRow = dataGridView1.SelectedCells[0].RowIndex;
                    int id = Convert.ToInt32(dataGridView1.Rows[currentRow].Cells["Id1C"].Value);
                    if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Предупреждение", MessageBoxButtons.OKCancel) != DialogResult.Cancel)
                    {
                        string[] res = BaseWorker.DeleteTZA(id);
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

        private void ЗавершитьСеансToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "Обновление таблицы через: " + (--i).ToString() + "с";
            if (i < 0)
            {
                BaseWorker.LoadTZA(dataGridView1);
                timer1.Stop();
                op();
            }
        }

        void op()
        {
            i = 30;
            label2.Text = "Обновление таблицы через: " + i.ToString();
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
        }
    }
}

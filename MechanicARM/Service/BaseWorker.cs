using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MechanicARM.Service
{
    class BaseWorker
    {
        public static bool CheckBaseConnection()
        {
            bool result = false;
            try
            {
                SqlConnection conn = GetConnection();
                conn.Open();
                conn.Close();
                result = true;
            }
            catch (Exception exc)
            {
                result = false;
            }
            return result;
        }

        public static SqlConnection GetConnection()
        {
            string server = Properties.Settings.Default.Server;
            string user = Properties.Settings.Default.Login;
            string pass = Properties.Settings.Default.Password;
            bool authentic = Convert.ToBoolean(Properties.Settings.Default.Authentication);
            string dbname = Properties.Settings.Default.DBName;

            string connection = "Data Source=" + server + ";Initial Catalog=" + dbname + ";";
            connection += "User ID=" + user + ";Password=" + pass + ";";
            connection += "Integrated Security=" + authentic.ToString() + ";";

            return new SqlConnection(connection);
        }

        public static string[] TryLogin(string log, string pas)
        {
            if (CheckBaseConnection())
            {
                SqlConnection conn = GetConnection();
                try
                {
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "EXEC dbo.Proc_USER_REGISTRATION @U_LOGIN = '" + log + "', @U_PSW = '" + pas + "'";
                    conn.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        string[] res = new string[5];
                        res[0] = sdr.GetInt32(0).ToString();
                        res[1] = sdr.GetString(1);
                        res[2] = sdr.GetInt32(2).ToString();
                        res[3] = sdr.GetInt32(3).ToString();
                        res[4] = sdr.GetString(4);
                        return res;
                    }
                    conn.Close();
                    return null;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                throw new Exception("Отсутствует подключение к базе данных");
            return null;
        }

        private static string[] GetResultOfExec(string command)
        {
            //проверяем соединение
            if (CheckBaseConnection())
            {
                //получаем подключение
                SqlConnection conn = GetConnection();
                try
                {
                    //формируем команду
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = command;
                    //открываем соединение
                    conn.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        string[] res = new string[3];
                        res[0] = sdr.GetString(0).ToString();
                        res[1] = sdr.GetInt32(1).ToString();
                        res[2] = sdr.GetString(2).ToString();
                        return res;
                    }
                    //закрываем соединение
                    conn.Close();
                    return null;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                throw new Exception("Отсутствует подключение к БД");
            return null;
        }

        private static void SetSource(DataGridView dgv, SqlCommand cmd)
        {
            dgv.Columns.Clear();
            DataTable data = null;
            dgv.DataSource = null;
            SqlDataAdapter dataAdapter = null;
            data = new DataTable();
            dataAdapter = new SqlDataAdapter(cmd);
            dataAdapter.Fill(data);
            dgv.DataSource = data;
        }

        public static void LoadUsers(DataGridView dgv)
        {
            SqlConnection conn = BaseWorker.GetConnection();
            try
            {
                //формируем запрос на выборку данных 
                string sqlQueryString;
                sqlQueryString = "SELECT U_LOGIN, U_FAMALY, U_NAME, AT_NAME, U_ACTIVE, U_ID, U_ACCESS_ID, U_KEY_ACTIVE FROM dbo.V_USERS";
                SqlCommand cmd = new SqlCommand(sqlQueryString, conn);
                //Выполнение запроса. Результат заносится в таблицу
                SetSource(dgv, cmd);
                //Настраиваем визуальное представление
                dgv.AllowUserToAddRows = false;
                dgv.ReadOnly = true;
                dgv.Columns[0].HeaderText = "Login";
                dgv.Columns[1].HeaderText = "Фамилия";
                dgv.Columns[2].HeaderText = "Имя";
                dgv.Columns[3].HeaderText = "Тип";
                dgv.Columns[4].HeaderText = "Активный";
                dgv.Columns[5].Visible = false;
                dgv.Columns[6].Visible = false;
                dgv.Columns[7].Visible = false;
                for (int i = 0; i < 5; i++)
                    dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            //При обнаружении ошибки - закрываем моединение с БД и выводим сообщение
            catch (Exception exc)
            {
                conn.Close();
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Добавление - редактирование пользователя
        internal static string[] InsUpd_User(string lc_U_Login, string lc_U_FAMALY, string lc_U_NAME, int lc_U_ACCESS_ID, int lc_U_KEY_Active, string psw, int id, int optype)
        {
            if (optype == 1)
                return GetResultOfExec("EXEC dbo.Proc_Ins_USER @U_LOGIN = '" + lc_U_Login + "', @U_NAME = '" + lc_U_NAME + "', @U_FAMALY = '" + lc_U_FAMALY +
                   "', @U_ACCESS_ID = " + lc_U_ACCESS_ID.ToString() + ", @U_PASSWORD = '" + psw + "', @USER_ID = " + GlobalParam.pb_USER_ID);
            else if (optype == 2)
                return GetResultOfExec("EXEC dbo.Proc_Upd_USER @U_ID = " + id + ", @U_LOGIN = '" + lc_U_Login + "', @U_NAME = '" + lc_U_NAME + "', @U_FAMALY = '" + lc_U_FAMALY +
                   "', @U_ACCESS_ID = " + lc_U_ACCESS_ID.ToString() + ", @U_KEY_ACTIVE = " + lc_U_KEY_Active + ", @USER_ID = " + GlobalParam.pb_USER_ID);
            else
                return GetResultOfExec("EXEC dbo.Proc_Upd_Psw_USER @U_ID = " + id + ", @U_PASSWORD = '" + psw + "', @USER_ID = " + GlobalParam.pb_USER_ID);
        }

        //удаление пользователя
        internal static string[] Delete_User(int id)
        {
            return GetResultOfExec("EXEC dbo.Proc_Del_USER @USER_ID = " + id + ", @U_ID = " + GlobalParam.pb_USER_ID.ToString());
        }

        //загрузка ТЗА
        public static void LoadTZA(DataGridView dgv)
        {
            SqlConnection conn = BaseWorker.GetConnection();
            try
            {
                //формируем запрос на выборку данных
                string sqlQueryString;
                sqlQueryString = "SELECT Id, Id1C, TZAName, GarageNumber, Type, LastUpdateTime, StatusTZA from TZA";
                SqlCommand cmd = new SqlCommand(sqlQueryString, conn);
                //выполнение запроса, результат заносится в таблицу
                SetSource(dgv, cmd);
                //настраиваем визуальное представление
                dgv.AllowUserToAddRows = false;
                dgv.ReadOnly = true;
                dgv.Columns[0].HeaderText = "Идентификатор";
                dgv.Columns[1].HeaderText = "Порядковый номер 1С";
                dgv.Columns[2].HeaderText = "Наименование ТЗА";
                dgv.Columns[3].HeaderText = "Гаражный номер";
                dgv.Columns[4].HeaderText = "Тип ТЗА";
                dgv.Columns[5].Visible = false;
                dgv.Columns[6].HeaderText = "Статус ТЗА";               
                dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;              
            }
            // при обнаружении ошибки - закрываем соединение с БД и выводим сообщение
            catch (Exception exc)
            {
                conn.Close();
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Редактирование статуса ТЗА
        public static string[] UpdateTZA(string lc_status_tza, string tzaid, int optype)
        {       
            if (optype == 1)
                return GetResultOfExec("EXEC dbo.Proc_Upd_TZA @StatusTZA = '" + lc_status_tza.Trim() + "', @U_ID = " + GlobalParam.pb_USER_ID.ToString());
            else
                return GetResultOfExec("EXEC dbo.Proc_Upd_TZA @StatusTZA = '" + lc_status_tza.Trim() + "', @Id1C = '" + tzaid.Trim() + "', @U_ID = " + GlobalParam.pb_USER_ID.ToString());
        }

        //Удаление статуса ТЗА
        public static string[] DeleteTZA(int id)
        {
            return GetResultOfExec("EXEC dbo.Proc_Del_TZA @Id1C = " + id.ToString() + ", @U_ID = " + GlobalParam.pb_USER_ID.ToString());
        }
    }
}

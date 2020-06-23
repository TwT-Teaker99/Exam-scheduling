using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExamScheduling
{
    public partial class Form1 : Form
    {
        ToMau tb = new ToMau();
        DataTable table = new DataTable();
        FormChooseDB form = new FormChooseDB();
        public Form1()
        {
            InitializeComponent();
        }
        private void DBAccess()
        {
            //form.readDatathroughAdapter("exec P1", table);

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = "exec P1";
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Connection = FormChooseDB.connection;
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            // DataTable dt = new DataTable();
            adapter.Fill(table);
            table.Columns.Add(new DataColumn("selected", typeof(bool)));
            foreach (DataRow row in table.Rows)
            {
                row["selected"] = false;
            }

            dataGridView1.DataSource = table;
        }
        private List<HocPhan> ToMau()
        {
            List<HocPhan> HP = new List<HocPhan>();
            int n = table.Rows.Count;
            HocPhan hp0 = new HocPhan();
            HP.Add(hp0);
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                if ((bool)table.Rows[i]["Selected"])
                {
                    HocPhan hp = new HocPhan(table.Rows[i]["MaMon"].ToString());
                    string s = table.Rows[i]["MaSV"].ToString().TrimEnd(',');
                    string[] arrS = s.Split(',');
                    int j = 0;
                    foreach (string arr in arrS)
                    {
                        hp.addSV(Convert.ToInt32(arr));
                        j++;
                    }
                    HP.Add(hp);
                    count++;
                }
                else
                {
                    continue;
                }
            }

            int[,] H = new int[n + 1, n + 1];
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    H[i, j] = 0;
                }
            }

            tb.convertDS1(count, HP, H);
            int[] Bac = new int[100];
            tb.BacDinh(H, count, Bac);
            int[] Dinh = new int[100];
            tb.MangDinh(count, Dinh);
            tb.SortDinh(Dinh, Bac, count);
            tb.Tomau1(H, Dinh, count, HP);
            label1.Text = "";
            for (int i = 1; i <= count; i++)
            {
                label1.Text = label1.Text + HP[i].show();
            }

            return HP;
        }
        private DataTable loadGrid(List<HocPhan> HP)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("MaMon", typeof(string)));
                dt.Columns.Add(new DataColumn("NgayThi", typeof(string)));
                dt.Columns.Add(new DataColumn("Phong", typeof(int)));
                DateTime Date = dateTimePicker1.Value;
                for (int i = 1; i <= tb.Mau; i++)
                {
                    List<HocPhan> HPtemp = new List<HocPhan>();
                    foreach (HocPhan hp in HP)
                    {
                        if (hp.Color == i)
                        {
                            HPtemp.Add(hp);
                        }
                    }
                    int k = 0;
                    foreach (HocPhan hptemp in HPtemp)
                    {
                        if (k < Convert.ToInt32(tbSLPhong.Text))
                        {
                            k++;
                            dt.Rows.Add(hptemp.MaHP, Date.ToString("dd/MM/yyyy"), k);
                        }
                        else
                        {
                            Date = Date.AddDays(1);
                            dt.Rows.Add(hptemp.MaHP, Date.ToString("dd/MM/yyyy"), k);
                            k = 0;
                        }

                    }
                    Date = Date.AddDays(1);
                }

                //dataGridView2.DataSource = dt;
                return dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString(), "Thông báo");
                throw err;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            DBAccess();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<HocPhan> hp = ToMau();

            if (tbSLPhong.Text == "")
            {
                MessageBox.Show("Nhập số lượng phòng", "Thông Báo");
                tbSLPhong.Focus();
            }
            else
            {
                //loadGrid(hp);
                Form2 f2 = new Form2(loadGrid(hp));
                f2.ShowDialog();
            }


        }



        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in table.Rows)
            {
                row["Selected"] = row["Selected"] == null ? false : !(bool)row["Selected"];
            }
        }

        private void textBox1_TextChanged(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                string rowFilter = string.Format("[{0}] like '%{1}%'", "TenMon", textBox1.Text);
                rowFilter += string.Format("OR [{0}] like '%{1}%'", "MaMon", textBox1.Text);
                table.DefaultView.RowFilter = rowFilter;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExamScheduling
{
    public partial class Form1 : Form
    {
        ToMau tb = new ToMau();
        DataTable table = new DataTable();
        List<HocPhan> HP;
        public Form1()
        {
            InitializeComponent();
        }
        private void DBAccess()
        {
            //string con = @"Data Source=ADMIN\SQLEXPRESS;Initial Catalog=LICHTHI;Integrated Security=True";
            string con ="Data Source = DESKTOP-VES4POV\\MSSQLSERVER03;Database =LICHTHI; Integrated Security=SSPI;";
        SqlConnection Con = new SqlConnection(con);
            SqlDataAdapter adapter = new SqlDataAdapter("exec P1", con);
            adapter.Fill(table);
            dataGridView1.DataSource = table;


        }
        private void ToMau()
        {
            HP = new List<HocPhan>();
            int n = table.Rows.Count;
            HocPhan hp0 = new HocPhan();
            HP.Add(hp0);
            for (int i = 0; i < n; i++)
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
            }

            int[,] H = new int[n + 1, n + 1];
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    H[i, j] = 0;
                }
            }

            tb.convertDS1(n, HP, H);
            int[] Bac = new int[10];
            tb.BacDinh(H, n, Bac);
            int[] Dinh = new int[10];
            tb.MangDinh(n, Dinh);
            tb.SortDinh(Dinh, Bac, n);
            tb.Tomau1(H, Dinh, n, HP);
            label1.Text = "";
            for (int i = 1; i <= n; i++)
            {
                label1.Text = label1.Text + HP[i].show();
            }

        }
        private void loadGrid()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("MaMon", typeof(string)));
            dt.Columns.Add(new DataColumn("NgayThi", typeof(DateTime)));
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
                    }
                    else
                    {
                        Date = Date.AddDays(1);
                        k++;
                    }
                    dt.Rows.Add(hptemp.MaHP, Date, k);
                }
                Date = Date.AddDays(1);
            }
            dataGridView2.DataSource = dt;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            DBAccess();

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ToMau();
            loadGrid();
        }
    }
}
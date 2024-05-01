﻿using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ProjectCompList : Form
    {
        private readonly string conn = @"datasource=127.0.0.1;port=3306;username=root;password=;database=napelem";
        private readonly string query = "SELECT * FROM projektraktar";
        
        public ProjectCompList()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) => GetProjCompList();

        private DataTable GetProjCompList()
        {
            DataTable dt = new DataTable();
            using(var con = new MySqlConnection(conn))
            {
                using(var cmd = new MySqlCommand(query, con))
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    dt.Load(dr);
                }
            }

            return dt;
        }
    }
}
﻿using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class Login : Form
    {
        // Idő alapú kizárás

        private int loginFail = 0;
        private DateTime lockoutEnd;

        // Segédfüggvények

        bool AuthUser(string user, string pwd)
        {
            bool yes = false;

            string hashPWD = HashPWD(pwd);
            string conn = @"datasource=127.0.0.1;port=3306;username=root;password=;database=napelem";
            string query = "SELECT Jelszo, Beosztas FROM felhasznalok WHERE Felhasznalonev = @Felhasznalonev";

            try
            {
                using (var con = new MySqlConnection(conn))
                {
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Felhasznalonev", user);
                        con.Open();
                        var dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            string storedHashPWD = dr["Jelszo"].ToString();
                            string mastery = dr["Beosztas"].ToString();

                            if (hashPWD.Equals(storedHashPWD))
                            {
                                switch (mastery)
                                {
                                    case "raktárvezető":
                                        Form1 f1 = new Form1(); f1.Show(); break;
                                    case "raktáros":
                                        Form2 f2 = new Form2(); f2.Show(); break;
                                    case "szakember":
                                        Form3 f3 = new Form3(); f3.Show(); break;
                                    case "admin":
                                        Admin a = new Admin(); a.Show(); break;
                                    default:
                                        MessageBox.Show("A felhasználó nem található az adatbázisban!"); break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Hiba a kapcsolat létesítésekor: {e}");
                Environment.Exit(1);
            }
            return yes;
        }

        private string HashPWD(string pwd)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        // Form-hoz kapcsolódó dolgok

        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DateTime.Now < lockoutEnd)
            {
                MessageBox.Show($"Jelenleg ki van zárva! Kérem próbálja újra {lockoutEnd - DateTime.Now} múlva!");
                return;
            }

            string user = textBox1.Text;
            string pwd = textBox2.Text;

            if (AuthUser(user, pwd))
            {
                loginFail = 0;
                Hide();
            }
            else
            {
                loginFail++;
                if (loginFail >= 3)
                {
                    lockoutEnd = DateTime.Now.AddMinutes(5);
                    MessageBox.Show("Maximális bejelentkezési próbálkozások száma elérve! Ki van zárva 5 percre!");
                }
                else MessageBox.Show("Helytelen bejelentkezési adat(ok)!");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

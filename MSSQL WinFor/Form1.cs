using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace MSSQL_WinFor
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;

        private SqlConnection NorNorthwndConnection = null;

        private List<string[]> rows = new List<string[]>();

        private List<string[]> filteredList = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);

            sqlConnection.Open();

            NorNorthwndConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwndDB"].ConnectionString);

            NorNorthwndConnection.Open();


            SqlDataReader dReader = null;
            string[] row = null;

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT ProductName, QuantityPerUnit, UnitPrice FROM Products",
                    NorNorthwndConnection);

                dReader = sqlCommand.ExecuteReader();

                while (dReader.Read())
                {
                    row = new string[]
                    {
                        Convert.ToString(dReader["ProductName"]),
                        Convert.ToString(dReader["QuantityPerUnit"]),
                        Convert.ToString(dReader["UnitPrice"])
                    };
                    rows.Add(row);
                }

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                if (dReader != null && !dReader.IsClosed)
                {
                    dReader.Close();
                }

            }
            RefreshList(rows);

            //-----------------------

            SqlDataAdapter datAdapter = new SqlDataAdapter("SELECT * FROM Products", NorNorthwndConnection);

            DataSet db = new DataSet();

            datAdapter.Fill(db);

            dataGridView2.DataSource = db.Tables[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SqlCommand command = new SqlCommand("INSERT INTO [Students](Name, Surname, Birthday) VALUES(N'Иван', N'Петров', '3/6/2000')", sqlConnection);

            //SqlCommand command = new SqlCommand($"INSERT INTO [Students](Name, Surname, Birthday) VALUES(N'{textBox1.Text}', N'{textBox2.Text}', '{textBox3.Text}')", sqlConnection);

            SqlCommand command = new SqlCommand($"INSERT INTO [Students](Name, Surname, Birthday, Mesto_rozheniya, Phone, Email) VALUES(@Name, @Surname, @Birthday, @Mesto_rozheniya, @Phone, @Email)", sqlConnection);

            DateTime date = DateTime.Parse(textBox3.Text);

            command.Parameters.AddWithValue("Name", textBox1.Text);
            command.Parameters.AddWithValue("Surname", textBox2.Text);
            command.Parameters.AddWithValue("Birthday", textBox3.Text);
            command.Parameters.AddWithValue("Mesto_rozheniya", $"{date.Month}/{date.Day}/{date.Year}");
            command.Parameters.AddWithValue("Phone", textBox5.Text);
            command.Parameters.AddWithValue("Email", textBox6.Text);

            //command.ExecuteNonQuery();

            MessageBox.Show(command.ExecuteNonQuery().ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter
                (/*"SELECT * FROM Products WHERE UnitPrice > 100"*/ textBox7.Text, 
                NorNorthwndConnection);

            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            dataGridView1.DataSource = dataSet.Tables[0];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            SqlDataReader dataReader = null;

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT ProductName, QuantityPerUnit, UnitPrice FROM Products" ,
                    NorNorthwndConnection);

                dataReader = sqlCommand.ExecuteReader();

                ListViewItem item = null;

                while (dataReader.Read())
                {
                    item = new ListViewItem(new string[] { Convert.ToString(dataReader ["ProductName"]), 
                        Convert.ToString(dataReader["QuantityPerUnit"]),
                        Convert.ToString(dataReader["UnitPrice"]) });
                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (dataReader != null && !dataReader.IsClosed)
                {
                    dataReader.Close();
                }
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"ProductName LIKE '%{textBox8.Text}%'";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"UnitsInStock <= 10";
                    break;

                case 1:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"UnitsInStock >= 10 AND UnitsInStock <= 50";
                    break;

                case 2:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"UnitsInStock >=50";
                    break;


                case 3:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = "";
                    break;

            }
        }

        private void RefreshList(List<string[]> list)
        {
            listView2.Items.Clear();

            foreach (string[] s in list)
            {
                listView2.Items.Add(new ListViewItem(s));
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            filteredList = rows.Where((x) => 
            x[0].ToLower().Contains(textBox9.Text.ToLower())).ToList();

            RefreshList(filteredList);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    filteredList = rows.Where((x) =>
                    Double.Parse(x[2]) <= 10).ToList();
                    RefreshList(filteredList);
                    break;

                case 1:
                    filteredList = rows.Where((x) =>
                    Double.Parse(x[2]) > 10 && Double.Parse(x[2]) <= 100).ToList();
                    RefreshList(filteredList);
                    break;

                case 2:
                    filteredList = rows.Where((x) =>
                    Double.Parse(x[2]) > 100).ToList();
                    RefreshList(filteredList);
                    break;

                case 3:
                    RefreshList(rows);
                    break;

            }
        }
    }
}

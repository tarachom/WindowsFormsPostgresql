using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Npgsql;

namespace WindowsFormsPostgresql
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

		private NpgsqlConnection Connection { get; set; }

		private BindingList<Records> RecordsBindingList { get; set; }

		//Підключення до сервера PostgreSQL
		private void Open(string connectionString)
		{
			Connection = new NpgsqlConnection(connectionString);
			Connection.Open();
		}

		private class Records
		{
			public Records(string _id, string _name)
			{
				ID = _id;
				Name = _name;
			}
			public string ID { get; set; }
			public string Name { get; set; }
		}

		//Завантаження даних
		private void LoadRecords()
        {
			RecordsBindingList.Clear();

			string query = "SELECT * FROM public.tab1 ORDER BY id ASC";

			NpgsqlCommand nCommand = new NpgsqlCommand(query, Connection);

			NpgsqlDataReader reader = nCommand.ExecuteReader();

			while (reader.Read())
			{
				RecordsBindingList.Add(new Records(
					reader["id"].ToString(),
					reader["name"].ToString()
					));
			}

			reader.Close();
		}

        private void Form1_Load(object sender, EventArgs e)
        {
			RecordsBindingList = new BindingList<Records>();
			dataGridViewRecords.DataSource = RecordsBindingList;

			dataGridViewRecords.Columns["Name"].Width = 300;

			//Підключення до сервера PostgreSQL
			Open($"Server=localhost;User Id=postgres;Password=1;Port=5433;Database=test;");

			//Заватаження даних з таблиці
			LoadRecords();
		}

		//Новий запис
        private void button1_Click(object sender, EventArgs e)
        {
			string Name = new Random().Next(int.MaxValue).ToString();

			string query = $"INSERT INTO public.tab1(name) VALUES('Назва {Name}')";

			NpgsqlCommand nCommand = new NpgsqlCommand(query, Connection);
			nCommand.ExecuteNonQuery();

			//Заватаження даних з таблиці
			LoadRecords();
		}

		//Обновлення таблиці
        private void button2_Click(object sender, EventArgs e)
        {
			//Заватаження даних з таблиці
			LoadRecords();
		}

		//Видалення рядочків
        private void button3_Click(object sender, EventArgs e)
        {
			if (dataGridViewRecords.SelectedRows.Count != 0 &&
				MessageBox.Show("Видалити записи?", "Повідомлення", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				string query = "DELETE FROM public.tab1 WHERE id = @id";

				NpgsqlCommand nCommand = new NpgsqlCommand(query, Connection);
				
				for (int i = 0; i < dataGridViewRecords.SelectedRows.Count; i++)
				{
					DataGridViewRow row = dataGridViewRecords.SelectedRows[i];
					string id = row.Cells[0].Value.ToString();

					if (!nCommand.Parameters.Contains("id"))
						nCommand.Parameters.Add(new NpgsqlParameter("id", int.Parse(id)));
					else
						nCommand.Parameters["id"].Value = int.Parse(id);

					nCommand.ExecuteNonQuery();
				}

				LoadRecords();
			}
		}


    }
}

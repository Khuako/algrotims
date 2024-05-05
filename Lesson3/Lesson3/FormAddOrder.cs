using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lesson3
{
    public partial class FormAddOrder : Form
    {
        public NpgsqlConnection conn;
        DataTable clients = new DataTable();
        DataSet dsClients = new DataSet();
        DataTable dtProducts = new DataTable();
        DataSet dsProducts = new DataSet();
        int quantity = 0;
        int productPrice = 0;
        public int id = -1;
        public int amount = 0;
        public string clientName;
        public string productName;
        public FormAddOrder(NpgsqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
            this.StartPosition = FormStartPosition.CenterScreen;
            LoadClients();
            LoadProducts();
        }
        public FormAddOrder(NpgsqlConnection conn, int id,int quantity, string clientName, string productName, int amount)
        {
            InitializeComponent();
            this.id = id;
            this.productName = productName;
            this.clientName = clientName;
            textBoxQuantity.Text = quantity.ToString();
          
            this.conn = conn;
            this.StartPosition = FormStartPosition.CenterScreen;
            LoadClients(); 
            LoadProducts();
            labelPrice.Text = amount.ToString();
        }
        private void LoadClients()
        {
            String sqlClients = "Select * from Client";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sqlClients, conn);
            dsClients.Reset();
            da.Fill(dsClients);
            clients = dsClients.Tables[0];
            var clientList = dsClients.Tables[0].AsEnumerable().Select(row => new
            {
                Id = row.Field<int>("client_id"),
                Name = row.Field<string>("client_name")
            }).ToList();


            comboBoxUsers.DataSource = clientList;
            comboBoxUsers.DisplayMember = "Name";
            comboBoxUsers.ValueMember = "Id";

        }

        private void LoadProducts()
        {
            String sqlClients = "Select * from Product";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sqlClients, conn);
            dsProducts.Reset();
            da.Fill(dsProducts);
            var products = dsProducts.Tables[0];
            var productList = dsProducts.Tables[0].AsEnumerable().Select(row => new
            {
                Id = row.Field<int>("product_id"),
                Name = row.Field<string>("product_name"),
                Price = row.Field<int>("product_price")
            }).ToList();


            comboBoxProducts.DataSource = productList;
            comboBoxProducts.DisplayMember = "Name";
            comboBoxProducts.ValueMember = "Id";
        }
        private void labelProductPrice_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxProducts.SelectedItem != null)
            {
                var selectedProduct = (dynamic)comboBoxProducts.SelectedItem;
                productPrice = selectedProduct.Price;
                labelPrice.Text = productPrice.ToString(); 
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonOrderCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxQuantity_TextChanged(object sender, EventArgs e)

        {
            double parsedValue;
            if (!double.TryParse(textBoxQuantity.Text, out parsedValue))
            {
                textBoxQuantity.Text = "";
            }
            else
            {
                quantity = int.Parse(textBoxQuantity.Text);
                labelPrice.Text = (quantity * productPrice).ToString();
            }
        }

        private void buttonOrderYes_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.id == -1)
                {
                    int clientId = (int)comboBoxUsers.SelectedValue;
                    int productId = (int)comboBoxProducts.SelectedValue;
                    quantity = int.Parse(textBoxQuantity.Text);
                    int totalAmount = quantity * productPrice;

                    string sql = @"INSERT INTO Orders (order_date, client_id, product_id, quantity, total_amount) 
                        VALUES (@orderDate, @clientId, @productId, @quantity, @totalAmount)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        cmd.Parameters.AddWithValue("@productId", productId);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@totalAmount", totalAmount);
                        cmd.ExecuteNonQuery();
                    }

                    Close();
                }
                else
                {
                    int clientId = (int)comboBoxUsers.SelectedValue;
                    int productId = (int)comboBoxProducts.SelectedValue;
                    quantity = int.Parse(textBoxQuantity.Text);
                    int totalAmount = quantity * productPrice;

                    string sql = @"UPDATE Orders SET client_id = @clientId, product_id = @product_id, quantity = @quantity, total_amount = @totalAmount WHERE order_id = @id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("id", this.id);
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        cmd.Parameters.AddWithValue("@productId", productId);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@totalAmount", totalAmount);
                        cmd.ExecuteNonQuery();
                    }

                    Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lesson3
{
    public partial class FormOrders : Form
    {
        public NpgsqlConnection conn;
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();

        public FormOrders(NpgsqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
            this.StartPosition = FormStartPosition.CenterScreen;
            Update();
        }
        private void Update()
        {
            string sql = @"SELECT Orders.order_id, Orders.order_date, Client.client_name, 
                      Product.product_name, Orders.quantity, Orders.total_amount
               FROM Orders
               INNER JOIN Client ON Orders.client_id = Client.client_id
               INNER JOIN Product ON Orders.product_id = Product.product_id";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridViewOrders.DataSource = dt;
            dataGridViewOrders.Columns["order_id"].HeaderText = "Номер";
            dataGridViewOrders.Columns["order_date"].HeaderText = "Дата";
            dataGridViewOrders.Columns["client_name"].HeaderText = "Клиент";
            dataGridViewOrders.Columns["product_name"].HeaderText = "Товар";
            dataGridViewOrders.Columns["quantity"].HeaderText = "Количество";
            dataGridViewOrders.Columns["total_amount"].HeaderText = "Сумма";
        }
        public void ExportOrdersToExcel(string filePath)
        {
            string sql = @"SELECT Orders.order_id, Orders.order_date, Client.client_name, 
                      Product.product_name, Orders.quantity, Orders.total_amount
               FROM Orders
               INNER JOIN Client ON Orders.client_id = Client.client_id
               INNER JOIN Product ON Orders.product_id = Product.product_id";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            var orders = dt.AsEnumerable().Select(row => new
            {
                order_id = row.Field<int>("order_id"),
                order_date = row.Field<DateTime>("order_date"),
                client_name = row.Field<string>("client_name"),
                product_name = row.Field<string>("product_name"),
                quantity = row.Field<int>("quantity"),
                total_amount = row.Field<int>("total_amount")
            }).ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Заказы");

                worksheet.Cell(1, 1).Value = "Номер заказа";
                worksheet.Cell(1, 2).Value = "Дата заказа";
                worksheet.Cell(1, 3).Value = "Клиент";
                worksheet.Cell(1, 4).Value = "Товар";
                worksheet.Cell(1, 5).Value = "Количество";
                worksheet.Cell(1, 6).Value = "Цена";
                int row = 2;
                foreach (var order in orders)
                {
                    worksheet.Cell(row, 1).Value = order.order_id;
                    worksheet.Cell(row, 2).Value = order.order_date.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 3).Value = order.client_name;
                    worksheet.Cell(row, 4).Value = order.product_name;
                    worksheet.Cell(row, 5).Value = order.quantity;
                    worksheet.Cell(row, 6).Value = order.total_amount;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
                Update();
            }
        }
        private void buttonCreateOrder_Click(object sender, EventArgs e)
        {
            FormAddOrder formAddOrder = new FormAddOrder(conn);
            formAddOrder.ShowDialog();
            Update();
        }

        private void buttonProductChange_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridViewOrders.CurrentRow.Cells["order_id"].Value;
            string clientName = (string)dataGridViewOrders.CurrentRow.Cells["client_name"].Value.ToString();
            string productName = (string)dataGridViewOrders.CurrentRow.Cells["product_name"].Value.ToString();
            int quantity = (int)dataGridViewOrders.CurrentRow.Cells["quantity"].Value;
            int totalAmount = (int)dataGridViewOrders.CurrentRow.Cells["total_amount"].Value;
            FormAddOrder formAddOrder = new FormAddOrder(conn, id, quantity, clientName,productName,totalAmount );
            formAddOrder.ShowDialog();
            Update();
        }

        private void buttonProductDel_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dataGridViewOrders.CurrentRow.Cells["order_id"].Value;
                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Orders WHERE order_id = :order_id", conn);
                command.Parameters.AddWithValue("order_id", id);
                command.ExecuteNonQuery();
                Update();
            }
            catch (Exception ex) { }
        }

        private void buttonExportExcel_Click(object sender, EventArgs e)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;

            string fileName = "Orders.xlsx";
            string filePath = Path.Combine(rootPath, fileName);

            ExportOrdersToExcel(filePath);
        }
    }
}

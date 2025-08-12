using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptGraficaTopProductosMasVendidos : Form
    {
        public FrmRptGraficaTopProductosMasVendidos()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaTopProductosMasVendidos_Load(object sender, EventArgs e)
        {
            LlenarComboBox1();
        }

        private void LlenarComboBox1()
        {
            List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
            for (int i = 10; i <= 50; i += 5) 
                items.Add(new KeyValuePair<string, int>($"{i} productos", i));
            comboBox1.DisplayMember = "Key";
            comboBox1.ValueMember = "Value";
            comboBox1.DataSource = items;
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTopProductos(Convert.ToInt32(comboBox1.SelectedValue));
        }

        private void CargarTopProductos(int topProductos)
        {
            groupBox1.Text = $"» Reporte gráfico top {topProductos} productos más vendidos «";
            DataTable dt = GetTopProductos(topProductos);
            if (dt != null)
            {
                reportViewer1.LocalReport.DataSources.Clear();
                var rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.SetParameters(new ReportParameter("NumProductos", comboBox1.SelectedValue.ToString()));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", $"Top {comboBox1.SelectedValue.ToString()} productos más vendidos"));
                reportViewer1.RefreshReport();
            }
        }

        private DataTable GetTopProductos(int topProductos)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    var productosVendidos = from od in context.Order_Details
                                            join p in context.Products on od.ProductID equals p.ProductID
                                            group od by p.ProductName into g
                                            select new
                                            {
                                                NombreProducto = g.Key,
                                                CantidadVendida = g.Sum(x => x.Quantity)
                                            };
                    var resultado = productosVendidos
                                    .OrderByDescending(x => x.CantidadVendida)
                                    .Take(topProductos)
                                    .AsEnumerable() // cambia a LINQ to Objects
                                    .Select((x, idx) => new
                                    {
                                        Posicion = idx + 1,
                                        NombreProducto = $"{idx + 1}. {x.NombreProducto}",
                                        CantidadVendida = x.CantidadVendida
                                    })
                                    .ToList();
                    dt.Columns.Add("Posicion", typeof(int));
                    dt.Columns.Add("NombreProducto", typeof(string));
                    dt.Columns.Add("CantidadVendida", typeof(int));
                    foreach (var item in resultado)
                        dt.Rows.Add(item.Posicion, item.NombreProducto, item.CantidadVendida);
                }
            }
            catch (SqlException ex)
            {
                Utils.MsgCatchOueclbdd(ex);
            }
            catch (Exception ex) 
            {
                Utils.MsgCatchOue(ex);
            }
            MDIPrincipal.ActualizarBarraDeEstado();
            return dt;
        }
    }
}

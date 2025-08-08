using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmGraficaTopProductosMasVendidos : Form
    {
        public FrmGraficaTopProductosMasVendidos()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaTopProductosMasVendidos_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
        }

        private void LlenarComboBox()
        {
            var items = new List<KeyValuePair<string, int>>();
            for (int i = 10; i <= 30; i += 5)
                items.Add(new KeyValuePair<string, int>($"{i} Productos", i));
            comboBox1.DataSource = items;
            comboBox1.DisplayMember = "Key";
            comboBox1.ValueMember = "Value";
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTopProductos(((KeyValuePair<string, int>)comboBox1.SelectedItem).Value);
        }

        private void CargarTopProductos(int cantidad)
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();

            Title titulo = new Title()
            {
                Text = $"Top {cantidad} productos más vendidos",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Alignment = ContentAlignment.TopCenter
            };
            chart1.Titles.Add(titulo);
            groupBox1.Text = $"» {titulo.Text} «";
            var datos = ObtenerTopProductos(cantidad);
            var serie = chart1.Series.Add("Productos más vendidos");
            serie.ChartType = SeriesChartType.Column;
            serie.IsValueShownAsLabel = true;
            serie.Label = "#VALY{n0}";
            serie.BorderWidth = 2;
            serie.ToolTip = "Producto: #VALX, Cantidad Vendida: #VALY{n0}";
            serie.Font = new Font("Arial", 10, FontStyle.Bold);
            serie.Points.Clear();
            // Paleta de 10 colores (ajusta a tu gusto)
            Color[] paleta = {
                Color.SteelBlue, Color.Orange, Color.MediumSeaGreen,
                Color.Goldenrod, Color.Crimson, Color.MediumPurple,
                Color.Tomato, Color.Teal, Color.SlateGray, Color.DeepPink
            };
            int idx = 0;
            foreach (DataRow row in datos.Rows)
            {
                string nombreProducto = (idx + 1).ToString() + ".- " + row["NombreProducto"];
                int cantidadVendida = Convert.ToInt32(row["CantidadVendida"]);
                serie.Points.AddXY(nombreProducto, cantidadVendida);
                serie.Points.Last().Color = paleta[idx % paleta.Length];
                idx++;
            }

            chart1.Legends.Clear();
            var area = chart1.ChartAreas[0];

            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 30; 
            area.Area3DStyle.Rotation = 20; 
            area.Area3DStyle.LightStyle = LightStyle.Realistic;

            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Title = "Productos más vendidos";
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.Black;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.LabelStyle.Angle = -45;
            area.AxisY.LabelStyle.Font = new Font("Arial", 8, FontStyle.Regular);
            area.AxisY.Title = "Cantidad vendida (unidades)";
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.LineColor = Color.Black;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.LineColor = Color.Black;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;
        }

        private DataTable ObtenerTopProductos(int cantidad)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            try
            {

                using (var context = new NorthwindTradersDataContext())
                {
                    // Sentencia usando las propiedades de navegación
                    var topProductos = context.Products
                        .Select(p => new
                        {
                            NombreProducto = p.ProductName,
                            CantidadVendida = p.Order_Details.Sum(od => od.Quantity)
                        })
                        .OrderByDescending(x => x.CantidadVendida)
                        .Take(cantidad)
                        .ToList();
                    // Sentencia equivalente a la anterior usando join
                    //var topProductos = (
                    //                    from od in context.Order_Details
                    //                    join p in context.Products on od.ProductID equals p.ProductID
                    //                    group od by p.ProductName into g
                    //                    select new
                    //                    {
                    //                        NombreProducto = g.Key,
                    //                        CantidadVendida = g.Sum(x => x.Quantity)
                    //                    }
                    //                    )
                    //                    .OrderByDescending(x => x.CantidadVendida)
                    //                    .Take(cantidad)
                    //                    .ToList();

                    // Convertir a DataTable
                    dt.Columns.Add("NombreProducto", typeof(string));
                    dt.Columns.Add("CantidadVendida", typeof(int));
                    foreach (var item in topProductos)
                    {
                        dt.Rows.Add(item.NombreProducto, item.CantidadVendida);
                    }
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

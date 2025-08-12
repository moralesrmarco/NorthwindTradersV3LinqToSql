using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmGraficaDeVentasDeVendedoresPorAnio : Form
    {
        public FrmGraficaDeVentasDeVendedoresPorAnio()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaDeVentasDeVendedoresPorAnio_Load(object sender, EventArgs e)
        {
            LlenarComboBox1();
        }

        private void LlenarComboBox1()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    var years = context.Orders
                        .Where(o => o.OrderDate != null)
                        .Select(o => o.OrderDate.Value.Year)
                        .Distinct()
                        .OrderByDescending(y => y);
                    foreach (var year in years)
                    {
                        comboBox1.Items.Add(year);
                    }
                }
                comboBox1.SelectedIndex = 0;
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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarVentasPorVendedores(Convert.ToInt32(comboBox1.SelectedItem.ToString()));
        }

        private void CargarVentasPorVendedores(int anio)
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();
            chart1.Legends.Clear();

            var dt = ObtenerDatos(anio);

            var leyenda = new Legend("Vendedores")
            {
                Title = "Vendedores",
                TitleFont = new Font("Arial", 10, FontStyle.Bold),
                Docking = Docking.Right,
                LegendStyle = LegendStyle.Table
            };
            chart1.Legends.Add(leyenda);
            Title titulo = new Title
            {
                Text = $"Gráfica de ventas por vendedores del año {anio}",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Alignment = ContentAlignment.TopCenter
            };
            chart1.Titles.Add(titulo);
            groupBox1.Text = $"» {titulo.Text} «";
            Series serie = new Series()
            {
                Name = "Ventas",
                //Color = Color.FromArgb(0, 51, 102),
                IsValueShownAsLabel = true,
                ChartType = SeriesChartType.Doughnut,
                Label = "#AXISLABEL: #VALY{C2}",
                ToolTip = "Vendedor: #AXISLABEL\nTotal Ventas: #VALY{C2}",
                Legend = leyenda.Name,
                LegendText = "#AXISLABEL: #VALY{C2}"
            };
            serie["PieLabelStyle"] = "Outside";
            serie["PieDrawingStyle"] = "Cylinder";
            serie["DoughnutRadius"] = "60";
            chart1.Series.Add(serie);
            var area = chart1.ChartAreas[0];
            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 40;
            area.Area3DStyle.Rotation = 60;
            area.Area3DStyle.LightStyle = LightStyle.Realistic;
            area.Area3DStyle.WallWidth = 0;

            foreach (DataRow row in dt.Rows)
            {
                string vendedor = row["Vendedor"].ToString();
                decimal totalVentas = Convert.ToDecimal(row["TotalVentas"]);
                int idx = serie.Points.AddXY(vendedor, totalVentas);
                serie.Points[idx].LegendText = $"{vendedor}: {totalVentas:C2}";
            }

            Title subTitulo = new Title
            {
                Text = $"Total de ventas del año {anio}: {dt.Compute("SUM(TotalVentas)", string.Empty):C2}",
                Docking = Docking.Top,
                Font = new Font("Arial", 8, FontStyle.Bold),
                Alignment = ContentAlignment.TopLeft,
                IsDockedInsideChartArea = false,
                DockingOffset = -3
            };
            chart1.Titles.Add(subTitulo);
        }

        private DataTable ObtenerDatos(int anio)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    var query = from e in context.Employees
                                join o in context.Orders on e.EmployeeID equals o.EmployeeID
                                join od in context.Order_Details on o.OrderID equals od.OrderID
                                where o.OrderDate.Value.Year == anio
                                group od by new { e.FirstName, e.LastName} into g
                                let totalVentas = g.Sum(x => x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                                orderby totalVentas descending
                                select new
                                {
                                    Vendedor = g.Key.FirstName + " " + g.Key.LastName,
                                    TotalVentas = totalVentas
                                };
                    dt.Columns.Add("Vendedor", typeof(string));
                    dt.Columns.Add("TotalVentas", typeof(decimal));
                    foreach (var item in query)
                    {
                        dt.Rows.Add(item.Vendedor, item.TotalVentas);
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

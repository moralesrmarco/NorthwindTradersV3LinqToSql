using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmGraficaVentasPorVendedores : Form
    {
        public FrmGraficaVentasPorVendedores()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaVentasPorVendedores_Load(object sender, EventArgs e)
        {
            CargarVentasPorVendedores();
        }

        private void CargarVentasPorVendedores()
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();
            Title titulo = new Title()
            {
                Text = "Gráfica ventas por vendedores de todos los años",
                Font = new Font("Arial", 16, FontStyle.Bold),
            };
            chart1.Titles.Add(titulo);
            groupBox1.Text = $"» {titulo.Text} «";
            DataTable dt = GetDatos();
            if (dt != null)
            {
                Series serie = new Series
                {
                    Name = "Ventas",
                    Color = Color.FromArgb(0, 51, 102),
                    IsValueShownAsLabel = true,
                    ChartType = SeriesChartType.Doughnut,
                    Label = "#VALX: #VALY{C2}"
                };
                serie["PieLabelStyle"] = "Outside";
                serie.SmartLabelStyle.Enabled = true;
                serie.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
                serie.SmartLabelStyle.CalloutLineColor = Color.Black;
                serie.LabelForeColor = Color.DarkSlateGray;
                serie.LabelBackColor = Color.WhiteSmoke;
                chart1.Series.Add(serie);
                foreach (DataRow row in dt.Rows)
                {
                    serie.Points.AddXY(row["Vendedor"], row["TotalVentas"]);
                }
                Title subTitulo = new Title
                {
                    Text = $"Total de ventas: {serie.Points.Sum(pt => pt.YValues[0]):C2}",
                    Docking = Docking.Top,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    ForeColor = Color.Black,
                    IsDockedInsideChartArea = false,
                    Alignment = ContentAlignment.TopLeft,
                    DockingOffset = -3
                };
                chart1.Titles.Add(subTitulo);
            }
        }

        private DataTable GetDatos()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    // Consulta LINQ para obtener las ventas por vendedor
                    var ventasPorVendedorQuery = from e in context.Employees
                                                 join o in context.Orders on e.EmployeeID equals o.EmployeeID
                                                 join od in context.Order_Details on o.OrderID equals od.OrderID
                                                 group od by new { e.FirstName, e.LastName } into g
                                                 let total = g.Sum(x => x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                                                 orderby total descending
                                                 select new
                                                 {
                                                     Vendedor = g.Key.FirstName + " " + g.Key.LastName,
                                                     TotalVentas = total
                                                 };
                    // Convertir a DataTable
                    dt.Columns.Add("Vendedor", typeof(string));
                    dt.Columns.Add("TotalVentas", typeof(decimal));
                    foreach (var item in ventasPorVendedorQuery)
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmGraficaVentasMensuales : Form
    {
        public FrmGraficaVentasMensuales()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaVentasMensuales_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
        }

        private void LlenarComboBox()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext(NorthwindTradersV3LinqToSql.Properties.Settings.Default.NwCn))
                {
                    var years = context.Orders
                        .Where(o => o.OrderDate != null)
                        .Select(o => o.OrderDate.Value.Year)
                        .Distinct()
                        .OrderByDescending(y => y)
                        .ToList();
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
            CargarVentasMensuales(Convert.ToInt32(comboBox1.SelectedItem));
        }

        private void CargarVentasMensuales(int year)
        {
            var datos = ObtenerVentasMensuales(year);
            var serie = chart1.Series["Ventas mensuales"];
            serie.Points.Clear();
            serie.ChartType = SeriesChartType.Line;
            serie.BorderWidth = 3;
            serie.ToolTip = "Ventas de #VALX: #VALY{C2}";
            serie.IsValueShownAsLabel = true;
            serie.LabelFormat = "C2";
            serie.MarkerStyle = MarkerStyle.Circle;
            serie.MarkerSize = 10;
            foreach (var punto in datos)
            {
                string nombreMes = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(punto.Mes);
                serie.Points.AddXY(nombreMes, punto.Total);
            }
            var area = chart1.ChartAreas[0];
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Title = "Meses";
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.Title = "Ventas Totales";
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.LineColor = Color.Gray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.LineColor = Color.LightGray;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;
            // ocultar las leyendas
            chart1.Legends[0].Enabled = false;

            Title titulo = new Title
            {
                Text = $"Ventas Mensuales del Año {year}",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Alignment = ContentAlignment.TopCenter
            };
            
            decimal totalVentas = datos.Sum(x => x.Total);
            Title subtitulo = new Title
            {
                Text = $"Total de ventas del año {year}: {totalVentas:C2}",
                Docking = Docking.Top,
                Font = new Font("Arial", 8, FontStyle.Bold),
                Alignment = ContentAlignment.TopRight,
                IsDockedInsideChartArea = false
            };
            chart1.Titles.Clear();
            chart1.Titles.Add(titulo);
            chart1.Titles.Add(subtitulo);
            groupBox1.Text = $"» Ventas mensuales del año: {year} «";
        }

        private List<MontlySales> ObtenerVentasMensuales(int year)
        {
            var ventasPorMes = new List<MontlySales>();
            var meses = Enumerable.Range(1, 12);

            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext(NorthwindTradersV3LinqToSql.Properties.Settings.Default.NwCn))
                {
                    ventasPorMes = (from mes in meses
                                    join venta in (
                                    from o in context.Orders
                                    join od in context.Order_Details on o.OrderID equals od.OrderID
                                    where o.OrderDate.HasValue && o.OrderDate.Value.Year == year
                                    group new { o, od } by o.OrderDate.Value.Month into g
                                    select new MontlySales
                                    {
                                        Mes = g.Key,
                                        Total = g.Sum(x => x.od.UnitPrice * x.od.Quantity * (1 - (decimal)x.od.Discount))
                                    }
                                    )
                                    on mes equals venta.Mes into ventasJoin
                                    from venta in ventasJoin.DefaultIfEmpty()
                                    orderby mes
                                    select new MontlySales
                                    {
                                        Mes = mes,
                                        Total = venta?.Total ?? 0m
                                    }).ToList();
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
            return ventasPorMes;
        }

        private class MontlySales
        {
            public int Mes { get; set; }
            public decimal Total { get; set; }
        }
    }
}

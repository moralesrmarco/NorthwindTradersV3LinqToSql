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
    public partial class FrmGraficaVentasAnuales : Form
    {
        public FrmGraficaVentasAnuales()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaVentasAnuales_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
            groupBox1.Text = $"» Comparativo de ventas mensuales de los últimos 2 años «";
        }

        private void LlenarComboBox()
        {
            var items = new List<KeyValuePair<string, int>>();
            for (int i = 2; i <= 8; i++)
                items.Add(new KeyValuePair<string, int>($"{i} Años", i));
            CmbUltimosAños.DataSource = items;
            CmbUltimosAños.DisplayMember = "Key";
            CmbUltimosAños.ValueMember = "Value";
            CmbUltimosAños.SelectedIndex = 0;
        }

        private void CmbUltimosAños_SelectedIndexChanged(object sender, EventArgs e)
        {
            var kv = (KeyValuePair<string, int>)CmbUltimosAños.SelectedItem;
            int years = kv.Value;

            if ( years >= 6)
            {
                MessageBox.Show("Solo existen datos en la base de datos hasta el año 1996", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            CargarComparativoVentasMensuales(years);
        }

        private void CargarComparativoVentasMensuales(int years)
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();
            chart1.Legends.Clear();

            var legend = new Legend("Default")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Center,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };
            chart1.Legends.Add(legend);

            int yearActual = DateTime.Now.Year;
            for (int i = 1; i <= years; i++)
            {
                if (yearActual == 2023)
                    yearActual = 1998;
                else if (yearActual == 1995)
                    break;
                var datos = ObtenerVentasMensuales(yearActual);
                decimal totalAnual = datos.AsEnumerable().Sum(row => row.Field<decimal>("Total"));
                string nombreSerie = $"Ventas {yearActual}"; // nombre de la serie para la leyenda
                chart1.Series.Add($"Ventas {yearActual}");
                chart1.Series[nombreSerie].ChartType = SeriesChartType.Line;
                chart1.Series[nombreSerie].IsValueShownAsLabel = false;
                chart1.Series[nombreSerie].Label = "#VALY{C}";
                chart1.Series[nombreSerie].BorderWidth = 2;
                chart1.Series[nombreSerie].ToolTip = $"{nombreSerie} #VALX: #VALY{{C2}}";
                chart1.Series[nombreSerie].Legend = legend.Name; // Asignar leyenda a la serie
                chart1.Series[nombreSerie].LegendText = $"{nombreSerie} (Total: {totalAnual:C2})"; // Texto de la leyenda
                foreach (DataRow row in datos.Rows)
                {
                    string nombreMes = row.Field<string>("NombreMes");
                    int index = chart1.Series[nombreSerie].Points.AddXY(nombreMes, row.Field<decimal>("Total"));
                    DataPoint dataPoint = chart1.Series[nombreSerie].Points[index];
                    if (row.Field<decimal>("Total") != 0)
                    {
                        dataPoint.Label = row.Field<decimal>("Total").ToString("C2");
                        dataPoint.MarkerStyle = MarkerStyle.Circle;
                        dataPoint.MarkerSize = 10;
                    }
                    else
                        dataPoint.Label = ""; 
                }
                yearActual--;
            }
            var area = chart1.ChartAreas[0];
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Title = "Meses";
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.LabelStyle.Angle = -45;
            area.AxisY.Title = "Ventas Totales";
            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.LineColor = Color.LightGray;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            Title titulo = new Title
            {
                Text = $"Comparativo de Ventas Mensuales de los Últimos {years} Años",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Alignment = ContentAlignment.TopCenter
            };
            groupBox1.Text = $"» Comparativo de ventas mensuales de los últimos {years} años «";
            chart1.Titles.Add(titulo);
        }

        private DataTable ObtenerVentasMensuales(int year)
        {
            DataTable dt = new DataTable();
            // 1. Arreglo in-memory de meses con su nombre abreviado
            var meses = new[]
            {
                new { Mes = 1, NombreMes = "Ene." },
                new { Mes = 2, NombreMes = "Feb." },
                new { Mes = 3, NombreMes = "Mar." },
                new { Mes = 4, NombreMes = "Abr." },
                new { Mes = 5, NombreMes = "May." },
                new { Mes = 6, NombreMes = "Jun." },
                new { Mes = 7, NombreMes = "Jul." },
                new { Mes = 8, NombreMes = "Ago." },
                new { Mes = 9, NombreMes = "Sep." },
                new { Mes = 10, NombreMes = "Oct." },
                new { Mes = 11, NombreMes = "Nov." },
                new { Mes = 12, NombreMes = "Dic." }
            };
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    // 2. Consulta LINQ para obtener las ventas mensuales del año especificado
                    var ventasMensualesQuery = from o in context.Orders
                                               where o.OrderDate != null && o.OrderDate.Value.Year == year
                                               join od in context.Order_Details on o.OrderID equals od.OrderID
                                               group od by o.OrderDate.Value.Month into g
                                               select new
                                               {
                                                   Mes = g.Key,
                                                   Total = g.Sum(x => x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                                               };
                    // 3. Unir con el arreglo de meses para asegurar que todos los meses estén representados
                    var ventasMensuales = from m in meses
                                          join v in ventasMensualesQuery on m.Mes equals v.Mes into mv
                                          from v in mv.DefaultIfEmpty()
                                          orderby m.Mes
                                          select new
                                          {
                                              Mes = m.Mes,
                                              NombreMes = m.NombreMes,
                                              Total = v != null ? v.Total : 0m
                                          };
                    // 4. Convertir a DataTable
                    dt.Columns.Add("Mes", typeof(int));
                    dt.Columns.Add("NombreMes", typeof(string));
                    dt.Columns.Add("Total", typeof(decimal));
                    foreach (var item in ventasMensuales)
                    {
                        dt.Rows.Add(item.Mes, item.NombreMes, item.Total);
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
            return dt;
        }
    }
}

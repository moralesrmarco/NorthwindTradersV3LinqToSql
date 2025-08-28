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
    public partial class FrmTableroControlVendedores : Form
    {

        private readonly string[] categorias = { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
        private readonly double[] valores = { 15, 30, 45, 20, 35, 50, 25, 40, 45, 40, 30, 50 };

        public FrmTableroControlVendedores()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmTableroControlAltaVendedores_Load(object sender, EventArgs e)
        {
            //LlenarCmbVentasMensualesDelAnio();
            //LlenarCmbTipoGrafica1();
            //CmbTipoGrafica1.SelectedIndex = 12; // SeriesChartType.Line
            LlenarCmbVentasMensualesPorVendedorPorAño();
            LlenarCmbTipoGrafica1();
            CmbTipoGrafica1.SelectedItem = SeriesChartType.Line;

            LlenarCmbUltimosAnios();
            LlenarCmbTipoGrafica2();
            CmbTipoGrafica2.SelectedItem = SeriesChartType.Line;

            LlenarCmbNumeroProductos();
            LlenarCmbTipoGrafica3();
            CmbTipoGrafica3.SelectedItem = SeriesChartType.Column;

            CargarVentasPorVendedores();

            LlenarCmbVentasVendedorAnio();
            LlenarCmbTipoGrafica5();
            CmbTipoGrafica5.SelectedItem = SeriesChartType.Doughnut;

            LlenarCmbTipoGrafica();
        }
        /******************************************************************************************************/
        private void LlenarCmbVentasMensualesPorVendedorPorAño()
        {
            cmbVentasMensualesPorVendedorPorAño.SelectedIndexChanged -= cmbVentasMensualesPorVendedorPorAño_SelectedIndexChanged;
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    var years = (
                        from o in context.Orders
                        where o.OrderDate.HasValue
                        select o.OrderDate.Value.Year
                    )
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();
                    cmbVentasMensualesPorVendedorPorAño.DataSource = years;
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
            cmbVentasMensualesPorVendedorPorAño.SelectedIndex = 0;
            cmbVentasMensualesPorVendedorPorAño.SelectedIndexChanged += cmbVentasMensualesPorVendedorPorAño_SelectedIndexChanged;
        }

        private void LlenarCmbTipoGrafica1()
        {
            CmbTipoGrafica1.SelectedIndexChanged -= CmbTipoGrafica1_SelectedIndexChanged;
            CmbTipoGrafica1.DataSource = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .Where(t => t != SeriesChartType.Doughnut && t != SeriesChartType.ErrorBar && t != SeriesChartType.Funnel && t != SeriesChartType.Kagi && t != SeriesChartType.Pie && t != SeriesChartType.PointAndFigure && t != SeriesChartType.Polar && t != SeriesChartType.Pyramid && t != SeriesChartType.Renko && t != SeriesChartType.ThreeLineBreak) // Excluye tipos no deseados
                .OrderBy(t => t.ToString())
                .ToList();
            CmbTipoGrafica1.SelectedIndexChanged += CmbTipoGrafica1_SelectedIndexChanged;
        }

        private void cmbVentasMensualesPorVendedorPorAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            DibujarGraficaChart1();
        }

        private void CmbTipoGrafica1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DibujarGraficaChart1();
        }

        private void DibujarGraficaChart1()
        {
            CargarVentasMensualesPorVendedorPorAnio(Convert.ToInt32(cmbVentasMensualesPorVendedorPorAño.SelectedItem), (SeriesChartType)CmbTipoGrafica1.SelectedItem);
        }

        private void CargarVentasMensualesPorVendedorPorAnio(int anio, SeriesChartType tipoGrafica)
        {
            var datos = ObtenerVentasMensualesPorVendedorPorAnio(anio);
            if (datos != null && datos.Rows.Count > 0)
            {
                chart1.Series.Clear();
                chart1.Titles.Clear();
                Title titulo = new Title
                {
                    Text = $"Ventas mensuales por vendedor del año: {anio}.\nTipo de gráfica: {tipoGrafica}.",
                    Font = new Font("Segoe UI", 8, FontStyle.Bold)
                };
                chart1.Titles.Add(titulo);
                groupBox1.Text = $"» Ventas mensuales por vendedor del año: {anio}. Tipo de gráfica: {tipoGrafica}. «";
                var area = chart1.ChartAreas[0];
                area.AxisX.Interval = 1;
                area.AxisX.Title = "Meses";
                area.AxisX.TitleFont = new Font("Segoe UI", 7, FontStyle.Bold);
                area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
                area.AxisX.LabelStyle.Angle = -45;
                area.AxisY.Title = "Ventas totales";
                area.AxisY.TitleFont = new Font("Segoe UI", 7, FontStyle.Bold);
                area.AxisY.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
                area.AxisY.LabelStyle.Format = "C0";
                area.AxisY.LabelStyle.Angle = -45;

                // 1) Preparo el listado de meses en orden usando tu DataTable (debe dar 12)
                var mesesOrdenados = datos.AsEnumerable()
                    .Select(r => new
                    {
                        Mes = r.Field<int>("Mes"),
                        Nombre = r.Field<string>("NombreMes")
                    })
                    .Distinct()
                    .OrderBy(x => x.Mes)
                    .ToList();
                // 2) Agrupo los datos por vendedor
                var grupos = datos.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Vendedor"))
                    .OrderBy(g => g.Key)
                    .ToList();
                foreach (var grupo in grupos)
                {
                    if (grupo.Key == "Sin Vendedor")
                        continue;
                    // Serie por vendedor
                    var serie = new Series(grupo.Key)
                    {
                        ChartType = tipoGrafica,
                        BorderWidth = 2,
                        MarkerStyle = MarkerStyle.Circle,
                        ToolTip = "#SERIESNAME\nMes: #AXISLABEL\nVentas: #VALY{C2}",
                        IsValueShownAsLabel = false
                    };
                    // 3) Inicializo los 12 puntos con valor 0 y nombre tal cual viene en tu DataTable
                    foreach (var m in mesesOrdenados)
                    {
                        serie.Points.AddXY(m.Nombre, 0D);
                    }
                    // 4) Relleno los valores reales
                    foreach (var row in grupo)
                    {
                        int mes = row.Field<int>("Mes");           // 1–12
                        decimal raw = row.Field<decimal>("TotalVentas");
                        double venta = Convert.ToDouble(raw);

                        // El índice debe coincidir con el orden de mesesOrdenados
                        // Como mesesOrdenados está ordenado por Mes y empieza en 1, uso mes-1
                        serie.Points[mes - 1].YValues[0] = venta;
                    }
                    chart1.Series.Add(serie);
                }
                chart1.Legends[0].Font = new Font("Segoe UI", 7, FontStyle.Regular);
                // ————— Aquí forzamos el recálculo de la escala del eje Y —————
                chart1.ResetAutoValues();
            }
        }

        private DataTable ObtenerVentasMensualesPorVendedorPorAnio(int anio)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            try
            {
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
                using (var context = new NorthwindTradersDataContext())
                {
                    var resultado = (
                        from e in context.Employees
                        join o in context.Orders on e.EmployeeID equals o.EmployeeID
                        join od in context.Order_Details on o.OrderID equals od.OrderID
                        where o.OrderDate.Value.Year == anio
                        group new { e, o, od } by new
                        {
                            Vendedor = e.FirstName + " " + e.LastName,
                            Mes = o.OrderDate.Value.Month
                        } into g
                        orderby g.Key.Vendedor, g.Key.Mes
                        select new
                        {
                            Vendedor = g.Key.Vendedor,
                            Mes = g.Key.Mes,
                            TotalVentas = g.Sum(x => x.od.UnitPrice * x.od.Quantity * (1 - (decimal)x.od.Discount))
                        }
                        ).ToList();
                    var resultadoConMeses = from m in meses
                                            join v in resultado on m.Mes equals v.Mes into grp
                                            from v in grp.DefaultIfEmpty()
                                            orderby m.Mes
                                            select new
                                            {
                                                Vendedor = v?.Vendedor ?? "Sin Vendedor",
                                                Mes = m.Mes,
                                                TotalVentas = v?.TotalVentas ?? 0m,
                                                NombreMes = m.NombreMes
                                            };
                    dt.Columns.Add("Vendedor", typeof(string));
                    dt.Columns.Add("Mes", typeof(int));
                    dt.Columns.Add("TotalVentas", typeof(decimal));
                    dt.Columns.Add("NombreMes", typeof(string));
                    foreach (var item in resultadoConMeses)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Vendedor"] = item.Vendedor;
                        dr["Mes"] = item.Mes;
                        dr["TotalVentas"] = item.TotalVentas;
                        dr["NombreMes"] = item.NombreMes;
                        dt.Rows.Add(dr);
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
        /******************************************************************************************************/
        private void LlenarCmbUltimosAnios()
        {
            cmbUltimosAnios.SelectedIndexChanged -= cmbUltimosAnios_SelectedIndexChanged;
            var items = new List<KeyValuePair<string, int>>();
            for (int i = 2; i <= 8; i++)
                items.Add(new KeyValuePair<string, int>($"{i} Años", i));
            cmbUltimosAnios.DataSource = items;
            cmbUltimosAnios.DisplayMember = "Key";
            cmbUltimosAnios.ValueMember = "Value";
            cmbUltimosAnios.SelectedIndexChanged += cmbUltimosAnios_SelectedIndexChanged;
        }

        private void LlenarCmbTipoGrafica2()
        {
            CmbTipoGrafica2.SelectedIndexChanged -= CmbTipoGrafica2_SelectedIndexChanged;
            CmbTipoGrafica2.DataSource = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .Where(t => t != SeriesChartType.Kagi && t != SeriesChartType.ErrorBar && t != SeriesChartType.PointAndFigure && t != SeriesChartType.Renko && t != SeriesChartType.StackedArea && t != SeriesChartType.StackedArea100 && t != SeriesChartType.StackedBar100 && t != SeriesChartType.StackedColumn100 && t != SeriesChartType.ThreeLineBreak) // Excluye tipos no deseados
                .OrderBy(t => t.ToString())
                .ToList();
            CmbTipoGrafica2.SelectedIndexChanged += CmbTipoGrafica2_SelectedIndexChanged;
        }

        private void cmbUltimosAnios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cmbUltimosAnios.SelectedValue) >= 6)
            {
                MessageBox.Show("Solo existen datos en la base de datos hasta el año 1996", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            CargarComparativoVentasMensuales(Convert.ToInt32(cmbUltimosAnios.SelectedValue), (SeriesChartType)CmbTipoGrafica2.SelectedItem);
        }

        private void CmbTipoGrafica2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarComparativoVentasMensuales(Convert.ToInt32(cmbUltimosAnios.SelectedValue), (SeriesChartType)CmbTipoGrafica2.SelectedItem);
        }

        private void CargarComparativoVentasMensuales(int years, SeriesChartType tipoGrafica)
        {
            chart2.Series.Clear();
            chart2.Titles.Clear();
            int yearActual = DateTime.Now.Year;
            for (int i = 1; i <= years; i++)
            {
                if (yearActual == 2023)
                    yearActual = 1998;
                else if (yearActual == 1995)
                    break;
                chart2.Series.Add($"Ventas {yearActual}");
                //chart2.Series[$"Ventas {yearActual}"].ChartType = SeriesChartType.Line;
                chart2.Series[$"Ventas {yearActual}"].ChartType = tipoGrafica;
                chart2.Series[$"Ventas {yearActual}"].IsValueShownAsLabel = false;
                chart2.Series[$"Ventas {yearActual}"].Label = "#VALY{C}"; // Formato de moneda
                chart2.Series[$"Ventas {yearActual}"].BorderWidth = 2;
                chart2.Series[$"Ventas {yearActual}"].LegendText = $"Ventas {yearActual}"; // Leyenda personalizada
                chart2.Series[$"Ventas {yearActual}"].ToolTip = "#LEGENDTEXT\nde #AXISLABEL:\n#VALY{C2}"; // tooltip con moneda y 2 decimales
                chart2.Series[$"Ventas {yearActual}"].Points.Clear();
                var datos = ObtenerVentasMensualesComparativo(yearActual);
                if (chart2.Legends.Count > 0)
                    chart2.Legends[0].Font = new Font("Segoe UI", 7, FontStyle.Regular);
                foreach (DataRow row in datos.Rows)
                {
                    string nombreMes = row.Field<string>("NombreMes");
                    int index = chart2.Series[$"Ventas {yearActual}"].Points.AddXY(nombreMes, row.Field<decimal>("Total"));
                    DataPoint dataPoint = chart2.Series[$"Ventas {yearActual}"].Points[index];
                    if (row.Field<decimal>("Total") > 0)
                    {
                        dataPoint.Label = $"${row.Field<decimal>("Total"):#,##0.00}";
                        dataPoint.Font = new Font("Segoe UI", 7, FontStyle.Regular);
                        dataPoint.MarkerStyle = MarkerStyle.Circle; // Estilo de marcador
                        dataPoint.MarkerSize = 5; // Tamaño del marcador
                    }
                    else
                    {
                        dataPoint.Label = string.Empty; // No mostrar etiqueta si el total es 0
                    }
                }
                yearActual--;
            }
            var area = chart2.ChartAreas[0];
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Title = "Meses";
            area.AxisX.TitleFont = new Font("Segoe UI", 7, FontStyle.Bold);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);

            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.Title = "Ventas Totales";
            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.TitleFont = new Font("Segoe UI", 7, FontStyle.Bold);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisY.LabelStyle.Angle = -45;

            Title titulo = new Title
            {
                Text = $"Comparativo de ventas mensuales de los últimos {years} años.\nTipo de gráfica: {tipoGrafica}.",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Alignment = ContentAlignment.TopCenter
            };
            groupBox2.Text = $"» Comparativo de ventas mensuales de los últimos {years} años. Tipo de gráfica: {tipoGrafica}. «";
            chart2.Titles.Add(titulo);
        }

        private DataTable ObtenerVentasMensualesComparativo(int year)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            try
            {
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
                using (var context = new NorthwindTradersDataContext())
                {
                    var ventasMensualesQuery = from o in context.Orders
                                               where o.OrderDate.HasValue && o.OrderDate.Value.Year == year
                                               join od in context.Order_Details on o.OrderID equals od.OrderID
                                               group od by o.OrderDate.Value.Month into g
                                               select new
                                               {
                                                   Mes = g.Key,
                                                   Total = g.Sum(x => x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                                               };
                    var ventasMensuales = from m in meses
                                          join v in ventasMensualesQuery on m.Mes equals v.Mes into mv
                                          from v in mv.DefaultIfEmpty()
                                          orderby m.Mes
                                          select new
                                          {
                                              Mes = m.Mes,
                                              NombreMes = m.NombreMes,
                                              Total = v?.Total ?? 0m
                                          };
                    dt.Columns.Add("Mes", typeof(int));
                    dt.Columns.Add("NombreMes", typeof(string));
                    dt.Columns.Add("Total", typeof(decimal));
                    foreach (var item in ventasMensuales)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Mes"] = item.Mes;
                        dr["NombreMes"] = item.NombreMes;
                        dr["Total"] = item.Total;
                        dt.Rows.Add(dr);
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
        /******************************************************************************************************/
        private void LlenarCmbNumeroProductos()
        {
            cmbNumeroProductos.SelectedIndexChanged -= cmbNumeroProductos_SelectedIndexChanged;
            var items = new List<KeyValuePair<string, int>>();
            for (int i = 10; i <= 30; i += 5)
                items.Add(new KeyValuePair<string, int>($"{i} Productos", i));
            cmbNumeroProductos.DataSource = items;
            cmbNumeroProductos.DisplayMember = "Key";
            cmbNumeroProductos.ValueMember = "Value";
            cmbNumeroProductos.SelectedIndexChanged += cmbNumeroProductos_SelectedIndexChanged;
        }

        private void LlenarCmbTipoGrafica3()
        {
            CmbTipoGrafica3.SelectedIndexChanged -= CmbTipoGrafica3_SelectedIndexChanged;
            CmbTipoGrafica3.DataSource = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .Where(t => t != SeriesChartType.Kagi && t != SeriesChartType.ErrorBar && t != SeriesChartType.PointAndFigure && t != SeriesChartType.Polar && t != SeriesChartType.Renko && t != SeriesChartType.ThreeLineBreak) // Excluye tipos no deseados
                .OrderBy(t => t.ToString())
                .ToList();
            CmbTipoGrafica3.SelectedIndexChanged += CmbTipoGrafica3_SelectedIndexChanged;
        }

        private void cmbNumeroProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTopProductos(Convert.ToInt32(cmbNumeroProductos.SelectedValue), (SeriesChartType)CmbTipoGrafica3.SelectedItem);
        }

        private void CmbTipoGrafica3_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTopProductos(Convert.ToInt32(cmbNumeroProductos.SelectedValue), (SeriesChartType)CmbTipoGrafica3.SelectedItem);
        }

        private void CargarTopProductos(int cantidad, SeriesChartType tipoGrafica)
        {
            chart3.Series.Clear();
            chart3.Titles.Clear();
            Title titulo = new Title
            {
                Text = $"Top {cantidad} productos más vendidos.\nTipo de gráfica: {tipoGrafica}.",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Alignment = ContentAlignment.TopCenter
            };
            chart3.Titles.Add(titulo);
            groupBox3.Text = $"» Top {cantidad} productos más vendidos. Tipo de gráfica: {tipoGrafica}. «";
            var series = new Series("Productos")
            {
                ChartType = tipoGrafica,
                IsValueShownAsLabel = true,
                Label = "#VALY{n0}",
                LabelFormat = "C2",
                BorderWidth = 2,
                ToolTip = "Producto: #VALX,\nCantidad Vendida: #VALY{n0}",
                Font = new Font("Segoe UI", 7, FontStyle.Bold)
            };
            series.Points.Clear();
            // Paleta de 10 colores (ajusta a tu gusto)
            Color[] paleta = {
                Color.SteelBlue, Color.Orange, Color.MediumSeaGreen,
                Color.Goldenrod, Color.Crimson, Color.MediumPurple,
                Color.Tomato, Color.Teal, Color.SlateGray, Color.DeepPink
            };
            var productos = ObtenerTopProductos(cantidad);
            int idx = 0;
            foreach (DataRow row in productos.Rows)
            {
                string nombreProducto = (idx + 1).ToString() + ".- " + row["NombreProducto"];
                int cantidadVendida = Convert.ToInt32(row["CantidadVendida"]);
                int pointIndex = series.Points.AddXY(nombreProducto, cantidadVendida);
                series.Points[pointIndex].Color = paleta[idx % paleta.Length];
                idx++;
            }
            chart3.Series.Add(series);
            chart3.Legends.Clear();
            var area = chart3.ChartAreas[0];
            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 30;
            area.Area3DStyle.Rotation = 20;
            area.Area3DStyle.LightStyle = LightStyle.Realistic;

            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisY.Title = "Cantidad Vendida (unidades)";
            area.AxisY.TitleFont = new Font("Segoe UI", 8, FontStyle.Regular);
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
        }

        private DataTable ObtenerTopProductos(int cantidad)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            var dt = new DataTable();
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    var topProductosQuery = context.Products
                        .Select(p => new
                        {
                            NombreProducto = p.ProductName,
                            CantidadVendida = p.Order_Details.Sum(od => od.Quantity)
                        })
                        .OrderByDescending(p => p.CantidadVendida)
                        .Take(cantidad)
                        .ToList();
                    dt.Columns.Add("NombreProducto", typeof(string));
                    dt.Columns.Add("CantidadVendida", typeof(int));
                    foreach (var item in topProductosQuery)
                    {
                        DataRow dr = dt.NewRow();
                        dr["NombreProducto"] = item.NombreProducto;
                        dr["CantidadVendida"] = item.CantidadVendida;
                        dt.Rows.Add(dr);
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
        /******************************************************************************************************/
        private void CargarVentasPorVendedores()
        {
            chart4.Series.Clear();
            chart4.Titles.Clear();
            Title titulo = new Title
            {
                Text = "Ventas por vendedores de todos los años",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
            };
            chart4.Titles.Add(titulo);
            groupBox4.Text = $"» {titulo.Text} «";
            var area = chart4.ChartAreas[0];
            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 40;
            area.Area3DStyle.Rotation = 60;
            area.Area3DStyle.LightStyle = LightStyle.Realistic;
            area.Area3DStyle.WallWidth = 0;
            Series serie = new Series("Ventas")
            {
                ChartType = SeriesChartType.Doughnut,
                IsValueShownAsLabel = false,
                Label = "#VALX: #VALY{C2}",
                ToolTip = "Vendedor: #VALX\nTotal Ventas: #VALY{C2}"
            };
            serie.Points.Clear();
            serie.SmartLabelStyle.Enabled = true;
            serie.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
            serie.SmartLabelStyle.CalloutLineColor = Color.Black;
            serie.LabelForeColor = Color.DarkSlateGray;
            serie.LabelBackColor = Color.WhiteSmoke;
            serie["PieLabelStyle"] = "Disabled";
            serie["PieDrawingStyle"] = "Cylinder";
            serie["DoughnutRadius"] = "60";
            chart4.Series.Add(serie);

            var legend = chart4.Legends[0];
            legend.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            DataTable dt = VentasTodosAños();
            foreach (DataRow row in dt.Rows)
            {
                string vendedor = row.Field<string>("Vendedor");
                decimal totalVentas = row.Field<decimal>("TotalVentas");
                int pointIndex = serie.Points.AddXY(vendedor, totalVentas);
                DataPoint dataPoint = serie.Points[pointIndex];
                dataPoint.Label = $"{vendedor}: {totalVentas:C2}";
                dataPoint.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            }
        }

        private DataTable VentasTodosAños()
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
        /******************************************************************************************************/
        private void LlenarCmbVentasVendedorAnio()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            cmbVentasVendedorAnio.SelectedIndexChanged -= cmbVentasVendedorAnio_SelectedIndexChanged;
            cmbVentasVendedorAnio.Items.Clear();
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    var years = (
                        from o in context.Orders
                        where o.OrderDate.HasValue
                        select o.OrderDate.Value.Year
                    )
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();
                    foreach (var year in years)
                    {
                        cmbVentasVendedorAnio.Items.Add(year);
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
            cmbVentasVendedorAnio.SelectedIndex = 0;
            cmbVentasVendedorAnio.SelectedIndexChanged += cmbVentasVendedorAnio_SelectedIndexChanged;
            MDIPrincipal.ActualizarBarraDeEstado();
        }

        private void LlenarCmbTipoGrafica5()
        {
            CmbTipoGrafica5.SelectedIndexChanged -= CmbTipoGrafica5_SelectedIndexChanged;
            CmbTipoGrafica5.DataSource = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .Where(t => t != SeriesChartType.BoxPlot && t != SeriesChartType.ErrorBar && t != SeriesChartType.Kagi && t != SeriesChartType.PointAndFigure && t != SeriesChartType.Polar && t != SeriesChartType.Renko && t != SeriesChartType.StackedArea && t != SeriesChartType.StackedArea100 && t != SeriesChartType.StackedColumn100 && t != SeriesChartType.ThreeLineBreak) // Excluye tipos no deseados
                .OrderBy(t => t.ToString())
                .ToList();
            CmbTipoGrafica5.SelectedIndexChanged += CmbTipoGrafica5_SelectedIndexChanged;
        }

        private void cmbVentasVendedorAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVentasVendedorAnio.SelectedIndex < 0)
                return;
            CargarVentasPorVendedoresAnio(Convert.ToInt32(cmbVentasVendedorAnio.SelectedItem), (SeriesChartType)CmbTipoGrafica5.SelectedItem);
        }

        private void CmbTipoGrafica5_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarVentasPorVendedoresAnio(Convert.ToInt32(cmbVentasVendedorAnio.SelectedItem), (SeriesChartType)CmbTipoGrafica5.SelectedItem);
        }

        private void CargarVentasPorVendedoresAnio(int year, SeriesChartType tipoGrafica)
        {
            chart5.Series.Clear();
            chart5.Titles.Clear();
            chart5.Legends.Clear();
            var leyenda = new Legend("Vendedores")
            {
                Title = "Vendedores",
                TitleFont = new Font("Segoe UI", 7, FontStyle.Bold),
                Docking = Docking.Right,
                LegendStyle = LegendStyle.Table,
                Font = new Font("Segoe UI", 7, FontStyle.Regular),
                IsTextAutoFit = false
            };
            chart5.Legends.Add(leyenda);
            Title titulo = new Title
            {
                Text = $"Ventas por vendedores del año {year}",
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            chart5.Titles.Add(titulo);
            groupBox5.Text = $"» {titulo.Text}. Tipo de grafica: {tipoGrafica} «";
            Series serie = new Series("Ventas")
            {
                ChartType = tipoGrafica,
                IsValueShownAsLabel = false,
                Label = "#AXISLABEL: #VALY{C2}",
                ToolTip = "Vendedor: #AXISLABEL\nTotal ventas: #VALY{C2}",
                Legend = leyenda.Name,
                LegendText = "#AXISLABEL: #VALY{C2}"
            };
            if (tipoGrafica == SeriesChartType.Area || tipoGrafica == SeriesChartType.Bar || tipoGrafica == SeriesChartType.Bubble || tipoGrafica == SeriesChartType.Candlestick || tipoGrafica == SeriesChartType.Column || tipoGrafica == SeriesChartType.FastLine || tipoGrafica == SeriesChartType.FastPoint || tipoGrafica == SeriesChartType.Funnel || tipoGrafica == SeriesChartType.Line || tipoGrafica == SeriesChartType.Point || tipoGrafica == SeriesChartType.Pyramid || tipoGrafica == SeriesChartType.Radar || tipoGrafica == SeriesChartType.Range || tipoGrafica == SeriesChartType.RangeBar || tipoGrafica == SeriesChartType.RangeColumn || tipoGrafica == SeriesChartType.Spline || tipoGrafica == SeriesChartType.SplineArea || tipoGrafica == SeriesChartType.SplineRange || tipoGrafica == SeriesChartType.StackedBar || tipoGrafica == SeriesChartType.StackedBar100 || tipoGrafica == SeriesChartType.StackedColumn || tipoGrafica == SeriesChartType.StepLine || tipoGrafica == SeriesChartType.Stock)
            {
                chart5.Legends.Clear();
            }
            var area = chart5.ChartAreas[0];
            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 30;
            area.Area3DStyle.Rotation = 20;
            area.Area3DStyle.LightStyle = LightStyle.Realistic;
            serie["PieLabelStyle"] = "Disabled";
            serie["PieDrawingStyle"] = "Cylinder";
            serie["DoughnutRadius"] = "60";
            chart5.Series.Clear();
            chart5.Series.Add(serie);
            var datos = ObtenerVentasPorVendedoresAnio(year);
            foreach (DataRow row in datos.Rows)
            {
                string vendedor = row.Field<string>("Vendedor");
                decimal totalVentas = row.Field<decimal>("TotalVentas");
                int pointIndex = serie.Points.AddXY(vendedor, totalVentas);
                DataPoint dataPoint = serie.Points[pointIndex];
                dataPoint.Label = $"{vendedor}: {totalVentas:C2}";
                dataPoint.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            }
        }

        private DataTable ObtenerVentasPorVendedoresAnio(int year)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    var ventasPorVendedorQuery = from e in context.Employees
                                                 join o in context.Orders on e.EmployeeID equals o.EmployeeID
                                                 where o.OrderDate.HasValue && o.OrderDate.Value.Year == year
                                                 join od in context.Order_Details on o.OrderID equals od.OrderID
                                                 group od by new { e.FirstName, e.LastName } into g
                                                 let total = g.Sum(x => x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                                                 orderby total descending
                                                 select new
                                                 {
                                                     Vendedor = g.Key.FirstName + " " + g.Key.LastName,
                                                     TotalVentas = total
                                                 };
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
        /******************************************************************************************************/
        private void LlenarCmbTipoGrafica()
        {
            // Obtiene todos los valores del enum
            var tipos = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .OrderBy(t => t.ToString()); // Ordena los tipos de gráfico
            cmbTipoGrafica.DataSource = tipos.ToList();
        }

        private void cmbTipoGrafica_SelectedIndexChanged(object sender, EventArgs e)
        {
            DibujarGraficaChart6((SeriesChartType)cmbTipoGrafica.SelectedItem);
        }

        private void DibujarGraficaChart6(SeriesChartType tipo)
        {
            chart6.Series.Clear();
            chart6.Titles.Clear();
            chart6.Titles.Add(new Title
            {
                Text = $"Tipo de gráfica: {tipo}",
                Docking = Docking.Top,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            });
            var serie = new Series("Ventas")
            {
                ChartType = tipo,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 10,
                ToolTip = "#SERIESNAME\nMes: #AXISLABEL\nVentas: #VALY{C2}"
            };
            for (int i = 0; i < categorias.Length; i++)
            {
                serie.Points.AddXY(categorias[i], valores[i]);
            }
            chart6.Series.Add(serie);
            // Ajusta automáticamente las escalas de ejes
            chart6.ResetAutoValues();
            // Configuración del eje X
            chart6.ChartAreas[0].AxisX.LabelStyle.Angle = -45; // Inclina los labels 45 grados hacia la izquierda
            chart6.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 7); // Fuente más pequeña
            chart6.ChartAreas[0].AxisX.Interval = 1; // Asegura que se muestren todos los meses (cada categoría)
            // Configuración del eje Y
            chart6.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 7); // Fuente más pequeña
            chart6.ChartAreas[0].AxisY.LabelStyle.Format = "$#,##0"; // Formato con símbolo de dólar
            double maxValor = valores.Max();
            // Configura el eje Y para que el máximo sea justo un poco mayor (opcional para espacio visual)
            chart6.ChartAreas[0].AxisY.Maximum = Math.Ceiling(maxValor * 1.0); // 5% de margen por estética
            // Si lo deseas, también puedes fijar el mínimo
            chart6.ChartAreas[0].AxisY.Minimum = 0; // Para que siempre comience en cero
            // Establecer fuente más pequeña para el nombre de la serie en la leyenda
            chart6.Legends[0].Font = new Font("Segoe UI", 7); // Tamaño de fuente reducido
        }
    }
}

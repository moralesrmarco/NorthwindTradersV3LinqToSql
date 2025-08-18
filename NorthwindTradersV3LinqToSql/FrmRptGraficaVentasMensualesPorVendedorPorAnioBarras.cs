using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptGraficaVentasMensualesPorVendedorPorAnioBarras : Form
    {
        public FrmRptGraficaVentasMensualesPorVendedorPorAnioBarras()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaVentasMensualesPorVendedorPorAnioBarras_Load(object sender, EventArgs e)
        {
            LlenarCmbVentasDelAño();
        }

        private void LlenarCmbVentasDelAño()
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
                        CmbVentasDelAño.Items.Add(year);
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
            CmbVentasDelAño.SelectedIndex = 0;
        }

        private void CmbVentasDelAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrafica(Convert.ToInt32(CmbVentasDelAño.SelectedItem.ToString()));
        }

        private void CargarGrafica(int anio)
        {
            groupBox1.Text = $"» Reporte gráfico comparativo de ventas mensuales por vendedores del año {anio} «";
            DataTable dt = ObtenerDatos(anio);
            if (dt != null)
            {
                reportViewer1.LocalReport.DataSources.Clear();
                var rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", $"Ventas mensuales por vendedores del año {anio}"));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Anio", anio.ToString()));
                reportViewer1.RefreshReport();
            }
        }

        private DataTable ObtenerDatos(int year)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    // 1. Definición estática de los meses
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
                    // 2. Vendedores que tuvieron pedidos en el año
                    var vendedores = from e in context.Employees
                                     where context.Orders
                                         .Where(o => o.OrderDate.Value.Year == year)
                                         .Select(o => o.EmployeeID)
                                         .Distinct()
                                         .Contains(e.EmployeeID)
                                     select new
                                     {
                                         e.EmployeeID,
                                         Vendedor = e.FirstName + " " + e.LastName
                                     };
                    // 3. Ventas agregadas por vendedor y mes
                    var ventas = from o in context.Orders
                                 where o.OrderDate.Value.Year == year
                                 from od in o.Order_Details
                                 group od by new
                                 {
                                     o.EmployeeID,
                                     Mes = o.OrderDate.Value.Month
                                 } into g
                                 select new
                                 {
                                     g.Key.EmployeeID,
                                     g.Key.Mes,
                                     TotalVentas = g.Sum(x => x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                                 };
                    // Convertir a listas para evitar múltiples enumeraciones
                    var listaVendedores = vendedores.ToList();
                    var listaVentas = ventas.ToList();

                    // 4. Cruzar vendedores con meses y hacer left join sobre ventas
                    var resultado = from v in listaVendedores
                                    from m in meses
                                    join vt in listaVentas
                                        on new { EmployeeID = (int?)v.EmployeeID, m.Mes } equals new { vt.EmployeeID, vt.Mes }
                                        into ventasPorMes
                                    from vt in ventasPorMes.DefaultIfEmpty()
                                    orderby v.Vendedor, m.Mes
                                    select new
                                    {
                                        v.Vendedor,
                                        Mes = m.Mes,
                                        m.NombreMes,
                                        TotalVentas = vt != null ? vt.TotalVentas : 0m
                                    };
                    dt.Columns.Add("Vendedor", typeof(string));
                    dt.Columns.Add("Mes", typeof(int));
                    dt.Columns.Add("NombreMes", typeof(string));
                    dt.Columns.Add("TotalVentas", typeof(decimal));
                    foreach (var item in resultado)
                    {
                        dt.Rows.Add(item.Vendedor, item.Mes, item.NombreMes, item.TotalVentas);
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

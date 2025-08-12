using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptGraficaDeVentasDeVendedoresPorAnio : Form
    {
        public FrmRptGraficaDeVentasDeVendedoresPorAnio()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaDeVentasDeVendedoresPorAnio_Load(object sender, EventArgs e)
        {
            LlenarCmbVentas();
        }

        private void LlenarCmbVentas() 
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
                        CmbVentas.Items.Add(year);
                    }
                }
                CmbVentas.SelectedIndex = 0;
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

        private void CmbVentas_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenarGrafico(Convert.ToInt32(CmbVentas.SelectedItem.ToString()));
        }

        private void LlenarGrafico(int year) 
        {
            groupBox1.Text = $"» Reporte gráfico ventas por vendedores del año {year} «";
            DataTable dt = ObtenerDatos(year);
            if (dt != null)
            {
                reportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", $"Ventas por vendedores del año {year}"));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Anio", year.ToString()));
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
                    var query = from e in context.Employees
                                join o in context.Orders on e.EmployeeID equals o.EmployeeID
                                join od in context.Order_Details on o.OrderID equals od.OrderID
                                where o.OrderDate.Value.Year == year
                                group od by new { e.FirstName, e.LastName } into g
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

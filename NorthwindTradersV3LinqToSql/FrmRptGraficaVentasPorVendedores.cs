using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptGraficaVentasPorVendedores : Form
    {
        public FrmRptGraficaVentasPorVendedores()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaVentasPorVendedores_Load(object sender, EventArgs e)
        {
            DataTable dt = ObtenerDatos();
            if (dt != null)
            {
                reportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", "Ventas por vendedores de todos los años"));
                reportViewer1.RefreshReport();
            }
        }

        private DataTable ObtenerDatos()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    // Consulta LINQ para obtener las ventas por vendedor
                    var ventasPorVendedor = from e in context.Employees
                                            join o in context.Orders on e.EmployeeID equals o.EmployeeID
                                            join od in context.Order_Details on o.OrderID equals od.OrderID
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
                    // Convertir a DataTable
                    foreach (var item in ventasPorVendedor)
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

using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptGraficaVentasMensuales : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmRptGraficaVentasMensuales()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaVentasMensuales_Load(object sender, EventArgs e)
        {
            LlenarCmbVentasMensualesDelAño();
        }

        private void LlenarCmbVentasMensualesDelAño()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                var years = context.Orders
                    .Where (o => o.OrderDate != null)
                    .Select (o => o.OrderDate.Value.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();
                foreach (var year in years)
                {
                    CmbVentasMensualesDelAño.Items.Add(year);
                }
                CmbVentasMensualesDelAño.SelectedIndex = 0;
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

        private void CmbVentasMensualesDelAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = GetTableGrafica(Convert.ToInt32(CmbVentasMensualesDelAño.SelectedItem.ToString()));
            if (dt != null)
            {
                groupBox1.Text = $"» Reporte gráfico de ventas mensuales del año {CmbVentasMensualesDelAño.SelectedItem} «";
                reportViewer1.LocalReport.DataSources.Clear();
                var rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Anio", CmbVentasMensualesDelAño.SelectedItem.ToString()));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", $"Ventas mensuales del año {CmbVentasMensualesDelAño.SelectedItem}"));
                reportViewer1.RefreshReport();
            }
        }

        private DataTable GetTableGrafica(int año)
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
                // 2. Subconsulta: ventas agrupadas por mes
                var ventasPorMesQuery = from o in context.Orders
                                        where o.OrderDate.HasValue && o.OrderDate.Value.Year == año
                                        join od in context.Order_Details on o.OrderID equals od.OrderID
                                        group od by o.OrderDate.Value.Month into g
                                        select new
                                        {
                                            Mes = g.Key,
                                            Total = g.Sum(x => x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                                        };
                // 3. Left join entre meses y ventasPorMesQuery
                var resultado = from m in meses
                                join v in ventasPorMesQuery on m.Mes equals v.Mes into grp
                                from v in grp.DefaultIfEmpty()
                                orderby m.Mes
                                select new 
                                {
                                    Mes = m.Mes,
                                    NombreMes = m.NombreMes,
                                    Total = v != null ? v.Total : 0m
                                };
                // 4. Construir el DataTable con la misma estructura que tu consulta T-SQL
                var dt = new DataTable();
                dt.Columns.Add("Mes", typeof(int));
                dt.Columns.Add("Total", typeof(decimal));
                dt.Columns.Add("NombreMes", typeof(string));
                foreach (var row in resultado)
                    dt.Rows.Add(row.Mes, row.Total, row.NombreMes);
                return dt;
            }
        }
    }
}

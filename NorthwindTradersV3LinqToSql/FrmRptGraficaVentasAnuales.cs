using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptGraficaVentasAnuales : Form
    {
        public FrmRptGraficaVentasAnuales()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaVentasAnuales_Load(object sender, EventArgs e)
        {
            LlenarCmbVentasAnuales();
        }

        private void LlenarCmbVentasAnuales()
        {
            var items = new List<KeyValuePair<string, int>>();
            for (int i = 2; i <= 8; i++)
                items.Add(new KeyValuePair<string, int>($"{i} Años", i));
            CmbVentasAnuales.DataSource = items;
            CmbVentasAnuales.DisplayMember = "Key";
            CmbVentasAnuales.ValueMember = "Value";
            CmbVentasAnuales.SelectedIndex = 0;
        }

        private void CmbVentasAnuales_SelectedIndexChanged(object sender, EventArgs e)
        {
            var kv = (KeyValuePair<string, int>)CmbVentasAnuales.SelectedItem;
            int years = kv.Value;
            if (years >= 6)
            {
                MessageBox.Show("Solo existen datos en la base de datos hasta el año 1996", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            CargaComparativoVentasAnuales(years);
        }

        private void CargaComparativoVentasAnuales(int years)
        {
            groupBox1.Text = $"» Comparativo de ventas anuales de los últimos {years} años «";
            int year = DateTime.Now.Year;
            List<int> listaAños = new List<int>();
            for (int i = 1; i <= years; i++)
            {
                if (year == 2023)
                    year = 1998;
                else if (year == 1995)
                    break;
                listaAños.Add(year);
                year--;
            }

            DataTable dt = GetVentasComparativas(listaAños);
            reportViewer1.LocalReport.DataSources.Clear();
            var rds = new ReportDataSource("DataSet1", dt);
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Anio", CmbVentasAnuales.Text));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", $"Comparativo de ventas anuales de los últimos {years} años"));
            reportViewer1.RefreshReport();
        }

        private DataTable GetVentasComparativas(List<int> años)
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = new DataTable();
            string filtroAños = string.Join(",", años);
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
                    var ventas = from o in context.Orders
                                 where años.Contains(o.OrderDate.Value.Year)
                                 from od in o.Order_Details
                                 group od by new
                                 {
                                     Año = o.OrderDate.Value.Year,
                                     Mes = o.OrderDate.Value.Month
                                 } into g
                                 select new
                                 {
                                     Año = g.Key.Año,
                                     Mes = g.Key.Mes,
                                     Total = g.Sum(x => x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                                 };
                    var añosDistinct = ventas.Select(v => v.Año).Distinct();
                    var query = from m in meses
                                from y in añosDistinct
                                join v in ventas
                                    on new { m.Mes, Año = y } equals new { v.Mes, v.Año } into gj
                                from v in gj.DefaultIfEmpty()
                                orderby m.Mes, y
                                select new
                                {
                                    Mes = m.Mes,
                                    NombreMes = m.NombreMes,
                                    Año = y,
                                    Total = v != null ? v.Total : 0M
                                };
                    var lista = query.ToList();
                    //                      };
                    // 4. Convertir a DataTable
                    dt.Columns.Add("Mes", typeof(int));
                    dt.Columns.Add("NombreMes", typeof(string));
                    dt.Columns.Add("Año", typeof(int));
                    dt.Columns.Add("Total", typeof(decimal));
                    foreach (var item in lista)
                    {
                        DataRow row = dt.NewRow();
                        row["Mes"] = item.Mes;
                        row["NombreMes"] = item.NombreMes;
                        row["Año"] = item.Año;
                        row["Total"] = item.Total;
                        dt.Rows.Add(row);
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

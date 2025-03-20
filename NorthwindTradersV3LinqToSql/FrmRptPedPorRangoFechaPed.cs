// https://www.youtube.com/watch?v=2-YkNo1Os3Y&list=PL_1AVI-bgZKQ2MSDejVmaaxNenhETwwx_&index=7
// https://www.youtube.com/watch?v=7AvCaq7a1fc&list=PL_1AVI-bgZKQ2MSDejVmaaxNenhETwwx_&index=5
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptPedPorRangoFechaPed : Form
    {
        public FrmRptPedPorRangoFechaPed()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptPedPorRangoFechaPed_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            MostrarReporte();
        }

        private void MostrarReporte()
        {
            string subtitulo;
            if (dateTimePicker1.Checked & dateTimePicker2.Checked)
                subtitulo = $"[ Fecha de pedido inicial: {dateTimePicker1.Value.ToShortDateString()} ] - [ Fecha de pedido final: {dateTimePicker2.Value.ToShortDateString()} ]";
            else
                subtitulo = "[ Fecha de pedido inicial: Nulo ] - [ Fecha de pedido final: Nulo ]";
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = ObtenerPedidosPorFechaPedido(dateTimePicker1.Value, dateTimePicker2.Value);
            MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {dt.Rows.Count} registros");
            if (dt.Rows.Count > 0)
            {
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                ReportParameter reportParameter = new ReportParameter("subtitulo", subtitulo);
                reportViewer1.LocalReport.SetParameters(new ReportParameter[] { reportParameter });
                reportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OrderDetailsSubReportProcessing);
                reportViewer1.RefreshReport();
            }
            else
            {
                reportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", new DataTable());
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                ReportParameter reportParameter = new ReportParameter("subtitulo", subtitulo);
                reportViewer1.LocalReport.SetParameters(new ReportParameter[] { reportParameter });
                reportViewer1.RefreshReport();
                MessageBox.Show(Utils.noDatos, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private DataTable ObtenerPedidosPorFechaPedido(DateTime fIni, DateTime fFin)
        {
            DataTable dt = new DataTable();
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    IQueryable<dynamic> query = null;
                    DateTime fInicial = fIni.Date;
                    DateTime fFinal = fFin.Date.AddDays(1);
                    if (dateTimePicker1.Checked & dateTimePicker2.Checked)
                        query = from o in context.Orders
                                join c in context.Customers on o.CustomerID equals c.CustomerID
                                where o.OrderDate >= fInicial & o.OrderDate < fFinal
                                orderby o.OrderDate descending, c.CompanyName
                                select new
                                {
                                    o.OrderDate,
                                    o.RequiredDate,
                                    o.ShippedDate,
                                    c.CompanyName,
                                    o.OrderID,
                                    o.Freight
                                };
                    else
                        query = from o in context.Orders
                                join c in context.Customers on o.CustomerID equals c.CustomerID
                                where o.OrderDate == null
                                orderby c.CompanyName
                                select new
                                {
                                    o.OrderDate,
                                    o.RequiredDate,
                                    o.ShippedDate,
                                    c.CompanyName,
                                    o.OrderID,
                                    o.Freight
                                };
                    dt = ConvertToDataTable(query.ToList());

                }
            }
            catch (SqlException ex) { Utils.MsgCatchOueclbdd(ex); }
            catch (Exception ex) { Utils.MsgCatchOue(ex); }
            return dt;
        }

        private DataTable ConvertToDataTable(IList<dynamic> data)
        {
            DataTable table = new DataTable();
            if (data == null || !data.Any()) return table;

            var firstRecord = data.First();
            foreach (var prop in firstRecord.GetType().GetProperties())
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var record in data)
            {
                var row = table.NewRow();
                foreach (var prop in record.GetType().GetProperties())
                {
                    row[prop.Name] = prop.GetValue(record) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private void OrderDetailsSubReportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int orderID = int.Parse(e.Parameters["OrderID"].Values[0].ToString());
            DataTable dt = ObtenerDetallePedidoPorOrderID(orderID);
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dt);
            e.DataSources.Add(reportDataSource);
        }

        private DataTable ObtenerDetallePedidoPorOrderID(int orderID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    IQueryable<dynamic> query = from od in context.Order_Details
                                        join p in context.Products on od.ProductID equals p.ProductID
                                        where od.OrderID == orderID
                                        select new
                                        {
                                            p.ProductName,
                                            od.UnitPrice,
                                            od.Quantity,
                                            od.Discount,
                                            Total = (od.Quantity * od.UnitPrice) * ( 1 - Convert.ToDecimal(od.Discount))
                                        };
                    dt = ConvertToDataTable(query.ToList());
                }
            }
            catch (SqlException ex) { Utils.MsgCatchOueclbdd(ex); }
            catch (Exception ex) { Utils.MsgCatchOue(ex); }
            return dt;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Checked)
                dateTimePicker2.Checked = true;
            else
                dateTimePicker2.Checked = false;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker2.Checked)
                dateTimePicker1.Checked = true;
            else
                dateTimePicker1.Checked = false;
        }

        private void dateTimePicker1_Leave(object sender, EventArgs e)
        {
            if (dateTimePicker1.Checked && dateTimePicker2.Checked)
                if (dateTimePicker2.Value < dateTimePicker1.Value)
                    dateTimePicker2.Value = dateTimePicker1.Value;
        }

        private void dateTimePicker2_Leave(object sender, EventArgs e)
        {
            if (dateTimePicker1.Checked && dateTimePicker2.Checked)
                if (dateTimePicker2.Value < dateTimePicker1.Value)
                    dateTimePicker1.Value = dateTimePicker2.Value;
        }
    }
}

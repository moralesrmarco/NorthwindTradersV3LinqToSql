﻿// https://www.youtube.com/watch?v=2-YkNo1Os3Y&list=PL_1AVI-bgZKQ2MSDejVmaaxNenhETwwx_&index=7
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
    public partial class FrmRptPedPorDifCriterios: Form
    {
        public FrmRptPedPorDifCriterios()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptPedPorDifCriterios_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBIdInicial.Text = txtBIdFinal.Text = txtBCliente.Text = txtBEmpleado.Text = txtBCompañiaT.Text = txtBDirigidoa.Text = "";
            dtpBFPedidoIni.Value = dtpBFPedidoFin.Value = dtpBFRequeridoIni.Value = dtpBFRequeridoFin.Value = dtpBFEnvioIni.Value = dtpBFEnvioFin.Value = DateTime.Today;
            dtpBFPedidoIni.Checked = dtpBFPedidoFin.Checked = dtpBFRequeridoIni.Checked = dtpBFRequeridoFin.Checked = dtpBFEnvioIni.Checked = dtpBFEnvioFin.Checked = false;
            chkBFPedidoNull.Checked = chkBFRequeridoNull.Checked = chkBFEnvioNull.Checked = false;
        }

        private void btnMostrarRep_Click(object sender, EventArgs e)
        {
            try
            {
                string subtitulo = string.Empty;
                if (txtBIdInicial.Text != "")
                    subtitulo += $"[Id inicial: {txtBIdInicial.Text}] - [Id final: {txtBIdFinal.Text}] ";
                if (txtBCliente.Text != "")
                    subtitulo += $"[Cliente: %{txtBCliente.Text}%] ";
                if (dtpBFPedidoIni.Checked)
                    subtitulo += $"[Fecha de pedido inicial: {dtpBFPedidoIni.Value.ToShortDateString()}] - [Fecha de pedido final: {dtpBFPedidoFin.Value.ToShortDateString()}] ";
                if (chkBFPedidoNull.Checked)
                    subtitulo += "[Fecha de pedido inicial: Nulo] - [Fecha de pedido final: Nulo] ";
                if (dtpBFRequeridoIni.Checked)
                    subtitulo += $"[Fecha de entrega inicial: {dtpBFRequeridoIni.Value.ToShortDateString()}] - [Fecha de entrega final: {dtpBFRequeridoFin.Value.ToShortDateString()}] ";
                if (chkBFRequeridoNull.Checked)
                    subtitulo += "[Fecha de entrega inicial: Nulo] - [Fecha de entrega final: Nulo] ";
                if (dtpBFEnvioIni.Checked)
                    subtitulo += $"[Fecha de envío inicial: {dtpBFEnvioIni.Value.ToShortDateString()}] - [Fecha de envío final: {dtpBFEnvioFin.Value.ToShortDateString()}] ";
                if (chkBFEnvioNull.Checked)
                    subtitulo += "[Fecha de envío inicial: Nulo] - [Fecha de envío final: Nulo] ";
                if (txtBEmpleado.Text != "")
                    subtitulo += $"[Vendedor: %{txtBEmpleado.Text}%] ";
                if (txtBCompañiaT.Text != "")
                    subtitulo += $"[Transportista: %{txtBCompañiaT.Text}%] ";
                if (txtBDirigidoa.Text != "")
                    subtitulo += $"[Enviar a: %{txtBDirigidoa.Text}%]";
                if (subtitulo == "")
                    subtitulo = "Ningun criterio  de selección fue especificado ( incluye todos los registros de pedidos )";
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DataTable dt = ObtenerPedidos();
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
                    reportViewer1.LocalReport.DataSources.Add (reportDataSource);
                    ReportParameter reportParameter = new ReportParameter("subtitulo", subtitulo);
                    reportViewer1.LocalReport.SetParameters (new ReportParameter[] { reportParameter });
                    reportViewer1.RefreshReport();
                    MessageBox.Show(Utils.noDatos, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information );
                }
            }
            catch (Exception ex) { Utils.MsgCatchOue(ex); }
        }

        private void OrderDetailsSubReportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int orderId = int.Parse(e.Parameters["OrderId"].Values[0].ToString());
            DataTable dt = ObtenerDetallePedidosPorOrderID(orderId);
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dt);
            e.DataSources.Add (reportDataSource);
        }

        private DataTable ObtenerPedidos()
        {
            DataTable dt = new DataTable();
            try
            {
                int intBIdIni = 0, intBIdFin = 0;
                bool boolFPedido = false, boolFRequerido = false, boolFEnvio = false;
                DateTime? FPedidoIni, FPedidoFin, FRequeridoIni, FRequeridoFin, FEnvioIni, FEnvioFin;
                if (txtBIdInicial.Text != "") intBIdIni = int.Parse(txtBIdInicial.Text);
                if (txtBIdFinal.Text != "") intBIdFin = int.Parse(txtBIdFinal.Text);
                if (dtpBFPedidoIni.Checked & dtpBFPedidoFin.Checked)
                {
                    boolFPedido = true;
                    FPedidoIni = dtpBFPedidoIni.Value.Date;
                    FPedidoFin = dtpBFPedidoFin.Value.Date.AddDays(1);
                }
                else
                {
                    boolFPedido = false;
                    FPedidoIni = null;
                    FPedidoFin = null;
                }
                if (dtpBFRequeridoIni.Checked & dtpBFRequeridoFin.Checked)
                {
                    boolFRequerido = true;
                    FRequeridoIni = dtpBFRequeridoIni.Value.Date;
                    FRequeridoFin = dtpBFRequeridoFin.Value.Date.AddDays(1);
                }
                else
                {
                    boolFRequerido = false;
                    FRequeridoIni = null;
                    FRequeridoFin = null;
                }
                if (dtpBFEnvioIni.Checked & dtpBFEnvioFin.Checked)
                {
                    boolFEnvio = true;
                    FEnvioIni = dtpBFEnvioIni.Value.Date;
                    FEnvioFin = dtpBFEnvioFin.Value.Date.AddDays(1);
                }
                else
                {
                    boolFEnvio = false;
                    FEnvioIni = null;
                    FEnvioFin = null;
                }
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    IQueryable<dynamic> query = (IQueryable<dynamic>)context.SP_PEDIDOS_BUSCAR2(intBIdIni, intBIdFin, txtBCliente.Text, boolFPedido, chkBFPedidoNull.Checked, FPedidoIni, FPedidoFin, boolFRequerido, chkBFRequeridoNull.Checked, FRequeridoIni, FRequeridoFin, boolFEnvio, chkBFEnvioNull.Checked, FEnvioIni, FEnvioFin, txtBEmpleado.Text, txtBCompañiaT.Text, txtBDirigidoa.Text).AsQueryable();
                    dt = ConvertToDataTable(query.ToList());
                }
            }
            catch (SqlException ex) { Utils.MsgCatchOueclbdd(ex); }
            catch (Exception ex) { Utils.MsgCatchOue(ex); }
            return dt;
        }

        private DataTable ObtenerDetallePedidosPorOrderID(int orderId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    IQueryable<dynamic> query = from od in context.Order_Details
                                                join p in context.Products on od.ProductID equals p.ProductID
                                                where od.OrderID == orderId
                                                select new
                                                {
                                                    p.ProductName,
                                                    od.UnitPrice,
                                                    od.Quantity,
                                                    od.Discount,
                                                    Total = (od.Quantity * od.UnitPrice) * (1 - Convert.ToDecimal(od.Discount))
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

        private void txtBIdInicial_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBIdInicial_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdIni(txtBIdInicial, txtBIdFinal);

        private void txtBIdFinal_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBIdFinal_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdFin(txtBIdInicial, txtBIdFinal);

        private void chkBFPedidoNull_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBFPedidoNull.Checked)
            {
                dtpBFPedidoIni.Checked = false;
                dtpBFPedidoFin.Checked = false;
            }
        }

        private void chkBFRequeridoNull_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBFRequeridoNull.Checked)
            {
                dtpBFRequeridoIni.Checked = false;
                dtpBFRequeridoFin.Checked = false;
            }
        }

        private void chkBFEnvioNull_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBFEnvioNull.Checked)
            {
                dtpBFEnvioIni.Checked = false;
                dtpBFEnvioFin.Checked = false;
            }
        }

        private void dtpBFPedidoIni_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFPedidoIni.Checked)
            {
                dtpBFPedidoFin.Checked = true;
                chkBFPedidoNull.Checked = false;
            }
            else
                dtpBFPedidoFin.Checked = false;
        }

        private void dtpBFPedidoIni_Leave(object sender, EventArgs e)
        {
            if (dtpBFPedidoIni.Checked && dtpBFPedidoFin.Checked)
                if (dtpBFPedidoFin.Value < dtpBFPedidoIni.Value)
                    dtpBFPedidoFin.Value = dtpBFPedidoIni.Value;
        }

        private void dtpBFPedidoFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFPedidoFin.Checked)
            {
                dtpBFPedidoIni.Checked = true;
                chkBFPedidoNull.Checked = false;
            }
            else
                dtpBFPedidoIni.Checked = false;
        }

        private void dtpBFPedidoFin_Leave(object sender, EventArgs e)
        {
            if (dtpBFPedidoIni.Checked && dtpBFPedidoFin.Checked)
                if (dtpBFPedidoFin.Value < dtpBFPedidoIni.Value)
                    dtpBFPedidoIni.Value = dtpBFPedidoFin.Value;
        }

        private void dtpBFRequeridoIni_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFRequeridoIni.Checked)
            {
                dtpBFRequeridoFin.Checked = true;
                chkBFRequeridoNull.Checked = false;
            }
            else
                dtpBFRequeridoFin.Checked = false;
        }

        private void dtpBFRequeridoIni_Leave(object sender, EventArgs e)
        {
            if (dtpBFRequeridoIni.Checked && dtpBFRequeridoFin.Checked)
                if (dtpBFRequeridoFin.Value < dtpBFRequeridoIni.Value)
                    dtpBFRequeridoFin.Value = dtpBFRequeridoIni.Value;
        }

        private void dtpBFRequeridoFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFRequeridoFin.Checked)
            {
                dtpBFRequeridoIni.Checked = true;
                chkBFRequeridoNull.Checked = false;
            }
            else
                dtpBFRequeridoIni.Checked = false;
        }

        private void dtpBFRequeridoFin_Leave(object sender, EventArgs e)
        {
            if (dtpBFRequeridoIni.Checked && dtpBFRequeridoFin.Checked)
                if (dtpBFRequeridoFin.Value < dtpBFRequeridoIni.Value)
                    dtpBFRequeridoIni.Value = dtpBFRequeridoFin.Value;
        }

        private void dtpBFEnvioIni_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFEnvioIni.Checked)
            {
                dtpBFEnvioFin.Checked = true;
                chkBFEnvioNull.Checked = false;
            }
            else
                dtpBFEnvioFin.Checked = false;
        }

        private void dtpBFEnvioIni_Leave(object sender, EventArgs e)
        {
            if (dtpBFEnvioIni.Checked && dtpBFEnvioFin.Checked)
                if (dtpBFEnvioFin.Value < dtpBFEnvioIni.Value)
                    dtpBFEnvioFin.Value = dtpBFEnvioIni.Value;
        }

        private void dtpBFEnvioFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFEnvioFin.Checked)
            {
                dtpBFEnvioIni.Checked = true;
                chkBFEnvioNull.Checked = false;
            }
            else
                dtpBFEnvioIni.Checked = false;
        }

        private void dtpBFEnvioFin_Leave(object sender, EventArgs e)
        {
            if (dtpBFEnvioIni.Checked && dtpBFEnvioFin.Checked)
                if (dtpBFEnvioFin.Value < dtpBFEnvioIni.Value)
                    dtpBFEnvioIni.Value = dtpBFEnvioFin.Value;
        }

        private void FrmRptPedPorDifCriterios_Load(object sender, EventArgs e)
        {

        }
    }
}

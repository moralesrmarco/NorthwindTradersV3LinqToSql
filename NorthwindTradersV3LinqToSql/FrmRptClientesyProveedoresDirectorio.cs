using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptClientesyProveedoresDirectorio: Form
    {

        string titulo = string.Empty;

        public FrmRptClientesyProveedoresDirectorio()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptClientesyProveedoresDirectorio_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (!checkBoxClientes.Checked && !checkBoxProveedores.Checked)
            {
                MessageBox.Show(Utils.errorCriterioSelec, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                    IQueryable<dynamic> query = null; // Inicializar la variable query
                    if (checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD_RPT
                                    orderby cliprov.Relacion, cliprov.NombreCompania
                                    select new
                                    {
                                        cliprov.Ciudad,
                                        cliprov.Pais,
                                        cliprov.NombreCompania,
                                        cliprov.NombreContacto,
                                        cliprov.Relacion,
                                        cliprov.Telefono,
                                        cliprov.Domicilio,
                                        cliprov.Region,
                                        cliprov.CodigoPostal,
                                        cliprov.Fax
                                    };
                        titulo = "» Reporte directorio de clientes y proveedores «";
                    }
                    else if (checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD_RPT
                                    where cliprov.Relacion == "Cliente"
                                    orderby cliprov.NombreCompania
                                    select new
                                    {
                                        cliprov.Ciudad,
                                        cliprov.Pais,
                                        cliprov.NombreCompania,
                                        cliprov.NombreContacto,
                                        cliprov.Relacion,
                                        cliprov.Telefono,
                                        cliprov.Domicilio,
                                        cliprov.Region,
                                        cliprov.CodigoPostal,
                                        cliprov.Fax
                                    };
                        titulo = "» Reporte directorio de clientes «";
                    }
                    else if (!checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD_RPT
                                    where cliprov.Relacion == "Proveedor"
                                    orderby cliprov.NombreCompania
                                    select new
                                    {
                                        cliprov.Ciudad,
                                        cliprov.Pais,
                                        cliprov.NombreCompania,
                                        cliprov.NombreContacto,
                                        cliprov.Relacion,
                                        cliprov.Telefono,
                                        cliprov.Domicilio,
                                        cliprov.Region,
                                        cliprov.CodigoPostal,
                                        cliprov.Fax
                                    };
                        titulo = "» Reporte directorio de proveedores «";
                    }
                    if (query != null)
                    {
                        groupBox1.Text = titulo;
                        var clientes = query.ToList();
                        Utils.ActualizarBarraDeEstado(this, $"Se encontraron {query.Count()} registros");
                        ReportDataSource reportDataSource = new ReportDataSource("DataSet1", clientes);
                        reportViewer1.LocalReport.DataSources.Clear();
                        reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                        ReportParameter rp = new ReportParameter("titulo", titulo);
                        reportViewer1.LocalReport.SetParameters(rp);
                        reportViewer1.LocalReport.Refresh();
                        reportViewer1.RefreshReport();
                    }
                    else
                    {
                        titulo = "» Reporte directorio de clientes y proveedores «";
                        groupBox1.Text = titulo;
                        Utils.ActualizarBarraDeEstado(this);
                        reportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", new DataTable());
                        reportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter rp = new ReportParameter("titulo", titulo);
                        reportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
                        reportViewer1.LocalReport.Refresh();
                        reportViewer1.RefreshReport();
                        MessageBox.Show(Utils.noDatos, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (SqlException ex)
            {
                Utils.MsgCatchOueclbdd(this, ex);
            }
            catch (Exception ex)
            {
                Utils.MsgCatchOue(this, ex);
            }
        }
    }
}

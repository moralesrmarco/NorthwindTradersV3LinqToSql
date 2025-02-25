using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptClientesyProveedoresDirectorioxPais: Form
    {
        public FrmRptClientesyProveedoresDirectorioxPais()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptClientesyProveedoresDirectorioxPais_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);

        private void FrmRptClientesyProveedoresDirectorioxPais_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
        }

        private void LlenarComboBox()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    var query = context.SP_CLIENTESPROVEEDORES_PAIS();
                    comboBox.DataSource = query;
                    comboBox.DisplayMember = "País";
                    comboBox.ValueMember = "IdPaís";
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
            finally
            {
                Utils.ActualizarBarraDeEstado(this);
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex <= 0 | (!checkBoxClientes.Checked & !checkBoxProveedores.Checked))
            {
                MessageBox.Show(Utils.errorCriterioSelec, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string titulo = string.Empty;
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    IQueryable<dynamic> query = null; // Inicializar la variable query
                    if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS_RPT
                                orderby cliprov.Pais, cliprov.Ciudad, cliprov.NombreCompania
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
                        titulo = "» Reporte directorio de clientes y proveedores por país [ Todos los países ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS_RPT
                                where cliprov.Pais == comboBox.SelectedValue.ToString()
                                orderby cliprov.Ciudad, cliprov.NombreCompania
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
                        titulo = $"» Reporte directorio de clientes y proveedores por país [ País: {comboBox.SelectedValue.ToString()} ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS_RPT
                                where cliprov.Relacion == "Cliente"
                                orderby cliprov.Pais, cliprov.Ciudad, cliprov.NombreCompania
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
                        titulo = "» Reporte directorio de clientes por país [ Todos los países ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() == "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS_RPT
                                where cliprov.Relacion == "Proveedor"
                                orderby cliprov.Pais, cliprov.Ciudad, cliprov.NombreCompania
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
                        titulo = "» Reporte directorio de proveedores por país [ Todos los países ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS_RPT
                                where cliprov.Pais == comboBox.SelectedValue.ToString() & cliprov.Relacion == "Cliente"
                                orderby cliprov.Ciudad, cliprov.NombreCompania
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
                        titulo = $"» Reporte directorio de clientes por país [ País: {comboBox.SelectedValue.ToString()} ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() != "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORPAIS_RPT
                                where cliprov.Pais == comboBox.SelectedValue.ToString() & cliprov.Relacion == "Proveedor"
                                orderby cliprov.Ciudad, cliprov.NombreCompania
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
                        titulo = $"» Reporte directorio de proveedores por país [ País: {comboBox.SelectedValue.ToString()} ] «";
                    }
                    groupBox1.Text = titulo;
                    Utils.ActualizarBarraDeEstado(this, $"Se encontraron {query.Count()} registros");
                    if (query.Count() > 0)
                    {
                        var clientesProveedores = query.ToList();
                        ReportDataSource reportDataSource = new ReportDataSource("DataSet1", clientesProveedores);
                        reportViewer1.LocalReport.DataSources.Clear();
                        reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                        ReportParameter rp = new ReportParameter("titulo", titulo);
                        reportViewer1.LocalReport.SetParameters(rp);
                        reportViewer1.LocalReport.Refresh();
                        reportViewer1.RefreshReport();
                    }
                    else
                    {
                        reportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource reportDataSource = new ReportDataSource("DataSet1", new DataTable());
                        reportViewer1.LocalReport.DataSources.Add(reportDataSource);
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

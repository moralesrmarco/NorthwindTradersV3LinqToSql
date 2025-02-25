using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptClientesyProveedoresDirectorioxCiudad: Form
    {

        public FrmRptClientesyProveedoresDirectorioxCiudad()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptClientesyProveedoresDirectorioxCiudad_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);

        private void FrmRptClientesyProveedoresDirectorioxCiudad_Load(object sender, EventArgs e)
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
                    var query = context.SP_CLIENTESPROVEEDORES_CIUDAD();
                    comboBox.DataSource = query;
                    comboBox.DisplayMember = "CiudadPaís";
                    comboBox.ValueMember = "Ciudad";
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
            if (comboBox.SelectedIndex <= 0 | (checkBoxClientes.Checked == false & checkBoxProveedores.Checked == false))
            {
                MessageBox.Show(Utils.errorCriterioSelec, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string titulo = string.Empty;
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    IQueryable<dynamic> query = null; // Inicializar la variable query
                    if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD_RPT
                                orderby cliprov.Ciudad, cliprov.Pais, cliprov.NombreCompania
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
                        titulo = "» Reporte directorio de clientes y proveedores por ciudad [ Todas las ciudades ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD_RPT
                                where cliprov.Ciudad == comboBox.SelectedValue.ToString()
                                orderby cliprov.Pais, cliprov.NombreCompania
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
                        titulo = $"» Reporte directorio de clientes y proveedores por ciudad [ Ciudad: {comboBox.SelectedValue.ToString()} ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD_RPT
                                where cliprov.Relacion == "Cliente"
                                orderby cliprov.Ciudad, cliprov.Pais, cliprov.NombreCompania
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
                        titulo = "» Reporte directorio de clientes por ciudad [ Todas las ciudades ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() == "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD_RPT
                                where cliprov.Relacion == "Proveedor"
                                orderby cliprov.Ciudad, cliprov.Pais, cliprov.NombreCompania
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
                        titulo = "» Reporte directorio de proveedores por ciudad [ Todas las ciudades ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD_RPT
                                where cliprov.Ciudad == comboBox.SelectedValue.ToString() & cliprov.Relacion == "Cliente"
                                orderby cliprov.Pais, cliprov.NombreCompania
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
                        titulo = $"» Reporte directorio de clientes por ciudad [ Ciudad: {comboBox.SelectedValue.ToString()} ] «";
                    }
                    else if (comboBox.SelectedValue.ToString() != "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    {
                        query = from cliprov in context.VW_CLIENTESPROVEEDORES_DIRECTORIOPORCIUDAD_RPT
                                where cliprov.Ciudad == comboBox.SelectedValue.ToString() & cliprov.Relacion == "Proveedor"
                                orderby cliprov.Pais, cliprov.NombreCompania
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
                        titulo = $"» Reporte directorio de proveedores por ciudad [ Ciudad: {comboBox.SelectedValue.ToString()} ] «";
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

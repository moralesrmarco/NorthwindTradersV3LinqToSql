using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptProductos: Form
    {

        private bool ImprimirTodo = true;
        private string titulo = "» Reporte de productos «";
        private string subtitulo = "";

        public FrmRptProductos()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptProductos_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void txtIdInicial_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtIdFinal_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtIdInicial_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdIni(txtIdInicial, txtIdFinal);

        private void txtIdFinal_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdFin(txtIdInicial, txtIdFinal);

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e) => btnLimpiar.PerformClick();

        private void tabcOperacion_SelectedIndexChanged(object sender, EventArgs e) => btnLimpiar.PerformClick();

        private void FrmRptProductos_Load(object sender, EventArgs e)
        {
            LlenarCboCategoria();
            LlenarCboProveedor();
        }

        private void LlenarCboCategoria()
        {
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    //var query = context.SP_CATEGORIAS_SELECCIONAR_V2();
                    var categorias = from cat in context.Categories
                                     orderby cat.CategoryName
                                     select new
                                     {
                                         Id = cat.CategoryID,
                                         Categoria = cat.CategoryName
                                     };
                    var categoriasFinal = categorias.ToList();
                    categoriasFinal.Insert(0, new { Id = 0, Categoria = "«--- Seleccione ---»" });
                    cboCategoria.DataSource = categoriasFinal;
                    cboCategoria.DisplayMember = "Categoria";
                    cboCategoria.ValueMember = "Id";
                    MDIPrincipal.ActualizarBarraDeEstado();
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
        }

        private void LlenarCboProveedor()
        {
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    var proveedores = from prov in context.Suppliers
                                      orderby prov.CompanyName
                                      select new
                                      {
                                          Id = prov.SupplierID,
                                          Proveedor = prov.CompanyName
                                      };
                    var proveedoresFinal = proveedores.ToList();
                    proveedoresFinal.Insert(0, new { Id = 0, Proveedor = "«--- Seleccione ---»" });
                    cboProveedor.DataSource = proveedoresFinal;
                    cboProveedor.DisplayMember = "Proveedor";
                    cboProveedor.ValueMember = "Id";
                    MDIPrincipal.ActualizarBarraDeEstado();
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
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtIdInicial.Text = txtIdFinal.Text = txtProducto.Text = "";
            cboCategoria.SelectedIndex = cboProveedor.SelectedIndex = 0;
            MDIPrincipal.ActualizarBarraDeEstado();
        }

        private void btnImprimirTodos_Click(object sender, EventArgs e)
        {
            ImprimirTodo = true;
            LlenarReporte();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            ImprimirTodo = false;
            LlenarReporte();
        }

        private void LlenarReporte()
        {
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    IQueryable<dynamic> query = null; // Inicializar la variable query
                    if (ImprimirTodo)
                    {
                        titulo = "» Reporte de todos los productos «";
                        groupBox1.Text = titulo;
                        subtitulo = "";
                        query = from prod in context.Products
                                join cat in context.Categories on prod.CategoryID equals cat.CategoryID into prodCat
                                from cat in prodCat.DefaultIfEmpty()
                                join prov in context.Suppliers on prod.SupplierID equals prov.SupplierID into prodProv
                                from prov in prodProv.DefaultIfEmpty()
                                orderby prod.ProductID ascending
                                select new
                                {
                                    Id = prod.ProductID,
                                    prod.ProductName,
                                    prod.QuantityPerUnit,
                                    prod.UnitPrice,
                                    prod.UnitsInStock,
                                    prod.UnitsOnOrder,
                                    prod.ReorderLevel,
                                    prod.Discontinued,
                                    CategoryName = cat != null ? cat.CategoryName : null,
                                    CompanyName = prov != null ? prov.CompanyName : null
                                };
                    }
                    else
                    {
                        titulo = "» Reporte de productos filtrados «";
                        subtitulo = $"Filtrado por: ";
                        if (txtIdInicial.Text != "" & txtIdFinal.Text != "")
                            subtitulo += $" [ Id: {txtIdInicial.Text} al {txtIdFinal.Text} ] ";
                        if (txtProducto.Text != "")
                            subtitulo += $" [ Producto: {txtProducto.Text} ] ";
                        if (cboCategoria.SelectedIndex > 0)
                            subtitulo += $" [ Categoría: {cboCategoria.Text}] ";
                        if (cboProveedor.SelectedIndex > 0)
                            subtitulo += $" [ Proveedor: {cboProveedor.Text}] ";
                        if (subtitulo == "Filtrado por: ")
                        {
                            titulo = "» Reporte de todos los productos «";
                            subtitulo = "";
                        }
                        groupBox1.Text = titulo;
                        int idIni = txtIdInicial.Text == "" ? 0 : Convert.ToInt32(txtIdInicial.Text);
                        int idFin = txtIdFinal.Text == "" ? 0 : Convert.ToInt32(txtIdFinal.Text);
                        int categoria = Convert.ToInt32(cboCategoria.SelectedValue);
                        int proveedor = Convert.ToInt32(cboProveedor.SelectedValue);
                        query = from prod in context.Products
                                join cat in context.Categories on prod.CategoryID equals cat.CategoryID into prodCat
                                from cat in prodCat.DefaultIfEmpty()
                                join prov in context.Suppliers on prod.SupplierID equals prov.SupplierID into prodProv
                                from prov in prodProv.DefaultIfEmpty()
                                where (idIni == 0 || (prod.ProductID >= idIni & prod.ProductID <= idFin)) &&
                                      (string.IsNullOrEmpty(txtProducto.Text) || prod.ProductName.Contains(txtProducto.Text)) &&
                                      (categoria == 0 || prod.CategoryID == categoria) &&
                                      (proveedor == 0 || prod.SupplierID == proveedor)
                                orderby prod.ProductID ascending
                                select new
                                {
                                    Id = prod.ProductID,
                                    prod.ProductName,
                                    prod.QuantityPerUnit,
                                    prod.UnitPrice,
                                    prod.UnitsInStock,
                                    prod.UnitsOnOrder,
                                    prod.ReorderLevel,
                                    prod.Discontinued,
                                    CategoryName = cat != null ? cat.CategoryName : null,
                                    CompanyName = prov != null ? prov.CompanyName : null
                                };
                    }
                    var productos = query.ToList();
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {query.Count()} registros");
                    if (query.Count() > 0)  
                    {
                        ReportDataSource reportDataSource = new ReportDataSource("DataSet1", productos);
                        reportViewer1.LocalReport.DataSources.Clear();
                        reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                        ReportParameter rp = new ReportParameter("titulo", titulo);
                        ReportParameter rp2 = new ReportParameter("subtitulo", subtitulo);
                        reportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp2 });
                        reportViewer1.RefreshReport();
                    }
                    else
                    {
                        reportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource reportDataSource = new ReportDataSource("DataSet1", new DataTable());
                        reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                        ReportParameter rp = new ReportParameter("titulo", titulo);
                        ReportParameter rp2 = new ReportParameter("subtitulo", subtitulo);
                        reportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp2 });
                        reportViewer1.RefreshReport();
                        MessageBox.Show(Utils.noDatos, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (SqlException ex) { Utils.MsgCatchOueclbdd(ex); }
            catch (Exception ex) { Utils.MsgCatchOue(ex); }
        }
    }
}

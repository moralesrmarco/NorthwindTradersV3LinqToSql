using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptProductosAlfabetico: Form
    {
        public FrmRptProductosAlfabetico()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptProductosAlfabetico_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptProductosAlfabetico_Load(object sender, EventArgs e)
        {
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    var query = from prod in context.Products
                                join cat in context.Categories on prod.CategoryID equals cat.CategoryID into prodCat
                                from cat in prodCat.DefaultIfEmpty()
                                join prov in context.Suppliers on prod.SupplierID equals prov.SupplierID into prodProv
                                from prov in prodProv.DefaultIfEmpty()
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
                    var productos = query.ToList();
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {query.Count()} registros");
                    if (query.Count() > 0)
                    {
                        ReportDataSource reportDataSource = new ReportDataSource("DataSet1", productos);
                        reportViewer1.LocalReport.DataSources.Clear();
                        reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                        reportViewer1.RefreshReport();
                    }
                    else
                    {
                        reportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource reportDataSource = new ReportDataSource("DataSet1", new DataTable());
                        reportViewer1.LocalReport.DataSources.Add(reportDataSource);
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

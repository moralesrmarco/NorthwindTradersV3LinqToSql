using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptProdPorProvConDetProv: Form
    {
        public FrmRptProdPorProvConDetProv()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptProdPorProvConDetProv_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptProdPorProvConDetProv_Load(object sender, EventArgs e)
        {
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    var query = from sup in context.Suppliers
                                join prod in context.Products on sup.SupplierID equals prod.SupplierID into productGroup
                                from prod in productGroup.DefaultIfEmpty()
                                join cat in context.Categories on prod.CategoryID equals cat.CategoryID into categoryGroup
                                from cat in categoryGroup.DefaultIfEmpty()
                                orderby sup.CompanyName, prod.ProductName
                                select new
                                {
                                    sup.SupplierID,
                                    sup.CompanyName,
                                    sup.ContactName,
                                    sup.ContactTitle,
                                    sup.Address,
                                    sup.City,
                                    sup.Region,
                                    sup.PostalCode,
                                    sup.Country,
                                    sup.Phone,
                                    sup.Fax,
                                    ProductID = (int?)prod.ProductID,
                                    prod.ProductName,
                                    prod.QuantityPerUnit,
                                    UnitPrice = (decimal?)prod.UnitPrice,
                                    UnitsInStock = (short?)prod.UnitsInStock,
                                    UnitsOnOrder = (short?)prod.UnitsOnOrder,
                                    ReorderLevel = (short?)prod.ReorderLevel,
                                    Discontinued = (bool?)prod.Discontinued,
                                    cat.CategoryName
                                };
                    var prodPorProv = query.ToList();
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {query.Count()} registros");
                    if (query.Count() > 0)
                    {
                        ReportDataSource reportDataSource = new ReportDataSource("DataSet1", prodPorProv);
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

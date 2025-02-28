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
    public partial class FrmRptProductosPorCategorias: Form
    {
        public FrmRptProductosPorCategorias()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptProductosPorCategorias_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptProductosPorCategorias_Load(object sender, EventArgs e)
        {
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    // esta seria la sentencia linq to sql si usaramos la vista VW_PRODUCTOSPORCATEGORIALISTADO_RPT
                    //var productos = from CategoriesProducts in context.VW_PRODUCTOSPORCATEGORIALISTADO_RPT
                    //                orderby CategoriesProducts.CategoryName, CategoriesProducts.ProductName
                    //                select new
                    //                {
                    //                    CategoriesProducts.CategoryName,
                    //                    CategoriesProducts.ProductName,
                    //                    CategoriesProducts.QuantityPerUnit,
                    //                    CategoriesProducts.UnitPrice,
                    //                    CategoriesProducts.UnitsInStock,
                    //                    CategoriesProducts.UnitsOnOrder,
                    //                    CategoriesProducts.ReorderLevel,
                    //                    CategoriesProducts.Discontinued,
                    //                    CategoriesProducts.CompanyName
                    //                };
                    // esta seria la sentencia linq to sql si no usaramos la vista VW_PRODUCTOSPORCATEGORIALISTADO_RPT
                    var productos = (from categories in context.Categories
                                     join products in context.Products on categories.CategoryID equals products.CategoryID into categoryProducts
                                     from products in categoryProducts.DefaultIfEmpty()
                                     join suppliers in context.Suppliers on products.SupplierID equals suppliers.SupplierID into suppliersProducts
                                     from suppliers in suppliersProducts.DefaultIfEmpty()
                                     orderby categories.CategoryName, products != null ? products.ProductName : "N/A"
                                     select new
                                     {
                                         CategoryName = categories.CategoryName ?? "N/A",
                                         ProductID = products != null ? products.ProductID : 0,
                                         ProductName = products != null ? products.ProductName : "N/A",
                                         products.QuantityPerUnit,
                                         products.UnitPrice,
                                         products.UnitsInStock,
                                         products.UnitsOnOrder,
                                         products.ReorderLevel,
                                         Discontinued = products != null ? products.Discontinued : false,
                                         CompanyName = suppliers != null ? suppliers.CompanyName : "N/A"
                                     }).Take(100).ToList(); // Esto es equivalente a SELECT TOP (100) PERCENT
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {productos.Count()} registros");
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", productos);
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer1.RefreshReport();
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
    }
}

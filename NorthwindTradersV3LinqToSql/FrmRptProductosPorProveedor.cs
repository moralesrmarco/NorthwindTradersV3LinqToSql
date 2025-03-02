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
    public partial class FrmRptProductosPorProveedor: Form
    {
        public FrmRptProductosPorProveedor()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptProductosPorProveedor_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptProductosPorProveedor_Load(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    var query = (from p in context.Products
                                join c in context.Categories on p.CategoryID equals c.CategoryID into pc
                                from c in pc.DefaultIfEmpty()
                                join s in context.Suppliers on p.SupplierID equals s.SupplierID into ps
                                from s in ps.DefaultIfEmpty()
                                select new
                                {
                                    CompanyName = s.CompanyName,
                                    ProductID = p.ProductID,
                                    ProductName = p.ProductName,
                                    QuantityPerUnit = p.QuantityPerUnit,
                                    UnitPrice = p.UnitPrice,
                                    UnitsInStock = p.UnitsInStock,
                                    UnitsOnOrder = p.UnitsOnOrder,
                                    ReorderLevel = p.ReorderLevel,
                                    Discontinued = p.Discontinued,
                                    CategoryName = c.CategoryName
                                }).ToList();
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {query.Count()} registros");
                    if (query.Count() > 0)
                    {
                        ReportDataSource reportDataSource = new ReportDataSource("DataSet1", query);
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

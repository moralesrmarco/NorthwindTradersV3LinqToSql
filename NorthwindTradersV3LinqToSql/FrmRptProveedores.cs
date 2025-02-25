using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptProveedores: Form
    {
        public FrmRptProveedores()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptProveedores_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);

        private void FrmRptProveedores_Load(object sender, EventArgs e)
        {
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                    var query = from prov in context.Suppliers
                                orderby prov.CompanyName
                                select new
                                {
                                    SupplierID = prov.SupplierID,
                                    CompanyName = prov.CompanyName,
                                    ContactName = prov.ContactName,
                                    ContactTitle = prov.ContactTitle,
                                    Address = prov.Address,
                                    City = prov.City,
                                    Region = prov.Region,
                                    PostalCode = prov.PostalCode,
                                    Country = prov.Country,
                                    Phone = prov.Phone,
                                    Fax = prov.Fax
                                };
                    var proveedores = query.ToList();
                    Utils.ActualizarBarraDeEstado(this, $"Se encontraron {query.Count()} registros");
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", proveedores);
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer1.RefreshReport();
                }
            }
            catch(SqlException ex)
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

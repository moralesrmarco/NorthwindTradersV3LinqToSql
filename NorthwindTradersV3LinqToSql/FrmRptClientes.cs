using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptClientes: Form
    {


        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmRptClientes()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptClientes_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);

        private void FrmRptClientes_Load(object sender, EventArgs e)
        {
            try
            {
                using (context)
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                    var query = from cli in context.Customers
                                orderby cli.CustomerID
                                select new 
                                {
                                    CustomerID = cli.CustomerID,
                                    CompanyName = cli.CompanyName,
                                    ContactName = cli.ContactName,
                                    ContactTitle = cli.ContactTitle,
                                    Address = cli.Address,
                                    City = cli.City,
                                    Region = cli.Region,
                                    PostalCode = cli.PostalCode,
                                    Country = cli.Country,
                                    Phone = cli.Phone,
                                    Fax = cli.Fax
                                };
                    var clientes = query.ToList();
                    Utils.ActualizarBarraDeEstado(this, $"Se encontraron {query.Count()} registros");
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", clientes);
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer1.RefreshReport();
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

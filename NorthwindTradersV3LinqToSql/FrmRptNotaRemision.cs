using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptNotaRemision: Form
    {

        public int Id;

        public FrmRptNotaRemision()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptNotaRemision_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptNotaRemision_Load(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DataTable dt = ObtenerPedido(Id);
                MDIPrincipal.ActualizarBarraDeEstado($"Se encontró el Pedido con Id: {Id}");
                if (dt.Rows.Count > 0)
                {
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dt);
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OrderDetailsSubReportProcessing);
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
            catch (Exception ex) { Utils.MsgCatchOue(ex); }
        }

        private DataTable ObtenerPedido(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    var query = from order in context.Orders
                                join employee in context.Employees on order.EmployeeID equals employee.EmployeeID into empJoin
                                from employee in empJoin.DefaultIfEmpty()
                                join shipper in context.Shippers on order.ShipVia equals shipper.ShipperID into shipJoin
                                from shipper in shipJoin.DefaultIfEmpty()
                                join customer in context.Customers on order.CustomerID equals customer.CustomerID into custJoin
                                from customer in custJoin.DefaultIfEmpty()
                                where order.OrderID == id
                                select new
                                {
                                    Id = order.OrderID,
                                    Cliente = customer.CompanyName,
                                    Vendedor = employee.LastName + ", " + employee.FirstName,
                                    FechaDePedido = order.OrderDate,
                                    FechaRequerido = order.RequiredDate,
                                    FechaDeEnvio = order.ShippedDate,
                                    CompaniaTransportista = shipper.CompanyName,
                                    DirigidoA = order.ShipName,
                                    Domicilio = order.ShipAddress,
                                    Ciudad = order.ShipCity,
                                    Region = order.ShipRegion,
                                    CodigoPostal = order.ShipPostalCode,
                                    Pais = order.ShipCountry,
                                    Flete = order.Freight
                                };
                    var listaResultado = query.ToList();
                    dt = listaResultado.ToDataTable();
                }
            }
            catch (SqlException ex) { Utils.MsgCatchOueclbdd(ex); }
            catch (Exception ex) { Utils.MsgCatchOue(ex); }
            return dt;
        }

        private void OrderDetailsSubReportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int orderId = int.Parse(e.Parameters["OrderID"].Values[0].ToString());
            DataTable dt = ObtenerDetallePedidoPorOrderID(orderId);
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dt);
            e.DataSources.Add(reportDataSource);
        }

        private DataTable ObtenerDetallePedidoPorOrderID(int orderId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    var query = from od in context.Order_Details
                                join p in context.Products on od.ProductID equals p.ProductID
                                where od.OrderID == orderId
                                select new
                                {
                                    ProductName = p.ProductName,
                                    UnitPrice = od.UnitPrice,
                                    Quantity = od.Quantity,
                                    Discount = od.Discount,
                                    Total = (od.Quantity * od.UnitPrice) * ( 1M - (decimal)od.Discount)
                                };
                    var listaResultado = query.ToList();
                    dt = listaResultado.ToDataTable();
                }
            }
            catch (SqlException ex) { Utils.MsgCatchOueclbdd(ex); }
            catch (Exception ex) { Utils.MsgCatchOue(ex); }
            return dt;
        }
    }
}

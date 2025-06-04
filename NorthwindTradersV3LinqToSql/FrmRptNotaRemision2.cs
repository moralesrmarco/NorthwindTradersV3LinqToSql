using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptNotaRemision2 : Form
    {

        public int Id;

        public FrmRptNotaRemision2()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptNotaRemision2_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptNotaRemision2_Load(object sender, EventArgs e)
        {
            ReportParameter[] parameters = new ReportParameter[1];
            parameters[0] = new ReportParameter("PedidoId", Id.ToString());
            reportViewer1.LocalReport.SetParameters(parameters);
            DataTable dt1 = ObtenerPedidoId(Id);
            ReportDataSource rds1 = new ReportDataSource("DataSet1", dt1);
            reportViewer1.LocalReport.DataSources.Add(rds1);
            DataTable dt2 = ObtenerDetallePedidoPorOrderID(Id);
            ReportDataSource rds2 = new ReportDataSource("DataSet2", dt2);
            reportViewer1.LocalReport.DataSources.Add(rds2);
            reportViewer1.LocalReport.Refresh();
            reportViewer1.RefreshReport();
        }

        private DataTable ObtenerPedidoId(int id)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id",  typeof(int));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("Vendedor", typeof (string));
            dt.Columns.Add("FechaDePedido", typeof(DateTime));
            dt.Columns.Add("FechaRequerido", typeof(DateTime));
            dt.Columns.Add("FechaDeEnvio", typeof(DateTime));
            dt.Columns.Add("CompaniaTransportista", typeof(string));
            dt.Columns.Add("DirigidoA", typeof(string));
            dt.Columns.Add("Domicilio", typeof(string));
            dt.Columns.Add("Ciudad", typeof(string));
            dt.Columns.Add("Region", typeof(string));
            dt.Columns.Add("CodigoPostal", typeof(string));
            dt.Columns.Add("Pais", typeof(string));
            dt.Columns.Add("Flete", typeof(decimal));
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    var resultado = (from order in context.Orders
                                     join employee in context.Employees on order.EmployeeID equals employee.EmployeeID into empGroup
                                     from employee in empGroup.DefaultIfEmpty()
                                     join shipper in context.Shippers on order.ShipVia equals shipper.ShipperID into shipGroup
                                     from shipper in shipGroup.DefaultIfEmpty()
                                     join customer in context.Customers on order.CustomerID equals customer.CustomerID into custGroup
                                     from customer in custGroup.DefaultIfEmpty()
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
                                     }).SingleOrDefault();
                    if (resultado !=  null)
                        dt.Rows.Add(resultado.Id, resultado.Cliente, resultado.Vendedor, resultado.FechaDePedido,
                            resultado.FechaRequerido, resultado.FechaDeEnvio, resultado.CompaniaTransportista,
                            resultado.DirigidoA, resultado.Domicilio, resultado.Ciudad, resultado.Region,
                            resultado.CodigoPostal, resultado.Pais, resultado.Flete);
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
            //return dt.Rows.Count > 0 ? dt : throw new ApplicationException("No se pudo recuperar información del pedido.");
            return dt;
        }

        private DataTable ObtenerDetallePedidoPorOrderID(int orderId)
        {
            // Definimos el esquema del DataTable con las columnas necesarias
            DataTable dt = new DataTable();
            dt.Columns.Add("NombreProducto", typeof(string));
            dt.Columns.Add("PrecioUnitario", typeof(decimal));
            dt.Columns.Add("Cantidad", typeof(short));
            dt.Columns.Add("Descuento", typeof(float));
            dt.Columns.Add("Total", typeof(decimal));
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    // Actualizamos la barra de estado (esto es opcional según tu implementación)
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);

                    // Ejecutamos la consulta LINQ haciendo la unión entre detalles de pedido y productos
                    var detalles = (from od in context.Order_Details
                                    join p in context.Products on od.ProductID equals p.ProductID
                                    where od.OrderID == orderId
                                    select new
                                    {
                                        p.ProductName,
                                        od.UnitPrice,
                                        od.Quantity,
                                        od.Discount,
                                        Total = (od.Quantity * od.UnitPrice) * (1M - (decimal)od.Discount)
                                    }).ToList();

                    // Recorremos el resultado y llenamos el DataTable
                    foreach (var item in detalles)
                    {
                        dt.Rows.Add(item.ProductName, item.UnitPrice, item.Quantity, item.Discount, item.Total);
                    }

                    // Actualizamos la barra de estado al finalizar
                    MDIPrincipal.ActualizarBarraDeEstado();

                }
            }
            catch (SqlException ex)
            {
                // Manejamos errores específicos de SQL
                Utils.MsgCatchOueclbdd(ex);
            }
            catch (Exception ex)
            {
                // Manejamos cualquier otro error
                Utils.MsgCatchOue(ex);
            }
            return dt;
        }
    }
}

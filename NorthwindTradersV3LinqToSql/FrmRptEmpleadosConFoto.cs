using Microsoft.Reporting.WinForms;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptEmpleadosConFoto: Form
    {
        public FrmRptEmpleadosConFoto()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptEmpleadosConFoto_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);

        private void FrmRptEmpleadosConFoto_Load(object sender, EventArgs e)
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                // Obtener la lista de empleados usando LINQ to SQL
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    var empleados = from emp in context.Employees
                                    join emp1 in context.Employees on emp.ReportsTo equals emp1.EmployeeID into emp2
                                    from emp1 in emp2.DefaultIfEmpty()
                                    select new
                                    {
                                        emp.EmployeeID,
                                        emp.LastName,
                                        emp.FirstName,
                                        emp.Title,
                                        emp.TitleOfCourtesy,
                                        emp.BirthDate,
                                        emp.HireDate,
                                        emp.Address,
                                        emp.City,
                                        emp.Region,
                                        emp.PostalCode,
                                        emp.Country,
                                        emp.HomePhone,
                                        emp.Extension,
                                        emp.Notes,
                                        ReportsToName = emp1 != null ? emp1.LastName + ", " + emp1.FirstName : "N/A",
                                        Photo = emp.Photo != null ? ConvertirABase64(emp.Photo.ToArray(), emp.EmployeeID) : null
                                    };
                    Utils.ActualizarBarraDeEstado(this, $"Se encontraron {empleados.Count()} empleados");
                    // Crear un objeto ReportDataSource y asignar la lista de empleados
                    ReportDataSource rds = new ReportDataSource("EmpleadoConReportsToDataSet", empleados.ToList());
                    // Limpiar los datos del visor de informes y agregar los datos del objeto ReportDataSource
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(rds);
                    this.reportViewer1.RefreshReport();
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

        private string ConvertirABase64(byte[] imageBytes, int empId)
        {
            try
            {
                if (empId <= 9)
                {

                    // Eliminar el encabezado OLE (78 bytes)
                    const int OLEHeaderLength = 78;
                    if (imageBytes.Length > OLEHeaderLength)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Write(imageBytes, OLEHeaderLength, imageBytes.Length - OLEHeaderLength);
                            ms.Seek(0, SeekOrigin.Begin);
                            Image image = Image.FromStream(ms);
                            using (MemoryStream jpgStream = new MemoryStream())
                            {
                                image.Save(jpgStream, ImageFormat.Jpeg);
                                return Convert.ToBase64String(jpgStream.ToArray());
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($"La imagen del empleado {empId} no es válida");
                    }
                }
                else
                {
                    return Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                // Maneja el error y devuelve null si la imagen no es válida
                Utils.MsgCatchOue(this, ex);
                return null;
            }
        }
    }
}

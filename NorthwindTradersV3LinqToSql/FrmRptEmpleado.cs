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
    public partial class FrmRptEmpleado : Form
    {
        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        public int Id { get; set; }

        public FrmRptEmpleado()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptEmpleado_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this.Owner);

        private void FrmRptEmpleado_Load(object sender, EventArgs e)
        {
            try
            {
                using (context)
                {
                    Utils.ActualizarBarraDeEstado(this.Owner, Utils.clbdd);
                    var empleado = from emp in context.Employees
                                   join emp1 in context.Employees on emp.ReportsTo equals emp1.EmployeeID into emp2
                                   from emp1 in emp2.DefaultIfEmpty()
                                   where emp.EmployeeID == Id
                                   select new EmpleadoConReportsTo
                                   {
                                       EmployeeID = emp.EmployeeID,
                                       LastName = emp.LastName,
                                       FirstName = emp.FirstName,
                                       Title = emp.Title,
                                       TitleOfCourtesy = emp.TitleOfCourtesy,
                                       BirthDate = emp.BirthDate,
                                       HireDate = emp.HireDate,
                                       Address = emp.Address,
                                       City = emp.City,
                                       Region = emp.Region,
                                       PostalCode = emp.PostalCode,
                                       Country = emp.Country,
                                       HomePhone = emp.HomePhone,
                                       Extension = emp.Extension,
                                       Notes = emp.Notes,
                                       ReportsToName = emp1 != null ? emp1.LastName + ", " + emp1.FirstName : "N/A",
                                       PhotoBase64 = emp.Photo != null ? ConvertirABase64(emp.Photo.ToArray(), emp.EmployeeID) : null
                                   };
                    Utils.ActualizarBarraDeEstado(this.Owner, $"Se encontró el registro {empleado.FirstOrDefault()?.EmployeeID}");
                    List<EmpleadoConReportsTo> empleados = empleado.ToList();
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", empleados);
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

        private string ConvertirABase64(byte[] foto, int id)
        {
            try
            {
                byte[] photoData;
                if (id <= 9) 
                {
                    // Eliminar el encabezado OLE (78 bytes)
                    const int OLEHeaderLength = 78;
                    long dataLength = foto.Length - OLEHeaderLength;
                    photoData = new byte[dataLength];
                    Array.Copy(foto, OLEHeaderLength, photoData, 0, dataLength);
                }
                else
                {
                    photoData = foto;
                }
                return Convert.ToBase64String(photoData);
            }
            catch (Exception ex)
            {
                Utils.MsgCatchOue(this, ex);
                return null;
            }
        }
    }
}

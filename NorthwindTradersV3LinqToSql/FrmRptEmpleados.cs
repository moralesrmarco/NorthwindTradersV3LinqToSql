using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptEmpleados : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmRptEmpleados()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void FrmRptEmpleados_Load(object sender, EventArgs e)
        {
            try
            {
                using (context)
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
                                        ReportsToName = emp1 != null ? emp1.LastName + ", " + emp1.FirstName : "N/A"
                                    };
                    ReportDataSource rds = new ReportDataSource("DataSet1", empleados.ToList());
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(rds);
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

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptEmpleados_FormClosed(object sender, FormClosedEventArgs e) => Utils.ActualizarBarraDeEstado(this);
    }
}

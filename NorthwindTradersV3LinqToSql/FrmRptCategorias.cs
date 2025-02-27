using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmRptCategorias: Form
    {
        public FrmRptCategorias()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptCategorias_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptCategorias_Load(object sender, EventArgs e)
        {
            try
            {
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    var categorias = from Categories in context.Categories
                                orderby Categories.CategoryID
                                select new
                                {
                                    CategoryID = Categories.CategoryID,
                                    CategoryName = Categories.CategoryName,
                                    Description = Categories.Description,
                                    Picture = Categories.Picture != null ? ConvertirABase64(Categories.Picture.ToArray(), Categories.CategoryID) : null
                                };
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {categorias.Count()} registros");
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", categorias.ToList());
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

        private string ConvertirABase64(byte[] imageBytes, int id)
        {
            try
            {
                if (id <= 8)
                {
                    // Eliminar el encabezado OLE (78 bytes)
                    const int OLEHeaderLength = 78;
                    if (imageBytes.Length > OLEHeaderLength)
                    {
                        byte[] foto = new byte[imageBytes.Length - OLEHeaderLength];
                        Array.Copy(imageBytes, OLEHeaderLength, foto, 0, foto.Length);
                        return Convert.ToBase64String(foto);
                    }
                    else
                    {
                        throw new Exception($"La imagen de la categoría {id} no es válida");
                    }
                }
                else
                { 
                    return Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                Utils.MsgCatchOue(ex);
                return null;
            }
        }
    }
}

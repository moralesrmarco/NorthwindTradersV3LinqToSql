using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmProductosPorEncimaPrecioPromedio : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        decimal? precioPromedioNullable = 0;

        public FrmProductosPorEncimaPrecioPromedio()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint(this, sender, e);
        }

        private void FrmProductosPorEncimaPrecioPromedio_Load(object sender, EventArgs e)
        {
            CalcularPrecioPromedio();
            Utils.ConfDgv(Dgv);
            LlenarDgv();
            ConfDgv();
        }

        private void CalcularPrecioPromedio()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                precioPromedioNullable = context.Products.Average(p => p.UnitPrice);
                decimal precioPromedio = precioPromedioNullable ?? 0m; // Maneja el caso donde el promedio pueda ser nulo
                string strPrecioPromedio = precioPromedio.ToString("C2");
                Grb.Text = $"»   Listado de productos con el precio por encima del precio promedio {strPrecioPromedio} :   «";
                Utils.ActualizarBarraDeEstado(this);
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

        private void LlenarDgv()
        {
            try
            {
                var query = from prod in context.VW_PRODUCTOSPORENCIMADELPRECIOPROMEDIO
                            select prod;

                Dgv.DataSource = query;
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

        private void ConfDgv()
        {
            Dgv.Columns["Fila"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Precio"].DefaultCellStyle.Format = "c";
            Dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void FrmProductosPorEncimaPrecioPromedio_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this);
        }
    }
}

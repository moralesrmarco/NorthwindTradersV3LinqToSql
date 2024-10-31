using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmProductosListado : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public FrmProductosListado()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void FrmProductosListado_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this);
        }

        private void GrbPaint(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint(this, sender, e);
        }

        private void FrmProductosListado_Load(object sender, EventArgs e)
        {
            LlenarCboCategoria();
            LlenarCboProveedor();
            Utils.ConfDgv(Dgv);
        }

        private void LlenarCboCategoria()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var query = context.SP_CATEGORIAS_SELECCIONAR_V2();
                cboCategoria.DataSource = query;
                cboCategoria.DisplayMember = "Categoria";
                cboCategoria.ValueMember = "Id";
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

        private void LlenarCboProveedor()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var query = context.SP_PROVEEDORES_SELECCIONAR();
                cboProveedor.DataSource = query;
                cboProveedor.DisplayMember = "Proveedor";
                cboProveedor.ValueMember = "Id";
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

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtIdInicial.Text = txtIdFinal.Text = txtProducto.Text = "";
            cboCategoria.SelectedIndex = cboProveedor.SelectedIndex = 0;
            Dgv.DataSource = null;
            Utils.ActualizarBarraDeEstado(this);
        }

        private void btnListarTodos_Click(object sender, EventArgs e)
        {
            LlenarDgv(sender);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenarDgv(sender);
        }

        private void LlenarDgv(object sender)
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                if (((Button)sender).Tag.ToString() == "Buscar")
                {
                    int intIdIni = 0, intIdFin = 0;
                    if (txtIdInicial.Text != "") intIdIni = int.Parse(txtIdInicial.Text);
                    if (txtIdFinal.Text != "") intIdFin = int.Parse(txtIdFinal.Text);
                    var query = context.SP_PRODUCTOS_BUSCAR_V2(intIdIni, intIdFin, txtProducto.Text, int.Parse(cboCategoria.SelectedValue.ToString()), int.Parse(cboProveedor.SelectedValue.ToString()));
                    Dgv.DataSource = query.ToList();
                }
                else
                {
                    var query = context.SP_PRODUCTOS_ALL(true);
                    Dgv.DataSource = query.ToList();
                }
                Utils.ActualizarBarraDeEstado(this, "Dando formato a la información");
                ConfDgv();
                Utils.ActualizarBarraDeEstado(this, $"Se encontraron {Dgv.RowCount} registros");
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
            Dgv.Columns["IdCategoria"].Visible = false;
            Dgv.Columns["IdProveedor"].Visible = false;

            Dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Cantidad_por_unidad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Precio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Unidades_en_inventario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Unidades_en_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Punto_de_pedido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Descontinuado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Categoría"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Unidades_en_inventario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Unidades_en_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Punto_de_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["Cantidad_por_unidad"].HeaderText = "Cantidad por unidad";
            Dgv.Columns["Unidades_en_inventario"].HeaderText = "Unidades en inventario";
            Dgv.Columns["Unidades_en_pedido"].HeaderText = "Unidades en pedido";
            Dgv.Columns["Punto_de_pedido"].HeaderText = "Punto de pedido";
            Dgv.Columns["Descripción_de_categoría"].HeaderText = "Descripción de categoría";

            Dgv.Columns["Precio"].DefaultCellStyle.Format = "c";
        }

        private void txtIdInicial_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtIdFinal_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtIdInicial_Leave(object sender, EventArgs e)
        {
            Utils.ValidaTxtBIdIni(txtIdInicial, txtIdFinal);
        }

        private void txtIdFinal_Leave(object sender, EventArgs e)
        {
            Utils.ValidaTxtBIdFin(txtIdInicial, txtIdFinal);
        }

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            btnLimpiar.PerformClick();
        }

        private void tabcOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnLimpiar.PerformClick();
        }
    }
}

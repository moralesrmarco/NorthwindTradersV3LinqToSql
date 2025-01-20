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
    public partial class FrmPedidosDetalleCrud : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        int IdDetalle = 1;

        public FrmPedidosDetalleCrud()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint(this, sender, e);
        }

        private void FrmPedidosDetalleCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cboCategoria.SelectedIndex > 0 || cboProducto.SelectedIndex > 0)
            {
                DialogResult respuesta = MessageBox.Show(Utils.preguntaCerrar, Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void FrmPedidosDetalleCrud_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this);
        }

        private void FrmPedidosDetalleCrud_Load(object sender, EventArgs e)
        {
            DeshabilitarControles();
            LlenarCboCategoria();
            LlenarDgvPedidos(null);
            Utils.ConfDgv(DgvPedidos);
            Utils.ConfDgv(DgvDetalle);
            ConfDgvPedidos();
            ConfDgvDetalle();
            InicializarValoresProducto();
        }

        private void DeshabilitarControles()
        {
            cboCategoria.Enabled = cboProducto.Enabled = false;
            btnAgregar.Enabled = false;
        }

        private void HabilitarControles()
        {
            cboCategoria.Enabled = cboProducto.Enabled = true;
            btnAgregar.Enabled = true;
        }

        private void DeshabilitarControlesProducto()
        {
            txtCantidad.Enabled = txtDescuento.Enabled = false;
        }

        private void HabilitarControlesProducto()
        {
            txtCantidad.Enabled = txtDescuento.Enabled = true;
        }

        private void LlenarCboCategoria()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                cboCategoria.DataSource = context.SP_CATEGORIAS_SELECCIONAR();
                cboCategoria.DisplayMember = "Categoria";
                cboCategoria.ValueMember = "Id";
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

        private void LlenarDgvPedidos(object sender)
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                if (sender == null)
                    DgvPedidos.DataSource = context.SP_PEDIDOS_LISTAR20().ToList();
                else
                {
                    int intBIdIni = 0, intBIdFin = 0;
                    bool boolFPedido = false, boolFRequerido = false, boolFEnvio = false;
                    DateTime? FPedidoIni, FPedidoFin, FRequeridoIni, FRequeridoFin, FEnvioIni, FEnvioFin;
                    if (txtBIdInicial.Text != "") intBIdIni = int.Parse(txtBIdInicial.Text);
                    if (txtBIdFinal.Text != "") intBIdFin = int.Parse(txtBIdFinal.Text);
                    if (dtpBFPedidoIni.Checked && dtpBFPedidoFin.Checked)
                    {
                        boolFPedido = true; // este parametro es requerido para que funcione el stored procedure con la misma lógica que he venido usando en las demas busquedas
                        dtpBFPedidoIni.Value = Convert.ToDateTime(dtpBFPedidoIni.Value.ToShortDateString() + " 00:00:00.000");
                        dtpBFPedidoFin.Value = Convert.ToDateTime(dtpBFPedidoFin.Value.ToShortDateString() + " 23:59:59.998"); // se usa .998 porque lo redondea a .997 por la presición de los campos tipo datetime de sql server, el cual es el maximo valor de milesimas de segundo que puede guardarse en la db. Si se usa .999 lo redondea al segundo 0.000 del siquiente dia e incluye los datos del siguiente día que es un comportamiento que no se quiere por que solo se deben mostrar los datos de la fecha indicada. Ya se comprobo el comportamiento en la base de datos.
                        FPedidoIni = dtpBFPedidoIni.Value;
                        FPedidoFin = dtpBFPedidoFin.Value;
                    }
                    else
                    {
                        boolFPedido = false;
                        FPedidoIni = null;
                        FPedidoFin = null;
                    }
                    if (dtpBFRequeridoIni.Checked && dtpBFRequeridoFin.Checked)
                    {
                        boolFRequerido = true;
                        dtpBFRequeridoIni.Value = Convert.ToDateTime(dtpBFRequeridoIni.Value.ToShortDateString() + " 00:00:00.000");
                        dtpBFRequeridoFin.Value = Convert.ToDateTime(dtpBFRequeridoFin.Value.ToShortDateString() + " 23:59:59:998");
                        FRequeridoIni = dtpBFRequeridoIni.Value;
                        FRequeridoFin = dtpBFRequeridoFin.Value;
                    }
                    else
                    {
                        boolFRequerido = false;
                        FRequeridoIni = null;
                        FRequeridoFin = null;
                    }
                    if (dtpBFEnvioIni.Checked && dtpBFEnvioFin.Checked)
                    {
                        boolFEnvio = true;
                        dtpBFEnvioIni.Value = Convert.ToDateTime(dtpBFEnvioIni.Value.ToShortDateString() + " 00:00:00.000");
                        dtpBFEnvioFin.Value = Convert.ToDateTime(dtpBFEnvioFin.Value.ToShortDateString() + " 23:59:59.998");
                        FEnvioIni = dtpBFEnvioIni.Value;
                        FEnvioFin = dtpBFEnvioFin.Value;
                    }
                    else
                    {
                        boolFEnvio = false;
                        FEnvioIni = null;
                        FEnvioFin = null;
                    }
                    DgvPedidos.DataSource = context.SP_PEDIDOS_BUSCAR(intBIdIni, intBIdFin, txtBCliente.Text, boolFPedido, chkBFPedidoNull.Checked, FPedidoIni, FPedidoFin, boolFRequerido, chkBFRequeridoNull.Checked, FRequeridoIni, FRequeridoFin, boolFEnvio, chkBFEnvioNull.Checked, FEnvioIni, FEnvioFin, txtBEmpleado.Text, txtBCompañiaT.Text, txtBDirigidoa.Text).ToList();
                }
                if (sender == null)
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran los últimos {DgvPedidos.RowCount} pedidos registrados");
                else
                    Utils.ActualizarBarraDeEstado(this, $"Se encontraron {DgvPedidos.RowCount} registros");
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

        private void ConfDgvPedidos()
        {
            DgvPedidos.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DgvPedidos.Columns["Fecha_de_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DgvPedidos.Columns["Fecha_requerido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DgvPedidos.Columns["Fecha_de_envío"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DgvPedidos.Columns["Compañía_transportista"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvPedidos.Columns["Vendedor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DgvPedidos.Columns["Fecha_de_pedido"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";
            DgvPedidos.Columns["Fecha_requerido"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";
            DgvPedidos.Columns["Fecha_de_envío"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";

            DgvPedidos.Columns["Nombre_de_contacto"].HeaderText = "Nombre de contacto";
            DgvPedidos.Columns["Fecha_de_pedido"].HeaderText = "Fecha de pedido";
            DgvPedidos.Columns["Fecha_requerido"].HeaderText = "Fecha requerido";
            DgvPedidos.Columns["Fecha_de_envío"].HeaderText = "Fecha de envío";
            DgvPedidos.Columns["Compañía_transportista"].HeaderText = "Compañía transportista";
            DgvPedidos.Columns["Dirigido_a"].HeaderText = "Dirigido a";
        }

        private void ConfDgvDetalle()
        {
            DgvDetalle.Columns["Id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvDetalle.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DgvDetalle.Columns["Cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvDetalle.Columns["Descuento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvDetalle.Columns["Importe"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void InicializarValoresProducto()
        {
            txtPrecio.Text = "$0.00";
            txtDescuento.Text = "0.00";
            txtTotal.Text = "$0.00";
            txtUInventario.Text = txtCantidad.Text = "0";
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarDatosPedido();
            BorrarMensajesError();
            BorrarDatosBusqueda();
            DeshabilitarControles();
            DgvPedidos.Focus();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosPedido();
            BorrarMensajesError();
            DeshabilitarControles();
            LlenarDgvPedidos(sender);
            DgvPedidos.Focus();
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            btnLimpiar.PerformClick();
            LlenarDgvPedidos(null);
            DgvPedidos.Focus();
        }

        private void BorrarDatosPedido()
        {
            txtId.Text = txtCliente.Text = "";
            cboCategoria.SelectedIndex = 0;
            cboProducto.DataSource = null;
            InicializarValoresProducto();
            DgvDetalle.Rows.Clear();
        }

        private void BorrarDatosBusqueda()
        {
            txtBIdInicial.Text = txtBIdFinal.Text = txtBCliente.Text = txtBEmpleado.Text = txtBCompañiaT.Text = txtBDirigidoa.Text = "";
            dtpBFPedidoIni.Checked = dtpBFPedidoFin.Checked = dtpBFRequeridoIni.Checked = dtpBFRequeridoFin.Checked = dtpBFEnvioIni.Checked = dtpBFEnvioFin.Checked = false;
            chkBFPedidoNull.Checked = chkBFRequeridoNull.Checked = chkBFEnvioNull.Checked = false;
        }

        private void BorrarMensajesError() => errorProvider1.Clear();

        private bool ValidarControles()
        {
            bool valida = true;
            if (cboCategoria.SelectedIndex <= 0)
            {
                valida = false;
                errorProvider1.SetError(cboCategoria, "Seleccione la categoría");
            }
            if (cboProducto.SelectedIndex <= 0)
            {
                valida = false;
                errorProvider1.SetError(cboProducto, "Seleccione el producto");
            }
            if (txtCantidad.Text == "" || int.Parse(txtCantidad.Text) == 0)
            {
                valida = false;
                errorProvider1.SetError(txtCantidad, "Ingrese la cantidad");
            }
            if (int.Parse(txtCantidad.Text) > int.Parse(txtUInventario.Text))
            {
                valida = false;
                errorProvider1.SetError(txtCantidad, "La cantidad de productos en el pedido excede el inventario disponible");
            }
            if (txtDescuento.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtDescuento, "Ingrese el descuento");
            }
            if (decimal.Parse(txtDescuento.Text) > 1 || decimal.Parse(txtDescuento.Text) < 0)
            {
                valida = false;
                errorProvider1.SetError(txtDescuento, "El descuento no puede ser mayor que 1 o menor que 0");
            }
            if (cboProducto.SelectedIndex > 0)
            {
                int numProd = int.Parse(cboProducto.SelectedValue.ToString());
                bool productoDuplicado = false;
                foreach (DataGridViewRow dgvr in DgvDetalle.Rows)
                {
                    if (int.Parse(dgvr.Cells["ProductoId"].Value.ToString()) == numProd)
                    {
                        productoDuplicado = true;
                        break;
                    }
                }
                if (productoDuplicado)
                {
                    valida = false;
                    errorProvider1.SetError(cboProducto, "No se puede tener un producto duplicado en el detalle del pedido");
                }
            }
            string total = txtTotal.Text;
            total = total.Replace("$", "");
            if (txtTotal.Text == "" || (decimal.Parse(total) + (decimal.Parse(txtPrecio.Text.Replace("$", "")) * int.Parse(txtCantidad.Text) * (1 - decimal.Parse(txtDescuento.Text))) == 0))
            {
                valida = false;
                errorProvider1.SetError(btnAgregar, "Ingrese el detalle del pedido");
            }
            return valida;
        }
    }
}

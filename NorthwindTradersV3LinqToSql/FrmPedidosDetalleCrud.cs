using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
            InicializarValores();
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
            btnAgregar.Enabled = false;
        }

        private void HabilitarControlesProducto()
        {
            txtCantidad.Enabled = txtDescuento.Enabled = true;
            btnAgregar.Enabled = true;
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

        private void InicializarValores()
        {
            txtTotal.Text = "$0.00";
            InicializarValoresProducto();
        }

        private void InicializarValoresProducto()
        {
            txtUInventario.Text = txtCantidad.Text = "0";
            txtPrecio.Text = "$0.00";
            txtDescuento.Text = "0.00";
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarDatosPedido();
            BorrarMensajesError();
            BorrarDatosBusqueda();
            DeshabilitarControles();
            BtnNota.Enabled = false;
            DgvPedidos.Focus();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosPedido();
            BorrarMensajesError();
            DeshabilitarControles();
            BtnNota.Enabled = false;
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
            InicializarValores();
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
            if (int.Parse(txtCantidad.Text) > int.Parse(txtUInventario.Text.Replace(",", "")))
            {
                valida = false;
                errorProvider1.SetError(txtCantidad, "La cantidad de productos en el pedido excede el inventario disponible");
            }
            if (txtDescuento.Text == "")
            {
                valida = false;
                errorProvider1.SetError(txtDescuento, "Ingrese el descuento");
            }
            if (float.Parse(txtDescuento.Text) > 1 || float.Parse(txtDescuento.Text) < 0)
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
            string total = txtTotal.Text.Replace("$", "");
            if (txtTotal.Text == "" || (float.Parse(total) + (float.Parse(txtPrecio.Text.Replace("$", "")) * int.Parse(txtCantidad.Text) * (1 - float.Parse(txtDescuento.Text))) == 0))
            {
                valida = false;
                errorProvider1.SetError(btnAgregar, "Ingrese el detalle del pedido");
            }
            return valida;
        }

        private void txtBIdInicial_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtBIdInicial_Leave(object sender, EventArgs e)
        {
            Utils.ValidaTxtBIdIni(txtBIdInicial, txtBIdFinal);
        }

        private void txtBIdFinal_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtBIdFinal_Leave(object sender, EventArgs e)
        {
            Utils.ValidaTxtBIdFin(txtBIdInicial, txtBIdFinal);
        }

        private void dtpBFPedidoIni_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFPedidoIni.Checked)
            {
                dtpBFPedidoFin.Checked = true;
                chkBFPedidoNull.Checked = false;
            }
            else
                dtpBFPedidoFin.Checked = false;
        }

        private void dtpBFPedidoFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFPedidoFin.Checked)
            {
                dtpBFPedidoIni.Checked = true;
                chkBFPedidoNull.Checked = false;
            }
            else
                dtpBFPedidoIni.Checked = false;
        }

        private void dtpBFRequeridoIni_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFRequeridoIni.Checked)
            {
                dtpBFRequeridoFin.Checked = true;
                chkBFRequeridoNull.Checked = false;
            }
            else
                dtpBFRequeridoFin.Checked = false;
        }

        private void dtpBFRequeridoFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFRequeridoFin.Checked)
            {
                dtpBFRequeridoIni.Checked = true;
                chkBFRequeridoNull.Checked = false;
            }
            else
                dtpBFRequeridoIni.Checked = false;
        }

        private void dtpBFEnvioIni_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFEnvioIni.Checked)
            {
                dtpBFEnvioFin.Checked = true;
                chkBFEnvioNull.Checked = false;
            }
            else
                dtpBFEnvioFin.Checked = false;
        }

        private void dtpBFEnvioFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFEnvioFin.Checked)
            {
                dtpBFEnvioIni.Checked = true;
                chkBFEnvioNull.Checked = false;
            }
            else
                dtpBFEnvioIni.Checked = false;
        }

        private void chkBFPedidoNull_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBFPedidoNull.Checked)
            {
                dtpBFPedidoIni.Checked = false;
                dtpBFPedidoFin.Checked = false;
            }
        }

        private void chkBFRequeridoNull_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBFRequeridoNull.Checked)
            {
                dtpBFRequeridoIni.Checked = false;
                dtpBFRequeridoFin.Checked = false;
            }
        }

        private void chkBFEnvioNull_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBFEnvioNull.Checked)
            {
                dtpBFEnvioIni.Checked = false;
                dtpBFEnvioFin.Checked = false;
            }
        }

        private void dtpBFPedidoIni_Leave(object sender, EventArgs e)
        {
            if (dtpBFPedidoIni.Checked && dtpBFPedidoFin.Checked)
                if (dtpBFPedidoFin.Value < dtpBFPedidoIni.Value)
                    dtpBFPedidoFin.Value = dtpBFPedidoIni.Value;
        }

        private void dtpBFPedidoFin_Leave(object sender, EventArgs e)
        {
            if (dtpBFPedidoIni.Checked && dtpBFPedidoFin.Checked)
                if (dtpBFPedidoFin.Value < dtpBFPedidoIni.Value)
                    dtpBFPedidoIni.Value = dtpBFPedidoFin.Value;
        }

        private void dtpBFRequeridoIni_Leave(object sender, EventArgs e)
        {
            if (dtpBFRequeridoIni.Checked && dtpBFRequeridoFin.Checked)
                if (dtpBFRequeridoFin.Value < dtpBFRequeridoIni.Value)
                    dtpBFRequeridoFin.Value = dtpBFRequeridoIni.Value;
        }

        private void dtpBFRequeridoFin_Leave(object sender, EventArgs e)
        {
            if (dtpBFRequeridoIni.Checked && dtpBFRequeridoFin.Checked)
                if (dtpBFRequeridoFin.Value < dtpBFRequeridoIni.Value)
                    dtpBFRequeridoIni.Value = dtpBFRequeridoFin.Value;
        }

        private void dtpBFEnvioIni_Leave(object sender, EventArgs e)
        {
            if (dtpBFEnvioIni.Checked && dtpBFEnvioFin.Checked)
                if (dtpBFEnvioFin.Value < dtpBFEnvioIni.Value)
                    dtpBFEnvioFin.Value = dtpBFEnvioIni.Value;
        }

        private void dtpBFEnvioFin_Leave(object sender, EventArgs e)
        {
            if (dtpBFEnvioIni.Checked && dtpBFEnvioFin.Checked)
                if (dtpBFEnvioFin.Value < dtpBFEnvioIni.Value)
                    dtpBFEnvioIni.Value = dtpBFEnvioFin.Value;
        }

        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            InicializarValoresProducto();
            BorrarMensajesError();
            if (cboCategoria.SelectedIndex > 0)
            {
                try
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                    cboProducto.DataSource = context.SP_PRODUCTOS_SELECCIONAR(int.Parse(cboCategoria.SelectedValue.ToString()));
                    cboProducto.ValueMember = "Id";
                    cboProducto.DisplayMember = "Producto";
                    cboProducto.Enabled = true;
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran {DgvPedidos.RowCount} registros en pedidos");
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
            else
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                DataTable tbl = new DataTable();
                tbl.Columns.Add("Id", typeof(int));
                tbl.Columns.Add("Producto", typeof(string));
                DataRow dr = tbl.NewRow();
                dr["Id"] = 0;
                dr["Producto"] = "«--- Seleccione ---»";
                tbl.Rows.Add(dr);
                cboProducto.DataSource = tbl;
                cboProducto.ValueMember = "Id";
                cboProducto.DisplayMember = "Producto";
                cboProducto.Enabled = false;
                Utils.ActualizarBarraDeEstado(this, $"Se muestran {DgvPedidos.RowCount} registros en pedidos");
            }
        }

        private void cboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            InicializarValoresProducto();
            BorrarMensajesError();
            if (cboProducto.SelectedIndex > 0)
            {
                try
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                    var query = context.Products
                        .Where(p => p.ProductID == int.Parse(cboProducto.SelectedValue.ToString()))
                        .Select(p => new
                        {
                            p.UnitPrice,
                            p.UnitsInStock
                        })
                        .FirstOrDefault();
                    if (query != null)
                    {
                        txtPrecio.Text = $"{query.UnitPrice:c2}";
                        txtUInventario.Text = short.Parse(query.UnitsInStock.ToString()).ToString("n0");
                        if (int.Parse(txtUInventario.Text.Replace(",", "")) == 0)
                        {
                            DeshabilitarControlesProducto();
                            MessageBox.Show("No hay este producto en existencia", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cboProducto.SelectedIndex = 0;
                        }
                        else
                            HabilitarControlesProducto();
                    }
                    else
                        DeshabilitarControlesProducto();
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran {DgvPedidos.RowCount} registros en pedidos");
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
            else
                DeshabilitarControlesProducto();
        }

        private void DgvPedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            BtnNota.Enabled = false;
            BorrarDatosPedido();
            DataGridViewRow dgvr = DgvPedidos.CurrentRow;
            txtId.Text = dgvr.Cells["Id"].Value.ToString();
            txtCliente.Text = dgvr.Cells["Cliente"].Value.ToString();
            LlenarDatosDetallePedido();
            cboCategoria.Enabled = true;
        }

        private void LlenarDatosDetallePedido()
        {
            try
            {
                IdDetalle = 1;
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var query = context.SP_DETALLEPEDIDOS_PRODUCTOS_LISTAR1(int.Parse(txtId.Text)).ToList();
                if (query.Count > 0)
                {
                    foreach (var pd in query)
                    {
                        var pedidoDetalle = new Order_Details()
                        {
                            ProductID = pd.Id_Producto,
                            UnitPrice = pd.Precio,
                            Quantity = pd.Cantidad,
                            Discount = pd.Descuento
                        };
                        string productName = pd.Producto;
                        float importe = (float.Parse(pedidoDetalle.UnitPrice.ToString()) * pedidoDetalle.Quantity) * (1 - pedidoDetalle.Discount);
                        DgvDetalle.Rows.Add(new object[] { IdDetalle, productName, pedidoDetalle.UnitPrice, pedidoDetalle.Quantity, pedidoDetalle.Discount, importe, "Modificar", "Eliminar", pedidoDetalle.ProductID });
                        ++IdDetalle;
                    }
                }
                else
                {
                    MessageBox.Show("No se encontraron detalles para el pedido especificado", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                CalcularTotal();
                Utils.ActualizarBarraDeEstado(this, $"Se muestran {DgvPedidos.RowCount} registros en pedidos");
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

        private void DgvDetalle_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.Value != null) e.Value = float.Parse(e.Value.ToString()).ToString("c");
            if (e.ColumnIndex == 3 && e.Value != null) e.Value = int.Parse(e.Value.ToString()).ToString("n0");
            if (e.ColumnIndex == 4 && e.Value != null) e.Value = float.Parse(e.Value.ToString()).ToString("n2");
            if (e.ColumnIndex == 5 && e.Value != null) e.Value = float.Parse(e.Value.ToString()).ToString("c");
        }

        private void CalcularTotal()
        {
            float total = 0;
            foreach(DataGridViewRow dgvr in DgvDetalle.Rows)
            {
                float importe = (float)dgvr.Cells["Importe"].Value;
                total += importe;
            }
            txtTotal.Text = string.Format("{0:c}", total);
        }

        private byte AgregarPedidoDetalle(Order_Details pedidoDetalle)
        {
            byte numRegs = 0;
            using (var context = new NorthwindTradersDataContext())
            {
                // Nos aseguramos que la conexión esté abierta
                if (context.Connection.State == ConnectionState.Closed)
                    context.Connection.Open();
                // Iniciamos una transacción
                using (var transaction = context.Connection.BeginTransaction())
                {
                    // las excepciones generadas en este segmento de código son capturadas en un nivel superior, por eso uso throw para relanzar la excepción y se procese en el nivel superior.
                    try
                    {
                        context.Transaction = transaction;
                        // Verificar la exestencia del producto
                        var product = context.Products.SingleOrDefault(p => p.ProductID == pedidoDetalle.ProductID);
                        if (product == null)
                            throw new InvalidOperationException("Producto no encontrado");
                        // Verificar si hay suficientes unidades en stock
                        if (product.UnitsInStock < pedidoDetalle.Quantity)
                            throw new InvalidOperationException("No hay suficientes unidades en stock");
                        // Restar Quantity a UnitsInStock en la tabla Products
                        product.UnitsInStock -= pedidoDetalle.Quantity;
                        // Insertar el nuevo registro en Order_Details
                        context.Order_Details.InsertOnSubmit(pedidoDetalle);
                        context.SubmitChanges();
                        // Si todo va bien, confirmamos la transacción
                        transaction.Commit();
                        numRegs = 1;
                    }
                    catch (SqlException)
                    {
                        // Si ocurre un error de base de datos, revertimos la transacción
                        transaction.Rollback();
                        throw; // se vuelve a lanzar la excepción para que sea procesada en un nivel superior
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return numRegs;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            byte numRegs = 0;
            BorrarMensajesError();
            if (ValidarControles())
            {
                try
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.insertandoRegistro);
                    cboCategoria.Enabled = false;
                    DeshabilitarControlesProducto();
                    Order_Details pedidoDetalle = new Order_Details();
                    pedidoDetalle.OrderID = int.Parse(txtId.Text);
                    pedidoDetalle.ProductID = int.Parse(cboProducto.SelectedValue.ToString());
                    pedidoDetalle.UnitPrice = decimal.Parse(txtPrecio.Text.Replace("$", ""));
                    pedidoDetalle.Quantity = short.Parse(txtCantidad.Text);
                    pedidoDetalle.Discount = float.Parse(txtDescuento.Text);
                    numRegs = AgregarPedidoDetalle(pedidoDetalle);
                    if (numRegs > 0)
                        MessageBox.Show($"El producto: {cboProducto.Text} del Pedido: {txtId.Text}, se registró satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show($"El producto: {cboProducto.Text} del Pedido: {txtId.Text}, NO se registró en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran {DgvPedidos.RowCount} registros de pedidos");
                }
                catch (SqlException ex) when (ex.Number == 2627)
                {
                    MessageBox.Show($"Error, ya existe un registro del producto {cboProducto.Text} en la base de datos, modifique la cantidad del producto de ese registro", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    BorrarDatosAgregarProducto();
                    cboCategoria.Enabled = true;
                }
                catch (SqlException ex)
                {
                    Utils.MsgCatchOueclbdd(this, ex);
                }
                catch (Exception ex)
                {
                    Utils.MsgCatchOue(this, ex);
                }
                if (numRegs > 0)
                {
                    BorrarDatosDetallePedido();
                    LlenarDatosDetallePedido();
                    cboCategoria.Enabled = true;
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran {DgvPedidos.RowCount} registros de pedidos");
                    BtnNota.Enabled = true;
                    DgvDetalle.Focus();
                }
            }
        }

        private void BorrarDatosAgregarProducto()
        {
            cboCategoria.SelectedIndex = 0;
            cboProducto.DataSource = null;
            InicializarValoresProducto();
        }

        private void BorrarDatosDetallePedido()
        {
            cboCategoria.SelectedIndex = 0;
            cboProducto.DataSource = null;
            InicializarValores();
            txtTotal.Text = "$0.00";
            DgvDetalle.Rows.Clear();
        }

        private void DgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == DgvDetalle.Columns["Eliminar"].Index)
            {
                DataGridViewRow dgvr = DgvDetalle.CurrentRow;
                string productName = dgvr.Cells["Producto"].Value.ToString();
                int productId = (int)dgvr.Cells["ProductoId"].Value;
                int orderId = int.Parse(txtId.Text);
                EliminarPedidoDetalle(orderId, productId, productName );
                BtnNota.Enabled = true;
            }
            if (e.ColumnIndex == DgvDetalle.Columns["Modificar"].Index)
            {
                DataGridViewRow dgvr = DgvDetalle.CurrentRow;
                using (FrmPedidosDetalleModificar frmPedidosDetalleModificar = new FrmPedidosDetalleModificar())
                {
                    frmPedidosDetalleModificar.Owner = this;
                    frmPedidosDetalleModificar.PedidoId = int.Parse(txtId.Text);
                    frmPedidosDetalleModificar.ProductoId = int.Parse(dgvr.Cells["ProductoId"].Value.ToString());
                    frmPedidosDetalleModificar.Producto = dgvr.Cells["Producto"].Value.ToString();
                    frmPedidosDetalleModificar.Precio = float.Parse(dgvr.Cells["Precio"].Value.ToString());
                    frmPedidosDetalleModificar.Cantidad = short.Parse(dgvr.Cells["Cantidad"].Value.ToString());
                    frmPedidosDetalleModificar.Descuento = float.Parse(dgvr.Cells["Descuento"].Value.ToString());
                    frmPedidosDetalleModificar.Importe = float.Parse(dgvr.Cells["Importe"].Value.ToString());
                    DialogResult dialogResult = frmPedidosDetalleModificar.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        BtnNota.Enabled = true;
                        BorrarDatosDetallePedido();
                        LlenarDatosDetallePedido();
                    }
                }
            }
            DgvDetalle.Focus();
        }

        private void EliminarPedidoDetalle(int orderId, int productId, string productName)
        {
            byte numRegs = 0;
            BorrarMensajesError();
            BorrarDatosAgregarProducto();
            try
            {
                DialogResult respuesta = MessageBox.Show($"¿Esta seguro de eliminar el producto: {productName} del Pedido: {orderId}?", Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.Yes)
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.eliminandoRegistro);
                    cboCategoria.Enabled = false;
                    DeshabilitarControlesProducto();
                    using (var context = new NorthwindTradersDataContext())
                    {
                        // Nos aseguramos de que la conexión esté abierta
                        if (context.Connection.State == ConnectionState.Closed)
                            context.Connection.Open();
                        // Iniciamos una transacción
                        using (var transaction = context.Connection.BeginTransaction())
                        {
                            try
                            {
                                context.Transaction = transaction;
                                // Buscar el registro que se va a eliminar
                                var pedidoDetalle = context.Order_Details.SingleOrDefault(od => od.OrderID == orderId & od.ProductID == productId);
                                if (pedidoDetalle == null)
                                    throw new InvalidOperationException("El detalle del pedido no existe, es posible que el registro haya sido eliminado por otro usuario de la red");
                                // Devolver la cantidad a UnitsInStock en la tabla Products
                                var product = context.Products.SingleOrDefault(p => p.ProductID == productId);
                                if (product != null)
                                    // Verificar desbordamiento
                                    checked
                                    {
                                        product.UnitsInStock = (short)(product.UnitsInStock + pedidoDetalle.Quantity);
                                    }
                                // Eliminar el registro
                                context.Order_Details.DeleteOnSubmit(pedidoDetalle);
                                context.SubmitChanges();
                                // Confirmar la transacción
                                transaction.Commit();
                                numRegs = 1;
                                MessageBox.Show($"El producto: {productName} del Pedido: {orderId}, se eliminó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (OverflowException)
                            {
                                transaction.Rollback();
                                throw new InvalidOperationException("Error al actualizar unidades en stock: se produjo un desbordamiento");
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                Utils.MsgCatchOue(this, ex);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 220)
            {
                MessageBox.Show("Al tratar de devolver las unidades vendidas al inventario, la cantidad de unidades excede las 32,767 unidades, rango máximo para un campo tipo smallint", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (numRegs > 0)
            {
                BorrarDatosDetallePedido();
                LlenarDatosDetallePedido();
                cboCategoria.Enabled = true;
                Utils.ActualizarBarraDeEstado(this, $"Se muestran {DgvPedidos.RowCount} registros de pedidos");
            }
        }

        private void BtnNota_Click(object sender, EventArgs e)
        {
            FrmRptNotaRemision frmRptNotaRemision = new FrmRptNotaRemision();
            frmRptNotaRemision.Id = int.Parse(txtId.Text);
            frmRptNotaRemision.ShowDialog();
        }
    }
}

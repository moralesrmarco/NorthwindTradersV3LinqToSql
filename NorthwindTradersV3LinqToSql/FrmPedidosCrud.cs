using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using NorthwindTradersV3LinqToSql;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmPedidosCrud : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        private TabPage lastSelectedTab;
        bool EventoCargardo = true; // esta variable es necesaria para controlar el manejador de eventos de la celda del dgv, ojo no quitar
        int IdDetalle = 1;

        public FrmPedidosCrud()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void GrbPaint(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint(this, sender, e);
        }

        private void GrbPaint2(object sender, PaintEventArgs e)
        {
            Utils.GrbPaint2(this, sender, e);
        }

        private void FrmPedidosCrud_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this);
        }

        private void FrmPedidosCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tabcOperacion.SelectedTab != tabpConsultar)
            {
                if (cboCliente.SelectedIndex > 0 || cboEmpleado.SelectedIndex > 0 || cboTransportista.SelectedIndex > 0 || cboCategoria.SelectedIndex > 0 || cboProducto.SelectedIndex > 0 || dgvDetalle.RowCount > 0)
                {
                    DialogResult respuesta = MessageBox.Show(Utils.preguntaCerrar, Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (respuesta == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void FrmPedidosCrud_Load(object sender, EventArgs e)
        {
            dtpHoraRequerido.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dtpHoraEnvio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dtpHoraRequerido.Enabled = false;
            dtpHoraEnvio.Enabled = false;
            //DeshabilitarControles();
            LlenarCboCliente();
            LlenarCboEmpleado();
            LlenarCboTransportista();
            LlenarCboCategoria();
            Utils.ConfDgv(dgvPedidos);
            Utils.ConfDgv(dgvDetalle);
            LlenarDgvPedidos(null);
            ConfDgvPedidos();
            ConfDgvDetalle();
            dgvDetalle.Columns["Eliminar"].Visible = false;
            txtPrecio.Text = txtFlete.Text = "$0.00";
            txtDescuento.Text = "0.00";
            txtUInventario.Text = "0";
        }

        private void DeshabilitarControles()
        {
            cboCliente.Enabled = cboEmpleado.Enabled = cboTransportista.Enabled = cboCategoria.Enabled = cboProducto.Enabled = false;
            dtpPedido.Enabled = dtpHoraPedido.Enabled = dtpRequerido.Enabled = dtpHoraRequerido.Enabled = dtpEnvio.Enabled = dtpHoraEnvio.Enabled = false;
            txtDirigidoa.ReadOnly = txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCP.ReadOnly = txtPais.ReadOnly = txtFlete.ReadOnly = true;
            txtCantidad.Enabled = txtDescuento.Enabled = false;
            btnAgregar.Enabled = btnGenerar.Enabled = false;
        }

        private void HabilitarControles()
        {
            cboCliente.Enabled = cboEmpleado.Enabled = cboTransportista.Enabled = cboCategoria.Enabled = cboProducto.Enabled = true;
            dtpPedido.Enabled = dtpHoraPedido.Enabled = dtpRequerido.Enabled = dtpHoraRequerido.Enabled = dtpEnvio.Enabled = dtpHoraEnvio.Enabled = true;
            txtDirigidoa.ReadOnly = txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCP.ReadOnly = txtPais.ReadOnly = txtFlete.ReadOnly = false;
            btnAgregar.Enabled = btnGenerar.Enabled = true;
        }

        private void HabilitarControlesProducto()
        {
            txtCantidad.Enabled = txtDescuento.Enabled = true;
        }

        private void DeshabilitarControlesProducto()
        {
            txtCantidad.Enabled = txtDescuento.Enabled = false;
        }

        private bool ValidarControles()
        {
            bool valida = true;
            if (cboCliente.SelectedIndex == 0)
            {
                errorProvider1.SetError(cboCliente, "Ingrese el cliente");
                valida = false;
            }
            if (cboEmpleado.SelectedIndex == 0)
            {
                errorProvider1.SetError(cboEmpleado, "Ingrese el empleado");
                valida = false;
            }
            if (dtpPedido.Checked == false)
            {
                errorProvider1.SetError(dtpPedido, "Ingrese la fecha de pedido");
                valida = false;
            }
            if (cboTransportista.SelectedIndex == 0)
            {
                errorProvider1.SetError(cboTransportista, "Ingrese la compañía transportista");
                valida = false;
            }
            string total = txtTotal.Text;
            total = total.Replace("$", "");
            if (txtTotal.Text == "" || decimal.Parse(total) == 0)
            {
                errorProvider1.SetError(btnAgregar, "Ingrese el detalle del pedido");
                valida = false;
            }
            if (cboProducto.SelectedIndex > 0)
            {
                errorProvider1.SetError(cboProducto, "Ha seleccionado un producto y no lo ha agregado al pedido");
                valida = false;
            }
            return valida;
        }

        private void LlenarCboCliente()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                cboCliente.DataSource = context.SP_CLIENTES_SELECCIONAR();
                cboCliente.DisplayMember = "Cliente";
                cboCliente.ValueMember = "Id";
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

        private void LlenarCboEmpleado()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                cboEmpleado.DataSource = context.SP_EMPLEADOS_SELECCIONAR();
                cboEmpleado.DisplayMember = "Empleado";
                cboEmpleado.ValueMember = "Id";
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

        private void LlenarCboTransportista()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                cboTransportista.DataSource = context.SP_TRANSPORTISTAS_SELECCIONAR();
                cboTransportista.DisplayMember = "Transportista";
                cboTransportista.ValueMember = "Id";
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

        private void ConfDgvPedidos()
        {
            dgvPedidos.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvPedidos.Columns["Fecha_de_pedido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPedidos.Columns["Fecha_requerido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPedidos.Columns["Fecha_de_envío"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPedidos.Columns["Compañía_transportista"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPedidos.Columns["Vendedor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvPedidos.Columns["Fecha_de_pedido"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";
            dgvPedidos.Columns["Fecha_requerido"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";
            dgvPedidos.Columns["Fecha_de_envío"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";

            dgvPedidos.Columns["Nombre_de_contacto"].HeaderText = "Nombre de contacto";
            dgvPedidos.Columns["Fecha_de_pedido"].HeaderText = "Fecha de pedido";
            dgvPedidos.Columns["Fecha_requerido"].HeaderText = "Fecha requerido";
            dgvPedidos.Columns["Fecha_de_envío"].HeaderText = "Fecha de envío";
            dgvPedidos.Columns["Compañía_transportista"].HeaderText = "Compañía transportista";
            dgvPedidos.Columns["Dirigido_a"].HeaderText = "Dirigido a";
        }

        private void ConfDgvDetalle()
        {
            dgvDetalle.Columns["Id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetalle.Columns["Cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns["Descuento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns["Importe"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void LlenarDgvPedidos(object sender)
        {
            Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
            try
            {
                if (sender == null)
                {
                    dgvPedidos.DataSource = context.SP_PEDIDOS_LISTAR20().ToList();
                }
                else
                {
                    int intBIdIni = 0, intBIdFin = 0;
                    bool boolFPedido = false, boolFRequerido = false, boolFEnvio = false;
                    DateTime? FPedidoIni, FPedidoFin, FRequeridoIni, FRequeridoFin, FEnvioIni, FEnvioFin;
                    if (txtBIdInicial.Text != "") intBIdIni = int.Parse(txtBIdInicial.Text);
                    if (txtBIdFinal.Text != "") intBIdFin = int.Parse(txtBIdFinal.Text);
                    if (dtpBFPedidoIni.Checked && dtpBFPedidoFin.Checked)
                    {
                        boolFPedido = true; // este parametro es requerido para que funcione el store procedure con la misma logica que he venido usando en las demas busquedas
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
                        dtpBFRequeridoFin.Value = Convert.ToDateTime(dtpBFRequeridoFin.Value.ToShortDateString() + " 23:59:59.998");
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
                        dtpBFEnvioFin.Value = Convert.ToDateTime(dtpBFEnvioFin.Value.ToShortDateString() + " 23:59:59:998");
                        FEnvioIni = dtpBFEnvioIni.Value;
                        FEnvioFin = dtpBFEnvioFin.Value;
                    }
                    else
                    {
                        boolFEnvio = false;
                        FEnvioIni = null;
                        FEnvioFin = null;
                    }
                    dgvPedidos.DataSource = context.SP_PEDIDOS_BUSCAR(intBIdIni, intBIdFin, txtBCliente.Text, boolFPedido, chkBFPedidoNull.Checked, FPedidoIni, FPedidoFin, boolFRequerido, chkBFRequeridoNull.Checked, FRequeridoIni, FRequeridoFin, boolFEnvio, chkBFEnvioNull.Checked, FEnvioIni, FEnvioFin, txtBEmpleado.Text, txtBCompañiaT.Text, txtBDirigidoa.Text).ToList();

                }
                if (sender == null)
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran los últimos {dgvPedidos.RowCount} pedidos registrados");
                else
                    Utils.ActualizarBarraDeEstado(this, $"Se encontraron {dgvPedidos.RowCount} registros");
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
            BorrarDatosPedido();
            BorrarMensajesError();
            BorrarDatosBusqueda();
            if (tabcOperacion.SelectedTab != tabpRegistrar)
                DeshabilitarControles();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosPedido();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tabpRegistrar)
                DeshabilitarControles();
            LlenarDgvPedidos(sender);

        }

        private void BorrarDatosPedido()
        {
            txtId.Text = "";
            cboCliente.SelectedIndex = cboEmpleado.SelectedIndex = cboTransportista.SelectedIndex = cboCategoria.SelectedIndex = 0;
            cboProducto.DataSource = null;
            dtpPedido.Value = dtpRequerido.Value = dtpEnvio.Value = DateTime.Now;
            dtpHoraPedido.Value = DateTime.Now;
            dtpHoraRequerido.Value = dtpHoraEnvio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dtpRequerido.Checked = dtpEnvio.Checked = false;
            txtDirigidoa.Text = txtDomicilio.Text = txtCiudad.Text = txtRegion.Text = txtCP.Text = txtPais.Text = "";
            txtFlete.Text = txtPrecio.Text = "$0.00";
            txtCantidad.Text = txtUInventario.Text = "0";
            txtDescuento.Text = "0.00";
            txtTotal.Text = "$0.00";
            dgvDetalle.Rows.Clear();
        }

        private void BorrarMensajesError()
        {
            errorProvider1.SetError(cboCategoria, "");
            errorProvider1.SetError(cboProducto, "");
            errorProvider1.SetError(txtCantidad, "");
            errorProvider1.SetError(txtDescuento, "");
            errorProvider1.SetError(cboCliente, "");
            errorProvider1.SetError(cboEmpleado, "");
            errorProvider1.SetError(dtpPedido, "");
            errorProvider1.SetError(cboTransportista, "");
            errorProvider1.SetError(btnAgregar, "");
        }

        private void BorrarDatosBusqueda()
        {
            txtBIdInicial.Text = txtBIdFinal.Text = txtBCliente.Text = txtBEmpleado.Text = txtBCompañiaT.Text = txtBDirigidoa.Text = "";
            dtpBFPedidoIni.Value = dtpBFPedidoFin.Value = dtpBFRequeridoIni.Value = dtpBFRequeridoFin.Value = dtpBFEnvioIni.Value = dtpBFEnvioFin.Value = DateTime.Today;
            dtpBFPedidoIni.Checked = dtpBFPedidoFin.Checked = dtpBFRequeridoIni.Checked = dtpBFRequeridoFin.Checked = dtpBFEnvioIni.Checked = dtpBFEnvioFin.Checked = false;
            chkBFPedidoNull.Checked = chkBFRequeridoNull.Checked = chkBFEnvioNull.Checked = false;
        }

        private void txtBIdInicial_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtBIdFinal_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtBIdInicial_Leave(object sender, EventArgs e)
        {
            Utils.ValidaTxtBIdIni(txtBIdInicial, txtBIdFinal);
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
            txtPrecio.Text = "$0.00";
            txtUInventario.Text = "0";
            txtCantidad.Text = "0";
            if (cboCategoria.SelectedIndex > 0)
            {
                try
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                    cboProducto.DataSource = context.SP_PRODUCTOS_SELECCIONAR(int.Parse(cboCategoria.SelectedValue.ToString()));
                    cboProducto.DisplayMember = "Producto";
                    cboProducto.ValueMember = "Id";
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros en pedidos");
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
                cboProducto.DisplayMember = "Producto";
                cboProducto.ValueMember = "Id";
                Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros en pedidos");
            }
        }

        private void cboCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCliente.SelectedIndex > 0)
            {
                try
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                    var resultado = context.Orders
                        .Where(o => o.CustomerID == cboCliente.SelectedValue.ToString())
                        .OrderByDescending(o => o.OrderID)
                        .Select(o => new
                        {
                            o.ShipName,
                            o.ShipAddress,
                            o.ShipCity,
                            o.ShipRegion,
                            o.ShipPostalCode,
                            o.ShipCountry
                        })
                        .FirstOrDefault();
                    if (resultado != null)
                    {
                        txtDirigidoa.Text = resultado.ShipName;
                        txtDomicilio.Text = resultado.ShipAddress;
                        txtCiudad.Text = resultado.ShipCity;
                        txtRegion.Text = resultado.ShipRegion;
                        txtCP.Text = resultado.ShipPostalCode;
                        txtPais.Text = resultado.ShipCountry;
                    }
                    else
                        txtDirigidoa.Text = txtDomicilio.Text = txtCiudad.Text = txtRegion.Text = txtCP.Text = txtPais.Text = "";
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros en pedidos");
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
                txtDirigidoa.Text = txtDomicilio.Text = txtCiudad.Text = txtRegion.Text = txtCP.Text = txtPais.Text = "";
        }

        private void cboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProducto.SelectedIndex > 0)
            {
                try
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                    var resultado = context.Products
                        .Where(p => p.ProductID == int.Parse(cboProducto.SelectedValue.ToString()))
                        .Select(p => new
                        {
                            p.UnitPrice,
                            p.UnitsInStock
                        })
                        .FirstOrDefault();
                    if (resultado != null)
                    {
                        txtPrecio.Text = $"{resultado.UnitPrice:C2}";
                        txtUInventario.Text = resultado.UnitsInStock.ToString();
                        if (int.Parse(txtUInventario.Text) == 0)
                        {
                            DeshabilitarControlesProducto();
                            MessageBox.Show("No hay este producto en existencia", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cboProducto.SelectedIndex = 0;
                            txtPrecio.Text = "$0.00";
                            txtUInventario.Text = "0";
                            txtCantidad.Text = "0";
                            txtDescuento.Text = "0.00";
                        }
                        else
                            HabilitarControlesProducto();
                    }
                    else
                    {
                        DeshabilitarControlesProducto();
                        txtPrecio.Text = "$0.00";
                        txtUInventario.Text = "0";
                        txtCantidad.Text = "0";
                        txtDescuento.Text = "0.00";
                    }
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros en pedidos");
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
                DeshabilitarControlesProducto();
                txtPrecio.Text = "$0.00";
                txtUInventario.Text = "0";
                txtCantidad.Text = "0";
                txtDescuento.Text = "0.00";
            }
        }

        private void CalcularTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow dgvr in dgvDetalle.Rows)
            {
                decimal importe = decimal.Parse(dgvr.Cells["Importe"].Value.ToString());
                total += importe;
            }
            txtTotal.Text = string.Format("{0:c}", total);
        }

        private void txtDescuento_Enter(object sender, EventArgs e)
        {
            txtDescuento.Text = "";
        }

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            if (txtDescuento.Text == "")
                txtDescuento.Text = "0.00";
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosConPunto(sender, e);
        }

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            if (txtCantidad.Text == "" || int.Parse(txtCantidad.Text) == 0) txtCantidad.Text = "1";
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtCantidad_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (txtCantidad.Text != "")
            {
                if (int.Parse(txtCantidad.Text.Replace(",", "")) > 32767)
                {
                    errorProvider1.SetError(txtCantidad, "La cantidad no puede ser mayor a 32767");
                    e.Cancel = true;
                    return;
                }
                else
                    errorProvider1.SetError(txtCantidad, "");
                if (int.Parse(txtCantidad.Text) > int.Parse(txtUInventario.Text))
                {
                    errorProvider1.SetError(txtCantidad, "La cantidad de productos en el pedido excede el inventario disponible");
                    e.Cancel = true;
                }
            }
        }

        private void txtFlete_Enter(object sender, EventArgs e)
        {
            if (txtFlete.Text.Contains("$")) txtFlete.Text = txtFlete.Text.Replace("$", "");
            if (decimal.Parse(txtFlete.Text) == 0) txtFlete.Text = "";
        }

        private void txtFlete_Leave(object sender, EventArgs e)
        {
            if (txtFlete.Text == "") txtFlete.Text = "0.00";
            decimal flete = decimal.Parse(txtFlete.Text);
            txtFlete.Text = flete.ToString("c");
        }

        private void txtFlete_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosConPunto(sender, e);
        }

        private void dtpPedido_ValueChanged(object sender, EventArgs e)
        {
            if (dtpPedido.Checked)
            {
                dtpHoraPedido.Value = DateTime.Now; // este es para que me ponga el componente del time
                dtpHoraPedido.Enabled = true;
            }
            else
            {
                dtpHoraPedido.Value = DateTime.Today; // este es para que no me ponga el componente del time
                dtpHoraPedido.Enabled = false;
            }
        }

        private void dtpRequerido_ValueChanged(object sender, EventArgs e)
        {
            if (dtpRequerido.Checked)
            {
                dtpHoraRequerido.Value = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 12:00:00.000");
                dtpHoraRequerido.Enabled = true;
            }
            else
            {
                dtpHoraRequerido.Value = DateTime.Today;
                dtpHoraRequerido.Enabled = false;
            }
        }

        private void dtpEnvio_ValueChanged(object sender, EventArgs e)
        {
            if (dtpEnvio.Checked)
            {
                dtpHoraEnvio.Value = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 12:00:00.000");
                dtpHoraEnvio.Enabled = true;
            }
            else
            {
                dtpHoraEnvio.Value = DateTime.Today;
                dtpHoraEnvio.Enabled = false;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            if (cboCategoria.SelectedIndex <= 0)
            {
                errorProvider1.SetError(cboCategoria, "Seleccione la categoría");
                return;
            }
            if (cboProducto.SelectedIndex <= 0)
            {
                errorProvider1.SetError(cboProducto, "Seleccione el producto");
                return;
            }
            if (txtCantidad.Text == "" || int.Parse(txtCantidad.Text) == 0)
            {
                errorProvider1.SetError(txtCantidad, "Ingrese la cantidad");
                return;
            }
            if (decimal.Parse(txtDescuento.Text) > 1 || decimal.Parse(txtDescuento.Text) < 0)
            {
                errorProvider1.SetError(txtDescuento, "El descuento no puede ser mayor que 1 o menor que 0");
                return;
            }
            if (int.Parse(txtCantidad.Text) > int.Parse(txtUInventario.Text))
            {
                errorProvider1.SetError(txtCantidad, "La cantidad de productos en el pedido excede el inventario disponible");
                return;
            }
            DeshabilitarControlesProducto();
            txtPrecio.Text = txtPrecio.Text.Replace("$", "");
            dgvDetalle.Rows.Add(new object[] { IdDetalle, cboProducto.Text, txtPrecio.Text, txtCantidad.Text, txtDescuento.Text, ((decimal.Parse(txtPrecio.Text) * decimal.Parse(txtCantidad.Text)) * (1 - decimal.Parse(txtDescuento.Text))).ToString(), "Eliminar", cboProducto.SelectedValue });
            CalcularTotal();
            ++IdDetalle;
            cboCategoria.SelectedIndex = cboProducto.SelectedIndex = 0;
            txtPrecio.Text = "$0.00";
            txtCantidad.Text = txtUInventario.Text = "0";
            txtDescuento.Text = "0.00";
            cboCategoria.Focus();
        }

        private void dgvDetalle_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.Value != null) e.Value = decimal.Parse(e.Value.ToString()).ToString("c");
            if (e.ColumnIndex == 3 && e.Value != null) e.Value = decimal.Parse(e.Value.ToString()).ToString("n0");
            if (e.ColumnIndex == 4 && e.Value != null) e.Value = decimal.Parse(e.Value.ToString()).ToString("n2");
            if (e.ColumnIndex == 5 && e.Value != null) e.Value = decimal.Parse(e.Value.ToString()).ToString("c");
        }

        private void dgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != dgvDetalle.Columns["Eliminar"].Index) return;
            dgvDetalle.Rows.RemoveAt(e.RowIndex);
            CalcularTotal();
        }

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            lastSelectedTab = e.TabPage;  // actualizar la pestaña actual
            IdDetalle = 1;
            BorrarDatosPedido();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tabpRegistrar)
            {
                if (EventoCargardo)
                {
                    dgvPedidos.CellClick -= new DataGridViewCellEventHandler(dgvPedidos_CellClick);
                    EventoCargardo = false;
                }
                BorrarDatosBusqueda();
                HabilitarControles();
                btnGenerar.Text = "Generar pedido";
                btnGenerar.Visible = true;
                btnGenerar.Enabled = true;
                btnAgregar.Visible = true;
                btnAgregar.Enabled = true;
                dgvDetalle.Columns["Eliminar"].Visible = true;
                grbProducto.Enabled = true;
            }
            else
            {
                if (!EventoCargardo)
                {
                    dgvPedidos.CellClick += new DataGridViewCellEventHandler(dgvPedidos_CellClick);
                    EventoCargardo = true;
                }
                DeshabilitarControles();
                btnGenerar.Enabled = false;
                dgvDetalle.Columns["Eliminar"].Visible = false;
                grbProducto.Enabled = false;
                if (tabcOperacion.SelectedTab == tabpConsultar)
                {
                    btnGenerar.Visible = false;
                    btnAgregar.Visible = false;
                }
                else if (tabcOperacion.SelectedTab == tabpModificar)
                {
                    btnGenerar.Text = "Modificar pedido";
                    btnGenerar.Visible = true;
                    btnAgregar.Visible = false;
                }
                else if (tabcOperacion.SelectedTab == tabpEliminar)
                {
                    btnGenerar.Text = "Eliminar Pedido";
                    btnGenerar.Visible = true;
                    btnAgregar.Visible = false;
                }
            }
        }

        private void dgvPedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tabcOperacion.SelectedTab != tabpRegistrar)
            {
                BorrarDatosPedido();
                DataGridViewRow dgvr = dgvPedidos.CurrentRow;
                txtId.Text = dgvr.Cells["Id"].Value.ToString();
                LlenarDatosPedido();
                LlenarDatosDetallePedido();
                DeshabilitarControles();
                if (tabcOperacion.SelectedTab == tabpModificar)
                {
                    HabilitarControles();
                    btnGenerar.Enabled = true;
                }
                else if(tabcOperacion.SelectedTab == tabpEliminar)
                {
                    btnGenerar.Enabled = true;
                }
            }
        }

        private void LlenarDatosPedido()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var resultado = context.SP_PEDIDOS_LISTAR1(int.Parse(txtId.Text)).FirstOrDefault();
                if (resultado != null)
                {
                    cboCliente.SelectedIndexChanged -= new EventHandler(cboCliente_SelectedIndexChanged);
                    cboCliente.SelectedValue = resultado.CustomerID;
                    cboCliente.SelectedIndexChanged += new EventHandler(cboCliente_SelectedIndexChanged);
                    cboEmpleado.SelectedValue = resultado.EmployeeID;
                    cboTransportista.SelectedValue = resultado.ShipVia;
                    txtDirigidoa.Text = resultado.ShipName;
                    txtDomicilio.Text = resultado.ShipAddress;
                    txtCiudad.Text = resultado.ShipCity;
                    txtRegion.Text = resultado.ShipRegion;
                    txtCP.Text = resultado.ShipPostalCode;
                    txtPais.Text = resultado.ShipCountry;
                    txtFlete.Text = string.Format("{0:c}", resultado.Freight.ToString());
                    //checar aqui procedimiento
                    decimal flete;
                    if (decimal.TryParse(txtFlete.Text, out flete))
                        txtFlete.Text = flete.ToString();
                    // hasta aqui
                    DateTime fecha;
                    if (DateTime.TryParse(resultado.OrderDate.ToString(), out fecha))
                        dtpPedido.Value = dtpHoraPedido.Value = fecha;
                    else
                    {
                        dtpPedido.Value = dtpHoraPedido.Value = dtpPedido.MinDate;
                        dtpPedido.Checked = false;
                    }
                    if (DateTime.TryParse(resultado.RequiredDate.ToString(), out fecha))
                        dtpRequerido.Value = dtpHoraRequerido.Value = fecha;
                    else
                    {
                        dtpRequerido.Value = dtpHoraRequerido.Value = dtpRequerido.MinDate;
                        dtpRequerido.Checked = false;
                    }
                    if (DateTime.TryParse(resultado.ShippedDate.ToString(), out fecha))
                        dtpEnvio.Value = dtpHoraEnvio.Value = fecha;
                    else
                    {
                        dtpEnvio.Value = dtpHoraEnvio.Value = dtpEnvio.MinDate;
                        dtpEnvio.Checked = false;
                    }
                }
                Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros en pedidos");
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

        private void LlenarDatosDetallePedido()
        {
            try
            {
                IdDetalle = 1;
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var resultado = context.SP_DETALLEPEDIDOS_PRODUCTOS_LISTAR1(int.Parse(txtId.Text));
                if (resultado != null && resultado.Any())
                {
                    //Order_Details pedidoDetalle;
                    foreach (var pd in resultado)
                    {
                        var pedidoDetalle = new Order_Details()
                        {
                            ProductID = pd.Id_Producto,
                            //pedidoDetalle.productName = pd.Producto;
                            UnitPrice = pd.Precio,
                            Quantity = pd.Cantidad,
                            Discount = float.Parse(pd.Descuento.ToString())
                        };
                        string productName = pd.Producto;
                        float total = float.Parse(pedidoDetalle.UnitPrice.ToString()) * float.Parse(pedidoDetalle.Quantity.ToString()) * (1 - pedidoDetalle.Discount);
                        dgvDetalle.Rows.Add(new object[] { IdDetalle, productName, pedidoDetalle.UnitPrice, pedidoDetalle.Quantity, pedidoDetalle.Discount, total, "Eliminar", pedidoDetalle.ProductID });
                        ++IdDetalle;
                    }
                }
                else
                {
                    MessageBox.Show("No se encontraron detalles para el pedido especificado", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                CalcularTotal();
                Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros en pedidos");
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

        private class PedidosDB
        {
            public int PedidoId { get; set; }

            public byte Add(Form form, Orders pedido, List<Order_Details> lst, TextBox textBox, string cliente)
            {
                byte numRegs = 0;
                using (var context = new NorthwindTradersDataContext())
                {
                    using (var transaction = context.Connection.BeginTransaction())
                    {
                        try
                        {
                            context.Transaction = transaction;
                            // Creamos una nueva instancia de Order
                            var nuevoPedido = new Orders()
                            {
                                CustomerID = pedido.CustomerID,
                                EmployeeID = pedido.EmployeeID,
                                OrderDate = pedido.OrderDate,
                                RequiredDate = pedido.RequiredDate,
                                ShippedDate = pedido.ShippedDate,
                                ShipVia = pedido.ShipVia,
                                Freight = pedido.Freight,
                                ShipName = string.IsNullOrWhiteSpace(pedido.ShipName) ? null : pedido.ShipName,
                                ShipAddress = string.IsNullOrWhiteSpace(pedido.ShipAddress) ? null : pedido.ShipAddress,
                                ShipCity = string.IsNullOrWhiteSpace(pedido.ShipCity) ? null : pedido.ShipCity,
                                ShipRegion = string.IsNullOrWhiteSpace(pedido.ShipRegion) ? null : pedido.ShipRegion,
                                ShipPostalCode = string.IsNullOrWhiteSpace(pedido.ShipPostalCode) ? null : pedido.ShipPostalCode,
                                ShipCountry = string.IsNullOrWhiteSpace(pedido.ShipCountry) ? null : pedido.ShipCountry
                            };
                            // Agregamos el nuevo pedido al contexto
                            context.Orders.InsertOnSubmit(nuevoPedido);
                            // Creamos los detalles del pedido y los agregamos al contexto
                            foreach (var item in lst)
                            {
                                var nuevoDetalle = new Order_Details
                                {
                                    ProductID = item.ProductID,
                                    UnitPrice = item.UnitPrice,
                                    Quantity = item.Quantity,
                                    Discount = (float)item.Discount,
                                    Orders = nuevoPedido // Relacionamos el detalle con el pedido
                                };
                                context.Order_Details.InsertOnSubmit(nuevoDetalle);
                                // Guardamos los cambios en la base de datos
                                context.SubmitChanges();
                                // Confirma la transacción
                                transaction.Commit();
                                // Obtenemos el Id del nuevo pedido
                                int PedidoId = nuevoPedido.OrderID;
                                textBox.Text = PedidoId.ToString();
                                numRegs = 1;
                                MessageBox.Show($"El pedido con Id: {PedidoId} del Cliente: {cliente}, se registró satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (SqlException ex)
                        {
                            // Si ocurre un error de base de datos, revertimos la transacción
                            transaction.Rollback();
                            Utils.MsgCatchOueclbdd(form, ex);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Utils.MsgCatchOue(form, ex);
                        }
                    }
                }
                return numRegs;
            }

            public byte Update(Form form, Orders pedido, string cliente)
            {
                byte numRegs = 0;
                using (var context = new NorthwindTradersDataContext())
                {
                    // Iniciamos una transacción
                    using (var transaction = context.Connection.BeginTransaction())
                    {
                        try
                        {
                            context.Transaction = transaction;
                            // Obtenemos el pedido que queremos actualizar
                            var ped = context.Orders.SingleOrDefault(o => o.OrderID == pedido.OrderID);
                            if (ped != null)
                            {
                                // Actualizamos los campos del pedido
                                ped.CustomerID = pedido.CustomerID;
                                ped.EmployeeID = pedido.EmployeeID;
                                ped.OrderDate = pedido.OrderDate == null ? (DateTime?)null : pedido.OrderDate;
                                ped.RequiredDate = pedido.RequiredDate == null ? (DateTime?)null : pedido.RequiredDate;
                                ped.ShippedDate = pedido.ShippedDate == null ? (DateTime?)null : pedido.ShippedDate;
                                ped.ShipVia = pedido.ShipVia;
                                ped.Freight = pedido.Freight;
                                ped.ShipName = string.IsNullOrWhiteSpace(pedido.ShipName) ? null : pedido.ShipName;
                                ped.ShipAddress = string.IsNullOrWhiteSpace(pedido.ShipAddress) ? null : pedido.ShipAddress;
                                ped.ShipCity = string.IsNullOrWhiteSpace(pedido.ShipCity) ? null : pedido.ShipCity;
                                ped.ShipRegion = string.IsNullOrWhiteSpace(pedido.ShipRegion) ? null : pedido.ShipRegion;
                                ped.ShipPostalCode = string.IsNullOrWhiteSpace(pedido.ShipPostalCode) ? null : pedido.ShipPostalCode;
                                ped.ShipCountry = string.IsNullOrWhiteSpace(pedido.ShipCountry) ? null : pedido.ShipCountry;
                                // Guardamos los cambios en la base de datos
                                context.SubmitChanges();
                                // Confirmamos la transacción
                                transaction.Commit();
                                numRegs = 1;
                                MessageBox.Show($"El pedido con Id: {pedido.OrderID} del cliente : {cliente}, se actualizó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                                MessageBox.Show("No se pudo realizar la modificación, es posible que el registro se haya eliminado previamente por otro usuario de la red", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (SqlException ex)
                        {
                            // Si ocurre un error, revertimos la transacción
                            transaction.Rollback();
                            Utils.MsgCatchOueclbdd(form, ex);
                        }
                        catch (Exception ex)
                        {
                            // Manejo general de excepciones
                            transaction.Rollback();
                            Utils.MsgCatchOue(form, ex);
                        }
                    }
                }
                return numRegs;
            }

            public byte Delete(Form form, Orders pedido, string cliente)
            {
                byte numRegs = 0;
                using (var context = new NorthwindTradersDataContext())
                {
                    // Iniciamos una transacción
                    using (var transaction = context.Connection.BeginTransaction())
                    {
                        try
                        {
                            context.Transaction = transaction;
                            // Obtenemos el pedido que queremos eliminar
                            var pedidosAEliminar = context.Orders.SingleOrDefault(o => o.OrderID == pedido.OrderID);
                            if (pedidosAEliminar != null)
                            {
                                // Obtenemos los detalles del pedido que queremos eliminar
                                var detallesAEliminar = context.Order_Details.Where(d => d.OrderID == pedido.OrderID).ToList();
                                // Eliminamos los detalles del pedido
                                context.Order_Details.DeleteAllOnSubmit(detallesAEliminar);
                                // Eliminamos el pedido
                                context.Orders.DeleteOnSubmit(pedidosAEliminar);
                                // Guardamos los cambios en la base de datos
                                context.SubmitChanges();
                                // Confirmamos la transacción
                                transaction.Commit();
                                numRegs = 1; // suponiendo que un registro ha sido eliminado
                                MessageBox.Show($"El pedido con Id: {pedido.OrderID} del Cliente, se eliminó satisfactoriamente junto con sus registros de detalle del pedido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                                MessageBox.Show("No se pudo realizar la eliminación, es posible que el registro haya sido eliminado previamente por otro usuario de la red", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (SqlException ex)
                        {
                            // Si ocurre un error, revertimos la transacción
                            transaction.Rollback();
                            Utils.MsgCatchOueclbdd(form, ex);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Utils.MsgCatchOue(form, ex);
                        }
                    }
                }
                return numRegs;
            }
        }
    }
}

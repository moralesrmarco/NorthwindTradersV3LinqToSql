using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmPedidosCrudV2 : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        private TabPage lastSelectedTab;
        bool EventoCargado = true; // esta variable es necesaria para controlar el manejador de eventos de la celda del dgv, ojo no quitar
        int IdDetalle = 1;
        bool PedidoGenerado = false;

        public FrmPedidosCrudV2()
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

        private void FrmPedidosCrudV2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.ActualizarBarraDeEstado(this);
        }

        private void FrmPedidosCrudV2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tabcOperacion.SelectedTab == tabpRegistrar)
            {
                if (cboCliente.SelectedIndex > 0 || cboEmpleado.SelectedIndex > 0 || cboTransportista.SelectedIndex > 0 || cboCategoria.SelectedIndex > 0 || cboProducto.SelectedIndex > 0 || dgvDetalle.RowCount > 0)
                {
                    DialogResult respuesta = MessageBox.Show(Utils.preguntaCerrar, Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (respuesta == DialogResult.None)
                        e.Cancel = true;
                    else
                        e.Cancel = false;
                }
            }
        }

        private void FrmPedidosCrudV2_Load(object sender, EventArgs e)
        {
            dtpHoraEnvio.Value = dtpHoraRequerido.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DeshabilitarControles();
            DeshabilitarControlesProducto();
            //Utils.LlenarCbo(this, cboCliente, "Sp_Clientes_Seleccionar", "Cliente", "Id", context);
            LlenarCboCliente();
            LlenarCboEmpleado();
            LlenarCboTransportista();
            LlenarCboCategoria();
            Utils.ConfDgv(dgvPedidos);
            Utils.ConfDgv(dgvDetalle);
            LlenarDgvPedidos(null);
            ConfDgvPedidos();
            ConfDgvDetalle();
            OcultarCols();
            InicializarValores();
        }

        private void DeshabilitarControles()
        {
            cboCliente.Enabled = cboEmpleado.Enabled = cboTransportista.Enabled = false;
            dtpPedido.Enabled = dtpHoraPedido.Enabled = dtpRequerido.Enabled = dtpHoraRequerido.Enabled = dtpEnvio.Enabled = dtpHoraEnvio.Enabled = false;
            txtDirigidoa.ReadOnly = txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCP.ReadOnly = txtPais.ReadOnly = txtFlete.ReadOnly = true;
            btnGenerar.Enabled = false;
        }

        private void HabilitarControles()
        {
            cboCliente.Enabled = cboEmpleado.Enabled = cboTransportista.Enabled = true;
            dtpPedido.Enabled = dtpRequerido.Enabled = dtpEnvio.Enabled = true;
            txtDirigidoa.ReadOnly = txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCP.ReadOnly = txtPais.ReadOnly = txtFlete.ReadOnly = false;
            btnGenerar.Enabled = true;
        }

        private void DeshabilitarControlesProducto()
        {
            cboCategoria.Enabled = cboProducto.Enabled = false;
            txtCantidad.Enabled = txtDescuento.Enabled = false;
            btnAgregar.Enabled = false;
        }

        private void HabilitarControlesProducto()
        {
            txtCantidad.Enabled = txtDescuento.Enabled = true;
            btnAgregar.Enabled = true;
        }

        private void LlenarCboCliente()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var query = context.SP_CLIENTES_SELECCIONAR().ToList();
                cboCliente.DataSource = query;
                cboCliente.DisplayMember = "Cliente";
                cboCliente.ValueMember = "Id";
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

        private void LlenarCboEmpleado()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var query = context.SP_EMPLEADOS_SELECCIONAR().ToList();
                cboEmpleado.DataSource = query;
                cboEmpleado.DisplayMember = "Empleado";
                cboEmpleado.ValueMember = "Id";
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

        private void LlenarCboTransportista()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var query = context.SP_TRANSPORTISTAS_SELECCIONAR();
                cboTransportista.DataSource = query.ToList();
                cboTransportista.DisplayMember = "Transportista";
                cboTransportista.ValueMember = "Id";
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

        private void LlenarCboCategoria()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var query = context.SP_CATEGORIAS_SELECCIONAR_V2();
                cboCategoria.DataSource = query.ToList();
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
            dgvPedidos.Columns["Fecha_requerido"].HeaderText = "Fecha de entrega";
            dgvPedidos.Columns["Fecha_de_envío"].HeaderText = "Fecha de envío";
            dgvPedidos.Columns["Compañía_transportista"].HeaderText = "Compañía transportista";
            dgvPedidos.Columns["Dirigido_a"].HeaderText = "Enviar a";
        }

        private void ConfDgvDetalle()
        {
            dgvDetalle.Columns["Id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetalle.Columns["Cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns["Descuento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns["Importe"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void OcultarCols() => dgvDetalle.Columns["Modificar"].Visible = dgvDetalle.Columns["Eliminar"].Visible = false;

        private void MostrarCols() => dgvDetalle.Columns["Modificar"].Visible = dgvDetalle.Columns["Eliminar"].Visible = true;

        private void InicializarValores()
        {
            txtFlete.Text = txtTotal.Text = txtPrecio.Text = "$0.00";
            txtDescuento.Text = "0.00";
            txtUInventario.Text = txtCantidad.Text = "0";
        }

        private void InicializarValoresTotalyProducto()
        {
            txtTotal.Text = "$0.00";
            InicializarValoresProducto();
        }

        private void InicializarValoresProducto()
        {
            txtPrecio.Text = "$0.00";
            txtUInventario.Text = txtCantidad.Text = "0";
            txtDescuento.Text = "0.00";
        }

        private void InicializarValoresTransportar() => txtDirigidoa.Text = txtDomicilio.Text = txtCiudad.Text = txtRegion.Text = txtCP.Text = txtPais.Text = "";

        private void LlenarDgvPedidos(object sender)
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                if (sender == null)
                    dgvPedidos.DataSource = context.SP_PEDIDOS_LISTAR20().ToList();
                else
                {
                    int intBIdIni = 0, intBIdFin = 0;
                    bool boolFPedido = false, boolFRequerido = false, boolFEnvio = false;
                    DateTime? FPedidoIni, FPedidoFin, FRequeridoIni, FRequeridoFin, FEnvioIni, FEnvioFin;
                    if (txtBIdInicial.Text != "") intBIdIni = int.Parse(txtBIdInicial.Text);
                    if (txtBIdFinal.Text != "") intBIdFin = int.Parse(txtBIdFinal.Text);
                    if (dtpBFPedidoIni.Checked && dtpBFPedidoFin.Checked)
                    {
                        boolFPedido = true; // este parametro es requerido para que funcione el stored procedure con la misma logica que he venido usando en las demas busquedas
                        FPedidoIni = dtpBFPedidoIni.Value = Convert.ToDateTime(dtpBFPedidoIni.Value.ToShortDateString() + " 00:00:00.000");
                        FPedidoFin = dtpBFPedidoFin.Value = Convert.ToDateTime(dtpBFPedidoFin.Value.ToShortDateString() + " 23:59:59.998"); // se usa .998 porque lo redondea a .997 por la presición de los campos tipo datetime de sql server, el cual es el maximo valor de milesimas de segundo que puede guardarse en la db. Si se usa .999 lo redondea al segundo 0.000 del siquiente dia e incluye los datos del siguiente día que es un comportamiento que no se quiere por que solo se deben mostrar los datos de la fecha indicada. Ya se comprobo el comportamiento en la base de datos.
                    }
                    else
                    {
                        boolFPedido = false;
                        FPedidoFin = FPedidoIni = null;
                    }
                    if (dtpBFRequeridoIni.Checked && dtpBFRequeridoFin.Checked)
                    {
                        boolFRequerido = true;
                        FRequeridoIni = dtpBFRequeridoIni.Value = Convert.ToDateTime(dtpBFRequeridoIni.Value.ToShortDateString() + " 00:00:00.000");
                        FRequeridoFin = dtpBFRequeridoFin.Value = Convert.ToDateTime(dtpBFRequeridoFin.Value.ToShortDateString() + " 23:59:59.998");
                        
                    }
                    else
                    {
                        boolFRequerido = false;
                        FRequeridoFin = FRequeridoIni = null;
                    }
                    if (dtpBFEnvioIni.Checked && dtpBFEnvioFin.Checked)
                    {
                        boolFEnvio = true;
                        FEnvioIni = dtpBFEnvioIni.Value = Convert.ToDateTime(dtpBFEnvioIni.Value.ToShortDateString() + " 00:00:00.000");
                        FEnvioFin = dtpBFEnvioFin.Value = Convert.ToDateTime(dtpBFEnvioFin.Value.ToShortDateString() + " 23:59:59.998");
                    }
                    else
                    {
                        boolFEnvio = false;
                        FEnvioFin = FEnvioIni = null;
                    }
                    dgvPedidos.DataSource = context.SP_PEDIDOS_BUSCAR(intBIdIni, intBIdFin, txtBCliente.Text, boolFPedido, chkBFPedidoNull.Checked, FPedidoIni, FPedidoFin, boolFRequerido, chkBFRequeridoNull.Checked, FRequeridoIni, FRequeridoFin, boolFEnvio, chkBFEnvioNull.Checked, FEnvioIni, FEnvioFin, txtBEmpleado.Text, txtBCompañiaT.Text, txtBDirigidoa.Text).ToList();
                    if (sender == null)
                        Utils.ActualizarBarraDeEstado(this, $"Se muestran los últimos {dgvPedidos.RowCount} pedidos registrados");
                    else
                        Utils.ActualizarBarraDeEstado(this, $"Se encontraron {dgvPedidos.RowCount} registros");
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

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarDatosPedido();
            BorrarMensajesError();
            BorrarDatosBusqueda();
            if (tabcOperacion.SelectedTab != tabpRegistrar)
            {
                DeshabilitarControles();
                DeshabilitarControlesProducto();
            }
            btnNota.Enabled = false;
            dgvPedidos.Focus();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosPedido();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tabpRegistrar)
            {
                DeshabilitarControles();
                DeshabilitarControlesProducto();
            }
            btnNota.Enabled = false;
            LlenarDgvPedidos(sender);
            dgvPedidos.Focus();
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            btnLimpiar.PerformClick();
            LlenarDgvPedidos(null);
            dgvPedidos.Focus();
        }

        private void BorrarDatosPedido()
        {
            txtId.Text = "";
            txtId.Tag = null;
            cboCliente.SelectedIndex = cboEmpleado.SelectedIndex = cboTransportista.SelectedIndex = cboCategoria.SelectedIndex = 0;
            cboProducto.DataSource = null;
            dtpPedido.Value = dtpRequerido.Value = dtpEnvio.Value = DateTime.Now;
            dtpHoraPedido.Value = DateTime.Now;
            dtpHoraRequerido.Value = dtpHoraEnvio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dtpRequerido.Checked = dtpEnvio.Checked = false;
            InicializarValoresTransportar();
            InicializarValores();
            btnNota.Visible = false;
            dgvDetalle.Rows.Clear();
        }

        private void BorrarMensajesError() => errorProvider1.Clear();

        private void BorrarDatosBusqueda()
        {
            txtBIdInicial.Text = txtBIdFinal.Text = txtBCliente.Text = txtBEmpleado.Text = txtBCompañiaT.Text = txtBDirigidoa.Text = "";
            dtpBFPedidoIni.Value = dtpBFPedidoFin.Value = dtpBFRequeridoIni.Value = dtpBFRequeridoFin.Value = dtpBFEnvioIni.Value = dtpBFEnvioFin.Value = DateTime.Today;
            dtpBFPedidoIni.Checked = dtpBFPedidoFin.Checked = dtpBFRequeridoIni.Checked = dtpBFRequeridoFin.Checked = dtpBFEnvioIni.Checked = dtpBFEnvioFin.Checked = false;
            chkBFPedidoNull.Checked = chkBFRequeridoNull.Checked = chkBFEnvioNull.Checked = false;
        }

        private void txtBIdInicial_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBIdFinal_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBIdInicial_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdIni(txtBIdInicial, txtBIdFinal);

        private void txtBIdFinal_Leave(object sender, EventArgs e) => Utils.ValidaTxtBIdFin(txtBIdInicial, txtBIdFinal);

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
            {
                dtpBFRequeridoFin.Checked = false;
            }
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
            if (cboCategoria.SelectedIndex > 0)
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                cboProducto.DataSource = context.SP_PRODUCTOS_SELECCIONAR(int.Parse(cboCategoria.SelectedValue.ToString()));
                cboProducto.DisplayMember = "Producto";
                cboProducto.ValueMember = "Id";
                cboProducto.Enabled = true;
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
            }
            Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros en pedidos");
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
                        InicializarValoresTransportar();
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
                InicializarValoresTransportar();
        }

        private void cboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProducto.SelectedIndex > 0)
            {
                try
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                    InicializarValoresProducto();
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
                        txtPrecio.Text = $"{resultado.UnitPrice:c2}";
                        txtUInventario.Text = resultado.UnitsInStock.ToString();
                        if (int.Parse(txtUInventario.Text) == 0)
                        {
                            DeshabilitarControlesProducto();
                            MessageBox.Show("No hay este producto en existencia", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            InicializarValoresProducto();
                            BorrarMensajesError();
                            cboCategoria.Enabled = true;
                            cboProducto.SelectedIndex = 0;
                        }
                        else
                            HabilitarControlesProducto();
                    }
                    else
                    {
                        DeshabilitarControlesProducto();
                        InicializarValoresProducto();
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
            else
            {
                DeshabilitarControlesProducto();
                cboCategoria.Enabled = true;
                InicializarValoresProducto();
            }
        }

        private void CalcularTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow dgvr in dgvDetalle.Rows)
                total += decimal.Parse(dgvr.Cells["Importe"].Value.ToString());
            txtTotal.Text = string.Format("{0:c}", total);
        }

        private void txtDescuento_Enter(object sender, EventArgs e) => txtDescuento.Text = "";

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            if (txtDescuento.Text == "")
                txtDescuento.Text = "0.00";
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosConPunto(sender, e);

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            if (txtCantidad.Text == "" || int.Parse(txtCantidad.Text) == 0) txtCantidad.Text = "1";
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtCantidad_Validating(object sender, CancelEventArgs e)
        {
            if (txtCantidad.Text != "")
            {
                if (int.Parse(txtCantidad.Text.Replace(",", "")) > 32767)
                {
                    errorProvider1.SetError(txtCantidad, "La cantidad no puede ser mayor a 32,767");
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
            txtFlete.Text = flete.ToString("c2");
        }

        private void txtFlete_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosConPunto(sender, e);

        private void dtpPedido_ValueChanged(object sender, EventArgs e)
        {
            if (dtpPedido.Checked)
            {
                dtpHoraPedido.Value = DateTime.Now;
                dtpHoraPedido.Enabled = true;
            }
            else
            {
                dtpHoraPedido.Value = DateTime.Today;
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
            if (!chkRowVersion())
            {
                MessageBox.Show("El registro ha sido modificado por otro usuario de la red, vuelva a cargar el registro para que se actualice con los datos proporcionados por el otro usuario", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
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
            int numProd = int.Parse(cboProducto.SelectedValue.ToString());
            bool productoDuplicado = false;
            foreach (DataGridViewRow dgvr in dgvDetalle.Rows)
            {
                if (int.Parse(dgvr.Cells["ProductoId"].Value.ToString()) == numProd)
                {
                    productoDuplicado = true;
                    break;
                }
            }
            if (productoDuplicado)
            {
                errorProvider1.SetError(cboProducto, "No se puede tener un producto duplicado en el detalle del pedido");
                return;
            }
            DeshabilitarControlesProducto();
            if (tabcOperacion.SelectedTab == tabpRegistrar & !PedidoGenerado)
            {
                txtPrecio.Text = txtPrecio.Text.Replace("$", "");
                dgvDetalle.Rows.Add(new object[] { IdDetalle, cboProducto.Text, txtPrecio.Text, txtCantidad.Text, txtDescuento.Text, ((decimal.Parse(txtPrecio.Text) * decimal.Parse(txtCantidad.Text)) * (1 - decimal.Parse(txtDescuento.Text))).ToString(), "Modificar", "Eliminar", cboProducto.SelectedValue, txtUInventario.Text });
                CalcularTotal();
                ++IdDetalle;
                cboCategoria.SelectedIndex = cboProducto.SelectedIndex = 0;
                InicializarValoresProducto();
                cboCategoria.Focus();
            }
            else if (tabcOperacion.SelectedTab == tabpModificar | (tabcOperacion.SelectedTab == tabpRegistrar & PedidoGenerado))
            {
                byte numRegs = 0;
                try
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.insertandoRegistro);
                    DeshabilitarControlesProducto();
                    Order_Details pedidoDetalle = new Order_Details();
                    pedidoDetalle.OrderID = int.Parse(txtId.Text);
                    pedidoDetalle.ProductID = (int)cboProducto.SelectedValue;
                    pedidoDetalle.UnitPrice = decimal.Parse(txtPrecio.Text.Replace("$", ""));
                    pedidoDetalle.Quantity = short.Parse(txtCantidad.Text);
                    pedidoDetalle.Discount = float.Parse(txtDescuento.Text);
                    numRegs = AgregarPedidoDetalle(pedidoDetalle);
                    if (numRegs > 0)
                        MessageBox.Show($"El producto: {cboProducto.Text} del Pedido: {txtId.Text}, se registró satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show($"El producto: {cboProducto.Text} del Pedido: {txtId.Text}, NO se registró en la base de datos", Utils.nwtr,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros de pedidos");
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
                    btnNota.Enabled = true;
                    btnNota.Visible = true;
                    Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros de pedidos");
                    dgvDetalle.Focus();
                }
            }
        }

        private byte AgregarPedidoDetalle(Order_Details pedidoDetalle)
        {
            byte numRegs = 0;
            using (var context = new NorthwindTradersDataContext())
            {
                // Nos aseguramos que la conexion esté abierta
                if (context.Connection.State == ConnectionState.Closed)
                    context.Connection.Open();
                // Iniciamos una transacción
                using (var transaction = context.Connection.BeginTransaction())
                {
                    // las excepciones generadas en este segmento de código son capturadas en un nivel superior, por eso uso throw para relanzar la excepción y se procese en el nivel superior.
                    try
                    {
                        context.Transaction = transaction;
                        // Verificar la existencia del producto
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
            InicializarValoresTotalyProducto();
            dgvDetalle.Rows.Clear();
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
                            Discount = pd.Descuento,
                            RowVersion = pd.RowVersion
                        };
                        string productName = pd.Producto;
                        float importe = (float.Parse(pedidoDetalle.UnitPrice.ToString()) * pedidoDetalle.Quantity) * (1 - pedidoDetalle.Discount);
                        dgvDetalle.Rows.Add(new object[] { IdDetalle, productName, pedidoDetalle.UnitPrice, pedidoDetalle.Quantity, pedidoDetalle.Discount, importe, "Modificar", "Eliminar", pedidoDetalle.ProductID, null, pedidoDetalle.RowVersion });
                        ++IdDetalle;
                    }
                }
                else
                    MessageBox.Show("No se encontraron detalles para el pedido especificado", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void dgvDetalle_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.Value != null) e.Value = float.Parse(e.Value.ToString()).ToString("c");
            if (e.ColumnIndex == 3 && e.Value != null) e.Value = int.Parse(e.Value.ToString()).ToString("n0");
            if (e.ColumnIndex == 4 && e.Value != null) e.Value = float.Parse(e.Value.ToString()).ToString("n2");
            if (e.ColumnIndex == 5 && e.Value != null) e.Value = float.Parse(e.Value.ToString()).ToString("c");
        }

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            lastSelectedTab = e.TabPage; // actualizar la pestaña actual
            IdDetalle = 1;
            BorrarDatosPedido();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tabpRegistrar)
            {
                if (EventoCargado)
                {
                    dgvPedidos.CellClick -= new DataGridViewCellEventHandler(dgvPedidos_CellClick);
                    EventoCargado = false;
                }
                PedidoGenerado = false;
                BorrarDatosBusqueda();
                HabilitarControles();
                cboCategoria.Enabled = true;
                btnGenerar.Text = "Generar pedido";
                btnGenerar.Visible = true;
                btnGenerar.Enabled = true;
                btnAgregar.Visible = true;
                btnAgregar.Enabled = true;
                dgvDetalle.Columns["Modificar"].Visible = true;
                dgvDetalle.Columns["Eliminar"].Visible = true;
                grbProducto.Enabled = true;
                btnNota.Visible = true;
                btnNota.Enabled = false;
                btnNuevo.Visible = true;
                btnNuevo.Enabled = false;
            }
            else
            {
                if (!EventoCargado)
                {
                    dgvPedidos.CellClick += new DataGridViewCellEventHandler(dgvPedidos_CellClick);
                    EventoCargado = true;
                }
                DeshabilitarControles();
                DeshabilitarControlesProducto();
                OcultarCols();
                if (tabcOperacion.SelectedTab == tabpConsultar)
                {
                    btnGenerar.Visible = false;
                    btnAgregar.Visible = false;
                    btnNota.Visible = true;
                    btnNota.Enabled = false;
                    btnNuevo.Visible = false;
                    btnNuevo.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tabpModificar)
                {
                    PedidoGenerado = false;
                    btnGenerar.Text = "Modificar pedido";
                    btnGenerar.Visible = true;
                    btnAgregar.Visible = true;
                    btnNota.Visible = true;
                    btnNota.Enabled = false;
                    btnNuevo.Visible = false;
                    btnNuevo.Enabled = false;
                    MostrarCols();
                }
                else if (tabcOperacion.SelectedTab == tabpEliminar)
                {
                    btnGenerar.Text = "Eliminar pedido";
                    btnGenerar.Visible = true;
                    btnAgregar.Visible = false;
                    btnNota.Visible = false;
                    btnNota.Enabled = false;
                    btnNuevo.Visible = false;
                    btnNuevo.Enabled = false;
                }
            }
        }

        private void dgvPedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnNota.Enabled=false;
            if (tabcOperacion.SelectedTab != tabpRegistrar)
            {
                BorrarDatosPedido();
                DataGridViewRow dgvr = dgvPedidos.CurrentRow;
                txtId.Text = dgvr.Cells["Id"].Value.ToString();
                LlenarDatosPedido();
                LlenarDatosDetallePedido();
                DeshabilitarControles();
                DeshabilitarControlesProducto();
                if (tabcOperacion.SelectedTab == tabpConsultar)
                {
                    btnNota.Visible = true;
                    btnNota.Enabled = true;
                    btnNuevo.Visible = false;
                }
                else if (tabcOperacion.SelectedTab == tabpModificar)
                {
                    HabilitarControles();
                    btnGenerar.Enabled = true;
                    cboCategoria.Enabled = true;
                    btnNota.Visible = true;
                    btnNota.Enabled = false;
                    btnNuevo.Visible = false;
                }
                else if (tabcOperacion.SelectedTab == tabpEliminar)
                {
                    btnGenerar.Enabled = true;
                    btnNota.Visible = false;
                    btnNuevo.Visible = false;
                }
            }
        }

        private void LlenarDatosPedido()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this, Utils.clbdd);
                var query = (from order in context.Orders
                            where order.OrderID == int.Parse(txtId.Text)
                            select new
                            {
                                order.CustomerID,
                                order.EmployeeID,
                                order.OrderDate,
                                order.RequiredDate,
                                order.ShippedDate,
                                order.ShipVia,
                                order.Freight,
                                order.ShipName,
                                order.ShipAddress,
                                order.ShipCity,
                                order.ShipRegion,
                                order.ShipPostalCode,
                                order.ShipCountry,
                                order.RowVersion
                            }).FirstOrDefault();
                Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros en pedidos");
                if (query != null)
                {
                    cboCliente.SelectedIndexChanged -= new EventHandler(cboCliente_SelectedIndexChanged);
                    cboCliente.SelectedValue = query.CustomerID;
                    cboCliente.SelectedIndexChanged += new EventHandler(cboCliente_SelectedIndexChanged);
                    cboEmpleado.SelectedValue = query.EmployeeID;
                    cboTransportista.SelectedValue = query.ShipVia;
                    txtDirigidoa.Text = query.ShipName;
                    txtDomicilio.Text = query.ShipAddress;
                    txtCiudad.Text = query.ShipCity;
                    txtRegion.Text = query.ShipRegion;
                    txtCP.Text = query.ShipPostalCode;
                    txtPais.Text = query.ShipCountry;
                    txtFlete.Text = $"{query.Freight:c2}";
                    DateTime fecha;
                    if (DateTime.TryParse(query.OrderDate.ToString(), out fecha))
                    {
                        dtpPedido.Value = fecha;
                        dtpHoraPedido.Value = fecha;
                    }
                    else
                    {
                        dtpPedido.Value = dtpPedido.MinDate;
                        dtpHoraPedido.Value = dtpHoraPedido.MinDate;
                        dtpPedido.Checked = false;
                    }
                    if (DateTime.TryParse(query.RequiredDate.ToString(), out fecha))
                    {
                        dtpRequerido.Value = fecha;
                        dtpHoraRequerido.Value = fecha;
                    }
                    else
                    {
                        dtpRequerido.Value = dtpHoraRequerido.Value = dtpRequerido.MinDate;
                        dtpRequerido.Checked = false;
                    }
                    if (DateTime.TryParse(query.ShippedDate.ToString(), out fecha))
                    {
                        dtpEnvio.Value = fecha;
                        dtpHoraEnvio.Value = fecha;
                    }
                    else
                    {
                        dtpEnvio.Value = dtpHoraEnvio.Value = dtpEnvio.MinDate;
                        dtpEnvio.Checked = false;
                    }
                    txtId.Tag = query.RowVersion;
                }
                else
                {
                    MessageBox.Show($"El pedido: {txtId.Text} fue eliminado por otro usuario de la red", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
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

        private void tabcOperacion_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!PedidoGenerado & (lastSelectedTab == tabpRegistrar && e.TabPage != tabpRegistrar && dgvDetalle.RowCount > 0))
            {
                DialogResult respuesta = MessageBox.Show("Se han agregado productos al detalle del pedido, si cambia de pestaña se perderan los datos no guardados.\n¿Desea cambiar de pestaña?", Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            byte numRegs = 0;
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tabpRegistrar)
            {
                try
                {
                    if (ValidarControles())
                    {
                        Utils.ActualizarBarraDeEstado(this, Utils.insertandoRegistro);
                        DeshabilitarControles();
                        DeshabilitarControlesProducto();
                        btnGenerar.Enabled = false;
                        List<Order_Details> lstDetalle = new List<Order_Details>();
                        // Llenado de elementos hijos
                        foreach (DataGridViewRow dgvr in dgvDetalle.Rows)
                        {
                            Order_Details detalle = new Order_Details();
                            detalle.ProductID = int.Parse(dgvr.Cells["ProductoId"].Value.ToString());
                            detalle.UnitPrice = decimal.Parse(dgvr.Cells["Precio"].Value.ToString());
                            detalle.Quantity = short.Parse(dgvr.Cells["Cantidad"].Value.ToString());
                            detalle.Discount = float.Parse(dgvr.Cells["Descuento"].Value.ToString());
                            lstDetalle.Add(detalle);
                        }
                        Orders pedido = new Orders();
                        pedido.CustomerID = cboCliente.SelectedValue.ToString();
                        pedido.EmployeeID = (int)cboEmpleado.SelectedValue;
                        if (!dtpPedido.Checked) pedido.OrderDate = null;
                        else pedido.OrderDate = Convert.ToDateTime(dtpPedido.Value.ToShortDateString() + " " + dtpHoraPedido.Value.ToLongTimeString());
                        if (!dtpRequerido.Checked) pedido.RequiredDate = null;
                        else pedido.RequiredDate = Convert.ToDateTime(dtpRequerido.Value.ToShortDateString() + " " + dtpHoraRequerido.Value.ToLongTimeString());
                        if (!dtpEnvio.Checked) pedido.ShippedDate = null;
                        else pedido.ShippedDate = Convert.ToDateTime(dtpEnvio.Value.ToShortDateString() + " " + dtpHoraEnvio.Value.ToLongTimeString());
                        pedido.ShipVia = (int)cboTransportista.SelectedValue;
                        pedido.ShipName = txtDirigidoa.Text;
                        pedido.ShipAddress = txtDomicilio.Text;
                        pedido.ShipCity = txtCiudad.Text;
                        pedido.ShipRegion = txtRegion.Text;
                        pedido.ShipPostalCode = txtCP.Text;
                        pedido.ShipCountry = txtPais.Text;
                        pedido.Freight = decimal.Parse(txtFlete.Text.Replace("$", ""));
                        numRegs = AgregarPedido(pedido, lstDetalle, txtId);
                        if (numRegs > 0)
                            MessageBox.Show($"El pedido con Id: {pedido.OrderID} del Cliente: {cboCliente.Text}, se registro satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show($"El pedido con Id: {pedido.OrderID} del Cliente: {cboCliente.Text}, NO se registró en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    MessageBox.Show("Algún producto en el pedido fue previamente eliminado por otro usuario de la red.", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (SqlException ex) when (ex.Number == 2627)
                {
                    MessageBox.Show($"Error, existe un producto duplicado en el pedido, elimine el producto duplicado y modifique la cantidad del producto", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    PedidoGenerado = true;
                    IdDetalle = 1;
                    btnNota.Enabled = true;
                    btnNota.Visible = true;
                    btnNuevo.Enabled = true;
                    btnNuevo.Visible = true;
                    cboCategoria.SelectedIndex = 0;
                    cboCategoria.Enabled = true;
                    BorrarDatosBusqueda();
                    LlenarDgvPedidos(null);
                    dgvDetalle.Rows.Clear();
                    LlenarDatosDetallePedido();
                }
            }
            else if (tabcOperacion.SelectedTab == tabpModificar)
            {
                try
                {
                    if (ValidarControles())
                    {
                        if (!chkRowVersion())
                        {
                            MessageBox.Show("El registro ha sido modificado por otro usuario de la red, no se realizará la actualización del registro, vuelva a cargar el registro para que se muestre el pedido con los datos proporcionados por el otro usuario", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        Utils.ActualizarBarraDeEstado(this, Utils.modificandoRegistro);
                        DeshabilitarControles();
                        DeshabilitarControlesProducto();
                        btnGenerar.Enabled = false;
                        Orders pedido = new Orders();
                        pedido.OrderID = int.Parse(txtId.Text);
                        pedido.CustomerID = cboCliente.SelectedValue.ToString();
                        pedido.EmployeeID = (int)cboEmpleado.SelectedValue;
                        if (!dtpPedido.Checked) pedido.OrderDate = null;
                        else pedido.OrderDate = Convert.ToDateTime(dtpPedido.Value.ToShortDateString() + " " + dtpHoraPedido.Value.ToLongTimeString());
                        if (!dtpRequerido.Checked) pedido.RequiredDate = null;
                        else pedido.RequiredDate = Convert.ToDateTime(dtpRequerido.Value.ToShortDateString() + " " + dtpHoraRequerido.Value.ToLongTimeString());
                        if (!dtpEnvio.Checked) pedido.ShippedDate = null;
                        else pedido.ShippedDate = Convert.ToDateTime(dtpEnvio.Value.ToShortDateString() + " " + dtpHoraEnvio.Value.ToLongTimeString());
                        pedido.ShipVia = (int)cboTransportista.SelectedValue;
                        pedido.ShipName = txtDirigidoa.Text;
                        pedido.ShipAddress = txtDomicilio.Text;
                        pedido.ShipCity = txtCiudad.Text;
                        pedido.ShipRegion = txtRegion.Text;
                        pedido.ShipPostalCode = txtCP.Text;
                        pedido.ShipCountry = txtPais.Text;
                        pedido.Freight = decimal.Parse(txtFlete.Text.Replace("$", ""));
                        numRegs = ModificarPedido(pedido, txtId);
                        if (numRegs > 0)
                            MessageBox.Show($"El pedido con Id: {pedido.OrderID} del Cliente : {cboCliente.Text}, se actualizó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("No se pudo realizar la modificación, es posible que el registro se haya eliminado previamente por otro usuario de la red", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (numRegs > 0)
                {
                    PedidoGenerado = true;
                    btnNota.Enabled = true;
                    btnNota.Visible = true;
                    btnNuevo.Visible = false;
                    cboCategoria.Enabled = true;
                    btnAgregar.Enabled = true;
                    LlenarDgvPedidos(null);
                }
            }
            else if (tabcOperacion.SelectedTab == tabpEliminar)
            {
                if (txtId.Text == "")
                {
                    MessageBox.Show("Seleccione el pedido a eliminar", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult respuesta = MessageBox.Show($"¿Está seguro de eliminar el pedido con Id: {txtId.Text} del Cliente: {cboCliente.Text}?", Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.Yes)
                {
                    if (!chkRowVersion())
                    {
                        MessageBox.Show("El registro ha sido modificado por otro usuario de la red, no se realizará la eliminación del registro, vuelva a cargar el registro para que se muestre el pedido con los datos proporcionados por el otro usuario", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    Utils.ActualizarBarraDeEstado(this, Utils.eliminandoRegistro);
                    btnGenerar.Enabled = false;
                    try
                    {
                        Orders pedido = new Orders();
                        pedido.OrderID = int.Parse(txtId.Text);
                        numRegs = EliminarPedido(pedido);
                        if (numRegs > 0 )
                            MessageBox.Show($"El pedido con Id: {pedido.OrderID} del Cliente: {cboCliente.Text}, se eliminó satisfactoriamente junto con sus registros de detalle del pedido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("No se pudo realizar la eliminación, es posible que el registro haya sido eliminado previamente por otro usuario de la red", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                        BorrarDatosBusqueda();
                        LlenarDgvPedidos(null);
                    }
                }
            }
        }

        private bool ValidarControles()
        {
            BorrarMensajesError();
            bool valida = true;
            if (cboCliente.SelectedIndex <= 0)
            {
                errorProvider1.SetError(cboCliente, "Ingrese el cliente");
                valida = false;
            }
            if (cboEmpleado.SelectedIndex <= 0)
            {
                errorProvider1.SetError(cboEmpleado, "Ingrese el empleado");
                valida = false;
            }
            if (dtpPedido.Checked == false)
            {
                errorProvider1.SetError(dtpPedido, "Ingrese la fecha de pedido");
                valida = false;
            }
            if (cboTransportista.SelectedIndex <= 0)
            {
                errorProvider1.SetError(cboTransportista, "Ingrese la compañía transportista");
                valida = false;
            }
            if (cboProducto.SelectedIndex > 0)
            {
                errorProvider1.SetError(cboProducto, "Ha seleccionado un producto y no lo ha agregado al pedido");
                valida = false;
            }
            string total = txtTotal.Text.Replace("$", "");
            if (txtTotal.Text == "" || float.Parse(total) == 0)
            {
                errorProvider1.SetError(btnAgregar, "Ingrese el detalle del pedido");
                errorProvider1.SetError(txtTotal, "El total del pedido no puede ser cero");
                valida = false;
            }
            return valida;
        }

        private byte AgregarPedido(Orders pedido, List<Order_Details> lst, TextBox textBox)
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
                    try
                    {
                        context.Transaction = transaction;
                        // Verificamos si alguna cantidad en el pedido excede el inventario disponible
                        var excedeInventario = lst.Any(od => context.Products.Any(p => p.ProductID == od.ProductID && od.Quantity > p.UnitsInStock));
                        if (excedeInventario)
                            throw new InvalidOperationException("La cantidad de algún producto en el pedido excede el inventario disponible");
                        context.Orders.InsertOnSubmit(pedido);
                        // Agregamos el nuevo pedido al contexto
                        context.SubmitChanges();
                        int pedidoId = pedido.OrderID;
                        byte[] rowVersion = pedido.RowVersion.ToArray(); // Guardamos la RowVersion del pedido recién insertado
                        // Insertamos en Order_Details
                        foreach (var detalle in lst)
                        {
                            detalle.OrderID = pedidoId;
                            context.Order_Details.InsertOnSubmit(detalle);
                        }
                        context.SubmitChanges();
                        // Actualizamos UnitsInStock en Products
                        foreach (var detalle in lst)
                        {
                            var producto = context.Products.SingleOrDefault(p => p.ProductID == detalle.ProductID);
                            if (producto != null)
                                producto.UnitsInStock -= detalle.Quantity;
                        }
                        context.SubmitChanges();
                        // Confirmamos la transacción
                        transaction.Commit();
                        // Obtenemos el Id del nuevo pedido
                        textBox.Text = pedidoId.ToString();
                        textBox.Tag = rowVersion; // Asignamos la RowVersion al TextBox para futuras modificaciones
                        numRegs = 1;
                    }
                    catch (SqlException)
                    {
                        // Si ocurre un error de base de datos, revertimos la transacción
                        transaction.Rollback();
                        throw;
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

        private byte ModificarPedido(Orders pedido, TextBox textBox)
        {
            byte numRegs = 0;
            using (var context = new NorthwindTradersDataContext())
            {
                if (context.Connection.State == ConnectionState.Closed)
                    context.Connection.Open();
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
                            ped.OrderDate = pedido.OrderDate;
                            ped.RequiredDate = pedido.RequiredDate;
                            ped.ShippedDate = pedido.ShippedDate;
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
                            byte[] rowVersion = ped.RowVersion.ToArray(); // Guardamos la RowVersion del pedido actualizado
                            textBox.Tag = rowVersion;
                            // Confirmamos la transacción
                            transaction.Commit();
                            numRegs = 1;
                        }
                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
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

        private byte EliminarPedido(Orders pedido)
        {
            byte numRegs = 0;
            using (var context = new NorthwindTradersDataContext())
            {
                if (context.Connection.State == ConnectionState.Closed)
                    context.Connection.Open();
                // Iniciamos una transacción
                using (var transaction = context.Connection.BeginTransaction())
                {
                    try
                    {
                        context.Transaction = transaction;
                        // Obtenemos el pedido que queremos eliminar
                        var pedidoAEliminar = context.Orders.SingleOrDefault(o => o.OrderID == pedido.OrderID);
                        if (pedidoAEliminar != null)
                        {
                            // Obtenemos los detalles del pedido que queremos eliminar
                            var detallesAEliminar = context.Order_Details.Where(d => d.OrderID == pedido.OrderID).ToList();
                            // Devolvemos las cantidades al inventario
                            foreach (var detalle in detallesAEliminar)
                            {
                                var producto = context.Products.SingleOrDefault(p => p.ProductID == detalle.ProductID);
                                if (producto != null)
                                    producto.UnitsInStock += detalle.Quantity;
                            }
                            // Guardamos los cambios en la base de datos
                            context.SubmitChanges();
                            // Eliminamos los detalles del pedido
                            context.Order_Details.DeleteAllOnSubmit(detallesAEliminar);
                            // Eliminamos el pedido
                            context.Orders.DeleteOnSubmit(pedidoAEliminar);
                            // Guardamos los cambios en la base de datos
                            context.SubmitChanges();
                            // Confirmamos la transacción
                            transaction.Commit();
                            numRegs = 1;
                        }
                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
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

        private void dgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || (e.ColumnIndex != dgvDetalle.Columns["Modificar"].Index & e.ColumnIndex != dgvDetalle.Columns["Eliminar"].Index))
                return;
            if (!PedidoGenerado & e.ColumnIndex == dgvDetalle.Columns["Eliminar"].Index & tabcOperacion.SelectedTab == tabpRegistrar)
            {
                dgvDetalle.Rows.RemoveAt(e.RowIndex);
                CalcularTotal();
            }
            if (!PedidoGenerado & e.ColumnIndex == dgvDetalle.Columns["Modificar"].Index & tabcOperacion.SelectedTab == tabpRegistrar)
            {
                DataGridViewRow dgvr = dgvDetalle.CurrentRow;
                using (FrmPedidosDetalleModificar2 frmPedidosDetalleModificar2 = new FrmPedidosDetalleModificar2())
                {
                    frmPedidosDetalleModificar2.Owner = this;
                    frmPedidosDetalleModificar2.ProductoId = int.Parse(dgvr.Cells["ProductoId"].Value.ToString());
                    frmPedidosDetalleModificar2.Producto = dgvr.Cells["Producto"].Value.ToString();
                    frmPedidosDetalleModificar2.Precio = float.Parse(dgvr.Cells["Precio"].Value.ToString());
                    frmPedidosDetalleModificar2.Cantidad = short.Parse(dgvr.Cells["Cantidad"].Value.ToString());
                    frmPedidosDetalleModificar2.Descuento = float.Parse(dgvr.Cells["Descuento"].Value.ToString());
                    frmPedidosDetalleModificar2.Importe = float.Parse(dgvr.Cells["Importe"].Value.ToString());
                    frmPedidosDetalleModificar2.UInventario = short.Parse(dgvr.Cells["UInventario"].Value.ToString());
                    DialogResult respuesta = frmPedidosDetalleModificar2.ShowDialog();
                    if (respuesta == DialogResult.OK)
                    {
                        // Actualiza los valores en la fila actual del DataGridView
                        dgvr.Cells["Cantidad"].Value = frmPedidosDetalleModificar2.Cantidad;
                        dgvr.Cells["Descuento"].Value = frmPedidosDetalleModificar2.Descuento;
                        dgvr.Cells["Importe"].Value = frmPedidosDetalleModificar2.Importe;
                        CalcularTotal();
                    }
                }
            }
            if ((e.ColumnIndex == dgvDetalle.Columns["Eliminar"].Index & tabcOperacion.SelectedTab == tabpModificar) | (PedidoGenerado & e.ColumnIndex == dgvDetalle.Columns["Eliminar"].Index & tabcOperacion.SelectedTab == tabpRegistrar))
            {
                if (!chkRowVersion())
                {
                    MessageBox.Show("El registro ha sido modificado por otro usuario de la red, vuelva a cargar el registro para que se actualice con los datos proporcionados por el otro usuario", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewRow dgvr = dgvDetalle.CurrentRow;
                string productName = dgvr.Cells["Producto"].Value.ToString();
                int productId = (int)dgvr.Cells["ProductoId"].Value;
                int orderId = int.Parse(txtId.Text);
                EliminarPedidoDetalle(productName, productId, orderId);
            }
            if ((e.ColumnIndex == dgvDetalle.Columns["Modificar"].Index & tabcOperacion.SelectedTab == tabpModificar) | (PedidoGenerado & e.ColumnIndex == dgvDetalle.Columns["Modificar"].Index & tabcOperacion.SelectedTab == tabpRegistrar))
            {
                if (!chkRowVersion())
                {
                    MessageBox.Show("El registro ha sido modificado por otro usuario de la red, vuelva a cargar el registro para que se actualice con los datos proporcionados por el otro usuario", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewRow dgvr = dgvDetalle.CurrentRow;
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
                        btnNota.Enabled = true;
                        btnNota.Visible = true;
                        BorrarDatosDetallePedido();
                        LlenarDatosDetallePedido();
                    }
                }
            }
        }

        private void EliminarPedidoDetalle(string productName, int productId, int orderId)
        {
            byte numRegs = 0;
            BorrarMensajesError();
            cboCategoria.SelectedIndex = 0;
            cboProducto.DataSource = null;
            InicializarValoresProducto();
            try
            {
                DialogResult respuesta = MessageBox.Show($"¿Esta seguro de eliminar el producto: {productName} del pedido: {orderId}?", Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.Yes)
                {
                    Utils.ActualizarBarraDeEstado(this, Utils.eliminandoRegistro);
                    DeshabilitarControlesProducto();
                    numRegs = EliminarPedidoDetalle(productId, orderId);
                    if (numRegs > 0)
                        MessageBox.Show($"El Producto: {productName} del Pedido: {orderId}, se eliminó satisfactoriamente", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show($"El Producto: {productName} del Pedido: {orderId}, NO se eliminó en la base de datos", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex) when (ex.Number == 220)
            {
                MessageBox.Show("Al tratar de devolver las unidades vendidas al inventario, la cantidad de unidades excede las 32,767 unidades, rango máximo para un campo de tipo smallint", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                btnNota.Enabled = true;
                btnNota.Visible = true;
                Utils.ActualizarBarraDeEstado(this, $"Se muestran {dgvPedidos.RowCount} registros de pedidos");
            }
        }

        private byte EliminarPedidoDetalle(int productId, int orderId)
        {
            byte numRegs = 0;
            try
            {
                using (var context = new NorthwindTradersDataContext())
                {
                    if (context.Connection.State == ConnectionState.Closed)
                        context.Connection.Open();
                    // Iniciamos una transaccion
                    using (var transaction = context.Connection.BeginTransaction())
                    {
                        try
                        {
                            context.Transaction = transaction;
                            // Buscar el registro que se va a eliminar
                            var pedidoDetalle = context.Order_Details.SingleOrDefault(pd => pd.OrderID == orderId & pd.ProductID == productId);
                            if (pedidoDetalle == null)
                                throw new InvalidOperationException("El detalle del pedido no existe, es posible que el registro haya sido eliminado previamente por otro usuario de la red");
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
            return numRegs;
        }

        private void btnNota_Click(object sender, EventArgs e)
        {
            if (!chkRowVersion())
            {
                MessageBox.Show("El registro ha sido modificado por otro usuario de la red, se mostrará la nota de remisión con los datos proporcionados por el otro usuario", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            FrmNotaRemision0 frmNotaRemision0 = new FrmNotaRemision0();
            frmNotaRemision0.Id = int.Parse(txtId.Text);
            frmNotaRemision0.ShowDialog();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            BorrarDatosPedido();
            HabilitarControles();
            btnNota.Enabled = false;
            btnNota.Visible = true;
            btnNuevo.Enabled = false;
            btnNuevo.Visible = true;
            PedidoGenerado = false;
        }

        private bool chkRowVersion()
        {
            bool rowVersionOk = true;
            if (txtId.Tag != null)
            {
                //byte[] rowVersion = ((System.Data.Linq.Binary)txtId.Tag).ToArray();
                byte[] rowVersion = null;
                object rowVersionObj1 = txtId.Tag;
                if (rowVersionObj1 is byte[])
                { 
                    rowVersion = (byte[])rowVersionObj1;
                }
                else if (rowVersionObj1 is System.Data.Linq.Binary)
                {
                    rowVersion = ((System.Data.Linq.Binary)rowVersionObj1).ToArray();
                }
                byte[] rowVersionActual = null;
                try
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    using (var context = new NorthwindTradersDataContext())
                    {
                        // Buscar el pedido por Id y obtener su RowVersion
                        var pedido = context.Orders
                            .Where(o => o.OrderID == int.Parse(txtId.Text))
                            .Select(o => new { o.RowVersion })
                            .SingleOrDefault();
                        if (pedido != null)
                        {
                            rowVersionActual = pedido.RowVersion.ToArray();
                            if (!rowVersion.SequenceEqual(rowVersionActual))
                            {
                                rowVersionOk = false;
                            }
                        }
                        else
                        {
                            rowVersionOk = false;
                        }
                    }
                    if (rowVersionOk)
                    {
                        // Comprobamos primero que todos los registros del gridview existan en la DB y tengan el mismo rowversion
                        using (var context = new NorthwindTradersDataContext())
                        {
                            foreach (DataGridViewRow dgvr in dgvDetalle.Rows)
                            {
                                int numPedido = int.Parse(txtId.Text);
                                int numProducto = int.Parse(dgvr.Cells["ProductoId"].Value.ToString());
                                object rowVersionObj = dgvr.Cells["RowVersion"].Value;
                                byte[] rowVersionDetalle = null;
                                if (rowVersionObj is byte[])
                                {
                                    rowVersionDetalle = (byte[])rowVersionObj;
                                }
                                else if (rowVersionObj is System.Data.Linq.Binary)
                                {
                                    rowVersionDetalle = ((System.Data.Linq.Binary)rowVersionObj).ToArray();
                                }
                                else
                                {
                                    throw new InvalidCastException("El valor en RowVersion no es de un tipo esperado.");
                                }
                                var objrowVersionDetalleEnDB = context.Order_Details
                                    .Where(od => od.OrderID == numPedido && od.ProductID == numProducto)
                                    .Select(od => new { od.RowVersion })
                                    .SingleOrDefault();
                                if (objrowVersionDetalleEnDB != null)
                                {
                                    byte[] rowVersionDetalleEnDB = objrowVersionDetalleEnDB.RowVersion.ToArray();
                                    if (!rowVersionDetalle.SequenceEqual(rowVersionDetalleEnDB))
                                    {
                                        rowVersionOk = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    rowVersionOk = false;
                                    break;
                                }
                            }
                        }
                        if (rowVersionOk)
                        {
                            // Comprobamos segundo que todos los registros de la DB existan en el DataGridView y tengan el mismo rowversion
                            using (var context = new NorthwindTradersDataContext())
                            {
                                int numPedido = int.Parse(txtId.Text);
                                // Obtenemos los detalles del pedido desde la base de datos
                                var detallesPedido = context.Order_Details
                                    .Where(od => od.OrderID == numPedido)
                                    .Select(od => new { od.ProductID, od.RowVersion })
                                    .ToList();
                                Order_Details pedidoDetalle;
                                if (detallesPedido.Any())
                                {
                                    foreach (var detalle in detallesPedido)
                                    {
                                        pedidoDetalle = new Order_Details
                                        {
                                            ProductID = detalle.ProductID,
                                            RowVersion = detalle.RowVersion
                                        };
                                        bool registroEncontradoEnGrid = false;
                                        foreach (DataGridViewRow dgvr in dgvDetalle.Rows)
                                        {
                                            if (pedidoDetalle.ProductID == int.Parse(dgvr.Cells["ProductoId"].Value.ToString()))
                                            {
                                                registroEncontradoEnGrid = true;
                                                object rowVersionGridObj = dgvr.Cells["RowVersion"].Value;
                                                byte[] rowVersionGrid = null;
                                                if (rowVersionGridObj is byte[])
                                                {
                                                    rowVersionGrid = (byte[])rowVersionGridObj;
                                                }
                                                else if (rowVersionGridObj is System.Data.Linq.Binary)
                                                {
                                                    rowVersionGrid = ((System.Data.Linq.Binary)rowVersionGridObj).ToArray();
                                                }
                                                else
                                                {
                                                    throw new InvalidCastException("El valor en RowVersion no es de un tipo esperado.");
                                                }
                                                if (!rowVersionGrid.SequenceEqual(pedidoDetalle.RowVersion.ToArray())) // Convertimos Binary a byte[]
                                                {
                                                    rowVersionOk = false;
                                                }
                                                break; // Salimos del foreach si encontramos el registro
                                            }
                                        }
                                        if (!registroEncontradoEnGrid)
                                            rowVersionOk = false; // Si no encontramos el registro en el grid, marcamos como false
                                        if (!rowVersionOk)
                                            break; // Salimos del foreach si no encontramos el registro
                                    }
                                }
                                else
                                {
                                    rowVersionOk = true; // No hay detalles en la base de datos, lo que significa que no hay coincidencias
                                }
                            }
                        }
                    }
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {dgvPedidos.RowCount} registros en pedidos");
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
            return rowVersionOk;
        }
    }
}

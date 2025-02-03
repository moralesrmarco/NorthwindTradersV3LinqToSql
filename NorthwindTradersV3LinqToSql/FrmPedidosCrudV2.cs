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
    public partial class FrmPedidosCrudV2 : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();
        private TabPage lastSelectedTab;
        bool EventoCargado = true; // esta variable es necesaria para controlar el manejador de eventos de la celda del dgv, ojo no quitar
        int IdDetalle = 1;

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
            cboCliente.SelectedIndex = cboEmpleado.SelectedIndex = cboTransportista.SelectedIndex = cboCategoria.SelectedIndex = 0;
            cboProducto.DataSource = null;
            dtpPedido.Value = dtpRequerido.Value = dtpEnvio.Value = DateTime.Now;
            dtpHoraPedido.Value = DateTime.Now;
            dtpHoraRequerido.Value = dtpHoraEnvio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dtpRequerido.Checked = dtpEnvio.Checked = false;
            InicializarValoresTransportar();
            InicializarValores();
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
            txtTotal.Text = string.Format("0:c", total);
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
            if (tabcOperacion.SelectedTab == tabpRegistrar)
            {
                txtPrecio.Text = txtPrecio.Text.Replace("$", "");
                dgvDetalle.Rows.Add(new object[] { IdDetalle, cboProducto.Text, txtPrecio.Text, txtCantidad.Text, txtDescuento.Text, ((decimal.Parse(txtPrecio.Text) * decimal.Parse(txtCantidad.Text)) * (1 - decimal.Parse(txtDescuento.Text))).ToString(), "Modificar", "Eliminar", cboProducto.SelectedValue, txtUInventario.Text });
                CalcularTotal();
                ++IdDetalle;
                cboCategoria.SelectedIndex = cboProducto.SelectedIndex = 0;
                InicializarValoresProducto();
                cboCategoria.Focus();
            }
            else if (tabcOperacion.SelectedTab == tabpModificar)
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
                    catch (SqlException ex)
                    {
                        // Si ocurre un error de base de datos, revertimos la transacción
                        transaction.Rollback();
                        throw; // se vuelve a lanzar la excepción para que sea procesada en un nivel superior
                    }
                    catch (Exception ex)
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
                            Discount = pd.Descuento
                        };
                        string productName = pd.Producto;
                        float importe = (float.Parse(pedidoDetalle.UnitPrice.ToString()) * pedidoDetalle.Quantity) * (1 - pedidoDetalle.Discount);
                        dgvDetalle.Rows.Add(new object[] { IdDetalle, productName, pedidoDetalle.UnitPrice, pedidoDetalle.Quantity, pedidoDetalle.Discount, importe, "Modificar", "Eliminar", pedidoDetalle.ProductID });
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
                }
                else if (tabcOperacion.SelectedTab == tabpModificar)
                {
                    btnGenerar.Text = "Modificar pedido";
                    btnGenerar.Visible = true;
                    btnAgregar.Visible = true;
                    MostrarCols();
                }
                else if (tabcOperacion.SelectedTab == tabpEliminar)
                {
                    btnGenerar.Text = "Eliminar pedido";
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
                DeshabilitarControlesProducto();
                if (tabcOperacion.SelectedTab == tabpModificar)
                {
                    HabilitarControles();
                    btnGenerar.Enabled = true;
                    cboCategoria.Enabled = true;
                }
                else if (tabcOperacion.SelectedTab == tabpEliminar)
                    btnGenerar.Enabled = true;
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
                                order.ShipCountry
                            }).FirstOrDefault();
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

        private void tabcOperacion_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (lastSelectedTab == tabpRegistrar && e.TabPage != tabpRegistrar && dgvDetalle.RowCount > 0)
            {
                DialogResult respuesta = MessageBox.Show("Se han agregado productos al detalle del pedido, si cambia de pestaña se perderan los datos no guardados.\n¿Desea cambiar de pestaña?", Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.No)
                    e.Cancel = true;
            }
        }
    }
}

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmPedidosDetalleModificar : Form
    {

        NorthwindTradersDataContext context = new NorthwindTradersDataContext();

        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public string Producto { get; set; }
        public float Precio { get; set; }
        public short Cantidad { get; set; }
        public float Descuento { get; set; }
        public float Importe { get; set; }
        public short? UInventario { get; set; }
        private short CantidadOld;
        private float DescuentoOld;

        public FrmPedidosDetalleModificar()
        {
            InitializeComponent();
        }

        private void FrmPedidosDetalleModificar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ( int.Parse(txtCantidad.Text.Replace(",", "")) != CantidadOld || float.Parse(txtDescuento.Text) != DescuentoOld)
            {
                DialogResult respuesta = MessageBox.Show(Utils.preguntaCerrar, Utils.nwtr, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmPedidosDetalleModificar_Load(object sender, EventArgs e)
        {
            ObtenerUInventario();
            txtPedido.Text = PedidoId.ToString();
            txtProducto.Text = Producto;
            txtPrecio.Text = Precio.ToString("c");
            txtUinventario.Text = string.Format("{0:n0}", UInventario);
            txtCantidad.Text = Cantidad.ToString("n0");
            txtDescuento.Text = Descuento.ToString("n2");
            txtImporte.Text = Importe.ToString("c");
            CantidadOld = Cantidad;
            DescuentoOld = Descuento;
        }

        private void ObtenerUInventario()
        {
            try
            {
                Utils.ActualizarBarraDeEstado(this.Owner, Utils.clbdd);
                UInventario = context.Products
                                .Where(p => p.ProductID == ProductoId)
                                .Select(p => p.UnitsInStock)
                                .FirstOrDefault();
                Utils.ActualizarBarraDeEstado(this.Owner);
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

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            if (ValidarControles())
                CalcularImporte();
        }

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            if (ValidarControles())
                CalcularImporte();
        }

        private bool ValidarControles()
        {
            bool valida = true;
            short cantidad = 0, diferencia = 0;
            float descuento = 0;
            btnModificar.Enabled = false;
            errorProvider1.Clear();
            try
            {
                // Validar txtCantidad
                if (!short.TryParse(txtCantidad.Text.Replace(",", ""), out cantidad))
                {
                    errorProvider1.SetError(txtCantidad, "Ingrese una cantidad valida, la cantidad debe ser mayor que 1 y menor que 32,767");
                    valida = false;
                }
                if (valida && cantidad == 0)
                {
                    errorProvider1.SetError(txtCantidad, "Ingrese la cantidad");
                    valida = false;
                }
                // Validar descuento
                if (string.IsNullOrWhiteSpace(txtDescuento.Text) || string.IsNullOrEmpty(txtDescuento.Text) || !float.TryParse(txtDescuento.Text, out descuento))
                {
                    errorProvider1.SetError(txtDescuento, "Ingrese el descuento");
                    valida = false;
                }
                else if (descuento > 1 || descuento < 0)
                {
                    errorProvider1.SetError(txtDescuento, "El descuento no puede ser mayor que 1 o menor que 0");
                    valida = false;
                }
                if (valida)
                {
                    // Calcula la diferencia de cantidad
                    checked
                    {
                        diferencia = (short)(cantidad - CantidadOld);
                    }
                    // Verifica la disponibilidad en el inventario
                    if (valida && UInventario != null)
                    {
                        // Aquí manejamos el caso de devolver productos al inventario
                        if (diferencia < 0)
                        {
                            // La validación es correcta al devolver productos
                            if (UInventario + Math.Abs(diferencia) > 32767)
                            {
                                errorProvider1.SetError(txtCantidad, "La cantidad de producto devuelto al inventario más las unidades en inventario exceden las 32,767 unidades");
                                valida = false;
                            }
                        }
                        // Aquí manejamos el caso de retirar productos del inventario
                        else if (diferencia > 0)
                        {
                            if (diferencia > UInventario)
                            {
                                errorProvider1.SetError(txtCantidad, "La cantidad de producto en el pedido excede el inventario disponible");
                                valida = false;
                            }
                        }
                    }
                    else if (UInventario == null)
                    {
                        errorProvider1.SetError(txtCantidad, "Es posible que el producto haya sido eliminado por otro usuario en la red");
                        valida = false;
                    }
                }
            }
            catch (OverflowException ex)
            {
                MessageBox.Show($"Desbordamiento atrapado: {ex.Message}");
            }
            // Habilitar el botón Modificar si la cantidad y descuento son validos y han cambiado
            if (valida && (cantidad != CantidadOld || descuento != DescuentoOld))
                btnModificar.Enabled = true;
            return valida;
        }

        private void CalcularImporte()
        {
            Cantidad = short.Parse(txtCantidad.Text.Replace(",", ""));
            Descuento = float.Parse(txtDescuento.Text);
            Importe = (Precio * Cantidad) * (1 - Descuento);
            txtImporte.Text = Importe.ToString("c");
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosSinPunto(sender, e);
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.ValidarDigitosConPunto(sender, e);
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            int? numRegs = 0;
            // No se realiza la validación porque ya se han realizado previamente en el evento leave de 
            // txtdescuento y txtcantidad
            try
            {
                btnModificar.Enabled = false;
                Utils.ActualizarBarraDeEstado(this, Utils.modificandoRegistro);
                context.SP_PEDIDOSDETALLE_ACTUALIZAR_V3(int.Parse(txtPedido.Text), ProductoId, short.Parse(txtCantidad.Text.Replace(",", "")), float.Parse(txtDescuento.Text), CantidadOld, DescuentoOld, ref numRegs);                
                if (numRegs == 0)
                    MessageBox.Show("No se pudo realizar la modificación, es posible que el registro se haya eliminado previamente por otro usuario de la red", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException ex)
            {
                Utils.MsgCatchOueclbdd(this.Owner, ex);
            }
            catch (Exception ex)
            {
                Utils.MsgCatchOue(this.Owner, ex);
            }
            if (numRegs > 0)
            {
                // Las siguientes dos lineas son necesarias para que se permita cerrar la ventana. 
                // ya que se validan las variables en FrmPedidosDetalleModificar_FormClosing
                CantidadOld = short.Parse(txtCantidad.Text.Replace(",", ""));
                DescuentoOld = float.Parse(txtDescuento.Text);
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}

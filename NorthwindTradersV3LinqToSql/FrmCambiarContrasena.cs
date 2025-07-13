using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmCambiarContrasena : Form
    {

        public string UsuarioLogueado;
        bool _imagenMostrada = true;

        public FrmCambiarContrasena()
        {
            InitializeComponent();
        }

        private void btnTogglePwd_Click(object sender, EventArgs e)
        {
            _imagenMostrada = !_imagenMostrada;
            txtPwd.UseSystemPasswordChar = !txtPwd.UseSystemPasswordChar;
            txtNewPwd.UseSystemPasswordChar = !txtNewPwd.UseSystemPasswordChar;
            txtConfirmarPwd.UseSystemPasswordChar = !txtConfirmarPwd.UseSystemPasswordChar;
            btnTogglePwd.Image = _imagenMostrada ? Properties.Resources.mostrarCh : Properties.Resources.ocultarCh;
        }

        private void FrmCambiarContrasena_Load(object sender, EventArgs e)
        {
            txtUsuario.Text = UsuarioLogueado;
            txtPwd.Focus();
        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            PonerNoVisibleBtnTogglePwd();
            if (!ValidarNuevaContrasena())
                return;
            try
            {
                string pwdHasheada = Utils.ComputeSha256Hash(txtNewPwd.Text.Trim());
                using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                {
                    var usuario = context.Usuarios.FirstOrDefault(u => u.Usuario == txtUsuario.Text.Trim() && u.Estatus == true);
                    if (usuario != null)
                    {
                        usuario.Password = pwdHasheada;
                        context.SubmitChanges();
                        MessageBox.Show("Contraseña cambiada correctamente.", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo cambiar la contraseña. Verifique que su cuenta esté activa.", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private bool ValidarNuevaContrasena()
        {
            errorProvider1.Clear();
            bool valida = true;
            txtPwd.Text = txtPwd.Text.Trim();
            if (string.IsNullOrWhiteSpace(txtPwd.Text))
            {
                errorProvider1.SetError(txtPwd, "Debe ingresar su contraseña actual");
                valida = false;
            }
            if (valida)
            {
                try
                {
                    string pwdHasheada = Utils.ComputeSha256Hash(txtPwd.Text);
                    using (NorthwindTradersDataContext context = new NorthwindTradersDataContext())
                    {
                        var usuario = context.Usuarios.FirstOrDefault(u => u.Usuario == txtUsuario.Text.Trim() && u.Password == pwdHasheada && u.Estatus == true);
                        if (usuario == null)
                        {
                            errorProvider1.SetError(txtPwd, "La contraseña actual es incorrecta");
                            valida = false;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Utils.MsgCatchOueclbdd(ex);
                    valida = false;
                }
                catch (Exception ex)
                {
                    Utils.MsgCatchOue(ex);
                    valida = false;
                }
                txtNewPwd.Text = txtNewPwd.Text.Trim();
                txtConfirmarPwd.Text = txtConfirmarPwd.Text.Trim();
                if (string.IsNullOrWhiteSpace(txtNewPwd.Text))
                {
                    errorProvider1.SetError(txtNewPwd, "La nueva contraseña es obligatoria");
                    valida = false;
                }
                if (string.IsNullOrWhiteSpace(txtConfirmarPwd.Text))
                {
                    errorProvider1.SetError(txtConfirmarPwd, "La confirmación de la contraseña es obligatoria");
                    valida = false;
                }
                if (valida)
                {
                    if (txtNewPwd.Text != txtConfirmarPwd.Text)
                    {
                        errorProvider1.SetError(txtNewPwd, "La nueva contraseña y la confirmación de la contraseña no coinciden");
                        errorProvider1.SetError(txtConfirmarPwd, "La nueva contraseña y la confirmación de la contraseña no coinciden");
                        valida = false;
                    }
                }
            }
            return valida;
        }

        private void PonerNoVisibleBtnTogglePwd()
        {
            txtPwd.UseSystemPasswordChar = txtNewPwd.UseSystemPasswordChar = txtConfirmarPwd.UseSystemPasswordChar = true;
            btnTogglePwd.Image = Properties.Resources.mostrarCh;
        }
    }
}

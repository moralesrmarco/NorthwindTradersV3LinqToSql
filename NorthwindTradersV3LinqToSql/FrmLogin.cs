using System;
using System.Linq;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmLogin : Form
    {

        public bool IsAuthenticated { get; private set; } = false;
        public string UsuarioLogueado;
        public int IdUsuarioLogueado;
        bool _imagenMostrada = true;
        byte numeroIntentos = 0;

        public FrmLogin()
        {
            InitializeComponent();
            this.Text = Utils.nwtr;
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            var usuario = txtUsuario.Text.Trim();
            var pass = txtPwd.Text.Trim();
            if (ValidateUser(usuario, pass))
            {
                IsAuthenticated = true;
                UsuarioLogueado = usuario;
                this.Close();
            }
            else
            { 
                numeroIntentos++;
                if (numeroIntentos >= 3)
                {
                    MessageBox.Show("Demasiados intentos fallidos. La aplicación se cerrará.", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
                MessageBox.Show("Error de autenticación.\nUsuario o contraseña incorrectos.", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPwd.Clear();
                txtPwd.Focus();
            }
        }

        private bool ValidateUser(string usuario, string pass)
        {
            string hashed = Utils.ComputeSha256Hash(pass);
            using (var context = new NorthwindTradersDataContext())
            {
                var user = context.Usuarios.FirstOrDefault(u => u.Usuario == usuario && u.Password == hashed && u.Estatus == true);
                if (user != null)
                {
                    IdUsuarioLogueado = user.Id;
                    return IdUsuarioLogueado > 0;
                }
                return false;
            }
        }

        private void btnTogglePwd_Click(object sender, EventArgs e)
        {
            _imagenMostrada = !_imagenMostrada;
            txtPwd.UseSystemPasswordChar = !txtPwd.UseSystemPasswordChar;
            btnTogglePwd.Image = _imagenMostrada ? Properties.Resources.mostrarCh : Properties.Resources.ocultarCh;
        }
    }
}

using System;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Inicializar el usuario autenticado como null
            string usuarioAutenticado = null;
            int idUsuarioLogueado = 0;
            // Mostrar el formulario de inicio de sesión
            using (var frmLogin = new FrmLogin())
            {
                frmLogin.ShowDialog();
                if (frmLogin.IsAuthenticated)
                {
                    usuarioAutenticado = frmLogin.UsuarioLogueado;
                    idUsuarioLogueado = frmLogin.IdUsuarioLogueado;
                }
                else
                {
                    // Si no se autenticó, salir de la aplicación
                    return;
                }
            }
            //Application.Run(new MDIPrincipal());
            var mdi = new MDIPrincipal
            {
                UsuarioLogueado = usuarioAutenticado,
                IdUsuarioLogueado = idUsuarioLogueado
            };
            Application.Run(mdi);
        }
    }
}

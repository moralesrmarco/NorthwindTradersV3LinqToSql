﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    internal class Utils
    {
        public static void ValidaTxtBIdIni(TextBox txtBIdIni, TextBox txtBIdFin)
        {
            int numBIdIni = 0, numBIdFin = 0;
            if (txtBIdIni.Text != "")
            {
                if (int.TryParse(txtBIdIni.Text, out int numTxtBIdIni))
                {
                    if (numTxtBIdIni == 0)
                    {
                        MessageBox.Show("El valor del Id inicial no puede ser cero", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBIdIni.Text = "1";
                        txtBIdIni.Focus();
                        return;
                    }
                    numBIdIni = numTxtBIdIni;
                    if (txtBIdFin.Text == "")
                        txtBIdFin.Text = txtBIdIni.Text;
                }
                else
                    MessageBox.Show("Por favor ingrese un número valido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (txtBIdFin.Text != "")
            {
                if (int.TryParse(txtBIdFin.Text, out int numTxtBIdFin))
                {
                    numBIdFin = numTxtBIdFin;
                }
                else
                    MessageBox.Show("Por favor ingrese un número valido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (numBIdFin < numBIdIni)
                txtBIdFin.Text = txtBIdIni.Text;
        }

        public static void ValidaTxtBIdFin(TextBox txtBIdIni, TextBox txtBIdFin)
        {
            int numBIdIni = 0, numBIdFin = 0;
            if (txtBIdIni.Text != "")
            {
                if (int.TryParse(txtBIdIni.Text, out int numTxtBIdIni))
                {
                    numBIdIni = numTxtBIdIni;
                }
                else
                    MessageBox.Show("Por favor ingrese un número valido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                txtBIdIni.Text = txtBIdFin.Text;
            }
            if (txtBIdFin.Text != "")
            {
                if (int.TryParse(txtBIdFin.Text, out int numTxtBIdFin))
                {
                    if (numTxtBIdFin == 0)
                    {
                        MessageBox.Show("El valor del Id final no puede ser cero", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBIdFin.Text = "1";
                        txtBIdFin.Focus();
                        Utils.ValidaTxtBIdIni(txtBIdIni, txtBIdFin);
                        return;
                    }
                    numBIdFin = numTxtBIdFin;
                }
                else
                    MessageBox.Show("Por favor ingrese un número valido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (numBIdIni > numBIdFin)
                txtBIdIni.Text = txtBIdFin.Text;
        }

        public static void ValidarDigitosConPunto(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar !='.')
                e.Handled = true;
            // valida que exista solo un punto decimal
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
            // forzar que solo se capturen como máximo dos dígitos despues del punto decimal
            if (e.KeyChar != 8)
            {
                string numsDecimales = (sender as TextBox).Text + e.KeyChar;
                if ((sender as TextBox).Text.IndexOf('.') > -1)
                {
                    int posComienzo = (sender as TextBox).Text.IndexOf('.');
                    numsDecimales = numsDecimales.Substring(posComienzo, numsDecimales.Length - posComienzo);
                    if (numsDecimales.Length > 3)
                        e.Handled = true;
                }
            }
        }

        public static void ValidarDigitosSinPunto(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || (int)e.KeyChar == 8);
        }

        public static void ConfDgv(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToOrderColumns = false;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.BackgroundColor = SystemColors.Control;
            dgv.RowHeadersVisible = false;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.AutoResizeColumns();
        }

        public static void GrbPaint(Form form, object sender, PaintEventArgs e)
        {
            GroupBox groupBox = sender as GroupBox;
            Utils.DrawGroupBox(form, groupBox, e.Graphics, Color.Black, Color.Black);
        }

        public static void MsgCatchOueclbdd(Form form, SqlException ex)
        {
            MessageBox.Show(Utils.oueclbdd + ex.Message, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Utils.ActualizarBarraDeEstado(form);
        }

        public static void MsgCatchOue(Form form, Exception ex)
        {
            MessageBox.Show(Utils.oue + ex.Message, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Utils.ActualizarBarraDeEstado(form);
        }

        public static void ActualizarBarraDeEstado(Form form, string mensaje = "Activo.", bool error = false)
        {
            // se requiere en el archivo MDIPrincipal.cs declarar la propiedad:
            //public ToolStripStatusLabel ToolStripEstado
            //{
            //    get { return tsslEstado; }
            //    set { tsslEstado = value; }
            //}
            MDIPrincipal mDIPrincipal = (MDIPrincipal)form.MdiParent;
            if (mDIPrincipal != null) // esta comprobación se requiere para que no marque error en los formularios heredados en el tiempo de diseño.
            {
                if (mensaje != "Activo.")
                    if (error)
                        mDIPrincipal.ToolStripEstado.BackColor = Color.Red;
                    else
                        mDIPrincipal.ToolStripEstado.BackColor = SystemColors.ActiveCaption;
                else
                    mDIPrincipal.ToolStripEstado.BackColor = SystemColors.Control;
                if (error)
                {
                    mDIPrincipal.ToolStripEstado.ForeColor = Color.White;
                    mDIPrincipal.ToolStripEstado.Font = new Font(mDIPrincipal.ToolStripEstado.Font, FontStyle.Bold);
                }
                else
                {
                    mDIPrincipal.ToolStripEstado.ForeColor = SystemColors.ControlText;
                    mDIPrincipal.ToolStripEstado.Font = new Font(mDIPrincipal.ToolStripEstado.Font, FontStyle.Regular);
                }
                mDIPrincipal.ToolStripEstado.Text = mensaje;
                mDIPrincipal.Refresh();
            }
        }

        public static void ActualizarBarraDeEstadoPrincipal(Form form)
        {
            // por su logica, este metodo solo se debe ejecutar desde el metodo Utils.CerrarFormularios
            MDIPrincipal mDIPrincipal = (MDIPrincipal)form;
            if (mDIPrincipal != null)
            {
                mDIPrincipal.ToolStripEstado.BackColor = SystemColors.Control;
                mDIPrincipal.ToolStripEstado.Text = "Activo.";
                mDIPrincipal.Refresh();
            }
        }

        public static void DrawGroupBox(Form form, GroupBox box, Graphics g, Color textColor, Color borderColor)
        {
            if (box != null)
            {
                Brush textBrush = new SolidBrush(textColor);
                Brush borderBrush = new SolidBrush(borderColor);
                Pen borderPen = new Pen(borderBrush);
                SizeF strSize = g.MeasureString(box.Text, box.Font);
                Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                                box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                                box.ClientRectangle.Width - 1,
                                                box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);
                // Clear text and border
                g.Clear(form.BackColor);
                // Draw text
                g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);
                // Drawing border
                // Left
                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                // Right
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                // Bottom
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                // Top1
                g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                // Top2
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
        }

        public static void CerrarFormularios()
        {
            //Declaramos una lista de tipo Form
            List<Form> formularios = new List<Form>();
            //Recorremos Application.OpenForms el cual tiene la lista de formularios y metemos todos los forms en la lista que declarmos
            foreach (Form form in Application.OpenForms)
                formularios.Add(form);
            // recorremos la lista de formularios
            for (int i = 0; i < formularios.Count; i++)
            {
                // validamos que el nombre de los formularios sean distintos al unico formulario que queremos abierto
                if (formularios[i].Name != "MDIPrincipal")
                    formularios[i].Close();
                else
                    Utils.ActualizarBarraDeEstadoPrincipal(formularios[i]);
            }

        }

        #region VariablesGlobales
        public static string clbdd = "Consultando la base de datos... ";
        public static string oueclbdd = "Ocurrio un error con la base de datos:\n";
        public static string oue = "Ocurrio un error:\n";
        public static string nwtr = "Northwind Traders Ver 3 Linq to Sql.";
        #endregion
    }
}

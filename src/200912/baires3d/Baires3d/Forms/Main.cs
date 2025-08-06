using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        public Control Panel
        {
            get { return splitContainer2.Panel1; }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout about = new FormAbout();
            about.Show();
        }


        private void opcionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void dataBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        private void tlvResultados_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {

            wbDestacados.Url = new Uri(Path.GetDirectoryName(Application.ExecutablePath) + "\\destacados.htm");
        }



        private void cmbEsquinaDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmbCull_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbCull.SelectedItem.ToString())
            {
                case "None":
                    Constants.ar3d.cullMode = CullMode.None;
                    break;
                case "Clockwise":
                    Constants.ar3d.cullMode = CullMode.CullClockwiseFace;
                    break;
                case "CounterClockwise":
                    Constants.ar3d.cullMode = CullMode.CullCounterClockwiseFace;
                    break;
            }
        }


        private void tvCapas_ItemActivate(object sender, EventArgs e)
        {

        }

        private void cmsLayers_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tvCapas_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
            }
        }



        private void gridOpciones_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
        }


        private void chkListBarrios_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (Constants.ar3d.isLoading == false)
            {
                //this.game.LoadBarrios();
            }
        }

        private void chkLuces_CheckedChanged(object sender, EventArgs e)
        {
            Constants.Settings.Graphics.Light.Enabled = chkLuces.Checked;
            Constants.SaveSettings();
        }

        private void chkFog_CheckedChanged(object sender, EventArgs e)
        {
            Constants.Settings.Graphics.Fog.Enabled = chkFog.Checked;
        }

        private void chkWireframe_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWireframe.Checked)
            {
                Constants.ar3d.fillMode = FillMode.WireFrame;
            }
            else
            {
                Constants.ar3d.fillMode = FillMode.Solid;
            }
            Constants.Settings.Graphics.Wireframe = (Constants.ar3d.fillMode == FillMode.WireFrame);
            Constants.SaveSettings();
        }

        private void chkPerPixelLighting_CheckedChanged(object sender, EventArgs e)
        {
            Constants.Settings.Graphics.Light.PerPixel = chkPerPixelLighting.Checked;
            Constants.SaveSettings();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (Constants.ar3d.ComponentManager.Capital.StreetManager != null)
            {
                if (cmbCalleInicial.SelectedIndex != -1 ) {

                    Constants.ar3d.ComponentManager.Capital.StreetManager.StreetToDraw = cmbCalleInicial.Items[cmbCalleInicial.SelectedIndex].ToString();
                }
                if (cmbCalleInicial.SelectedIndex != -1 && cmbCalleDestino.SelectedIndex != -1)
                {
                    Constants.ar3d.ComponentManager.Capital.StreetManager.StreetToDraw = cmbCalleInicial.Items[cmbCalleInicial.SelectedIndex].ToString();
                    Constants.ar3d.ComponentManager.Capital.StreetManager.Find(cmbCalleInicial.Items[cmbCalleInicial.SelectedIndex].ToString(),
                                                 cmbCalleDestino.Items[cmbCalleDestino.SelectedIndex].ToString());
                }
            }
        }



        private void splitContainer3_Panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void cmbColectivos_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void tvResultados_AfterSelect(object sender, TreeViewEventArgs e) {

        }



    }
}
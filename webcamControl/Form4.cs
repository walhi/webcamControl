using DirectShowLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace webcamControl
{
    public partial class Form4 : Form
    {
        private IniFile INI = new IniFile("config.ini");
        public Form4()
        {
            InitializeComponent();
            DsDevice[] capDev;
            capDev = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            TableLayoutPanel tl = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            foreach (DsDevice dev in capDev)
            {
                TabPage tabPage = new TabPage();
                tabPage.Text = dev.Name;
                object camDevice;
                Guid iid = typeof(IBaseFilter).GUID;

                dev.Mon.BindToObject(null, null, ref iid, out camDevice);
                IBaseFilter camFilter = camDevice as IBaseFilter;
                IAMCameraControl pCameraControl = camFilter as IAMCameraControl;
                IAMVideoProcAmp pVideoProcAmp = camFilter as IAMVideoProcAmp;
                if (pCameraControl != null)
                {
                    SelectedProperties gb = new SelectedProperties(dev);
                    tl.Controls.Add(gb);

                }
            }
            Controls.Add(tl);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

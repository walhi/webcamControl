using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DirectShowLib;

namespace webcamControl
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            DsDevice[] capDev;
            capDev = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

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
                    AllProperties ggg = new AllProperties(dev);
                    ggg.AutoSize = true;
                    ggg.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    tabPage.Controls.Add(ggg);
                    tabControl.Controls.Add(tabPage);
                }
            }
            InitSelected();
        }

        public void InitSelected()
        {
            DsDevice[] capDev;
            capDev = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            AllPage.Controls.Clear();

            TableLayoutPanel tl = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            foreach (DsDevice dev in capDev)
            {
                object camDevice;
                Guid iid = typeof(IBaseFilter).GUID;

                dev.Mon.BindToObject(null, null, ref iid, out camDevice);
                IBaseFilter camFilter = camDevice as IBaseFilter;
                IAMCameraControl pCameraControl = camFilter as IAMCameraControl;
                IAMVideoProcAmp pVideoProcAmp = camFilter as IAMVideoProcAmp;
                if (pCameraControl != null)
                {
                    if (AllProperties.countSelected(dev) > 0)
                    {
                        SelectedProperties gb = new SelectedProperties(dev);
                        tl.Controls.Add(gb);
                    }
                }
            }
            AllPage.Controls.Add(tl);
        }
    }
}

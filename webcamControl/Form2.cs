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
                TabPage tabPage = new TabPage
                {
                    // TODO: Возможность переименовать камеру
                    Text = dev.Name
                };
                object camDevice;
                Guid iid = typeof(IBaseFilter).GUID;

                dev.Mon.BindToObject(null, null, ref iid, out camDevice);
                IBaseFilter camFilter = camDevice as IBaseFilter;
                if (camFilter is IAMCameraControl pCameraControl)
                {
                    AllProperties ggg = new AllProperties(dev);
                    tabPage.Controls.Add(ggg);
                    tabControl.Controls.Add(tabPage);
                    ggg.Dock = DockStyle.Top;
                }
            }
            InitSelected();

            tabControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        }

        public void InitSelected()
        {
            DsDevice[] capDev;
            capDev = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            AllPage.Controls.Clear();

            TableLayoutPanel tl = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Dock = DockStyle.Top
            };

            foreach (DsDevice dev in capDev)
            {
                object camDevice;
                Guid iid = typeof(IBaseFilter).GUID;

                dev.Mon.BindToObject(null, null, ref iid, out camDevice);
                IBaseFilter camFilter = camDevice as IBaseFilter;
                if (camFilter is IAMCameraControl pCameraControl)
                {
                    if (AllProperties.countSelected(dev) > 0)
                    {
                        SelectedProperties gb = new SelectedProperties(dev);
                        tl.Controls.Add(gb);
                        gb.Dock = DockStyle.Top;
                        //gb.Width = 
                    }
                }
            }
            AllPage.Controls.Add(tl);
        }
    }
}

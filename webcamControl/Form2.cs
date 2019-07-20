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

            int verticalPosition = 0;
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
                    webcam ggg = new webcam(dev);

                    tabPage.Controls.Add(ggg);
                    tabControl.Controls.Add(tabPage);
                    Debug.WriteLine(ggg.countAll);
                    Debug.WriteLine(verticalPosition);
                    if (ggg.countAll > 0)
                    {
                        ggg = new webcam(dev, 1);
                        AllPage.Controls.Add(ggg);
                        ggg.Location = new Point(0, verticalPosition);
                        Debug.WriteLine(ggg.Height);

                        verticalPosition += ggg.Height;
                    }
                }
            }

        }
    }
}

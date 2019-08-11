using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DirectShowLib;

namespace webcamControl
{
    public partial class TabPageCustom : TabPage
    {
        private DsDevice webcam;
        public SelectedProperties sProp;
        public AllProperties aProp;
        public TabPageCustom(DsDevice dev)
        {
            InitializeComponent();

            webcam = dev;
            //Globals._USBControl.DShowConnected += new EventHandler(WebcamConnected);
            //Globals._USBControl.DShowDisconnected += new EventHandler(WebcamDisconnected);
        }

        public DsDevice GetDsDevice()
        {
            return webcam;
        }
        public void WebcamDisconnected()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    WebcamDisconnectedImpl();
                }
                ));
            }
            else
            {
                WebcamDisconnectedImpl();
            }
        }
        private void WebcamDisconnectedImpl()
        {
            if (sProp != null) sProp.WebcamDisconnect();
            Label lb = new Label
            {
                Text = "Disconnected"
            };
            this.Controls.Add(lb);
        }
        public void WebcamConnected(DsDevice dev)
        {
            webcam = dev;
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    WebcamConnectedImpl();
                }
                ));
            }
            else
            {
                WebcamConnectedImpl();
            }
        }
        private void WebcamConnectedImpl()
        {
            this.Controls.Clear();
            this.BackColor = Color.Transparent;
            aProp = new AllProperties(webcam)
            {
                Dock = DockStyle.Top,
            };
            //aProp.CreateSelectedProperties += new EventHandler(CreateSelectedProperties);
            if (sProp != null)
            {
                sProp.SetAllProp(aProp);
                aProp.AddPropertyUpdateHandler(sProp);
                sProp.WebcamConnect();
            }
            this.Controls.Add(aProp);
        }
    }
}

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
        private TableLayoutPanel main;

        public Form2()
        {
            InitializeComponent();


            //init tray
            this.Resize += new System.EventHandler(this.Form_Resize);

            main = new TableLayoutPanel
            {
                AutoSize = true, 
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Dock = DockStyle.Top
            };
            AllPage.Controls.Add(main);

            foreach (DsDevice dev in Globals._USBControl.GetDsDevices())
                tabControl.Controls.Add(InitTabPage(dev));

            tabControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            Globals._USBControl.DShowConnected += new EventHandler(DsDeviceConnect);
            Globals._USBControl.DShowDisconnected += new EventHandler(DsDeviceDisconnect);
        }

        private TabPageCustom InitTabPage(DsDevice dev)
        {
            TabPageCustom tabPage = new TabPageCustom(dev);

            AllProperties aProp = new AllProperties(dev)
            {
                Dock = DockStyle.Top,
            };
            aProp.CreateSelectedProperties += new EventHandler(CreateSelectedProperties);
            tabPage.Text = aProp.GetWebcamName();
            tabPage.Controls.Add(aProp);


            if (aProp.CountFavorites > 0)
            {
                SelectedProperties sProp = new SelectedProperties(dev)
                {
                    Dock = DockStyle.Top
                };
                main.Controls.Add(sProp);
                sProp.SetAllProp(aProp);
                aProp.SelectedPropertiesVar = sProp;
                aProp.AddPropertyUpdateHandler();
                tabPage.sProp = sProp;
            }
            return tabPage;
        }
        private void DsDeviceConnect(object sender, EventArgs e)
        {
            if (!(sender is DsDevice)) return;
            DsDevice dev = (DsDevice)sender;

            Guid iid = typeof(IBaseFilter).GUID;
            dev.Mon.BindToObject(null, null, ref iid, out object camDevice);
            IBaseFilter camFilter = camDevice as IBaseFilter;
            var pCameraControl = camFilter as IAMCameraControl;
            var pVideoProcAmp = camFilter as IAMVideoProcAmp;

            if (pCameraControl != null) Debug.WriteLine("DsDeviceConnect1");
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    DsDeviceConnectImpl(dev);
                }
                ));
            }
            else
            {
                DsDeviceConnectImpl(dev);
            }
        }

        private void DsDeviceConnectImpl(DsDevice devIn)
        {
            // Костыль...
            foreach (DsDevice dev in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                if (dev.DevicePath.Equals(devIn.DevicePath))
                {
                    bool flag = true;
                    foreach (object tab in tabControl.Controls)
                    {
                        if (tab is TabPageCustom tp)
                        {
                            if (dev.DevicePath.Equals(tp.GetDsDevice().DevicePath))
                            {
                                tp.WebcamConnected(dev);
                                notifyIcon.ShowBalloonTip(1000, "Webcam connected", dev.Name, ToolTipIcon.Info);
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        tabControl.Controls.Add(InitTabPage(dev));
                    }
                    break;
                }
            }
            
        }
        private void DsDeviceDisconnect(object sender, EventArgs e)
        {
            if (!(sender is DsDevice dev)) return;
            foreach (object tab in tabControl.Controls)
            {
                if (tab is TabPageCustom tp)
                {
                    if (dev.DevicePath.Equals(tp.GetDsDevice().DevicePath))
                    {
                        tp.WebcamDisconnected();
                        notifyIcon.ShowBalloonTip(2000, "Webcam disconnected", dev.Name, ToolTipIcon.Warning);
                    }
                }
            }
        }

        private void CreateSelectedProperties(object sender, EventArgs e)
        {
            AllProperties aProp = (AllProperties)sender;
            SelectedProperties sProp = new SelectedProperties(aProp.GetDevice())
            {
                Dock = DockStyle.Top
            };
            sProp.SetAllProp(aProp);
            main.Controls.Add(sProp);
            aProp.SelectedPropertiesVar = sProp;
            aProp.AddPropertyUpdateHandler();
        }
        private void Form_Resize(object sender, EventArgs e)
        {     
            if (WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                Show.Enabled = true;
                Hide.Enabled = false;
            }
            else
            {
                this.ShowInTaskbar = true;
                Show.Enabled = false;
                Hide.Enabled = true;
            }
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //notifyIcon.Visible = false;
            if (this.ShowInTaskbar)
            {
                this.ShowInTaskbar = false;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                this.ShowInTaskbar = true;
                WindowState = FormWindowState.Normal;
            }
        }

        private void trayMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            if (item.Equals(Show) || item.Equals(Hide)) NotifyIcon_MouseDoubleClick(notifyIcon, null);
            if (item.Equals(Exit)) this.Close();

        }
    }
}

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
using HidLibrary;
using System.Diagnostics;

namespace webcamControl
{
    public partial class SelectedProperties : UserControl
    {
        private const int VendorId = 0x0483;
        private const int ProductId = 0x5750;

        private IniFile INI = new IniFile("config.ini");
        private DsDevice webcam;
        private HidDevice hid;

        private System.Timers.Timer mulTimer;
        private int mul = 1;
        private bool oldDir = false;
        private PropertyControl current;
        private PropertyControl button1;
        private PropertyControl button2;

        public SelectedProperties(DsDevice dev)
        {
            InitializeComponent();
            webcam = dev;
            object camDevice;
            Guid iid = typeof(IBaseFilter).GUID;
            webcam.Mon.BindToObject(null, null, ref iid, out camDevice);
            IBaseFilter camFilter = camDevice as IBaseFilter;
            IAMCameraControl pCameraControl = camFilter as IAMCameraControl;
            IAMVideoProcAmp pVideoProcAmp = camFilter as IAMVideoProcAmp;
            if (pCameraControl != null)
            {
                GroupBox gb = new GroupBox
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Text = webcam.Name
                };

                TableLayoutPanel tl = new TableLayoutPanel
                {
                    Name = "TableLayout",
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Location = new Point(gb.Padding.Left, gb.Padding.Top + 20)
                };

                foreach (var prop in Enum.GetValues(typeof(CameraControlProperty)))
                {
                    string key = "All_" + prop.ToString();
                    if (
                        INI.KeyExists(key, dev.DevicePath) &&
                        "True".Equals(INI.ReadINI(dev.DevicePath, key))
                        )
                    {
                        PropertyControl pc = new PropertyControl(pCameraControl, prop);
                        if (INI.KeyExists("USB_" + prop.ToString(), dev.DevicePath))
                            pc.SetFlagUSB("True".Equals(INI.ReadINI(dev.DevicePath, "USB_" + prop.ToString())));
                        //gb.Controls.Add(pc);
                        tl.Controls.Add(pc);
                    }
                }
                foreach (var prop in Enum.GetValues(typeof(VideoProcAmpProperty)))
                {
                    string key = "All_" + prop.ToString();
                    if (
                        INI.KeyExists(key, dev.DevicePath) &&
                        "True".Equals(INI.ReadINI(dev.DevicePath, key))
                        )
                    {
                        PropertyControl pc = new PropertyControl(pCameraControl, prop);
                        if (INI.KeyExists("USB_" + prop.ToString(), dev.DevicePath))
                            pc.SetFlagUSB("True".Equals(INI.ReadINI(dev.DevicePath, "USB_" + prop.ToString())));
                        //gb.Controls.Add(pc);
                        tl.Controls.Add(pc);
                    }
                }

                gb.Controls.Add(tl);
                Controls.Add(gb);

                if (INI.KeyExists("HID", dev.DevicePath))
                {
                    string hidPath = INI.ReadINI(dev.DevicePath, "HID");
                    foreach (HidDevice x in HidDevices.Enumerate(VendorId, ProductId))
                    {
                        x.OpenDevice();
                        if (hidPath.Equals(x.DevicePath)) hid = x;
                        x.CloseDevice();
                    }
                }
                if (hid != null)
                {
                    hid.OpenDevice();
                    mulTimer = new System.Timers.Timer
                    {
                        Interval = 300,
                        AutoReset = true,
                        Enabled = true,
                    };
                    mulTimer.Elapsed += OnTimedEvent;
                    hid.ReadReport(OnReport);

                    string Button1PropName = INI.ReadINI(dev.DevicePath, "BUTTON1");
                    string Button2PropName = INI.ReadINI(dev.DevicePath, "BUTTON2");
                    foreach (Control item in gb.Controls["TableLayout"].Controls)
                    {
                        if (item is PropertyControl)
                        {
                            ((PropertyControl)item).SelectItem(false);
                            if (((PropertyControl)item).GetFlagUSB())
                            {
                                if (current == null)
                                {
                                    current = (PropertyControl)item;
                                    current.SelectItem(true);
                                }
                                if (Button1PropName.Equals(((PropertyControl)item).GetPropertyName())) button1 = (PropertyControl)item;
                                if (Button2PropName.Equals(((PropertyControl)item).GetPropertyName())) button2 = (PropertyControl)item;
                            }
                        }
                    }

                }


            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            mul = 1;
        }

        private void OnReport(HidReport report)
        {
            //if (attached == false) { return; }
            Debug.WriteLine("ID: " + report.ReportId);
            Debug.WriteLine("TYPE: " + report.Data[0]);
            if (report.ReportId != 4) return;
            switch (report.Data[0])
            {
                case 1:
                    current.AutoMode(false);
                    current.UpdateValue(mul, (report.Data[1] == 0) ? 1 : -1);
                    break;
                case 2:
                    current.AutoMode(true);
                    break;
                case 3:
                    current.SelectItem(false);
                    current = button1;
                    current.SelectItem(true);
                    break;
                case 4:
                    current.SelectItem(false);
                    current = button2;
                    current.SelectItem(true);
                    break;
            }
            // we need to start listening again for more data
            hid.ReadReport(OnReport);
        }
    }
}

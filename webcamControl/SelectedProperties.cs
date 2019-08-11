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
        public EventHandler PropertiesSync;
        private AllProperties AllPropertiesVar;

        private EventHandler HIDConnectEvent;
        private EventHandler HIDDisconnectEvent;

        private IniFile INI = new IniFile("config.ini");
        private DsDevice webcam;
        private IAMCameraControl pCameraControl;
        private IAMVideoProcAmp pVideoProcAmp;
        private string webcamName;
        private HidDevice hid;

        private System.Timers.Timer mulTimer;
        private System.Timers.Timer alertTimer;
        private int mul = 0;
        private bool oldDir = false;

        private GroupBox gBox;
        private TableLayoutPanel mainLayout;
        private PropertyControl currentProperty;
        private int currentIndex = 0;
        private int PropertyHIDButton1Index = -1;
        private int PropertyHIDButton2Index = -1;

        public SelectedProperties(DsDevice dev)
        {
            InitializeComponent();

            webcam = dev;
            HIDConnectEvent = new EventHandler(HIDConnected);
            HIDDisconnectEvent = new EventHandler(HIDDisconnected);

            Guid iid = typeof(IBaseFilter).GUID;
            webcam.Mon.BindToObject(null, null, ref iid, out object camDevice);
            IBaseFilter camFilter = camDevice as IBaseFilter;
            pCameraControl = camFilter as IAMCameraControl;
            pVideoProcAmp = camFilter as IAMVideoProcAmp;

            webcamName = INI.KeyExists("Name", webcam.DevicePath) ? INI.ReadINI(webcam.DevicePath, "Name") : webcam.Name;

            Anchor = AnchorStyles.Left | AnchorStyles.Right;
            Padding = new Padding(0);
            gBox = new GroupBox
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Dock = DockStyle.Top,
                Text = webcamName,
                Margin = new Padding(0)
            };

            mainLayout = new TableLayoutPanel
            {
                Name = "TableLayout",
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(gBox.Padding.Left, gBox.Padding.Top + 20),
                Dock = DockStyle.Top,
            };
            gBox.Controls.Add(mainLayout);
            Controls.Add(gBox);

            InitProperties();
            InitHID();

            if (hid == null)
            {
                HIDConnectEvent = new EventHandler(HIDConnected);
                Globals._USBControl.HIDConnected += HIDConnectEvent;

            }


            alertTimer = new System.Timers.Timer
            {
                Interval = 600,
                AutoReset = true,
                Enabled = false,
            };
            alertTimer.Elapsed += AlertTimedEvent;
        }

        public void SetAllProp(AllProperties aProp)
        {
            AllPropertiesVar = aProp;
            AllPropertiesVar.ConfigurationUpdate += new EventHandler(UpdateWebcamConfiguration);
            AllPropertiesVar.DeleteSelectedProperties += new EventHandler(Destroy);
            AllPropertiesVar.PropertiesSync += new EventHandler(ExternPropertyUpdate);
        }

        private void Destroy(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void UpdateWebcamConfiguration(object sender, EventArgs e)
        {
            InitProperties();
            InitHID();
            gBox.Text = AllPropertiesVar.GetWebcamName();
        }

        private void InitProperties()
        {
            mainLayout.Controls.Clear();

            foreach (var prop in Enum.GetValues(typeof(CameraControlProperty)))
                if (INI.KeyExists(prop.ToString(), webcam.DevicePath) && "True".Equals(INI.ReadINI(webcam.DevicePath, prop.ToString())))
                    mainLayout.Controls.Add(CreatePropertyControl(prop));
            foreach (var prop in Enum.GetValues(typeof(VideoProcAmpProperty)))
                if (INI.KeyExists(prop.ToString(), webcam.DevicePath) && "True".Equals(INI.ReadINI(webcam.DevicePath, prop.ToString())))
                    mainLayout.Controls.Add(CreatePropertyControl(prop));

            string PropertyHIDButton1Selected = INI.ReadINI(webcam.DevicePath, "PropertyHIDButton1");
            string PropertyHIDButton2Selected = INI.ReadINI(webcam.DevicePath, "PropertyHIDButton2");
            for (int i = 0; i < mainLayout.Controls.Count; i++)
            {
                PropertyControl item = (PropertyControl)mainLayout.Controls[i];
                item.Dock = DockStyle.Top;
                item.ValueUpdate += new EventHandler(PropertyValueUpdate);
                if (PropertyHIDButton1Selected.Equals(item.ToString()))
                    PropertyHIDButton1Index = i;
                if (PropertyHIDButton2Selected.Equals(item.ToString()))
                    PropertyHIDButton2Index = i;
            }
            if (hid != null) SelectNextProperty();
        }

        private PropertyControl CreatePropertyControl(object property)
        {
            bool none, autoSupport, manualSupport, auto, manual;
            int pMax, pMin, pValue, pSteppingDelta, defaultValue;

            if (Object.ReferenceEquals(property.GetType(), new CameraControlProperty().GetType()))
            {
                CameraControlFlags cameraFlags;
                pCameraControl.GetRange((CameraControlProperty)property, out pMin, out pMax, out pSteppingDelta, out defaultValue, out cameraFlags);
                none = cameraFlags == CameraControlFlags.None;
                autoSupport = (cameraFlags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
                manualSupport = (cameraFlags & CameraControlFlags.Manual) == CameraControlFlags.Manual;
                pCameraControl.Get((CameraControlProperty)property, out pValue, out cameraFlags);
                auto = (cameraFlags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
                manual = (cameraFlags & CameraControlFlags.Manual) == CameraControlFlags.Manual;
            }
            else
            {
                // VideoProcAmpProperty
                VideoProcAmpFlags cameraFlags;
                pVideoProcAmp.GetRange(
                    (VideoProcAmpProperty)property, out pMin, out pMax, out pSteppingDelta, out defaultValue, out cameraFlags);
                none = cameraFlags == VideoProcAmpFlags.None;
                autoSupport = (cameraFlags & VideoProcAmpFlags.Auto) == VideoProcAmpFlags.Auto;
                manualSupport = (cameraFlags & VideoProcAmpFlags.Manual) == VideoProcAmpFlags.Manual;
                pVideoProcAmp.Get((VideoProcAmpProperty)property, out pValue, out cameraFlags);
                auto = (cameraFlags & VideoProcAmpFlags.Auto) == VideoProcAmpFlags.Auto;
                manual = (cameraFlags & VideoProcAmpFlags.Manual) == VideoProcAmpFlags.Manual;
            }
            return new PropertyControl(property, none, autoSupport, manualSupport, auto, manual, pMax, pMin, pValue, pSteppingDelta, defaultValue);
        }

        private void InitHID()
        {
            if (INI.KeyExists("HID", webcam.DevicePath))
            {
                string hidPath = INI.ReadINI(webcam.DevicePath, "HID");
                if (hid != null) hid.CloseDevice();
                hid = HidDevices.GetDevice(hidPath);
                if (hid != null)
                {
                    hid.OpenDevice();
                    mulTimer = new System.Timers.Timer
                    {
                        Interval = 600,
                        AutoReset = true,
                        Enabled = true,
                    };
                    mulTimer.Elapsed += HIDTimedEvent;
                    if (currentProperty == null)
                        SelectNextProperty();
                    else
                        currentProperty.SelectItem(true);
                    hid.ReadReport(OnReport);
                    Globals._USBControl.HIDDisconnected += HIDDisconnectEvent;

                }
                else
                {
                    Globals._USBControl.HIDConnected -= HIDConnectEvent;
                }
            }
        }

        private void HIDConnected(object sender, EventArgs e)
        {
            InitHID();
        }

        private void HIDDisconnected(object sender, EventArgs e)
        {
            string hidPath = INI.ReadINI(webcam.DevicePath, "HID");
            hid = HidDevices.GetDevice(hidPath);
            if (hid == null) HIDDeInit();
        }

        private void PropertyValueUpdate(object sender, EventArgs e)
        {
            PropertiesSync?.Invoke(sender, e);
        }

        private void ExternPropertyUpdate(object sender, EventArgs e)
        {
            if (sender is PropertyControlSave pcs)
            {
                foreach (Control item in mainLayout.Controls)
                {
                    if (item is PropertyControl pc)
                    {
                        if (pcs.ToString().Equals(pc.ToString()))
                        {
                            pc.SyncValue(pcs);
                        }
                    }
                }
            }
        }

        private void SelectNextProperty()
        {
            if (++currentIndex == mainLayout.Controls.Count) currentIndex = 0;
            if (currentProperty != null) currentProperty.SelectItem(false);
            currentProperty = (PropertyControl)mainLayout.Controls[currentIndex];
            currentProperty.SelectItem(true);
        }

        private void HIDTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            mul = 0;
        }

        public void WebcamDisconnect()
        {
            alertTimer.Enabled = true;
            AlertTimedEvent(alertTimer, null);
        }

        private void AlertTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    AlertTimedEventImpl();
                }
                ));
            }
            else
            {
                AlertTimedEventImpl();
            }
        }
        private void AlertTimedEventImpl()
        {
            if (this.ForeColor != Color.Red)
            {
                this.ForeColor = Color.Red;
            }
            else
            {
                this.ForeColor = SystemColors.ControlText;
            }
        }
        public void WebcamConnect()
        {
            alertTimer.Enabled = false;
            if (this.ForeColor == Color.Red)
                AlertTimedEvent(alertTimer, null);

            foreach (Control item in mainLayout.Controls)
            {
                if (item is PropertyControl pc)
                {
                    PropertyValueUpdate(pc, new EventArgs());
                }
            }
        }


        private void HIDDeInit()
        {
            Globals._USBControl.HIDConnected += HIDConnectEvent;
            foreach (PropertyControl item in mainLayout.Controls)
            {
                item.SelectItem(false);
            }
        }

        private void OnReport(HidReport report)
        {
            if (report.ReportId != 4) return;
            switch (report.Data[0])
            {
                case 1:
                    currentProperty.AutoMode(false);
                    bool dir = report.Data[1] != 0;
                    Debug.WriteLine(dir);
                    if (oldDir != dir)
                    {
                        oldDir = dir;
                        mul = 1;
                    }
                    else
                    {
                        mul++;
                    }
                    currentProperty.UpdateValue(mul, dir ? 1 : -1);
                    break;
                case 2:
                    currentProperty.AutoMode(true);
                    Debug.WriteLine("Button auto");
                    break;
                case 3:
                    Debug.WriteLine("Button 1");
                    if (PropertyHIDButton1Index < 0) break;
                    currentProperty.SelectItem(false);
                    currentIndex = PropertyHIDButton1Index;
                    currentProperty = (PropertyControl)mainLayout.Controls[currentIndex];
                    currentProperty.SelectItem(true);
                    break;
                case 4:
                    Debug.WriteLine("Button 2");
                    if (PropertyHIDButton2Index < 0) break;
                    currentProperty.SelectItem(false);
                    currentIndex = PropertyHIDButton2Index;
                    currentProperty = (PropertyControl)mainLayout.Controls[currentIndex];
                    currentProperty.SelectItem(true);
                    break;
                case 5:
                    Debug.WriteLine("Button next");
                    if (report.Data[1] == 0) break;
                    SelectNextProperty();
                    break;
            }
            // we need to start listening again for more data
            hid.ReadReport(OnReport);
        }
    }
}

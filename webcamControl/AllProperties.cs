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
    public partial class AllProperties : UserControl
    {
        private const int VendorId = 0x0483;
        private const int ProductId = 0x5750;

        private static IniFile INI = new IniFile("config.ini");
        private DsDevice webcam;
        private HidDevice hid;
        public int selected = 0;
        public AllProperties(DsDevice dev)
        {
            InitializeComponent();
            webcam = dev;
            Guid iid = typeof(IBaseFilter).GUID;
            webcam.Mon.BindToObject(null, null, ref iid, out object camDevice);
            IBaseFilter camFilter = camDevice as IBaseFilter;
            IAMCameraControl pCameraControl = camFilter as IAMCameraControl;
            IAMVideoProcAmp pVideoProcAmp = camFilter as IAMVideoProcAmp;

            if (pCameraControl != null)
            {
                TableLayoutPanel tl = new TableLayoutPanel
                {
                    Name = "TableLayout",
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    //Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                    Dock = DockStyle.Top,
                    Location = new Point(this.Padding.Left, this.Padding.Top)
                };

                foreach (var prop in Enum.GetValues(typeof(CameraControlProperty)))
                {
                    PropertyControlSave pc = new PropertyControlSave(pCameraControl, prop);
                    bool sel = "True".Equals(INI.ReadINI(webcam.DevicePath, "All_" + prop.ToString()));
                    bool usb = "True".Equals(INI.ReadINI(webcam.DevicePath, "USB_" + prop.ToString()));
                    if (usb) sel = true;
                    if (sel) selected++;
                    pc.SetFlagAll(sel);
                    pc.SetFlagUSB(usb);
                    tl.Controls.Add(pc);
                }
                foreach (var prop in Enum.GetValues(typeof(VideoProcAmpProperty)))
                {
                    PropertyControlSave pc = new PropertyControlSave(pVideoProcAmp, prop);
                    bool sel = "True".Equals(INI.ReadINI(webcam.DevicePath, "All_" + prop.ToString()));
                    bool usb = "True".Equals(INI.ReadINI(webcam.DevicePath, "USB_" + prop.ToString()));
                    if (usb) sel = true;
                    if (sel) selected++;
                    pc.SetFlagAll(sel);
                    pc.SetFlagUSB(usb);
                    tl.Controls.Add(pc);
                }

                TableLayoutPanel tlActions = new TableLayoutPanel
                {
                    Name = "Actions",
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Dock = DockStyle.Top,
                    Location = new Point(this.Padding.Left, this.Padding.Top),
                    ColumnCount = 2,
                };
                tlActions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                tlActions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

                /*
                Button FocusPreset1 = new Button
                {
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left |AnchorStyles.Right,
                    Text = "Save focus preset 1"
                };
                Button FocusPreset2 = new Button
                {
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Text = "Save focus preset 2"
                };
                tlActions.Controls.Add(FocusPreset1);
                tlActions.Controls.Add(FocusPreset2);
                */
                ComboBox ButtonProp1 = new ComboBox
                {
                    Name = "ButtonProp1",
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                ComboBox ButtonProp2 = new ComboBox
                {
                    Name = "ButtonProp2",
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                string button1 = INI.ReadINI(webcam.DevicePath, "BUTTON1");
                string button2 = INI.ReadINI(webcam.DevicePath, "BUTTON2");
                INI.ReadINI(webcam.DevicePath, "BUTTON1");
                foreach (var prop in Enum.GetValues(typeof(CameraControlProperty)))
                {
                    int i = ButtonProp1.Items.Add(prop.ToString());
                    if (button1.Equals(prop.ToString())) ButtonProp1.SelectedIndex = i;
                    i = ButtonProp2.Items.Add(prop.ToString());
                    if (button2.Equals(prop.ToString())) ButtonProp2.SelectedIndex = i;
                }
                foreach (var prop in Enum.GetValues(typeof(VideoProcAmpProperty)))
                {
                    int i = ButtonProp1.Items.Add(prop.ToString());
                    if (button1.Equals(prop.ToString())) ButtonProp1.SelectedIndex = i;
                    i = ButtonProp2.Items.Add(prop.ToString());
                    if (button2.Equals(prop.ToString())) ButtonProp2.SelectedIndex = i;
                }
                tlActions.Controls.Add(ButtonProp1);
                tlActions.Controls.Add(ButtonProp2);
                tl.Controls.Add(tlActions);

                ComboBox hidDevices = new ComboBox
                {
                    Dock = DockStyle.Top,
                };
                string hidPath = INI.ReadINI(webcam.DevicePath, "HID");
                // TODO: добавить вариант none
                foreach (HidDevice x in HidDevices.Enumerate(VendorId, ProductId))
                {
                    x.OpenDevice();
                    x.ReadSerialNumber(out byte[] sb_buf);
                    String sn = Encoding.Unicode.GetString(sb_buf);
                    int i = hidDevices.Items.Add(sn);
                    // TODO
                    if (hidPath.Equals(x.DevicePath))
                    {
                        hid = x;
                        hidDevices.SelectedIndex = i;
                    }
                    x.CloseDevice();
                }
                hidDevices.SelectedIndexChanged += new System.EventHandler(this.HID_SelectedIndexChanged);
                tl.Controls.Add(hidDevices);

                Button SaveButton = new Button
                {
                    Dock = DockStyle.Top,
                    Text = "Save settings"
                };
                SaveButton.Click += new EventHandler(SaveSetting);
                tl.Controls.Add(SaveButton);


                Controls.Add(tl);

            }
        }

        private void HID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = ((ComboBox)sender).SelectedItem.ToString();

            Debug.WriteLine(selected);

            HidReport report = new HidReport(1);
            byte[] data = new byte[1];
            report.ReportId = 3;

            foreach (HidDevice x in HidDevices.Enumerate(VendorId, ProductId))
            {
                x.OpenDevice();
                x.ReadSerialNumber(out byte[] sb_buf);
                String sn = Encoding.Unicode.GetString(sb_buf);
                Debug.WriteLine(sn);
                if (selected.Equals(sn))
                {
                    Debug.WriteLine("true");
                    hid = x;
                    data[0] = 7;
                }
                else
                {
                    Debug.WriteLine("false");
                    data[0] = 0;
                }
                report.Data = data;
                x.WriteReportSync(report);
                x.CloseDevice();
            }
        }

        private void SaveSetting(object sender, EventArgs e)
        {
            foreach (Control item in Controls["TableLayout"].Controls)
            {
                if (item is PropertyControlSave)
                {
                    INI.Write(webcam.DevicePath, "All_" + ((PropertyControlSave)item).GetPropertyName(), ((PropertyControlSave)item).GetFlagAll().ToString());
                    INI.Write(webcam.DevicePath, "USB_" + ((PropertyControlSave)item).GetPropertyName(), ((PropertyControlSave)item).GetFlagUSB().ToString());
                }
            }


            ComboBox cb = (ComboBox)Controls["TableLayout"].Controls["Actions"].Controls["ButtonProp1"];
            INI.Write(webcam.DevicePath, "BUTTON1", cb.Items[cb.SelectedIndex].ToString());
            cb = (ComboBox)Controls["TableLayout"].Controls["Actions"].Controls["ButtonProp2"];
            INI.Write(webcam.DevicePath, "BUTTON2", cb.Items[cb.SelectedIndex].ToString());

            if (hid != null)
            {
                byte[] data = new byte[1];
                data[0] = 0;
                HidReport report = new HidReport(1)
                {
                    ReportId = 3,
                    Data = data
                };
                hid.OpenDevice();
                hid.WriteReportSync(report);
                INI.Write(webcam.DevicePath, "HID", hid.DevicePath);
                hid.CloseDevice();
            }
            else
                INI.DeleteKey("HID", webcam.DevicePath);

            // обновить страницу
            if (Application.OpenForms["Form2"] != null)
            {
                (Application.OpenForms["Form2"] as Form2).InitSelected();
            }
        }

        public static int countSelected(DsDevice dev)
        {
            int count = 0;

            foreach (var prop in Enum.GetValues(typeof(CameraControlProperty)))
            {
                bool sel = "True".Equals(INI.ReadINI(dev.DevicePath, "All_" + prop.ToString()));
                bool usb = "True".Equals(INI.ReadINI(dev.DevicePath, "USB_" + prop.ToString()));
                if (usb) sel = true;
                if (sel) count++;
            }
            foreach (var prop in Enum.GetValues(typeof(VideoProcAmpProperty)))
            {
                bool sel = "True".Equals(INI.ReadINI(dev.DevicePath, "All_" + prop.ToString()));
                bool usb = "True".Equals(INI.ReadINI(dev.DevicePath, "USB_" + prop.ToString()));
                if (usb) sel = true;
                if (sel) count++;
            }
            return count;
        }
    }
}

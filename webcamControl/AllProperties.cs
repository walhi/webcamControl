﻿using System;
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
using System.Runtime.InteropServices;

namespace webcamControl
{
    public partial class AllProperties : UserControl
    {


        public EventHandler PropertiesSync;
        public EventHandler ConfigurationUpdate;
        public EventHandler CreateSelectedProperties;
        public EventHandler DeleteSelectedProperties;

        public SelectedProperties SelectedPropertiesVar;

        private const int VendorId = 0x0483;
        private const int ProductId = 0x5750;

        private static IniFile INI = new IniFile("config.ini");

        private DsDevice webcam;
        private IAMCameraControl pCameraControl;
        private IAMVideoProcAmp pVideoProcAmp;
        private string webcamName;

        private const string NotUsedText = "None";
        private HidDevice hid;
        private ComboBox hidDevices;

        private TableLayoutPanel main;
        private ComboBox PropertyHIDButton1;
        private ComboBox PropertyHIDButton2;
        private TextBox webcamNameEdit;

        public int CountFavorites = 0;

        public AllProperties(DsDevice dev)
        {
            InitializeComponent();

            webcam = dev;
            Debug.WriteLine(webcam.Name);
            Guid iid = typeof(IBaseFilter).GUID;
            webcam.Mon.BindToObject(null, null, ref iid, out object camDevice);
            IBaseFilter camFilter = camDevice as IBaseFilter;
            pCameraControl = camFilter as IAMCameraControl;
            pVideoProcAmp = camFilter as IAMVideoProcAmp;

            webcamName = INI.KeyExists("Name", webcam.DevicePath) ? INI.ReadINI(webcam.DevicePath, "Name") : webcam.Name;

            main = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                Location = new Point(this.Padding.Left, this.Padding.Top)
            };
            Controls.Add(main);

            // Блок с ползунками
            foreach (var prop in Enum.GetValues(typeof(CameraControlProperty)))
                main.Controls.Add(CreatePropertyControlSave(prop));
            foreach (var prop in Enum.GetValues(typeof(VideoProcAmpProperty)))
                main.Controls.Add(CreatePropertyControlSave(prop));
            foreach (PropertyControlSave pc in main.Controls)
            {
                if ("True".Equals(INI.ReadINI(webcam.DevicePath, pc.ToString())))
                {
                    pc.SetFavorite(true);
                    CountFavorites++;
                }
                pc.ValueUpdate += new EventHandler(SetPropertyValue);
                pc.SyncControls += new EventHandler(SendPropertyUpdate);
                pc.FavoriteUpdate += new EventHandler(FavoritesUpdate);
            }


            // Пресеты
            TableLayoutPanel tlPresets = new TableLayoutPanel
            {
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                Location = new Point(this.Padding.Left, this.Padding.Top),
                ColumnCount = 4,
            };
            tlPresets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlPresets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlPresets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlPresets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            Button loadP1Button = new Button
            {
                Dock = DockStyle.Top,
                Tag = "Preset_1",
                Enabled = INI.KeyExists("Preset_1", webcam.DevicePath),
                Text = "Load 1"     
            };
            Button loadP2Button = new Button
            {
                Dock = DockStyle.Top,
                Tag = "Preset_2",
                Enabled = INI.KeyExists("Preset_2", webcam.DevicePath),
                Text = "Load 2"
            };
            Button loadP3Button = new Button
            {
                Dock = DockStyle.Top,
                Tag = "Preset_3",
                Enabled = INI.KeyExists("Preset_3", webcam.DevicePath),
                Text = "Load 3"
            };
            Button loadAutostartButton = new Button
            {
                Dock = DockStyle.Top,
                Tag = "Autostart",
                Enabled = INI.KeyExists("Autostart", webcam.DevicePath),
                Text = "Load"
            };
            Button saveP1Button = new Button
            {
                Dock = DockStyle.Top,
                Tag = loadP1Button,
                Text = "Save 1"
            };
            Button saveP2Button = new Button
            {
                Dock = DockStyle.Top,
                Tag = loadP2Button,
                Text = "Save 2"
            };
            Button saveP3Button = new Button
            {
                Dock = DockStyle.Top,
                Tag = loadP3Button,
                Text = "Save 3"
            };
            Button saveAutostartButton = new Button
            {
                Dock = DockStyle.Top,
                Tag = loadAutostartButton,
                Text = "Autostart"
            };
            saveP1Button.Click += new EventHandler(SavePresetSetting);
            saveP2Button.Click += new EventHandler(SavePresetSetting);
            saveP3Button.Click += new EventHandler(SavePresetSetting);
            saveAutostartButton.Click += new EventHandler(SavePresetSetting);
            loadP1Button.Click += new EventHandler(LoadPresetSetting);
            loadP2Button.Click += new EventHandler(LoadPresetSetting);
            loadP3Button.Click += new EventHandler(LoadPresetSetting);
            loadAutostartButton.Click += new EventHandler(LoadPresetSetting);
            if (INI.KeyExists("Autostart_enable", webcam.DevicePath)) loadAutostartButton.PerformClick();
            tlPresets.Controls.Add(saveP1Button);
            tlPresets.Controls.Add(saveP2Button);
            tlPresets.Controls.Add(saveP3Button);
            tlPresets.Controls.Add(saveAutostartButton);
            tlPresets.Controls.Add(loadP1Button);
            tlPresets.Controls.Add(loadP2Button);
            tlPresets.Controls.Add(loadP3Button);
            tlPresets.Controls.Add(loadAutostartButton);
            main.Controls.Add(tlPresets);

            // Действия на кнопки (аппаратные)
            TableLayoutPanel tlActions = new TableLayoutPanel
            {
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                Location = new Point(this.Padding.Left, this.Padding.Top),
                ColumnCount = 2,
            };
            tlActions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlActions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            PropertyHIDButton1 = new ComboBox
            {
                Name = "ButtonProp1",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            PropertyHIDButton2 = new ComboBox
            {
                Name = "ButtonProp2",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            UpdatePropertyList();
            tlActions.Controls.Add(PropertyHIDButton1);
            tlActions.Controls.Add(PropertyHIDButton2);
            main.Controls.Add(tlActions);

            // Выбор HID устройства
            hidDevices = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            HIDList();
            Globals._USBControl.HIDDisconnected += new EventHandler(HIDUpdateHandler);
            Globals._USBControl.HIDConnected += new EventHandler(HIDUpdateHandler);
            main.Controls.Add(hidDevices);

            // Имя устройства
            webcamNameEdit = new TextBox
            {
                Dock = DockStyle.Top,
                Text = webcamName
            };
            main.Controls.Add(webcamNameEdit);


            // Кнопка сохранения
            Button SaveButton = new Button
            {
                Dock = DockStyle.Top,
                Text = "Save settings"
            };
            SaveButton.Click += new EventHandler(SaveSetting);
            main.Controls.Add(SaveButton);

            Globals._USBControl.DShowDisconnected += new EventHandler(WebcamDisconnected);
        }

        private PropertyControlSave CreatePropertyControlSave(object property)
        {
            bool none, autoSupport, manualSupport, auto, manual;
            int pMax, pMin, pValue, pSteppingDelta, defaultValue;

            if (Object.ReferenceEquals(property.GetType(), new CameraControlProperty().GetType()) && pCameraControl != null)
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
            else if (pVideoProcAmp != null)
            {
                // VideoProcAmpProperty
                VideoProcAmpFlags cameraFlags;
                pVideoProcAmp.GetRange((VideoProcAmpProperty)property, out pMin, out pMax, out pSteppingDelta, out defaultValue, out cameraFlags);
                none = cameraFlags == VideoProcAmpFlags.None;
                autoSupport = (cameraFlags & VideoProcAmpFlags.Auto) == VideoProcAmpFlags.Auto;
                manualSupport = (cameraFlags & VideoProcAmpFlags.Manual) == VideoProcAmpFlags.Manual;
                pVideoProcAmp.Get((VideoProcAmpProperty)property, out pValue, out cameraFlags);
                auto = (cameraFlags & VideoProcAmpFlags.Auto) == VideoProcAmpFlags.Auto;
                manual = (cameraFlags & VideoProcAmpFlags.Manual) == VideoProcAmpFlags.Manual;
            }
            else
            {
                none = true;
                autoSupport = manualSupport = auto = manual = false;
                pMax = pMin = pValue = pSteppingDelta = defaultValue = 0;

            }
            return new PropertyControlSave(property, none, autoSupport, manualSupport, auto, manual, pMax, pMin, pValue, pSteppingDelta, defaultValue);
        }

        private void WebcamDisconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("WebcamDisconnected" + sender.GetType().ToString());
            if (!(sender is DsDevice dev)) return;
            Debug.WriteLine("t1");
            if (dev.DevicePath.Equals(webcam.DevicePath))
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
        }

        private void WebcamDisconnectedImpl()
        {
            this.Dispose();
        }

        private void SetPropertyValue(object sender, EventArgs e)
        {
            PropertyControlSave pc = (PropertyControlSave)sender;

            int value = pc.GetValue();
            bool auto = pc.GetAutoMode();

            if (Object.ReferenceEquals(pc.GetProperty().GetType(), new CameraControlProperty().GetType()))
            {
                pCameraControl.Set((CameraControlProperty)pc.GetProperty(), value, auto ? CameraControlFlags.Auto : CameraControlFlags.Manual);
            }
            else
            {
                // VideoProcAmpProperty
                pVideoProcAmp.Set((VideoProcAmpProperty)pc.GetProperty(), value, auto ? VideoProcAmpFlags.Auto : VideoProcAmpFlags.Manual);
            }
        }

        private void SendPropertyUpdate(object sender, EventArgs e)
        {
            PropertiesSync?.Invoke(sender, e);
        }


        private void FavoritesUpdate(object sender, EventArgs e)
        {
            UpdatePropertyList();
        }

        private void UpdatePropertyList()
        {
            // TODO none
            string item1, item2;
            if (PropertyHIDButton1.Items.Count != 0)
            {
                item1 = PropertyHIDButton1.Items[PropertyHIDButton1.SelectedIndex].ToString();
                item2 = PropertyHIDButton2.Items[PropertyHIDButton2.SelectedIndex].ToString();
            }
            else
            {
                item1 = INI.ReadINI(webcam.DevicePath, "PropertyHIDButton1");
                item2 = INI.ReadINI(webcam.DevicePath, "PropertyHIDButton2");
            }
            PropertyHIDButton1.Items.Clear();
            PropertyHIDButton2.Items.Clear();
            PropertyHIDButton1.Items.Add(NotUsedText);
            PropertyHIDButton2.Items.Add(NotUsedText);
            PropertyHIDButton1.SelectedIndex = 0;
            PropertyHIDButton2.SelectedIndex = 0;


            foreach (Control item in main.Controls)
            {
                if (item is PropertyControlSave prop)
                {
                    if (prop.GetFavorite())
                    {
                        int i = PropertyHIDButton1.Items.Add(prop.ToString());
                        if (item1.Equals(prop.ToString())) PropertyHIDButton1.SelectedIndex = i;
                        i = PropertyHIDButton2.Items.Add(prop.ToString());
                        if (item2.Equals(prop.ToString())) PropertyHIDButton2.SelectedIndex = i;
                    }

                }
            }
        }

        private void HIDUpdateHandler(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    HIDUpdateHandlerImpl();
                }
                ));
            }
            else
            {
                HIDUpdateHandlerImpl();
            }
        }

        private void HIDUpdateHandlerImpl()
        {
            HIDList();
        }

        private void HIDList()
        {
            hidDevices.SelectedIndexChanged -= new EventHandler(this.HID_SelectedIndexChanged);
            hidDevices.Items.Clear();
            hidDevices.SelectedIndex = hidDevices.Items.Add(NotUsedText);
            string hidPath = INI.ReadINI(webcam.DevicePath, "HID");
            foreach (HidDevice x in HidDevices.Enumerate(VendorId, ProductId))
            {
                x.OpenDevice();
                x.ReadSerialNumber(out byte[] sb_buf);
                String sn = Encoding.Unicode.GetString(sb_buf);
                int i = hidDevices.Items.Add(sn);
                if (hidPath.Equals(x.DevicePath))
                {
                    hid = x;
                    hidDevices.SelectedIndex = i;
                }
                x.CloseDevice();
            }
            // На случай, если устройство было выбрано ранее, но сейчас не подключено.
            if (hid == null && hidPath.Length > 0)
            {
                hidDevices.SelectedIndex = hidDevices.Items.Add(hidPath + " (disconnected)");
            }
            hidDevices.SelectedIndexChanged += new System.EventHandler(this.HID_SelectedIndexChanged);
        }

        private void HID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = ((ComboBox)sender).SelectedItem.ToString();
            if (selected.Equals(NotUsedText)) hid = null;


            HidReport report = new HidReport(1);
            byte[] data = new byte[1];
            report.ReportId = 3;

            foreach (HidDevice x in HidDevices.Enumerate(VendorId, ProductId))
            {
                x.OpenDevice();
                x.ReadSerialNumber(out byte[] sb_buf);
                String sn = Encoding.Unicode.GetString(sb_buf);
                if (selected.Equals(sn))
                {
                    hid = x;
                    data[0] = 7;
                }
                else
                {
                    data[0] = 0;
                }
                report.Data = data;
                x.WriteReportSync(report);
                x.CloseDevice();
            }
        }

        private void SaveSetting(object sender, EventArgs e)
        {
            int count = 0;
            foreach (Control item in main.Controls)
            {
                if (item is PropertyControlSave)
                {
                    count += ((PropertyControlSave)item).GetFavorite() ? 1 : 0;
                    INI.Write(webcam.DevicePath, ((PropertyControlSave)item).ToString(), ((PropertyControlSave)item).GetFavorite().ToString());
                }
            }


            INI.Write(webcam.DevicePath, "PropertyHIDButton1", PropertyHIDButton1.Items[PropertyHIDButton1.SelectedIndex].ToString());
            INI.Write(webcam.DevicePath, "PropertyHIDButton2", PropertyHIDButton2.Items[PropertyHIDButton2.SelectedIndex].ToString());

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

            INI.Write(webcam.DevicePath, "Name", webcamNameEdit.Text);
            ((TabPage)Parent).Text = webcamName = webcamNameEdit.Text;



            ConfigurationUpdate?.Invoke(this, new EventArgs());
            if (CountFavorites == 0 && count > 0)
                CreateSelectedProperties?.Invoke(this, new EventArgs());
            if (CountFavorites > 0 && count == 0)
                DeleteSelectedProperties?.Invoke(this, new EventArgs());

            CountFavorites = count;
        }

        private void SavePresetSetting(object sender, EventArgs e)
        {
            Button load = (Button)((Button)sender).Tag;
            String tag = load.Tag.ToString();
            INI.Write(webcam.DevicePath, tag, true.ToString());
            foreach (Control item in main.Controls)
            {
                if (item is PropertyControlSave)
                {
                    INI.Write(webcam.DevicePath, tag + "_auto_" + ((PropertyControlSave)item).ToString(), ((PropertyControlSave)item).GetAutoMode().ToString());
                    INI.Write(webcam.DevicePath, tag + "_value_" + ((PropertyControlSave)item).ToString(), ((PropertyControlSave)item).GetValue().ToString());
                }
            }
            load.Enabled = true;
        }
        public void LoadPresetSetting(object sender, EventArgs e)
        {
            String tag = "";
            if (sender is Button)
            {
                tag = ((Button)sender).Tag.ToString();
            } else if (sender is String){
                tag = (String)sender;
            }
            foreach (Control item in main.Controls)
            {
                if (item is PropertyControlSave)
                {
                    ((PropertyControlSave)item).SetAutoMode(Boolean.Parse(INI.ReadINI(webcam.DevicePath, tag + "_auto_" + ((PropertyControlSave)item).ToString())));
                    ((PropertyControlSave)item).SetValue(Int32.Parse(INI.ReadINI(webcam.DevicePath, tag + "_value_" + ((PropertyControlSave)item).ToString())));
                }
            }
        }

        public DsDevice GetDevice()
        {
            return webcam;
        }

        public string GetWebcamName()
        {
            return webcamName;
        }

        public void AddPropertyUpdateHandler()
        {
            SelectedPropertiesVar.PropertiesSync += new EventHandler(ExternPropertyUpdate);
        }
        public void AddPropertyUpdateHandler(SelectedProperties sProp)
        {
            SelectedPropertiesVar = sProp;
            AddPropertyUpdateHandler();
        }

        private void ExternPropertyUpdate(object sender, EventArgs e)
        {
            Debug.WriteLine("PropertyUpdate");
            PropertyControl pc = (PropertyControl)sender;
            foreach (Control item in main.Controls)
            {
                if (item is PropertyControlSave)
                {
                    if (pc.ToString().Equals(((PropertyControlSave)item).ToString()))
                    {
                        ((PropertyControlSave)item).SyncValue(pc);
                    }
                }
            }
        }
    }
}

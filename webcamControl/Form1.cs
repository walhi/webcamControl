using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DirectShowLib;
using HidLibrary;
using System.Timers;

namespace webcamControl
{
    public partial class Form1 : Form
    {
        //private const int VendorId = 0x0603;
        //private const int ProductId = 0x00f5;
        private const int VendorId = 0x0483;
        private const int ProductId = 0x5750;

        private HidDevice device;

        private TrackBar tBarFocusHID;
        private CheckBox cbFocusHID;

        private System.Timers.Timer mulTimer;
        private int mul = 1;
        private bool oldDir = false;

        IAMVideoProcAmp ggghhh;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DsDevice[] capDev;
            IFilterGraph2 m_FilterGraph = new FilterGraph() as IFilterGraph2;
            IBaseFilter pDeviceFilter = null;
            IAMCameraControl pCameraControl;
            IAMVideoProcAmp pVideoProcAmp;
            CameraControlFlags pCapsFlags;
            VideoProcAmpFlags pCapsFlags2;
            int pSteppingDelta;
            int pMin;
            int pMax;
            int pDefault;
            int pValue;

            int yLocation = 40;
            const int yStep = 113;

            overWindowToggleButton.Appearance = Appearance.Button;

            // TODO
            updateButton.Visible = false;

            // Get the collection of video devices
            capDev = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            foreach (DsDevice d in capDev)
            {
                //listBox1.Items.Add(d.Name);


                int hr;
                object camDevice;
                Guid iid = typeof(IBaseFilter).GUID;
                d.Mon.BindToObject(null, null, ref iid, out camDevice);
                IBaseFilter camFilter = camDevice as IBaseFilter;
                pCameraControl = camFilter as IAMCameraControl;
                pVideoProcAmp = camFilter as IAMVideoProcAmp;

            
                if (pCameraControl != null)
                {
                    GroupBox gb = new GroupBox();
                    CheckBox cbFocus = cbFocusHID = new CheckBox();
                    CheckBox cbExposure = new CheckBox();
                    Button butForce = new Button();
                    TrackBar tBarFocus = tBarFocusHID = new TrackBar();
                    TrackBar tBarExposure = new TrackBar();

                    gb.Text = d.Name;
                    gb.Width = 295;
                    gb.Height = 106;
                    gb.Location = new Point(13, yLocation);


                    pCameraControl.GetRange(CameraControlProperty.Focus, out pMin, out pMax, out pSteppingDelta, out pDefault, out pCapsFlags);

                    cbFocus.Text = "Auto focus";
                    cbFocus.Width = 78;
                    cbFocus.Height = 17;
                    cbFocus.Location = new Point(6, 23);
                    cbFocus.Tag = butForce;
                    cbFocus.Checked = (pCapsFlags == CameraControlFlags.Auto);
                    cbFocus.CheckedChanged += new EventHandler(this.checkBoxFocus_CheckedChanged);

                    butForce.Text = "Force";
                    butForce.Width = 75;
                    butForce.Height = 23;
                    butForce.Location = new Point(90, 19);
                    //butForce.Enabled = false;
                    butForce.Tag = pCameraControl;
                    butForce.Click += new EventHandler(this.ButtonForce_Click);

                    // TODO
                    //butBind.Visible = false;
                    //butBind.Text = "Bind key";
                    //butBind.Width = 75;
                    //butBind.Height = 23;
                    //butBind.Location = new Point(214, 19);

                    tBarFocus.Minimum = pMin;
                    tBarFocus.Maximum = pMax;
                    tBarFocus.Value = pDefault;
                    tBarFocus.Name = "TrackBarFocus";
                    tBarFocus.AutoSize = false;
                    tBarFocus.Width = 283;
                    tBarFocus.Height = 23;
                    tBarFocus.TickFrequency = pSteppingDelta;
                    tBarFocus.Location = new Point(6, 46);
                    tBarFocus.Tag = pCameraControl;
                    tBarFocus.Enabled = (pCapsFlags == CameraControlFlags.Manual);
                    pCameraControl.Get(CameraControlProperty.Focus, out pValue, out pCapsFlags);
                    tBarFocus.Value = pMax - pValue + pMin;
                    tBarFocus.Scroll += new EventHandler(this.TrackBarFocus_Scroll);

                    if (pVideoProcAmp != null)
                    {
                        ggghhh = pVideoProcAmp;
                        //pCameraControl.GetRange(CameraControlProperty.Exposure, out pMin, out pMax, out pSteppingDelta, out pDefault, out pCapsFlags);
                        ggghhh.GetRange(VideoProcAmpProperty.WhiteBalance, out pMin, out pMax, out pSteppingDelta, out pDefault, out pCapsFlags2);

                        cbExposure.Text = "Auto exposure";
                        cbExposure.Width = 98;
                        cbExposure.Height = 17;
                        cbExposure.Location = new Point(191, 23);
                        cbExposure.Tag = butForce;
                        cbExposure.Checked = true;
                        cbExposure.CheckedChanged += new EventHandler(this.checkBoxExposure_CheckedChanged);

                        tBarExposure.Minimum = pMin;
                        tBarExposure.Maximum = pMax;
                        tBarExposure.Value = pDefault;
                        tBarExposure.Name = "TrackBarExposure";
                        tBarExposure.AutoSize = false;
                        tBarExposure.Width = 283;
                        tBarExposure.Height = 23;
                        tBarExposure.TickFrequency = pSteppingDelta;
                        tBarExposure.Location = new Point(6, 73);
                        tBarExposure.Tag = pCameraControl;
                        tBarExposure.Enabled = false;
                        //pCameraControl.Get(CameraControlProperty.Exposure, out value, out pCapsFlags);
                        //tBarExposure.Value = pMax - value + pMin;
                        tBarExposure.Scroll += new EventHandler(this.TrackBarExposure_Scroll);
                    }
                    gb.Controls.Add(cbFocus);
                    gb.Controls.Add(butForce);
                    gb.Controls.Add(cbExposure);
                    gb.Controls.Add(tBarFocus);
                    gb.Controls.Add(tBarExposure);
                    Controls.Add(gb);

                    yLocation += yStep;
                }
                pVideoProcAmp = pDeviceFilter as IAMVideoProcAmp;
            }
            this.Height = 13 + yLocation + 39;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            foreach (var x in HidDevices.Enumerate(VendorId, ProductId)){
                device = x;
                device.OpenDevice();
                byte data = 0xff;
                HidReport out1 = new HidReport(data);
                x.WriteReport(out1);
                //byte[] str = new byte[x.Capabilities.OutputReportByteLength - 1]; ;
                //x.ReadManufacturer(out str);
                // Debug.WriteLine(System.Text.ASCIIEncoding.ASCII.GetString(str));
                device.ReadReport(OnReport);
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
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            DoUpdateUI((report.Data[1] != 0));

                        }
                        ));
                    }
                    else
                    {
                        DoUpdateUI((report.Data[1] != 0));
                    }
                    break;
                case 2:
                        this.Invoke((MethodInvoker)(() =>
                        {
                            cbFocusHID.Checked = !cbFocusHID.Checked;
                            checkBoxFocus_CheckedChanged(cbFocusHID, new EventArgs());
                        }
                        ));
                    break;
            }
            // we need to start listening again for more data
            device.ReadReport(OnReport);
        }

        private void DoUpdateUI2()
        {
        }
        private void DoUpdateUI(bool dir)
        {
            if (oldDir != dir) mul = 1;
            int tick = tBarFocusHID.TickFrequency * mul;
            if (dir)
            {
                if ((tBarFocusHID.Value + tick) < tBarFocusHID.Maximum)
                {
                    tBarFocusHID.Value += tick;
                }
                else
                {
                    tBarFocusHID.Value = tBarFocusHID.Maximum;
                }
            }
            else
            {
                if ((tBarFocusHID.Value - tick) > tBarFocusHID.Minimum)
                {
                    tBarFocusHID.Value -= tick;
                }
                else
                {
                    tBarFocusHID.Value = tBarFocusHID.Minimum;
                }
            }
            Focus_change(tBarFocusHID);
            mul++;
            oldDir = dir;
        }

        private void TrackBarExposure_Scroll(object sender, EventArgs e)
        {
            TrackBar trackBar = (TrackBar)sender;
            int Value = trackBar.Value - trackBar.Value % trackBar.TickFrequency;
            int inverceValue = trackBar.Maximum - Value + trackBar.Minimum;
            trackBar.Value = Value;
            IAMCameraControl pCameraControl = (IAMCameraControl)trackBar.Tag;
            pCameraControl.Set(CameraControlProperty.Exposure, inverceValue, CameraControlFlags.Manual);
            ggghhh.Set(VideoProcAmpProperty.WhiteBalance, Value, VideoProcAmpFlags.Manual);
        }

        private void Focus_change(TrackBar trackBar)
        {
            int Value = trackBar.Value - trackBar.Value % trackBar.TickFrequency;
            trackBar.Value = Value;
            int inverceValue = trackBar.Maximum - Value + trackBar.Minimum;
            IAMCameraControl pCameraControl = (IAMCameraControl)trackBar.Tag;
            pCameraControl.Set(CameraControlProperty.Focus, inverceValue, CameraControlFlags.Manual);
        }

        private void TrackBarFocus_Scroll(object sender, EventArgs e)
        {
            Focus_change((TrackBar)sender);
        }

        private void ButtonForce_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            IAMCameraControl pCameraControl = (IAMCameraControl)btn.Tag;
            pCameraControl.Set(CameraControlProperty.Focus, 0, CameraControlFlags.Manual);
            pCameraControl.Set(CameraControlProperty.Focus, 0, CameraControlFlags.Auto);
        }

        private void checkBoxFocus_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Button forceButton = (Button)cb.Tag;
            TrackBar tBar = (TrackBar)cb.Parent.Controls.Find("TrackBarFocus", false)[0];
            IAMCameraControl pCameraControl = (IAMCameraControl)forceButton.Tag;
            forceButton.Enabled = cb.Checked;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pCameraControl.Set(CameraControlProperty.Focus, tBar.Minimum, CameraControlFlags.Manual);
                pCameraControl.Set(CameraControlProperty.Focus, 0, CameraControlFlags.Auto);
                if (mulTimer != null)
                    mulTimer.Close();
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Focus, (tBar.Maximum - tBar.Value + tBar.Minimum), CameraControlFlags.Manual);
                if (device != null)
                {
                    device.ReadReport(OnReport);
                    mulTimer = new System.Timers.Timer();
                    mulTimer.Interval = 300;
                    mulTimer.Elapsed += OnTimedEvent;
                    mulTimer.AutoReset = true;
                    mulTimer.Enabled = true;
                }
            }
        }

        private void checkBoxExposure_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Button btn = (Button)cb.Tag;
            TrackBar tBar = (TrackBar)cb.Parent.Controls.Find("TrackBarExposure", false)[0];
            IAMCameraControl pCameraControl = (IAMCameraControl)btn.Tag;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                pCameraControl.Set(CameraControlProperty.Exposure, tBar.Minimum, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Exposure, (tBar.Maximum - tBar.Value + tBar.Minimum), CameraControlFlags.Manual);
            }
        }
        
        private void updateButton_Click(object sender, EventArgs e)
        {
        }

        private void overWindowToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = ((CheckBox)sender).Checked;
        }
    }
}

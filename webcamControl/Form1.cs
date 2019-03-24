using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DirectShowLib;


namespace webcamControl
{
    public partial class Form1 : Form
    {

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
            CameraControlFlags pCapsFlags;

            int pSteppingDelta;
            int pMin;
            int pMax;
            int pDefault;

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
                int hr = m_FilterGraph.AddSourceFilterForMoniker(d.Mon, null, d.Name, out pDeviceFilter);
                pCameraControl = pDeviceFilter as IAMCameraControl;
                if (pCameraControl != null)
                {
                    GroupBox gb = new GroupBox();
                    CheckBox cbFocus = new CheckBox();
                    CheckBox cbExposure = new CheckBox();
                    Button butForce = new Button();
                    TrackBar tBarFocus = new TrackBar();
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
                    cbFocus.Checked = true;
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
                    tBarFocus.Enabled = false;
                    tBarFocus.Scroll += new EventHandler(this.TrackBarFocus_Scroll);


                    pCameraControl.GetRange(CameraControlProperty.Exposure, out pMin, out pMax, out pSteppingDelta, out pDefault, out pCapsFlags);

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
                    tBarExposure.Scroll += new EventHandler(this.TrackBarExposure_Scroll);

                    gb.Controls.Add(cbFocus);
                    gb.Controls.Add(butForce);
                    gb.Controls.Add(cbExposure);
                    gb.Controls.Add(tBarFocus);
                    gb.Controls.Add(tBarExposure);
                    Controls.Add(gb);

                    yLocation += yStep;
                }
            }
            this.Height = 13 + yLocation + 39;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void TrackBarExposure_Scroll(object sender, EventArgs e)
        {
            TrackBar trackBar = (TrackBar)sender;
            int Value = trackBar.Value - trackBar.Value % trackBar.TickFrequency;
            trackBar.Value = Value;
            IAMCameraControl pCameraControl = (IAMCameraControl)trackBar.Tag;
            pCameraControl.Set(CameraControlProperty.Exposure, Value, CameraControlFlags.Manual);
        }

        private void TrackBarFocus_Scroll(object sender, EventArgs e)
        {
            TrackBar trackBar = (TrackBar)sender;
            int Value = trackBar.Value - trackBar.Value % trackBar.TickFrequency;
            trackBar.Value = Value;
            int inverceValue = trackBar.Maximum - Value + trackBar.Minimum;
            IAMCameraControl pCameraControl = (IAMCameraControl)trackBar.Tag;
            pCameraControl.Set(CameraControlProperty.Focus, inverceValue, CameraControlFlags.Manual);
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
            Button btn = (Button)cb.Tag;
            TrackBar tbar = (TrackBar)cb.Parent.Controls.Find("TrackBarFocus", false)[0];
            IAMCameraControl pCameraControl = (IAMCameraControl)btn.Tag;
            btn.Enabled = cb.Checked;
            tbar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                pCameraControl.Set(CameraControlProperty.Focus, 0, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Focus, (tbar.Maximum - tbar.Value + tbar.Minimum), CameraControlFlags.Manual);
            }
        }

        private void checkBoxExposure_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Button btn = (Button)cb.Tag;
            TrackBar tbar = (TrackBar)cb.Parent.Controls.Find("TrackBarExposure", false)[0];
            IAMCameraControl pCameraControl = (IAMCameraControl)btn.Tag;
            tbar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                pCameraControl.Set(CameraControlProperty.Exposure, 0, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Exposure, tbar.Value, CameraControlFlags.Manual);
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

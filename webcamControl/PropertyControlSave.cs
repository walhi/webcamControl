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
    public partial class PropertyControlSave : UserControl
    {
        private object Property;
        private object Control;
        private int defaultValue;

        public PropertyControlSave(object ctrl, object prop)
        {
            InitializeComponent();

            Property = prop;
            Control = ctrl;
            bool none, autoSupport, manualSupport, auto, manual;
            int pMax, pMin, pValue, pSteppingDelta;

            if (Object.ReferenceEquals(Property.GetType(), new CameraControlProperty().GetType()))
            {
                label.Text = ((CameraControlProperty)prop).ToString();
                CameraControlFlags cameraFlags;
                ((IAMCameraControl)Control).GetRange((CameraControlProperty)prop, out pMin, out pMax, out pSteppingDelta, out defaultValue, out cameraFlags);
                none = cameraFlags == CameraControlFlags.None;
                autoSupport = cameraFlags == CameraControlFlags.Auto;
                manualSupport = cameraFlags == CameraControlFlags.Manual;
                ((IAMCameraControl)Control).Get((CameraControlProperty)prop, out pValue, out cameraFlags);
                auto = cameraFlags == CameraControlFlags.Auto;
                manual = cameraFlags == CameraControlFlags.Manual;
            }
            else
            {
                // VideoProcAmpProperty
                label.Text = ((VideoProcAmpProperty)Property).ToString();
                VideoProcAmpFlags cameraFlags;
                ((IAMVideoProcAmp)Control).GetRange((VideoProcAmpProperty)prop, out pMin, out pMax, out pSteppingDelta, out defaultValue, out cameraFlags);
                none = cameraFlags == VideoProcAmpFlags.None;
                autoSupport = cameraFlags == VideoProcAmpFlags.Auto;
                manualSupport = cameraFlags == VideoProcAmpFlags.Manual;
                ((IAMVideoProcAmp)Control).Get((VideoProcAmpProperty)prop, out pValue, out cameraFlags);
                auto = cameraFlags == VideoProcAmpFlags.Auto;
                manual = cameraFlags == VideoProcAmpFlags.Manual;
            }


            label.Enabled = !none;
            cbAuto.Enabled = !none;
            cbAll.Enabled = !none;
            cbUSB.Enabled = !none;
            trackBar.Enabled = !none;
            if (!none)
            {
                cbAuto.Checked = auto;
                cbAuto.Enabled = !manualSupport;
                trackBar.Enabled = manual;
                trackBar.Minimum = pMin;
                trackBar.Maximum = pMax;
                trackBar.TickFrequency = pSteppingDelta;
                trackBar.Value = pValue;
                //trackBar.Value = pMax - pValue + pMin;
                trackBar.MouseDown += new MouseEventHandler(resetDefault);
            }
        }

        public void SelectItem(bool state)
        {
            label.Font = new Font(label.Font, state?FontStyle.Bold:FontStyle.Regular);
        }

        public string GetPropertyName()
        {
            if (Object.ReferenceEquals(Property.GetType(), new CameraControlProperty().GetType()))
            {
                return ((CameraControlProperty)Property).ToString();
            }
            else
            {
                return ((VideoProcAmpProperty)Property).ToString();
            }
        }

        public void SetFlagAll(bool state)
        {
            cbAll.Checked = state;
        }

        public void SetFlagUSB(bool state)
        {
            cbUSB.Checked = state;
        }

        public bool GetFlagAll()
        {
            return cbAll.Checked;
        }

        public bool GetFlagUSB()
        {
            return cbUSB.Checked;
        }

        private void setValue(int newValue, bool auto)
        {

            if (Object.ReferenceEquals(Property.GetType(), new CameraControlProperty().GetType()))
            {
                ((IAMCameraControl)Control).Set((CameraControlProperty)Property, newValue, auto? CameraControlFlags.Auto:CameraControlFlags.Manual);
            }
            else
            {
                // VideoProcAmpProperty
                ((IAMVideoProcAmp)Control).Set((VideoProcAmpProperty)Property, newValue, auto ? VideoProcAmpFlags.Auto : VideoProcAmpFlags.Manual);
            }
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            int Value = trackBar.Value - trackBar.Value % trackBar.TickFrequency;
            setValue(trackBar.Value, false);
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            trackBar.Enabled = !cbAuto.Checked;
            if (cbAuto.Checked)
            {
                // todo
                setValue(trackBar.Minimum, false);
                setValue(trackBar.Minimum, true);
            }
            else
            {
                setValue(trackBar.Value, false);
            }
        }

        private void resetDefault(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left) return;
            trackBar.Value = defaultValue;
            setValue(trackBar.Value, false);
        }
    }
}

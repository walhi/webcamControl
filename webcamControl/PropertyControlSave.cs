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
        public EventHandler FavoriteUpdate;
        public EventHandler ValueUpdate;

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
                autoSupport = (cameraFlags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
                manualSupport = (cameraFlags & CameraControlFlags.Manual) == CameraControlFlags.Manual;
                ((IAMCameraControl)Control).Get((CameraControlProperty)prop, out pValue, out cameraFlags);
                auto = (cameraFlags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
                manual = (cameraFlags & CameraControlFlags.Manual) == CameraControlFlags.Manual;
            }
            else
            {
                // VideoProcAmpProperty
                label.Text = ((VideoProcAmpProperty)Property).ToString();
                VideoProcAmpFlags cameraFlags;
                ((IAMVideoProcAmp)Control).GetRange((VideoProcAmpProperty)prop, out pMin, out pMax, out pSteppingDelta, out defaultValue, out cameraFlags);
                none = cameraFlags == VideoProcAmpFlags.None;
                autoSupport = (cameraFlags & VideoProcAmpFlags.Auto) == VideoProcAmpFlags.Auto;
                manualSupport = (cameraFlags & VideoProcAmpFlags.Manual) == VideoProcAmpFlags.Manual;
                ((IAMVideoProcAmp)Control).Get((VideoProcAmpProperty)prop, out pValue, out cameraFlags);
                auto = (cameraFlags & VideoProcAmpFlags.Auto) == VideoProcAmpFlags.Auto;
                manual = (cameraFlags & VideoProcAmpFlags.Manual) == VideoProcAmpFlags.Manual;
            }


            label.Enabled = !none;
            cbAuto.Enabled = !none;
            cbFavorite.Enabled = !none;
            trackBar.Enabled = !none;
            if (!none)
            {
                cbAuto.Checked = auto;
                cbAuto.Enabled = autoSupport;
                trackBar.Enabled = auto?false:manual;
                trackBar.Minimum = pMin;
                trackBar.Maximum = pMax;
                trackBar.TickFrequency = pSteppingDelta;
                trackBar.Value = pValue;
                //trackBar.Value = pMax - pValue + pMin;
                trackBar.MouseDown += new MouseEventHandler(resetDefault);
            }
            Dock = DockStyle.Top;
        }

        override public string ToString()
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

        public void SetFavorite(bool state)
        {
            cbFavorite.Checked = state;
        }

        public bool GetFavorite()
        {
            return cbFavorite.Checked;
        }

        public bool GetAutoMode()
        {
            return cbAuto.Checked;
        }

        public int GetPropertyValue()
        {
            return trackBar.Value;
        }

        private void SetValue(int newValue, bool auto)
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
            SetValue(trackBar.Value, false);
            ValueUpdate?.Invoke(this, new EventArgs());
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            trackBar.Enabled = !cbAuto.Checked;
            if (cbAuto.Checked)
            {
                // todo
                SetValue(trackBar.Minimum, false);
                SetValue(trackBar.Minimum, true);
            }
            else
            {
                SetValue(trackBar.Value, false);
            }
            ValueUpdate?.Invoke(this, new EventArgs());
        }

        private void resetDefault(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left) return;
            trackBar.Value = defaultValue;
            SetValue(trackBar.Value, false);
            ValueUpdate?.Invoke(this, new EventArgs());
        }

        private void cbFavorite_CheckedChanged(object sender, EventArgs e)
        {
            // raise the event
            FavoriteUpdate?.Invoke(this, new EventArgs());
        }

        public void UpdateValue(PropertyControl pc)
        {
            trackBar.Value = pc.GetPropertyValue();
            cbAuto.Checked = pc.GetAutoMode();
        }
    }
}

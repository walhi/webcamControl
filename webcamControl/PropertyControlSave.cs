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
using System.Diagnostics;

namespace webcamControl
{
    public partial class PropertyControlSave : UserControl
    {
        private object Property;
        private int defaultValue;
        private int startupValue;
        public EventHandler FavoriteUpdate;
        public EventHandler ValueUpdate;
        public EventHandler SyncControls;

        public PropertyControlSave(object prop, bool none, bool autoSupport, bool manualSupport, bool auto, bool manual, int pMax, int pMin, int pValue, int pSteppingDelta, int defaultValueInit)
        {
            InitializeComponent();

            Property = prop;

            defaultValue = defaultValueInit;

            label.Text = this.ToString();

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
                trackBar.MouseDown += new MouseEventHandler(resetDefault);
                if (!autoSupport)
                    startupValue = pValue; // TODO
                else
                    startupValue = defaultValue; 
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

        public object GetProperty()
        {
            return Property;
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

        public int GetValue()
        {
            return trackBar.Value;
        }

        public void SetAutoMode(bool auto)
        {
            cbAuto.Checked = auto;
            ValueUpdate?.Invoke(this, new EventArgs());
            SyncControls?.Invoke(this, new EventArgs());
        }
        public void SetValue(int newValue)
        {
            Debug.Print("SetValue");
            trackBar.Value = newValue;
            ValueUpdate?.Invoke(this, new EventArgs());
            SyncControls?.Invoke(this, new EventArgs());
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            int Value = trackBar.Value - trackBar.Value % trackBar.TickFrequency;
            SetValue(trackBar.Value);
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            trackBar.Enabled = !cbAuto.Checked;
            if (cbAuto.Checked)
            {
                // todo
                SetValue(trackBar.Minimum);
                SetValue(trackBar.Minimum);
            }
            else
            {
                SetValue(trackBar.Value);
            }
        }

        private void resetDefault(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left) return;
            trackBar.Value = defaultValue;
            SetValue(trackBar.Value);
        }

        private void cbFavorite_CheckedChanged(object sender, EventArgs e)
        {
            // raise the event
            FavoriteUpdate?.Invoke(this, new EventArgs());
        }

        public void SyncValue(PropertyControl pc)
        {
            trackBar.Value = pc.GetPropertyValue();
            cbAuto.Checked = pc.GetAutoMode();
            ValueUpdate?.Invoke(this, new EventArgs());
        }
    }
}

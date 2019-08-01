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
    public partial class PropertyControl : UserControl
    {
        public EventHandler ValueUpdate;

        private object Property;
        private int defaultValue;
        private bool usb = false;

        public PropertyControl(object prop, bool none, bool autoSupport, bool manualSupport, bool auto, bool manual, int pMax, int pMin, int pValue, int pSteppingDelta, int defaultValueInit)
        {
            InitializeComponent();

            Property = prop;

            label.Text = this.ToString();

            defaultValue = defaultValueInit;

            label.Enabled = !none;
            cbAuto.Enabled = !none;
            trackBar.Enabled = !none;
            if (!none)
            {
                cbAuto.Checked = auto;
                cbAuto.Enabled = autoSupport;
                trackBar.Enabled = auto ? false : manual;
                trackBar.Minimum = pMin;
                trackBar.Maximum = pMax;
                trackBar.TickFrequency = pSteppingDelta;
                trackBar.Value = pValue;
                trackBar.MouseDown += new MouseEventHandler(ResetDefault);
            }
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

        public bool GetAutoMode()
        {
            return cbAuto.Checked;
        }

        public object GetProperty()
        {
            return Property;
        }

        public int GetValue()
        {
            return trackBar.Value;
        }

        public int GetPropertyValue()
        {
            return trackBar.Value;
        }

        public void SelectItem(bool state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    SelectItemImpl(state);
                }
                ));
            }
            else
            {
                SelectItemImpl(state);
            }
        }

        private void SelectItemImpl(bool state)
        {
            label.Font = new Font(label.Font, state ? FontStyle.Bold : FontStyle.Regular);
        }

        public void AutoMode(bool state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    AutoModeImpl(state);
                }
                ));
            }
            else
            {
                AutoModeImpl(state);
            }
        }

        private void AutoModeImpl(bool state)
        {
            if (!cbAuto.Enabled) return;
            cbAuto.Checked = state;
            cbAuto_CheckedChanged(cbAuto, new EventArgs());
            ValueUpdate?.Invoke(this, new EventArgs());
        }

        public void UpdateValue(int mul, int dir)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    UpdateValueImpl(mul, dir);
                }
                ));
            }
            else
            {
                UpdateValueImpl(mul, dir);
            }
        }

        private void UpdateValueImpl(int mul, int dir)
        {
            int value = trackBar.Value - trackBar.TickFrequency * mul * dir;
            if (value < trackBar.Minimum)
                trackBar.Value = trackBar.Minimum;
            else if (value > trackBar.Maximum)
                trackBar.Value = trackBar.Maximum;
            else
                trackBar.Value = value;
            trackBar_Scroll(trackBar, new EventArgs());
            ValueUpdate?.Invoke(this, new EventArgs());
        }

        private void SetValue(int newValue, bool auto)
        {
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

        private void ResetDefault(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left) return;
            trackBar.Value = defaultValue;
            SetValue(trackBar.Value, false);
            ValueUpdate?.Invoke(this, new EventArgs());
        }

        public void SyncValue(PropertyControlSave pc)
        {
            trackBar.Value = pc.GetValue();
            cbAuto.Checked = pc.GetAutoMode();
        }
    }
}

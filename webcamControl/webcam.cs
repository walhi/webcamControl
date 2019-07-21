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
using System.Collections.Generic;
using System.Management; // need to add System.Management to your project references.

namespace webcamControl
{
    public partial class webcam : UserControl
    {
        private DsDevice device;
        private IAMCameraControl pCameraControl;
        private IAMVideoProcAmp pVideoProcAmp;
        private int mode;
        public int countAll = 0;

        IniFile INI = new IniFile("config.ini");

        public webcam(DsDevice dev, int modeIn = 0)
        {
            object camDevice;
            Guid iid = typeof(IBaseFilter).GUID;
            mode = modeIn;
            device = dev;
            device.Mon.BindToObject(null, null, ref iid, out camDevice);
            IBaseFilter camFilter = camDevice as IBaseFilter;
            pCameraControl = camFilter as IAMCameraControl;
            pVideoProcAmp = camFilter as IAMVideoProcAmp;

            string path = device.DevicePath;

            InitializeComponent();

            gBox.Text = device.Name;

            if (mode != 0)
            {
                // Изменение размера Groupbox
                //verticalCorrect += saveButton.Location.Y + saveButton.Height - comboBox1.Location.Y;
                //gBox.Height -= verticalCorrect;
                //this.Height -= verticalCorrect;
                preset2Button.Visible = false;
                preset1Button.Visible = false;
                comboBox1.Visible = false;
                defaultButton.Visible = false;
                saveButton.Visible = false;
            }
            InitCameraControlPropertyItem2(VideoProcAmpProperty.Sharpness);
            InitCameraControlPropertyItem2(CameraControlProperty.Iris);
        }
        private void InitCameraControlPropertyItem2(/*Label lb, TrackBar tBar, CheckBox cbAuto, CheckBox cbAll, CheckBox cbUSB, */object prop)
        {
            Debug.WriteLine(prop.GetType());


        }
        private void cbFocus_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarFocus;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pCameraControl.Set(CameraControlProperty.Focus, tBar.Minimum, CameraControlFlags.Manual);
                pCameraControl.Set(CameraControlProperty.Focus, 0, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Focus, (tBar.Maximum - tBar.Value + tBar.Minimum), CameraControlFlags.Manual);
            }
        }

        private void tBarFocus_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pCameraControl.Set(CameraControlProperty.Focus, inverceValue, CameraControlFlags.Manual);
        }

        private void cbExposure_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarExposure;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pCameraControl.Set(CameraControlProperty.Exposure, tBar.Minimum, CameraControlFlags.Manual);
                pCameraControl.Set(CameraControlProperty.Exposure, 0, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Exposure, (tBar.Maximum - tBar.Value + tBar.Minimum), CameraControlFlags.Manual);
            }
        }

        private void tBarExposure_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pCameraControl.Set(CameraControlProperty.Exposure, inverceValue, CameraControlFlags.Manual);
        }

        private void cbIris_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarIris;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pCameraControl.Set(CameraControlProperty.Iris, tBar.Minimum, CameraControlFlags.Manual);
                pCameraControl.Set(CameraControlProperty.Iris, 0, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Iris, (tBar.Maximum - tBar.Value + tBar.Minimum), CameraControlFlags.Manual);
            }
        }

        private void tBarIris_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pCameraControl.Set(CameraControlProperty.Iris, inverceValue, CameraControlFlags.Manual);

        }

        private void cbZoom_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarZoom;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pCameraControl.Set(CameraControlProperty.Zoom, tBar.Minimum, CameraControlFlags.Manual);
                pCameraControl.Set(CameraControlProperty.Zoom, 0, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Zoom, (tBar.Maximum - tBar.Value + tBar.Minimum), CameraControlFlags.Manual);
            }
        }

        private void tBarZoom_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pCameraControl.Set(CameraControlProperty.Zoom, inverceValue, CameraControlFlags.Manual);

        }

        private void cbPan_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarPan;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pCameraControl.Set(CameraControlProperty.Pan, tBar.Minimum, CameraControlFlags.Manual);
                pCameraControl.Set(CameraControlProperty.Pan, 0, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Pan, (tBar.Maximum - tBar.Value + tBar.Minimum), CameraControlFlags.Manual);
            }
        }

        private void tBarPan_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pCameraControl.Set(CameraControlProperty.Pan, inverceValue, CameraControlFlags.Manual);
        }

        private void cbTilt_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarTilt;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pCameraControl.Set(CameraControlProperty.Tilt, tBar.Minimum, CameraControlFlags.Manual);
                pCameraControl.Set(CameraControlProperty.Tilt, 0, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Tilt, (tBar.Maximum - tBar.Value + tBar.Minimum), CameraControlFlags.Manual);
            }
        }

        private void tBarTilt_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pCameraControl.Set(CameraControlProperty.Tilt, inverceValue, CameraControlFlags.Manual);
        }

        private void cbRoll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarRoll;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pCameraControl.Set(CameraControlProperty.Roll, tBar.Minimum, CameraControlFlags.Manual);
                pCameraControl.Set(CameraControlProperty.Roll, 0, CameraControlFlags.Auto);
            }
            else
            {
                pCameraControl.Set(CameraControlProperty.Roll, (tBar.Maximum - tBar.Value + tBar.Minimum), CameraControlFlags.Manual);
            }
        }

        private void tBarRoll_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pCameraControl.Set(CameraControlProperty.Roll, inverceValue, CameraControlFlags.Manual);
        }

        private void cbBrightness_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarBrightness;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.Brightness, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.Brightness, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.Brightness, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarBrightness_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.Brightness, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void cbContrast_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarContrast;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.Contrast, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.Contrast, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.Contrast, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarContrast_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.Contrast, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void cbHue_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarHue;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.Hue, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.Hue, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.Hue, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarHue_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.Hue, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void cbSaturation_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarSaturation;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.Saturation, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.Saturation, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.Saturation, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarSaturation_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.Saturation, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void cbSharpness_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarSharpness;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.Sharpness, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.Sharpness, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.Sharpness, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarSharpness_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.Sharpness, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void cbGamma_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarGamma;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.Gamma, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.Gamma, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.Gamma, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarGamma_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.Gamma, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void cbColorEnable_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarColorEnable;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.ColorEnable, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.ColorEnable, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.ColorEnable, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarColorEnable_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.ColorEnable, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void cbWhiteBalance_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarWhiteBalance;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.WhiteBalance, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.WhiteBalance, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.WhiteBalance, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarWhiteBalance_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.WhiteBalance, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void cbBacklightCompensation_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarBacklightCompensation;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.BacklightCompensation, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.BacklightCompensation, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.BacklightCompensation, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarBacklightCompensation_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.BacklightCompensation, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void cbGain_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TrackBar tBar = tBarGain;
            tBar.Enabled = !cb.Checked;
            if (cb.Checked)
            {
                // todo
                pVideoProcAmp.Set(VideoProcAmpProperty.Gain, tBar.Minimum, VideoProcAmpFlags.Manual);
                pVideoProcAmp.Set(VideoProcAmpProperty.Gain, 0, VideoProcAmpFlags.Auto);
            }
            else
            {
                pVideoProcAmp.Set(VideoProcAmpProperty.Gain, (tBar.Maximum - tBar.Value + tBar.Minimum), VideoProcAmpFlags.Manual);
            }
        }

        private void tBarGain_Scroll(object sender, EventArgs e)
        {
            TrackBar tBar = (TrackBar)sender;
            int Value = tBar.Value - tBar.Value % tBar.TickFrequency;
            tBar.Value = Value;
            int inverceValue = tBar.Maximum - Value + tBar.Minimum;
            pVideoProcAmp.Set(VideoProcAmpProperty.Gain, inverceValue, VideoProcAmpFlags.Manual);
        }

        private void defaultButton_Click(object sender, EventArgs e)
        {
            SetDefaultCameraControlPropertyItem(lbFocus, tBarFocus, cbFocus, CameraControlProperty.Focus);
            SetDefaultCameraControlPropertyItem(lbExposure, tBarExposure, cbExposure, CameraControlProperty.Exposure);
            SetDefaultCameraControlPropertyItem(lbIris, tBarIris, cbIris, CameraControlProperty.Iris);
            SetDefaultCameraControlPropertyItem(lbZoom, tBarZoom, cbZoom, CameraControlProperty.Zoom);
            SetDefaultCameraControlPropertyItem(lbPan, tBarPan, cbPan, CameraControlProperty.Pan);
            SetDefaultCameraControlPropertyItem(lbTilt, tBarTilt, cbTilt, CameraControlProperty.Tilt);
            SetDefaultCameraControlPropertyItem(lbRoll, tBarRoll, cbRoll, CameraControlProperty.Roll);

            SetDefaultVideoProcAmpItem(lbBrightness, tBarBrightness, cbBrightness, VideoProcAmpProperty.Brightness);
            SetDefaultVideoProcAmpItem(lbContrast, tBarContrast, cbContrast, VideoProcAmpProperty.Contrast);
            SetDefaultVideoProcAmpItem(lbHue, tBarHue, cbHue, VideoProcAmpProperty.Hue);
            SetDefaultVideoProcAmpItem(lbSaturation, tBarSaturation, cbSaturation, VideoProcAmpProperty.Saturation);
            SetDefaultVideoProcAmpItem(lbSharpness, tBarSharpness, cbSharpness, VideoProcAmpProperty.Sharpness);
            SetDefaultVideoProcAmpItem(lbGamma, tBarGamma, cbGamma, VideoProcAmpProperty.Gamma);
            SetDefaultVideoProcAmpItem(lbColorEnable, tBarColorEnable, cbColorEnable, VideoProcAmpProperty.ColorEnable);
            SetDefaultVideoProcAmpItem(lbWhiteBalance, tBarWhiteBalance, cbWhiteBalance, VideoProcAmpProperty.WhiteBalance);
            SetDefaultVideoProcAmpItem(lbBacklightCompensation, tBarBacklightCompensation, cbBacklightCompensation, VideoProcAmpProperty.BacklightCompensation);
            SetDefaultVideoProcAmpItem(lbGain, tBarGain, cbGain, VideoProcAmpProperty.Gain);

        }
        private void SetDefaultCameraControlPropertyItem(Label lb, TrackBar tBar, CheckBox cb, CameraControlProperty prop)
        {
            if (pCameraControl == null) return;
            int pMax, pMin, pValue, pDefault, pSteppingDelta;
            CameraControlFlags cameraFlags;
            pCameraControl.GetRange(prop, out pMin, out pMax, out pSteppingDelta, out pDefault, out cameraFlags);
            if (cameraFlags == CameraControlFlags.None) return;
            cb.Checked = (cameraFlags == CameraControlFlags.Auto);
            tBar.Enabled = !(cameraFlags == CameraControlFlags.Auto);
            tBar.Value = pMax - pDefault + pMin;
            pCameraControl.Set(prop, pDefault, (cameraFlags == CameraControlFlags.Auto)?CameraControlFlags.Auto:CameraControlFlags.Manual);
        }
        private void SetDefaultVideoProcAmpItem(Label lb, TrackBar tBar, CheckBox cb, VideoProcAmpProperty prop)
        {
            if (pVideoProcAmp == null) return;
            int pMax, pMin, pValue, pDefault, pSteppingDelta;
            VideoProcAmpFlags cameraFlags;
            pVideoProcAmp.GetRange(prop, out pMin, out pMax, out pSteppingDelta, out pDefault, out cameraFlags);
            if (cameraFlags == VideoProcAmpFlags.None) return;
            cb.Checked = (cameraFlags == VideoProcAmpFlags.Auto);
            tBar.Enabled = !(cameraFlags == VideoProcAmpFlags.Auto);
            tBar.Value = pMax - pDefault + pMin;
            pVideoProcAmp.Set(prop, pDefault, (cameraFlags == VideoProcAmpFlags.Auto) ? VideoProcAmpFlags.Auto : VideoProcAmpFlags.Manual);
        }

        private void lbFocus_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultCameraControlPropertyItem(lbFocus, tBarFocus, cbFocus, CameraControlProperty.Focus);
        }

        private void lbExposure_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultCameraControlPropertyItem(lbExposure, tBarExposure, cbExposure, CameraControlProperty.Exposure);
        }

        private void lbIris_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultCameraControlPropertyItem(lbIris, tBarIris, cbIris, CameraControlProperty.Iris);
        }

        private void lbZoom_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultCameraControlPropertyItem(lbZoom, tBarZoom, cbZoom, CameraControlProperty.Zoom);
        }

        private void lbPan_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultCameraControlPropertyItem(lbPan, tBarPan, cbPan, CameraControlProperty.Pan);
        }

        private void lbTilt_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultCameraControlPropertyItem(lbTilt, tBarTilt, cbTilt, CameraControlProperty.Tilt);
        }

        private void lbRoll_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultCameraControlPropertyItem(lbRoll, tBarRoll, cbRoll, CameraControlProperty.Roll);
        }

        private void lbBrightness_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbBrightness, tBarBrightness, cbBrightness, VideoProcAmpProperty.Brightness);
        }

        private void lbContrast_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbContrast, tBarContrast, cbContrast, VideoProcAmpProperty.Contrast);
        }

        private void lbHue_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbHue, tBarHue, cbHue, VideoProcAmpProperty.Hue);
        }

        private void lbSaturation_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbSaturation, tBarSaturation, cbSaturation, VideoProcAmpProperty.Saturation);
        }

        private void lbSharpness_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbSharpness, tBarSharpness, cbSharpness, VideoProcAmpProperty.Sharpness);
        }

        private void lbGamma_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbGamma, tBarGamma, cbGamma, VideoProcAmpProperty.Gamma);
        }

        private void lbColorEnable_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbColorEnable, tBarColorEnable, cbColorEnable, VideoProcAmpProperty.ColorEnable);
        }

        private void lbWhiteBalance_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbWhiteBalance, tBarWhiteBalance, cbWhiteBalance, VideoProcAmpProperty.WhiteBalance);
        }

        private void lbBacklightCompensation_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbBacklightCompensation, tBarBacklightCompensation, cbBacklightCompensation, VideoProcAmpProperty.BacklightCompensation);
        }

        private void lbGain_DoubleClick(object sender, EventArgs e)
        {
            if ((sender is TrackBar) && (((MouseEventArgs)e).Button == MouseButtons.Left)) return;
            SetDefaultVideoProcAmpItem(lbGain, tBarGain, cbGain, VideoProcAmpProperty.Gain);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Отображение на главной
            INI.Write(device.DevicePath, "All_" + CameraControlProperty.Focus.ToString(), cbFocusAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + CameraControlProperty.Exposure.ToString(), cbExposureAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + CameraControlProperty.Iris.ToString(), cbIrisAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + CameraControlProperty.Zoom.ToString(), cbZoomAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + CameraControlProperty.Pan.ToString(), cbPanAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + CameraControlProperty.Tilt.ToString(), cbTiltAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + CameraControlProperty.Roll.ToString(), cbRollAll.Checked.ToString());

            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.Brightness.ToString(), cbBrightnessAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.Contrast.ToString(), cbContrastAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.Hue.ToString(), cbHueAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.Saturation.ToString(), cbSaturationAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.Sharpness.ToString(), cbSharpnessAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.Gamma.ToString(), cbGammaAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.ColorEnable.ToString(), cbColorEnableAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.WhiteBalance.ToString(), cbWhiteBalanceAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.BacklightCompensation.ToString(), cbBacklightCompensationAll.Checked.ToString());
            INI.Write(device.DevicePath, "All_" + VideoProcAmpProperty.Gain.ToString(), cbGainAll.Checked.ToString());

            // Управление аппаратной крутилкой
            INI.Write(device.DevicePath, "USB_" + CameraControlProperty.Focus.ToString(), cbFocusUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + CameraControlProperty.Exposure.ToString(), cbExposureUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + CameraControlProperty.Iris.ToString(), cbIrisUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + CameraControlProperty.Zoom.ToString(), cbZoomUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + CameraControlProperty.Pan.ToString(), cbPanUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + CameraControlProperty.Tilt.ToString(), cbTiltUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + CameraControlProperty.Roll.ToString(), cbRollUSB.Checked.ToString());

            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.Brightness.ToString(), cbBrightnessUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.Contrast.ToString(), cbContrastUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.Hue.ToString(), cbHueUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.Saturation.ToString(), cbSaturationUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.Sharpness.ToString(), cbSharpnessUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.Gamma.ToString(), cbGammaUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.ColorEnable.ToString(), cbColorEnableUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.WhiteBalance.ToString(), cbWhiteBalanceUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.BacklightCompensation.ToString(), cbBacklightCompensationUSB.Checked.ToString());
            INI.Write(device.DevicePath, "USB_" + VideoProcAmpProperty.Gain.ToString(), cbGainUSB.Checked.ToString());
        }

        private void preset1Button_Click(object sender, EventArgs e)
        {
        }

        private void preset2Button_Click(object sender, EventArgs e)
        {
        }
    }
}

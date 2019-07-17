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
    public partial class webcam : UserControl
    {
        private DsDevice device;
        private IAMCameraControl pCameraControl;
        private IAMVideoProcAmp pVideoProcAmp;
        private int verticalCorrect = 0;
        //private int verticalCorrectTick = 23;

        public webcam(DsDevice dev)
        {
            object camDevice;
            Guid iid = typeof(IBaseFilter).GUID;


            device = dev;
            device.Mon.BindToObject(null, null, ref iid, out camDevice);
            IBaseFilter camFilter = camDevice as IBaseFilter;
            pCameraControl = camFilter as IAMCameraControl;
            pVideoProcAmp = camFilter as IAMVideoProcAmp;


            InitializeComponent();

            gBox.Text = device.Name;

            Debug.WriteLine(device.Name);

            if (pCameraControl != null)
            {
                InitCameraControlPropertyItem(lbFocus, tBarFocus, cbFocus, CameraControlProperty.Focus);
                InitCameraControlPropertyItem(lbExposure, tBarExposure, cbExposure, CameraControlProperty.Exposure);
                InitCameraControlPropertyItem(lbIris, tBarIris, cbIris, CameraControlProperty.Iris);
                InitCameraControlPropertyItem(lbZoom, tBarZoom, cbZoom, CameraControlProperty.Zoom);
                InitCameraControlPropertyItem(lbPan, tBarPan, cbPan, CameraControlProperty.Pan);
                InitCameraControlPropertyItem(lbTilt, tBarTilt, cbTilt, CameraControlProperty.Tilt);
                InitCameraControlPropertyItem(lbRoll, tBarRoll, cbRoll, CameraControlProperty.Roll);
            }

            if (pVideoProcAmp != null)
            {
                InitVideoProcAmpItem(lbBrightness, tBarBrightness, cbBrightness, VideoProcAmpProperty.Brightness);
                InitVideoProcAmpItem(lbContrast, tBarContrast, cbContrast, VideoProcAmpProperty.Contrast);
                InitVideoProcAmpItem(lbHue, tBarHue, cbHue, VideoProcAmpProperty.Hue);
                InitVideoProcAmpItem(lbSaturation, tBarSaturation, cbSaturation, VideoProcAmpProperty.Saturation);
                InitVideoProcAmpItem(lbSharpness, tBarSharpness, cbSharpness, VideoProcAmpProperty.Sharpness);
                InitVideoProcAmpItem(lbGamma, tBarGamma, cbGamma, VideoProcAmpProperty.Gamma);
                InitVideoProcAmpItem(lbColorEnable, tBarColorEnable, cbColorEnable, VideoProcAmpProperty.ColorEnable);
                InitVideoProcAmpItem(lbWhiteBalance, tBarWhiteBalance, cbWhiteBalance, VideoProcAmpProperty.WhiteBalance);
                InitVideoProcAmpItem(lbBacklightCompensation, tBarBacklightCompensation, cbBacklightCompensation, VideoProcAmpProperty.BacklightCompensation);
                InitVideoProcAmpItem(lbGain, tBarGain, cbGain, VideoProcAmpProperty.Gain);
            }


            // Изменение размера Groupbox
            defaultButton.Location = new Point(defaultButton.Location.X, defaultButton.Location.Y - verticalCorrect);
            gBox.Height -= verticalCorrect;
        }

        private void InitCameraControlPropertyItem(Label lb, TrackBar tBar, CheckBox cb, CameraControlProperty prop)
        {
            int pMax, pMin, pValue, pDefault, pSteppingDelta;
            CameraControlFlags cameraFlags;
            pCameraControl.GetRange(prop, out pMin, out pMax, out pSteppingDelta, out pDefault, out cameraFlags);
            Debug.WriteLine("{0} {1},{2},{3},{4},{5}", prop, pMin, pMax, pSteppingDelta, pDefault, cameraFlags);
            if (cameraFlags == CameraControlFlags.None)
            {
                //lb.Visible = false;
                //cb.Visible = false;
                //tBar.Visible = false;
                //verticalCorrect += verticalCorrectTick;
                lb.Enabled = false;
                cb.Enabled = false;
                tBar.Enabled = false;
            }
            else
            {
                CameraControlFlags currentFlags;
                pCameraControl.Get(prop, out pValue, out currentFlags);
                Debug.WriteLine("Current value: {0},{1}", pValue, currentFlags);
                lb.Enabled = true;
                cb.Checked = (currentFlags == CameraControlFlags.Auto);
                cb.Enabled = !(cameraFlags == CameraControlFlags.Manual);
                tBar.Enabled = (currentFlags == CameraControlFlags.Manual);
                tBar.Minimum = pMin;
                tBar.Maximum = pMax;
                tBar.TickFrequency = pSteppingDelta;
                //tBarFocus.Value = pValue;
                tBar.Value = pMax - pValue + pMin;

                //lb.Location = new Point(lb.Location.X, lb.Location.Y - verticalCorrect);
                //cb.Location = new Point(cb.Location.X, cb.Location.Y - verticalCorrect);
                //tBar.Location = new Point(tBar.Location.X, tBar.Location.Y - verticalCorrect);
            }
        }
        private void InitVideoProcAmpItem(Label lb, TrackBar tBar, CheckBox cb, VideoProcAmpProperty prop)
        {
            int pMax, pMin, pValue, pDefault, pSteppingDelta;
            VideoProcAmpFlags cameraFlags;
            pVideoProcAmp.GetRange(prop, out pMin, out pMax, out pSteppingDelta, out pDefault, out cameraFlags);
            Debug.WriteLine("{0} {1},{2},{3},{4},{5}", prop, pMin, pMax, pSteppingDelta, pDefault, cameraFlags);
            if (cameraFlags == VideoProcAmpFlags.None)
            {
                //lb.Visible = false;
                //cb.Visible = false;
                //tBar.Visible = false;
                //verticalCorrect += verticalCorrectTick;
                lb.Enabled = false;
                cb.Enabled = false;
                tBar.Enabled = false;
            }
            else
            {
                VideoProcAmpFlags currentFlags;
                pVideoProcAmp.Get(prop, out pValue, out currentFlags);
                Debug.WriteLine("Current value: {0},{1}", pValue, currentFlags);
                lb.Enabled = true;
                cb.Checked = (currentFlags == VideoProcAmpFlags.Auto);
                cb.Enabled = !(cameraFlags == VideoProcAmpFlags.Manual);
                tBar.Enabled = (currentFlags == VideoProcAmpFlags.Manual);
                tBar.Minimum = pMin;
                tBar.Maximum = pMax;
                tBar.TickFrequency = pSteppingDelta;
                //tBarFocus.Value = pValue;
                tBar.Value = pMax - pValue + pMin;

                //lb.Location = new Point(lb.Location.X, lb.Location.Y - verticalCorrect);
                //cb.Location = new Point(cb.Location.X, cb.Location.Y - verticalCorrect);
                //tBar.Location = new Point(tBar.Location.X, tBar.Location.Y - verticalCorrect);
            }
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
            if (pCameraControl != null)
            {
                SetDefaultCameraControlPropertyItem(lbFocus, tBarFocus, cbFocus, CameraControlProperty.Focus);
                SetDefaultCameraControlPropertyItem(lbExposure, tBarExposure, cbExposure, CameraControlProperty.Exposure);
                SetDefaultCameraControlPropertyItem(lbIris, tBarIris, cbIris, CameraControlProperty.Iris);
                SetDefaultCameraControlPropertyItem(lbZoom, tBarZoom, cbZoom, CameraControlProperty.Zoom);
                SetDefaultCameraControlPropertyItem(lbPan, tBarPan, cbPan, CameraControlProperty.Pan);
                SetDefaultCameraControlPropertyItem(lbTilt, tBarTilt, cbTilt, CameraControlProperty.Tilt);
                SetDefaultCameraControlPropertyItem(lbRoll, tBarRoll, cbRoll, CameraControlProperty.Roll);
            }

            if (pVideoProcAmp != null)
            {
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

        }
        private void SetDefaultCameraControlPropertyItem(Label lb, TrackBar tBar, CheckBox cb, CameraControlProperty prop)
        {
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
            int pMax, pMin, pValue, pDefault, pSteppingDelta;
            VideoProcAmpFlags cameraFlags;
            pVideoProcAmp.GetRange(prop, out pMin, out pMax, out pSteppingDelta, out pDefault, out cameraFlags);
            if (cameraFlags == VideoProcAmpFlags.None) return;
            cb.Checked = (cameraFlags == VideoProcAmpFlags.Auto);
            tBar.Enabled = !(cameraFlags == VideoProcAmpFlags.Auto);
            tBar.Value = pMax - pDefault + pMin;
            pVideoProcAmp.Set(prop, pDefault, (cameraFlags == VideoProcAmpFlags.Auto) ? VideoProcAmpFlags.Auto : VideoProcAmpFlags.Manual);
        }
    }
}

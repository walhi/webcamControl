using DirectShowLib;
using HidLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace webcamControl
{
    public partial class Form3 : Form
    {
        private IEnumerable<HidDevice> list;
        private const int VendorId = 0x0483;
        private const int ProductId = 0x5750;

        public Form3()
        {
            InitializeComponent();

            list = HidDevices.Enumerate(VendorId, ProductId);
            foreach (HidDevice x in list)
            {
                byte[] sb_buf;
                x.OpenDevice();
                
                Debug.WriteLine(x.DevicePath);
                x.ReadSerialNumber(out sb_buf);
                String sn = Encoding.Unicode.GetString(sb_buf);
                object item = new object();
                comboBox1.Items.Add(sn);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            HidReport report = new HidReport(1);
            byte[] data = new byte[1];
            report.ReportId = 3;
            if (cb.Checked)
            {
                Debug.WriteLine("en");
                data[0] = 7;
            }
            else
            {
                Debug.WriteLine("dis");
                data[0] = 0;
            }
            report.Data = data;
            //if (device != null)
            //    device.WriteReportSync(report);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selected = comboBox.Items[comboBox.SelectedIndex].ToString();
            Debug.WriteLine(selected);

            HidReport report = new HidReport(1);
            byte[] data = new byte[1];
            report.ReportId = 3;

            foreach (HidDevice x in list)
            {
                byte[] sb_buf;
                x.OpenDevice();
                x.ReadSerialNumber(out sb_buf);
                String sn = Encoding.Unicode.GetString(sb_buf);
                Debug.WriteLine(sn);
                if (selected.Equals(sn))
                {
                    Debug.WriteLine("true");
                    data[0] = 7;
                }
                else
                {
                    Debug.WriteLine("false");
                    data[0] = 0;
                }
                report.Data = data;
                if (x.IsOpen)
                {
                    Debug.WriteLine("open");
                    x.WriteReportSync(report);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkBox2.Dispose();
            Debug.WriteLine(CameraControlProperty.Exposure);
        }
    }
}

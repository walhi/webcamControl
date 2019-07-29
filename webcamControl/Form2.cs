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
using DirectShowLib;

namespace webcamControl
{
    public partial class Form2 : Form
    {
        private TableLayoutPanel main;

        public Form2()
        {
            InitializeComponent();

            //if (_USBControl == null) _USBControl = new USBControl();

            Guid iid = typeof(IBaseFilter).GUID;
            DsDevice[] capDev;
            capDev = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            main = new TableLayoutPanel
            {
                AutoSize = true, 
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Dock = DockStyle.Top
            };
            AllPage.Controls.Add(main);

            foreach (DsDevice dev in capDev)
            {
                dev.Mon.BindToObject(null, null, ref iid, out object camDevice);
                IBaseFilter camFilter = camDevice as IBaseFilter;
                if (camFilter is IAMCameraControl pCameraControl)
                {
                    TabPage tabPage = new TabPage();
                    tabControl.Controls.Add(tabPage);

                    AllProperties aProp = new AllProperties(dev)
                    {
                        Dock = DockStyle.Top,
                    };
                    aProp.CreateSelectedProperties += new EventHandler(CreateSelectedProperties);
                    tabPage.Text = aProp.GetWebcamName();
                    tabPage.Controls.Add(aProp);


                    if (aProp.CountFavorites > 0)
                    {
                        SelectedProperties sProp = new SelectedProperties(dev, aProp)
                        {
                            Dock = DockStyle.Top
                        };
                        main.Controls.Add(sProp);
                        aProp.SelectedPropertiesVar = sProp;
                        aProp.AddPropertyUpdateHandler();
                    }
                }

            }

            tabControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;


        }


        private void CreateSelectedProperties(object sender, EventArgs e)
        {
            AllProperties x = (AllProperties)sender;
            SelectedProperties gb = new SelectedProperties(x.GetDevice(), x);
            main.Controls.Add(gb);
            gb.Dock = DockStyle.Top;
            x.SelectedPropertiesVar = gb;
            x.AddPropertyUpdateHandler();
        }


    }
}

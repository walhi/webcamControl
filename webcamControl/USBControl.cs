using DirectShowLib;
using HidLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace webcamControl
{
    class USBControl : IDisposable
    {
        // used for monitoring plugging and unplugging of USB devices.
        private ManagementEventWatcher watcherAttach;
        private ManagementEventWatcher watcherDetach;

        public EventHandler HIDConnected;
        public EventHandler HIDDisconnected;
        public EventHandler DShowConnected;
        public EventHandler DShowDisconnected;

        private const int VendorId = 0x0483;
        private const int ProductId = 0x5750;

        private int countDShow = 0;
        private int countHID = 0;

        private DsDevice[] list;

        public USBControl()
        {
            countDShow = GetCountDShow();
            countHID = GetCountHID();
            // Add USB plugged event watching
            watcherAttach = new ManagementEventWatcher();
            watcherAttach.EventArrived += UpdateUSBDevices;
            watcherAttach.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            watcherAttach.Start();

            // Add USB unplugged event watching
            watcherDetach = new ManagementEventWatcher();
            watcherDetach.EventArrived += UpdateUSBDevices;
            watcherDetach.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            watcherDetach.Start();

            list = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
        }

        public void Dispose()
        {
            watcherAttach.Stop();
            watcherDetach.Stop();
            //you may want to yield or Thread.Sleep
            watcherAttach.Dispose();
            watcherDetach.Dispose();
            //you may want to yield or Thread.Sleep
        }

        private int GetCountDShow()
        {
            return DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice).Length;
        }

        private int GetCountHID()
        {
            int count = 0;
            foreach (var x in HidDevices.Enumerate(VendorId, ProductId))
                count++;
            return count;
        }

        private void UpdateUSBDevices(object sender, EventArrivedEventArgs e)
        {
            int currentCountDShow = GetCountDShow();
            int currentCountHID = GetCountHID();

            if (currentCountDShow != countDShow)
            {
                Debug.WriteLine("CountDShow {0}, {1}", currentCountDShow, countDShow);
                DsDevice[] currentList = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
                if (currentCountDShow < countDShow)
                    foreach (DsDevice dev in list)
                    {
                        bool flag = true;
                        foreach (DsDevice dev2 in currentList)
                        {
                            if (dev.DevicePath.Equals(dev2.DevicePath))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            Debug.WriteLine("Disconnect " + dev.Name);
                            DShowDisconnected?.Invoke(dev, new EventArgs());
                        }
                    }

                if (currentCountDShow > countDShow)
                    foreach (DsDevice dev in currentList)
                    {
                        bool flag = true;
                        foreach (DsDevice dev2 in list)
                        {
                            if (dev.DevicePath.Equals(dev2.DevicePath))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            Debug.WriteLine("Connect " + dev.Name);
                            DShowConnected?.Invoke(dev, new EventArgs());
                        }
                    }
                list = currentList;
                countDShow = currentCountDShow;
            }
            if (currentCountHID != countHID)
            {
                Debug.WriteLine("currentCountHID {0}, {1}", currentCountHID, countHID);
                if (currentCountHID < countHID) HIDDisconnected?.Invoke(this, new EventArgs());
                if (currentCountHID > countHID) HIDConnected?.Invoke(this, new EventArgs());
                countHID = currentCountHID;
            }
        }

        ~USBControl()
        {
            this.Dispose();// for ease of readability I left out the complete Dispose pattern
        }
    }
}

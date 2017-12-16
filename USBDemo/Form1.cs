using System;
using System.Threading;
using System.Windows.Forms;
using USB;
using USB.DeviceNotify;

namespace USBDemo
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 热插拔消息通知
        /// </summary>
        public IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
        public Form1()
        {
            InitializeComponent();
            UsbDeviceNotifier.OnDeviceNotify += UsbDeviceNotifier_OnDeviceNotify;
            InitUSB();
        }
        private void UsbDeviceNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            if (e.EventType == EventType.DeviceArrival)
            {
                Console.WriteLine("usb DeviceArrival");
                new Thread(() => { Thread.Sleep(500); InitUSB(); }).Start();
            }
            else if (e.EventType == EventType.DeviceRemoveComplete)
            {
                Console.WriteLine("usb Remove");
                CloseUSB();
            }
        }
        private  USBDevice _usbDevice = null;
        private  int vid = 0x0012, pid = 0x0023;
        private  void InitUSB()
        {
            if (_usbDevice == null)
            {
                _usbDevice = new USBDevice(vid, pid);
                _usbDevice.OnUsbDataReceived += _usbDevice_OnUsbDataReceived;
            }
            if (_usbDevice != null && !_usbDevice.IsOpen)
            {
                _usbDevice.Open();
            }
        }

        private  void CloseUSB()
        {
            if (_usbDevice != null && !_usbDevice.IsOpen)
            {
                _usbDevice.OnUsbDataReceived -= _usbDevice_OnUsbDataReceived;
                _usbDevice.Close();

            }
            _usbDevice = null;
        }
        private  void _usbDevice_OnUsbDataReceived(byte[] data)
        {
            //数据解析
            Console.WriteLine(BitConverter.ToString(data));
        }
    }
}

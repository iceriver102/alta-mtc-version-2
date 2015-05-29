using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Alta.Class
{
    public static class SerialPortExtension
    {
        public static void sendCommand(this SerialPort serialPort, string cmd)
        {
            if (serialPort == null)
                return;
            if (serialPort.IsOpen)
            {
                serialPort.Write(cmd);
                serialPort.Write("\r\n");
            }
        }
        public static void openCOM(this SerialPort s, string nameCOM, int baurate, int databits, StopBits @StopBits = StopBits.One)
        {
            try
            {
                s.PortName = nameCOM;
                s.BaudRate = baurate;
                s.DataBits = databits;
                s.StopBits = StopBits;
                s.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

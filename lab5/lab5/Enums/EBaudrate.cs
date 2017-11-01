using System.ComponentModel;

namespace lab4.Enums
{
    public enum EBaudrate
    {
        [Description("4800")]
        Baudrate4800 = 4800,
        [Description("9600")]
        Baudrate9600 = 9600,
        [Description("14400")]
        Baudrate14400 = 14400,
        [Description("19200")]
        Baudrate19200 = 19200,
        [Description("38400")]
        Baudrate38400 = 38400,
        [Description("57600")]
        Baudrate57600 = 57600,
        [Description("115200")]
        Baudrate115200 = 115200
    }
}
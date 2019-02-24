using System;
using CPUInfoDLL;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;

namespace FanManager_Rev.Center2
{
    class FanManager : ApplicationContext
    {
        string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        const string userRoot = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Rev.Center";
        const string keyName = userRoot + "\\" + "Rev.Center2.0";

        private RegistryKey Regedit = Registry.CurrentUser.CreateSubKey("Rev.Center2.0");
        private int last_set = -1;
        private int new_set;
        private uint static_fan;
        private int temp;
        private CPUInfo CpuInfo; 
        private uint[] array_fan = new uint[7];
        private int[] array_temp = new int[7];
        private DispatcherTimer Timer_Fan; //Timer_ Fan will check when the temperature change and will regulate it.
        private DispatcherTimer Timer1_Fan;

        public FanManager()
        {
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            InitializeComponent();
            new_set = Convert.ToInt32(Registry.GetValue(keyName, "FanMode", 1));
           
            static_fan = Convert.ToUInt32(Registry.GetValue(keyName, "StaticFan", 0));
            this.CpuInfo = new CPUInfo();
            switch (new_set)
            {
                case 1:
                    Setfan_Auto(0UL);
                    Application.Exit();
                    break;
                case 2:
                    Setfan_Max(64UL);
                    Application.Exit();
                    break;
                case 4:
                    Setfan_Static(static_fan);
                    Application.Exit();
                    break;
                case 3:
                    Timer_Fan = new DispatcherTimer();
                    Timer_Fan.Tick += timer1_Tick;
                    Timer_Fan.Start();
                    Timer_Fan.Interval = new TimeSpan(0, 0, 0, 1, 500);
                    break;
                case 5:
                    for (int i = 0; i < 7; i++)
                    {
                        array_fan[i] = Convert.ToUInt32(Registry.GetValue(keyName + "\\FAN", "Fan_" + i.ToString(), -1));
                        array_temp[i] = Convert.ToInt32(Registry.GetValue(keyName + "\\FAN", "Temp_" + i.ToString(), -1));
                    }
                    if (array_temp[0] == -1)
                    {
                        Setfan_Auto(0UL);
                        Application.Exit();
                    }
                    Timer1_Fan = new DispatcherTimer();
                    Timer1_Fan.Tick += timer1_fancustom_Tick;
                    Timer1_Fan.Start();
                    Timer1_Fan.Interval = new TimeSpan(0, 0, 0, 1, 500);
                    break;
            }
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            //Here you can do stuff if the tray icon is doubleclicked
        }

        private void InitializeComponent()
        {

        }
        //TIMER FOR SILENT MODE
        private void timer1_Tick(object sender, EventArgs e)
        {
            int temp = (int)CpuInfo.GetCPUTemerature();
            if (temp <= 50)
            {
                if (last_set != 0)
                {
                    WMIEC.WMIWriteECRAM(1873UL, 128UL + 0U);
                    last_set = 0;
                }
                return;
            }
            else if (temp <= 60)
            {
                if (last_set != 1)
                {
                    WMIEC.WMIWriteECRAM(1873UL, 128UL + 1U);
                    last_set = 1;
                }
                return;
            }
            else if (temp <= 65)
            {
                if (last_set != 2)
                {
                    WMIEC.WMIWriteECRAM(1873UL, 128UL + 2U);
                    last_set = 2;
                }
                return;
            }
            else if (temp <= 75)
            {
                if (last_set != 3)
                {
                    WMIEC.WMIWriteECRAM(1873UL, 128UL + 4U);
                    last_set = 3;
                }
                return;
            }
            else if (temp > 75)
            {
                if (last_set != 4)
                {
                    WMIEC.WMIWriteECRAM(1873UL, 0UL);
                    last_set = 4;
                }
                return;
            }
        }

        //TIMER FOR CUSTOM FAN MODE
        private void timer1_fancustom_Tick(object sender, EventArgs e)
        {
            int temp = (int)CpuInfo.GetCPUTemerature();
            if (temp <= array_temp[0])
            {
                if (last_set != 0)
                {
                    if ((int)array_fan[0] != 8)
                        WMIEC.WMIWriteECRAM(1873UL, 128UL + array_fan[0]);
                    else WMIEC.WMIWriteECRAM(1873UL, 64UL);
                    last_set = 0;
                }
                return;
            }

            else if (temp <= array_temp[1])
            {
                if (last_set != 1)
                {
                    if ((int)array_fan[1] != 8)
                        WMIEC.WMIWriteECRAM(1873UL, 128UL + array_fan[1]);
                    else WMIEC.WMIWriteECRAM(1873UL, 64UL);
                    last_set = 1;

                }
                return;
            }

            else if (temp <= array_temp[2])
            {
                if (last_set != 2)
                {
                    if ((int)array_fan[2] != 8)
                        WMIEC.WMIWriteECRAM(1873UL, 128UL + array_fan[2]);
                    else WMIEC.WMIWriteECRAM(1873UL, 64UL);
                    last_set = 2;
                }
                return;
            }

            else if (temp <= array_temp[3])
            {
                if (last_set != 3)
                {
                    if ((int)array_fan[3] != 8)
                        WMIEC.WMIWriteECRAM(1873UL, 128UL + array_fan[3]);
                    else WMIEC.WMIWriteECRAM(1873UL, 64UL);
                    last_set = 3;
                }
                return;
            }

            else if (temp <= array_temp[4])
            {
                if (last_set != 4)
                {
                    if ((int)array_fan[4] != 8)
                        WMIEC.WMIWriteECRAM(1873UL, 128UL + array_fan[4]);
                    else WMIEC.WMIWriteECRAM(1873UL, 64UL);
                    last_set = 4;
                }
                return;
            }

            else if (temp <= array_temp[5])
            {
                if (last_set != 5)
                {
                    if ((int)array_fan[5] != 8)
                        WMIEC.WMIWriteECRAM(1873UL, 128UL + array_fan[5]);
                    else WMIEC.WMIWriteECRAM(1873UL, 64UL);
                    last_set = 5;
                }
                return;
            }

            else if (temp <= array_temp[6])
            {
                if (last_set != 6)
                {
                    if ((int)array_fan[6] != 8)
                        WMIEC.WMIWriteECRAM(1873UL, 128UL + array_fan[6]);
                    else WMIEC.WMIWriteECRAM(1873UL, 64UL);
                    last_set = 6;
                }
                return;
            }




            /*
            for (int i = 0; i < 8; i++)
            {
                if (temp <= array_temp[i] && last_set != i)
                    {
                        if ((int)array_fan[i] != 8)
                            WMIEC.WMIWriteECRAM(1873UL, 128UL + array_fan[i]);
                        else WMIEC.WMIWriteECRAM(1873UL, 64UL);
                        last_set = i;
                    }
                break;


            }*/
        }

        public void Setfan_Max(ulong value)
        {
            if (static_fan != 9)
            {
                WMIEC.WMIWriteECRAM(1873UL, value);
                static_fan = 9;
                last_set = -1;
            }
        }

        private void Setfan_Auto(ulong value)
        {
            WMIEC.WMIWriteECRAM(1873UL, value);

        }

        private void Setfan_Static(uint value)
        {
            WMIEC.WMIWriteECRAM(1873UL, 128UL + value);

        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FanManager_Rev.Center2
{


    static public class WMIEC
    {
        static private ManagementEventWatcher watcher = null;
        static public bool WMIReadECRAM(UInt64 Addr, ref object data)
        {
            try
            {
                ManagementObject classInstance =
                    new ManagementObject("root\\WMI",
                    "AcpiTest_MULong.InstanceName='ACPI\\PNP0C14\\1_1'",
                    null);

                // Obtain in-parameters for the method
                ManagementBaseObject inParams =
                    classInstance.GetMethodParameters("GetSetULong");

                // Add the input parameters.

                Addr = 0x10000000000 + Addr;
                inParams["Data"] = Addr;

                // Execute the method and obtain the return values.
                // Wordaround to avoid busy flag
                System.Threading.Thread.Sleep(200);
                ManagementBaseObject outParams =
                    classInstance.InvokeMethod("GetSetULong", inParams, null);
                // List outParams
                data = outParams["Return"];
                return true;
            }
            catch (ManagementException err)
            {
                Console.WriteLine("GetSetULong failed" + err.Message);
                return false;
            }
        }
        static public String WMIWriteECRAM(UInt64 Addr, UInt64 Value)
        {
            try
            {
                ManagementObject classInstance =
                    new ManagementObject("root\\WMI",
                    "AcpiTest_MULong.InstanceName='ACPI\\PNP0C14\\1_1'",
                    null);

                // Obtain in-parameters for the method
                ManagementBaseObject inParams =
                    classInstance.GetMethodParameters("GetSetULong");

                // Add the input parameters.
                Value = Value << 16;
                Addr = 0x0000000000000000 + Value + Addr;
                inParams["Data"] = Addr;

                // Execute the method and obtain the return values.
                // Wordaround to avoid busy flag
                System.Threading.Thread.Sleep(200);
                ManagementBaseObject outParams =
                    classInstance.InvokeMethod("GetSetULong", inParams, null);
                // List outParams
                return outParams["Return"].ToString();

            }
            catch (ManagementException err)
            {
                Console.WriteLine("GetSetULong failed" + err.Message);
                return "Failed";
            }
        }
        static public void StartWMIReceiveEvent(EventArrivedEventHandler WMIHandleEvent)
        {
            try
            {

                WqlEventQuery query = new WqlEventQuery("SELECT * FROM AcpiTest_EventULong");

                watcher = new ManagementEventWatcher(new ManagementScope("\\\\.\\Root\\WMI"), query);
                //Console.WriteLine("Waiting for an event...");

                watcher.EventArrived += WMIHandleEvent;

                // Start listening for events
                watcher.Start();


                return;
            }
            catch (ManagementException err)
            {
                Console.WriteLine("An error occurred while trying to receive an event: " + err.Message);
            }
        }
        static public void EndWMIRecieveEvent()
        {
            if (watcher != null)
            {
                watcher.Stop();
                watcher = null;
            }

        }

    }
}

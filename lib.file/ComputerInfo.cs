using Microsoft.Win32;
using System;
using System.Management;
using System.Net.NetworkInformation;

namespace lib.file
{
    public class ComputerInfo
    {
        /// <summary>
        /// 获取CUP信息
        /// </summary>
        /// <returns></returns>
        public static string GetCPUInfo()
        {
            return GetHardWareInfo("Win32_Processor", "ProcessorId");
        }

        /// <summary>
        /// 获取BOIS信息
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSInfo()
        {
            return GetHardWareInfo("Win32_BIOS", "SerialNumber");
        }

        /// <summary>
        /// 获取主板信息
        /// </summary>
        /// <returns></returns>
        public static string GetBaseBoardInfo()
        {
            return GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
        }
        /// <summary>  
        /// 获取硬盘序号  
        /// </summary>  
        /// <returns>硬盘序号</returns>  
        public static string GetDiskID()
        {
            return GetHardWareInfo("Win32_DiskDrive", "Model");
        }

        /// <summary>
        /// 获取硬件信息
        /// </summary>
        /// <param name="typePath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetHardWareInfo(string typePath, string key)
        {
            try
            {
                using (ManagementClass managementClass = new ManagementClass(typePath))
                {
                    using (ManagementObjectCollection mn = managementClass.GetInstances())
                    {
                        foreach (ManagementObject m in mn)
                        {
                            foreach (PropertyData property in m.Properties)
                            {
                                if (property.Name == key)
                                {
                                    return property.Value.ToString();
                                }
                            }
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

      

        public static string GetMacAddressByNetworkInformation()
        {
            string key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
            string macAddress = string.Empty;
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                        && adapter.GetPhysicalAddress().ToString().Length != 0)
                    {
                        string fRegistryKey = key + adapter.Id + "\\Connection";
                        RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                        if (rk != null)
                        {
                            string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                            int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                            if (fPnpInstanceID.Length > 3 &&
                                fPnpInstanceID.Substring(0, 3) == "PCI")
                            {
                                macAddress = adapter.GetPhysicalAddress().ToString();
                                for (int i = 1; i < 6; i++)
                                {
                                    macAddress = macAddress.Insert(3 * i - 1, ":");
                                }
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return macAddress;
        }
    }
}

using System;
using System.Linq;
using System.Management;                
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

internal static class MachineIdHelper
{
    internal static string GetMachineId()
    {
        StringBuilder sb = new StringBuilder();
        TryAppendWmi(sb, "Win32_BaseBoard", "SerialNumber");
        TryAppendWmi(sb, "Win32_BIOS", "SerialNumber");
        TryAppendWmi(sb, "Win32_ComputerSystemProduct", "UUID");
        TryAppendWmi(sb, "Win32_DiskDrive", "SerialNumber");
        try
        {
            string mac = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n =>
                    n != null &&
                    n.OperationalStatus == OperationalStatus.Up &&
                    n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    n.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                    !ToLower(n.Description).Contains("virtual") &&
                    !ToLower(n.Name).Contains("virtual"))
                .Select(n => n.GetPhysicalAddress() != null ? n.GetPhysicalAddress().ToString() : null)
                .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m));
            if (!string.IsNullOrWhiteSpace(mac))
                sb.Append("|MAC=").Append(mac);
        }
        catch
        {
            
        }
        string raw = sb.ToString();
        if (string.IsNullOrWhiteSpace(raw))
            raw = Environment.MachineName; 
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
    private static void TryAppendWmi(StringBuilder sb, string wmiClass, string prop)
    {
        try
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT " + prop + " FROM " + wmiClass))
            using (ManagementObjectCollection results = searcher.Get())
            {
                foreach (ManagementObject mo in results)
                {
                    var valueObj = mo != null ? mo[prop] : null;
                    string val = valueObj != null ? valueObj.ToString() : null;
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        sb.Append("|").Append(wmiClass).Append(".").Append(prop).Append("=").Append(val.Trim());
                        break; 
                    }
                }
            }
        }
        catch
        {
            
        }
    }
    private static string ToLower(string s)
    {
        return string.IsNullOrEmpty(s) ? string.Empty : s.ToLowerInvariant();
    }
}

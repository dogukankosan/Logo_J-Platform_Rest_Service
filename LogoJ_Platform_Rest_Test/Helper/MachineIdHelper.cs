using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

internal static class MachineIdHelper
{
    internal static string GetHardwareBoundMachineId()
    {
        List<string> parts = new List<string>();
        parts.Add(FormatPart("REG", "MachineGuid", GetMachineGuid()));
        parts.Add(FormatPart("WMI", "CSP.UUID", GetFirstWmiValue("Win32_ComputerSystemProduct", "UUID")));
        parts.Add(FormatPart("WMI", "BIOS.SN", GetFirstWmiValue("Win32_BIOS", "SerialNumber")));
        parts.Add(FormatPart("WMI", "Board.SN", GetFirstWmiValue("Win32_BaseBoard", "SerialNumber")));
        parts.Add(FormatPart("WMI", "SysDisk.SN", GetSystemDiskSerial()));
        parts = parts.Where(s => !string.IsNullOrEmpty(s)).ToList();
        if (parts.Count == 0)
            parts.Add("FALLBACK=" + Environment.MachineName);
        string raw = string.Join("|", parts);
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
    private static string GetMachineGuid()
    {
        try
        {
            using (RegistryKey k = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
            {
                if (k != null)
                {
                    object val = k.GetValue("MachineGuid");
                    return Normalize(val != null ? val.ToString() : null);
                }
            }
        }
        catch { }
        return null;
    }
    private static string GetFirstWmiValue(string wmiClass, string prop)
    {
        try
        {
            using (ManagementObjectSearcher s = new ManagementObjectSearcher("SELECT " + prop + " FROM " + wmiClass))
            using (ManagementObjectCollection col = s.Get())
            {
                var list = col.Cast<ManagementObject>()
                              .Select(mo => Normalize(mo[prop] != null ? mo[prop].ToString() : null))
                              .Where(v => !string.IsNullOrEmpty(v))
                              .Distinct(StringComparer.OrdinalIgnoreCase)
                              .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
                              .ToList();
                return list.FirstOrDefault();
            }
        }
        catch { }
        return null;
    }
    private static string GetSystemDiskSerial()
    {
        try
        {
            string systemDriveLetter = Environment.SystemDirectory.Substring(0, 2); // örn "C:"
            string q1 = "ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + systemDriveLetter + "'} WHERE AssocClass=Win32_LogicalDiskToPartition";
            using (ManagementObjectSearcher assoc1 = new ManagementObjectSearcher(q1))
            using (ManagementObjectCollection partResults = assoc1.Get())
            {
                ManagementObject partition = partResults.Cast<ManagementObject>().FirstOrDefault();
                if (partition == null) return null;
                string q2 = "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass=Win32_DiskDriveToDiskPartition";
                using (ManagementObjectSearcher assoc2 = new ManagementObjectSearcher(q2))
                using (ManagementObjectCollection diskResults = assoc2.Get())
                {
                    ManagementObject disk = diskResults.Cast<ManagementObject>().FirstOrDefault();
                    if (disk == null) return null;
                    string sn = Normalize(disk["SerialNumber"] != null ? disk["SerialNumber"].ToString() : null);
                    if (!string.IsNullOrEmpty(sn)) return sn;
                    string deviceId = Normalize(disk["DeviceID"] != null ? disk["DeviceID"].ToString() : null);
                    if (string.IsNullOrEmpty(deviceId)) return null;
                    using (ManagementObjectSearcher pmSearch = new ManagementObjectSearcher("SELECT Tag, SerialNumber FROM Win32_PhysicalMedia"))
                    using (ManagementObjectCollection pmResults = pmSearch.Get())
                    {
                        foreach (ManagementObject mo in pmResults)
                        {
                            string tag = Normalize(mo["Tag"] != null ? mo["Tag"].ToString() : null);
                            string pmSn = Normalize(mo["SerialNumber"] != null ? mo["SerialNumber"].ToString() : null);
                            if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(pmSn) &&
                                string.Equals(tag, deviceId, StringComparison.OrdinalIgnoreCase))
                            {
                                return pmSn;
                            }
                        }
                    }
                }
            }
        }
        catch { }
        return null;
    }
    private static string Normalize(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim().Replace("\0", "");
        string up = s.ToUpperInvariant();
        if (up == "UNKNOWN" || up == "TO BE FILLED BY O.E.M." || up == "DEFAULT STRING" || up == "NONE")
            return null;
        return s;
    }
    private static string FormatPart(string src, string key, string val)
    {
        return string.IsNullOrEmpty(val) ? null : src + "." + key + "=" + val;
    }
}
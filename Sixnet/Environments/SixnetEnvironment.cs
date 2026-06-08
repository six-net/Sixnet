// "Company © 2025. All rights reserved."

using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Win32;

using Sixnet.Logging;

namespace Sixnet.Environments
{
    /// <summary>
    /// Environment
    /// </summary>
    public static class SixnetEnvironment
    {
        /// <summary>
        /// Gets the machine name
        /// </summary>
        public static string GetMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                SixnetLogger.LogError(ex, ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Get machine id
        /// </summary>
        /// <returns></returns>
        public static string GetMachineId()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    static string readMachineGuidValue(RegistryView view)
                    {
                        using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
                        using var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                        return key?.GetValue("MachineGuid")?.ToString();
                    }
                    var machineId = readMachineGuidValue(RegistryView.Registry64);
                    if (!string.IsNullOrWhiteSpace(machineId))
                    {
                        return machineId;
                    }
                    machineId = readMachineGuidValue(RegistryView.Registry32);
                    return machineId ?? string.Empty;
                }
                if (OperatingSystem.IsLinux())
                {
                    string[] paths =
                    {
                        "/etc/machine-id",
                        "/var/lib/dbus/machine-id"
                    };

                    foreach (var path in paths)
                    {
                        if (File.Exists(path))
                        {
                            return File.ReadAllText(path).Trim();
                        }
                    }
                }

                if (OperatingSystem.IsMacOS())
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "ioreg",
                        Arguments = "-rd1 -c IOPlatformExpertDevice",
                        RedirectStandardOutput = true
                    };

                    using var process = System.Diagnostics.Process.Start(psi);
                    var output = process!.StandardOutput.ReadToEnd();

                    var line = output.Split('\n')
                        .FirstOrDefault(l => l.Contains("IOPlatformUUID"));

                    return line?.Split('=').Last().Trim().Trim('"');
                }
            }
            catch (Exception ex)
            {
                SixnetLogger.LogError(ex, ex.Message);
                throw;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets network adapters
        /// </summary>
        public static List<NetworkInterface> GetNetworkAdapters()
        {
            return NetworkInterface.GetAllNetworkInterfaces()?.ToList() ?? new List<NetworkInterface>(0);
        }

        /// <summary>
        /// Get mac addresses
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMacAddresses()
        {
            return NetworkInterface.GetAllNetworkInterfaces()?
            .Select(ni => ni.GetPhysicalAddress()?.ToString())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList() ?? new List<string>(0);
        }

        /// <summary>
        /// Get first primary mac address
        /// </summary>
        /// <returns></returns>
        public static string GetFirstPrimaryMacAddress()
        {
            return GetNetworkAdapters()
                   .Where(n =>
                       n.OperationalStatus == OperationalStatus.Up &&
                       n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                       n.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                       !n.Description.ToLower().Contains("virtual") &&
                       !n.Description.ToLower().Contains("docker"))
                   .Select(n => n.GetPhysicalAddress()?.ToString())
                   .Where(c => !string.IsNullOrWhiteSpace(c))
                   .OrderBy(c => c)
                   .FirstOrDefault();
        }

        /// <summary>
        /// Get local IPv4 addresses
        /// </summary>
        /// <returns>Local IPv4 addresses</returns>
        public static List<IPAddress> GetIPAddresses()
        {
            return GetNetworkAdapters()?
                .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                             ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
                .Select(ua => ua.Address)
                .Where(ip => !IPAddress.IsLoopback(ip)).ToList()
                ?? new List<IPAddress>(0);
        }

        /// <summary>
        /// Get local IPv4 addresses
        /// </summary>
        /// <returns>Local IPv4 addresses</returns>
        public static IEnumerable<IPAddress> GetIPv4Addresses()
        {
            return GetIPAddresses()?.Where(c => c.AddressFamily == AddressFamily.InterNetwork).ToList() ?? new List<IPAddress>(0);
        }

        /// <summary>
        /// Get machine unique code
        /// </summary>
        /// <returns></returns>
        public static string GetMachineUniqueCode()
        {
            var parts = new List<string>
            {
                GetMachineName(),
                GetMachineId(),
                GetFirstPrimaryMacAddress(),
                Environment.ProcessorCount.ToString(),
            };
            string raw = string.Join("|", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return System.Convert.ToHexString(hash);
        }
    }
}

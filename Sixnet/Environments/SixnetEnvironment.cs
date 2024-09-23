using Sixnet.DependencyInjection;
using Sixnet.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

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
        public static string MachineName
        {
            get
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
        }

        /// <summary>
        /// Gets all mac address
        /// </summary>
        public static List<string> AllMacs
        {
            get
            {
                try
                {
                    var macs = new List<string>();
                    var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                    if (networkInterfaces.IsNullOrEmpty())
                    {
                        return new List<string>(0);
                    }
                    foreach (var ni in networkInterfaces)
                    {
                        var macAddress = ni.GetPhysicalAddress()?.ToString();
                        if (!string.IsNullOrWhiteSpace(macAddress))
                        {
                            macs.Add(macAddress);
                        }
                    }
                    return macs;
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogError(ex, ex.Message);
                    return new List<string>(0);
                }
            }
        }

        /// <summary>
        /// Gets the main mac
        /// </summary>
        public static string MainMac
        {
            get
            {
                try
                {
                    var allMacs = NetworkInterface.GetAllNetworkInterfaces();
                    if (allMacs.IsNullOrEmpty())
                    {
                        return string.Empty;
                    }
                    var mainMac = allMacs
                            .FirstOrDefault(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback && ni.OperationalStatus == OperationalStatus.Up)
                            ?? allMacs.FirstOrDefault();
                    return mainMac?.GetPhysicalAddress()?.ToString() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogError(ex, ex.Message);
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets all ips
        /// </summary>
        public static List<string> AllIps 
        { 
            get 
            {
                try
                {
                    var hostName = Dns.GetHostName();
                    var ipAddresses = Dns.GetHostAddresses(hostName);
                    return ipAddresses?.Select(ip => ip.ToString()).ToList() ?? new List<string>(0);
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogError(ex, ex.Message);
                    return new List<string>(0);
                }
            } 
        }

        /// <summary>
        /// Gets the main ip
        /// </summary>
        public static string MainIp
        {
            get
            {
                try
                {
                    var hostName = Dns.GetHostName();
                    var allIps = Dns.GetHostAddresses(hostName);
                    if(allIps.IsNullOrEmpty())
                    {
                        return string.Empty;
                    }
                    var ipAddress = allIps.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork) ?? allIps.FirstOrDefault();
                    return ipAddress?.ToString() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogError(ex, ex.Message);
                    return string.Empty;
                }
            }
        }
    }
}

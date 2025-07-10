using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;



public class NetworkUtils
{
    public static IPAddress GetBroadcastAddress(IPAddress localIpAddress, IPAddress subnetMask)
    {
        byte[] ipBytes = localIpAddress.GetAddressBytes();
        byte[] maskBytes = subnetMask.GetAddressBytes();

        if (ipBytes.Length != maskBytes.Length)
        {
            throw new ArgumentException("IP address and subnet mask lengths do not match.");
        }

        byte[] broadcastBytes = new byte[ipBytes.Length];
        for (int i = 0; i < ipBytes.Length; i++)
        {
            broadcastBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
        }

        return new IPAddress(broadcastBytes);
    }

    public static IPAddress GetSubnetMask(IPAddress localIpAddress)
    {
       

        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (UnicastIPAddressInformation unicastInfo in networkInterface.GetIPProperties().UnicastAddresses)
            {
              
                if (unicastInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                    unicastInfo.Address.Equals(localIpAddress))
                {
                    Debug.WriteLine("DEBUGGING TEST, IPv4 MASK: " + unicastInfo.IPv4Mask.ToString());
                    return unicastInfo.IPv4Mask;
                }
            }
        }

        throw new ArgumentException("Subnet mask not found for the given IP address.");
    }





    //public static void Main()
    //{
    //    // Get the local IP address
    //    string hostName = Dns.GetHostName();
    //    IPAddress localIpAddress = Dns.GetHostEntry(hostName).AddressList
    //        .First(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

    //    // Get the subnet mask
    //    IPAddress subnetMask = GetSubnetMask(localIpAddress);

    //    // Calculate the broadcast address
    //    IPAddress broadcastAddress = GetBroadcastAddress(localIpAddress, subnetMask);

    //    Console.WriteLine($"Local IP Address: {localIpAddress}");
    //    Console.WriteLine($"Subnet Mask: {subnetMask}");
    //    Console.WriteLine($"Broadcast Address: {broadcastAddress}");
    //}
}

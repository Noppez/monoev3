using System;
using System.IO;
using System.Net.NetworkInformation;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.Connections
{
	public class WiFiDevice
	{
		private static string WpaSupplicantFileName = "/mnt/bootpar/wpa_supplicant.conf";

		public static bool IsLinkUp()
		{
			bool up = false;
			try {
				string output = ProcessHelper.RunAndWaitForProcessWithOutput ("ip", "link show wlan0");
				string[] outputs = output.Split (new string[] { "\r\n", "\n" }, StringSplitOptions.None);
				foreach (var s in outputs){
					if(s.ToLower().Contains("up")){
						up = true;
					}
				}
			} 
			catch{
				
			}
			return up;
		}
		
		public static string Gateway ()
		{
			if (IsLinkUp ()) {
				string s = ProcessHelper.RunAndWaitForProcessWithOutput ("route");
				string[] result = s.Split (new [] { '\r', '\n' });
				if (result.Length >= 4) {
					
					//The gateway is in line 4 second word. Regex is used to remove whitespaces between words
					string ipString = System.Text.RegularExpressions.Regex.Replace(result[3],@"\s+"," ").Split(' ')[1];
					try{
						System.Net.IPAddress.Parse (ipString);
						return ipString;
					}
					catch{
					
					}
				}
			}
			return null;
		}
		
		public static void TurnOff()
		{
			ProcessHelper.RunAndWaitForProcess("killall","wpa_supplicant");
		}
		
		public static string GetIpAddress()
		{
			if (IsLinkUp ()) {
				NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces ();
				foreach (var ni in interfaces) {
					foreach (var addr in ni.GetIPProperties().UnicastAddresses) {
						if (addr.Address.ToString () != "127.0.0.1")
							return addr.Address.ToString ();					
					}
				}
			}
			return "Unknown";
		}

		public static bool TurnOn (int timeout = 0)
		{
			if (!IsLinkUp ()) {
				if (ProcessHelper.RunAndWaitForProcess ("/home/root/lejos/bin/startwlan", "", timeout) == 0) 
				{
					return true;
				}
				TurnOff();
			}
			return false; 
		}

		public static bool WriteWpaSupplicantConfiguration (string ssid, string password, bool useEncryption)
		{
			ProcessHelper.RunAndWaitForProcess("rm",WpaSupplicantFileName); 
			string encryption;
			if(useEncryption)
				encryption = "WPA-PSK";
			else
				encryption = "NONE";	             
			string[] lines = { 
				"#This file is auto generated by MonoBrick - will be overwritten at startup", 
				"#ap_scan=1", 
				"#fast_reauth=1",
				"# Pers", 
				"ctrl_interface=/var/run/wpa_supplicant", 
				"network={", 
				"  ssid=\"" + ssid + "\"",
				"  key_mgmt="+encryption, 
				"  psk=\"" + password + "\"", 
				"  pairwise=CCMP TKIP",
				"  group=CCMP TKIP", 
				"}", 
			};
			bool ok = true;
			try
			{
				File.WriteAllLines(@WpaSupplicantFileName, lines);
			}
			catch(IOException)
			{
				ok = false;
			}
			return ok;
		}



	}
}


using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Specialized;
using System.Xml;

namespace MonoBrickFirmware.Settings
{
	
	public class DebugSettings{
		[XmlElement("Port")]
		private int port = 12345;
		
		[XmlElement("TerminateWithEscape")]
		private bool terminateWithEscape = true;
		
		public bool TerminateWithEscape	
		{
			get { return terminateWithEscape; }
			set { terminateWithEscape = value; }
		}
		
		public int Port
		{
			get { return port; }
			set { port = value; }
		}
	}
	
	public class WiFiSettings{
		[XmlElement("SSID")]
		private string ssid = "YourSSID";
		
		[XmlElement("Password")]
		private string password = "YourPassword";
		
		[XmlElement("ConnectAtStartUp")]
		private bool connectAtStartUp = false;
		
		[XmlElement("Encryption")]
		private bool encryption = true;
		
		public string SSID {
			get{return ssid; }
			set { ssid = value; }
		}
		public string Password{
			get{return  password; }
			set {  password = value; }
		}
		
		public bool ConnectAtStartUp	
		{
			get { return connectAtStartUp; }
			set { connectAtStartUp = value; }
		}
		
		public bool Encryption	
		{
			get { return encryption; }
			set { encryption = value; }
		}
	}
	
	public class GeneralSettings{
		[XmlElement("CheckForSwUpdatesAtStartUp")]
		private bool checkForSwUpdatesAtStartUp = true;
		
		public bool CheckForSwUpdatesAtStartUp	
		{
			get { return checkForSwUpdatesAtStartUp; }
			set { checkForSwUpdatesAtStartUp = value; }
		}
	}
	
	public class SoundSettings{
		[XmlElement("Volume")]
		private int volume = 60;
		
		[XmlElement("EnableSound")]
		private bool enableSound = true;
		
		public bool EnableSound	
		{
			get { return enableSound; }
			set { enableSound = value; }
		}

		
		public int Volume	
		{
			get { return volume; }
			set { volume = value; }
		}
	}
	
	[XmlRoot("ConfigRoot")]
	public class FirmwareSettings
	{
		private static FirmwareSettings instance = null;
		private static object readWriteLock = new object();
		private static string SettingsFileName = "/mnt/bootpar/firmwareSettings.xml";
		
		
		[XmlElement("GeneralSettings")]
		public GeneralSettings GeneralSettings { get; set; }
		
		[XmlElement("WiFiSettings")]
		public WiFiSettings WiFiSettings { get; set; }

		[XmlElement("DebugSettings")]
		public DebugSettings DebugSettings{ get; set; }

		[XmlElement("SoundSettings")]
		public SoundSettings SoundSettings{ get; set; }

		private FirmwareSettings ()
		{
			GeneralSettings = new GeneralSettings();
			WiFiSettings = new WiFiSettings();
			DebugSettings = new DebugSettings();
			SoundSettings = new SoundSettings();	
		}
		
		public static FirmwareSettings Instance {
			get {
				if (instance == null) {
					lock (readWriteLock) 
					{
						instance = new FirmwareSettings();
						instance = instance.LoadFromXML(SettingsFileName);
					} 
				} 
				return instance;
			} 
		}
		
		public bool SaveToXML ()
		{
			try {
				lock (readWriteLock) 
				{
					XmlSerializer serializer = new XmlSerializer (typeof(FirmwareSettings));
					TextWriter textWriter = new StreamWriter (SettingsFileName);
					serializer.Serialize (textWriter, this);
					textWriter.Close ();
				}
				return true;
			} 
			catch{}
			return false;
		}
		
		private FirmwareSettings LoadFromXML (String filepath)
		{
			XmlSerializer deserializer = new XmlSerializer (typeof(FirmwareSettings));
			TextReader textReader = new StreamReader (filepath);
			Object obj = deserializer.Deserialize (textReader);
			FirmwareSettings myNewSettings = (FirmwareSettings)obj;
			textReader.Close ();
			return myNewSettings;
		}
	}
}

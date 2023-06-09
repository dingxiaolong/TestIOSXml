using System;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Foundation;

namespace MySimpleApp
{
	public class Program
	{
		static int Main (string[] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			try {
				XmlSerializationTest.Bug1820_GenericList ();
				Console.WriteLine ("✅ SUCCESS");
			} catch (Exception e) {
				Console.WriteLine ("❌ EXCEPTION: {0}", e);
			}

			return args.Length;
		}


		// we want the test to be availble if we use the linker
		[Preserve (AllMembers = true)]
		public partial class XmlSerializationTest {
			
			[Serializable]
			[XmlType (AnonymousType = true)]
			[XmlRoot (Namespace = "", IsNullable = false)]
			public class Response {
				private DataUpdates dataUpdatesField;
				private static XmlSerializer serializer;

				public Response ()
				{
					this.dataUpdatesField = new DataUpdates ();
				}
		
				[XmlElement (Order = 0)]
				public DataUpdates DataUpdates {
					get { return this.dataUpdatesField; }
					set { this.dataUpdatesField = value; }
				}

				private static XmlSerializer Serializer {
					get {
						if ((serializer == null))
							serializer = new XmlSerializer (typeof (Response));
						return serializer;
					}
				}

				public static Response Deserialize (string xml)
				{
					StringReader stringReader = null;
					try {
						stringReader = new StringReader (xml);
						return ((Response)(Serializer.Deserialize (XmlReader.Create (stringReader))));
					} finally {
						if ((stringReader != null)) {
							stringReader.Dispose ();
						}
					}
				}
			}

			[Serializable]
			[XmlType (AnonymousType = true)]
			[XmlRoot (Namespace = "", IsNullable = false)]
			public class DataUpdates {
				private List<DataUpdatesDataUpdateInfo> dataUpdateInfoField;
				private static XmlSerializer serializer;
		
				public DataUpdates ()
				{
					this.dataUpdateInfoField = new List<DataUpdatesDataUpdateInfo> ();
				}

				// BUG1820 is here! remove Order = 0 and it works
				[XmlElement ("DataUpdateInfo", Form = XmlSchemaForm.Unqualified, Order = 0)]
				public List<DataUpdatesDataUpdateInfo> DataUpdateInfo {
					get { return this.dataUpdateInfoField; }
					set { this.dataUpdateInfoField = value; }
				}

				private static XmlSerializer Serializer {
					get {
						if ((serializer == null))
							serializer = new XmlSerializer (typeof (DataUpdates));
						return serializer;
					}
				}

				public static DataUpdates Deserialize (string xml)
				{
					StringReader stringReader = null;
					try {
						stringReader = new StringReader (xml);
						return ((DataUpdates)(Serializer.Deserialize (XmlReader.Create (stringReader))));
					} finally {
						if ((stringReader != null)) {
							stringReader.Dispose ();
						}
					}
				}
			}
		
			[Serializable]
			[XmlType (AnonymousType = true)]
			public class DataUpdatesDataUpdateInfo {
		
				private DateTime dataDateField;
				private string dataTypeField;
				private DateTime lastUpdatedDateField;
				private static XmlSerializer serializer;
		
				public DataUpdatesDataUpdateInfo ()
				{
				}
		
				[XmlAttribute]
				public DateTime DataDate {
					get { return this.dataDateField; }
					set { this.dataDateField = value; }
				}
		
				[XmlAttribute]
				public string DataType {
					get { return this.dataTypeField; }
					set { this.dataTypeField = value; }
				}
		
				[XmlAttribute]
				public DateTime LastUpdatedDate {
					get { return this.lastUpdatedDateField; }
					set { this.lastUpdatedDateField = value; }
				}
		
				private static XmlSerializer Serializer {
					get {
						if ((serializer == null))
							serializer = new XmlSerializer (typeof(DataUpdatesDataUpdateInfo));
						return serializer;
					}
				}
		
				public static DataUpdatesDataUpdateInfo Deserialize (string xml)
				{
					StringReader stringReader = null;
					try {
						stringReader = new StringReader (xml);
						return ((DataUpdatesDataUpdateInfo)(Serializer.Deserialize (XmlReader.Create (stringReader))));
					} finally {
						if ((stringReader != null)) {
							stringReader.Dispose ();
						}
					}
				}
			}
			
			// http://bugzilla.xamarin.com/show_bug.cgi?id=1820
			// note: this also test the linker (5.1+) ability not to remove 'unused' XML setters and .ctors used for serialization
			public static void Bug1820_GenericList ()
			{
				string input = @"
					<?xml version=""1.0"" encoding=""UTF-8""?>
					<Response>
					    <DataUpdates>
					        <DataUpdateInfo DataDate=""2009-04-13T00:00:00"" DataType=""Data"" LastUpdatedDate=""2010-12-12T02:53:19.257"" />
					        <DataUpdateInfo DataDate=""2009-04-14T00:00:00"" DataType=""Data"" LastUpdatedDate=""2010-12-12T02:53:19.257"" />
					        <DataUpdateInfo DataDate=""2009-04-15T00:00:00"" DataType=""Data"" LastUpdatedDate=""2010-12-12T01:52:51.047"" />
					    </DataUpdates>
					</Response>
					".Trim ();
				Response r = Response.Deserialize (input);
				Console.WriteLine (r);
			}
		}
	}
}

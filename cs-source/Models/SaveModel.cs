using System;
using System.Xml.Serialization;

namespace SaveXML.Model
{
	[XmlRoot("rdpclient")]
	public class XMLData
	{
		public string? server { get; set; }
		public string? domain { get; set; }
		public string? user { get; set; }
		public string? pwd { get; set; }
		public bool? hasmemory { get; set; }
		public bool? fullscreen { get; set; }
		public bool? timeout { get; set; }
		public bool? ignorecertificate { get; set; }

		public XMLData ()
		{

		}
	}
}

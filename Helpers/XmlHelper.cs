using System;
using System.Xml.Serialization;
using SaveXML.Model;

namespace XmlHelper.Open
{
	public static class XmlRead
	{
		private static T? XmlReader<T> (string path) where T : class
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));

				using(StreamReader reader = new StreamReader(path))
				{
					return serializer.Deserialize(reader) as T;
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public static XMLData? GetXmlData ()
		{
			return XmlReader<XMLData>(AppContext.BaseDirectory + "client.xml");
		}
	}
}

namespace XmlHelper.Etch
{
	public static class XmlWrite
	{
		private static bool XmlWriter<T> (string path, T dataObject)
		{
			try
			{
				XmlSerializer xml = new XmlSerializer(typeof(T));
				XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
				namespaces.Add("GIO", "RDP Client");

				using(TextWriter sw = new StreamWriter(path))
				{
					xml.Serialize(sw, dataObject, namespaces);
				}

				return true;
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);

				return false;
			}
		}

		public static bool EtchDataXml<T> (T dataModel)
		{
			return XmlWriter<T>(AppContext.BaseDirectory + "client.xml", dataModel);
		}
	}
}
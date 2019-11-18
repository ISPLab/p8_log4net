using log4net.Core;
using log4net.Layout;
using log4net.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace log4net.Layout
{
	public class P8XmlLayoutSchemaLog4j : XmlLayoutSchemaLog4j
	{
		private static readonly DateTime Date1970 = new DateTime(1970, 1, 1);
		public override void ActivateOptions()
		{
			base.ActivateOptions();
		}
		protected override void FormatXml(XmlWriter writer1, LoggingEvent loggingEvent)
		{
			XmlTextWriter writer = (XmlTextWriter)writer1;
			writer.Namespaces = true;
			if (loggingEvent.LookupProperty("log4net:HostName") != null &&
				loggingEvent.LookupProperty("log4jmachinename") == null)
			{
				loggingEvent.GetProperties()["log4jmachinename"] = loggingEvent.LookupProperty("log4net:HostName");
			}
			if (loggingEvent.LookupProperty("log4japp") == null && !string.IsNullOrEmpty(loggingEvent.Domain))
			{
				loggingEvent.GetProperties()["log4japp"] = loggingEvent.Domain;
			}
			if (!string.IsNullOrEmpty(loggingEvent.Identity) &&
				loggingEvent.LookupProperty("log4net:Identity") == null)
			{
				loggingEvent.GetProperties()["log4net:Identity"] = loggingEvent.Identity;
			}
			if (!string.IsNullOrEmpty(loggingEvent.UserName) &&
				loggingEvent.LookupProperty("log4net:UserName") == null)
			{
				loggingEvent.GetProperties()["log4net:UserName"] = loggingEvent.UserName;
			}
			//writer.WriteStartElement("log4j:event");

			writer.WriteStartElement("log4j", "event", "log4j");
			writer.WriteAttributeString("logger", loggingEvent.LoggerName);
			writer.WriteAttributeString("timestamp",
										XmlConvert.ToString(
											(long)
											(loggingEvent.TimeStamp.ToUniversalTime() - Date1970)
												.TotalMilliseconds));
			writer.WriteAttributeString("level", loggingEvent.Level.DisplayName);
			writer.WriteAttributeString("thread", loggingEvent.ThreadName);
			// writer.WriteStartElement("log4j:message");
			writer.WriteStartElement("log4j", "message", "log4j");
			Transform.WriteEscapedXmlString(writer, loggingEvent.RenderedMessage, InvalidCharReplacement);
			writer.WriteEndElement();
			object obj = loggingEvent.LookupProperty("NDC");
			if (obj != null)
			{
				string text = loggingEvent.Repository.RendererMap.FindAndRender(obj);
				if (!string.IsNullOrEmpty(text))
				{
					//   writer.WriteStartElement("log4j:NDC");
					writer.WriteStartElement("log4j", "NDC", "log4j");
					Transform.WriteEscapedXmlString(writer, text, InvalidCharReplacement);
					writer.WriteEndElement();
				}
			}
			PropertiesDictionary properties = loggingEvent.GetProperties();
			if (properties.Count > 0)
			{
				writer.WriteStartElement("log4j", "properties", "log4j");
				//  writer.WriteStartElement("log4j:properties");
				foreach (DictionaryEntry dictionaryEntry in properties)
				{
					// writer.WriteStartElement("log4j:data");
					writer.WriteStartElement("log4j", "data", "log4j");
					writer.WriteAttributeString("name", (string)dictionaryEntry.Key);
					string text = loggingEvent.Repository.RendererMap.FindAndRender(dictionaryEntry.Value);
					writer.WriteAttributeString("value", text);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			var exceptionString = loggingEvent.ExceptionObject;
			if (exceptionString != null && !string.IsNullOrEmpty(exceptionString.StackTrace))
			{
				writer.WriteStartElement("log4j", "throwable", "log4j");
				//  writer.WriteStartElement("log4j:throwable");
				Transform.WriteEscapedXmlString(writer, exceptionString.StackTrace, InvalidCharReplacement);
				writer.WriteEndElement();
			}

			if (LocationInfo)
			{
				LocationInfo locationInformation = loggingEvent.LocationInformation;
				writer.WriteStartElement("log4j", "locationInfo", "log4j");
				//  writer.WriteStartElement("log4j:locationInfo");
				writer.WriteAttributeString("class", locationInformation.ClassName);
				writer.WriteAttributeString("method", locationInformation.MethodName);
				writer.WriteAttributeString("file", locationInformation.FileName);
				writer.WriteAttributeString("line", locationInformation.LineNumber);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}

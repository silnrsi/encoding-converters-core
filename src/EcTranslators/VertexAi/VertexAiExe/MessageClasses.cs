using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilEncConverters40.EcTranslators.VertexAi.VertexAiExe
{
	public class ChatConversation
	{
		public string context { get; set; }
		public List<Example> examples { get; set; }
		public List<ChatMessage> messages { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
		}
	}

	public class Example
	{
		public Input input { get; set; }
		public Output output { get; set; }
	}

	public class Input
	{
		public string content { get; set; }
	}

	public class Output
	{
		public string content { get; set; }
	}

	public class ChatMessage
	{
		public string author { get; set; }
		public string content { get; set; }
	}
}
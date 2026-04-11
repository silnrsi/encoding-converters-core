using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SpellingFixer30
{
    [XmlRoot("SpellFixerHashtableStore", Namespace = "http://www.sil.org/computing/schemas/HashtableStore.xsd")]
    public class SpellFixerHashtableStore
    {
        [XmlElement("Item")]
        public List<Item> Items { get; set; } = new List<Item>();

        [XmlAttribute("caseSensitive")]
        public bool CaseSensitive { get; set; }

        public static SpellFixerHashtableStore LoadFromXml(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SpellFixerHashtableStore));
            using (var reader = System.IO.File.OpenRead(filePath))
            {
                return (SpellFixerHashtableStore)serializer.Deserialize(reader);
            }
        }

        public void SaveToXml(string filePath)
        {
            while (!CscProjectStore.CanWriteToFile(filePath))
            {
                if (MessageBox.Show($"Can't write to the file {filePath}. But you can (manually) delete it, so it'll be recreated.", CscProject.ApplicationCaption, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(SpellFixerHashtableStore));
            using (var writer = System.IO.File.Create(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }
    }

    public class Item
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement("ContextList")]
        public ContextList ContextList { get; set; }
    }

    public class ContextList
    {
        [XmlElement("Context")]
        public List<string> Contexts { get; set; } = new List<string>();
    }
}
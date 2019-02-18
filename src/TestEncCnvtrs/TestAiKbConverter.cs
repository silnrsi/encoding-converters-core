using NUnit.Framework;
using SilEncConverters40;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestEncCnvtrs
{
    [TestFixture]
    public class TestAiKbConverter
    {
        [Test]
        public void Test_Removing_blank_target_translations()
        {
            Assert.IsNotNull(typeof(AdaptItEncConverter).AssemblyQualifiedName);
            var contents = LoadEmbeddedResourceFileAsStringExecutingAssembly("BlankTargetFormsAiKb.xml");
            var map = MapOfMaps.ParseXml(contents);
            var tempFilename = Path.GetTempFileName();
            map.SaveFile(tempFilename);
            string newContents = File.ReadAllText(tempFilename);
            var index = newContents.IndexOf("a=\"\"");
            Assert.AreEqual(-1, index);
            index = newContents.IndexOf("k=\"\"");
            Assert.AreEqual(-1, index);
        }

        public static string LoadEmbeddedResourceFileAsStringExecutingAssembly(string strResourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            strResourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.Contains(strResourceName));
            if (String.IsNullOrEmpty(strResourceName))
                return null;

            var resourceAsStream = assembly.GetManifestResourceStream(strResourceName);
            StreamReader reader = new StreamReader(resourceAsStream);
            string text = reader.ReadToEnd();
            return text;
        }

    }
}

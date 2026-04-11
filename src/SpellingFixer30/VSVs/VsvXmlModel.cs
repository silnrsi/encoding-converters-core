using SpellingFixer30;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

[XmlRoot("VernScriptSystem", Namespace = "http://www.sil.org/computing/schemas/SpellFixerVernScript.xsd")]
public class VernScriptSystem
{
    [XmlElement("VernScriptSystemProperties")]
    public VernScriptSystemProperties Properties { get; set; }

    [XmlElement("ExtraPunctuation")]
    public ExtraPunctuation ExtraPunctuation { get; set; }

    [XmlElement("IgnoreCharactersBeforeTransliteration")]
    public IgnoreCharactersBefore IgnoreCharactersBeforeTransliteration { get; set; }

    [XmlElement("IgnoreCharactersAfterTransliteration")]
    public IgnoreCharactersAfter IgnoreCharactersAfterTransliteration { get; set; }

    [XmlElement("PreprocessDataViaRegex")]
    public RegexChanges PreprocessDataViaRegex { get; set; }

    [XmlElement("Distinctions")]
    public Distinctions Distinctions { get; set; }

    public static VernScriptSystem LoadVsvXml(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(VernScriptSystem));
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            return (VernScriptSystem)serializer.Deserialize(fs);
        }
    }

    public static void SaveVsvXml(string filePath, VernScriptSystem data)
    {
        while (!CscProjectStore.CanWriteToFile(filePath))
        {
            if (MessageBox.Show($"Can't write to the file {filePath}. But you can (manually) delete it, so it'll be recreated.", CscProject.ApplicationCaption, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(VernScriptSystem));
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(fs, data);
        }
    }
}

public class VernScriptSystemProperties
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("isRightToLeft")]
    public bool IsRightToLeftLanguage { get; set; } = false;

    [XmlElement]
    public string ProgId { get; set; }

    [XmlElement]
    public string TransliteratorEncConverterName { get; set; }

    [XmlElement]
    public bool? TransliteratorEncConverterDirectionForward { get; set; }

    [XmlElement]
    public string TransliteratorEncConverterNormalize { get; set; }
}

public class ExtraPunctuation
{
    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlElement("IgnorePunctuation")]
    public List<IgnorePunctuation> IgnorePunctuations { get; set; }
}

public class IgnorePunctuation
{
    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlText]
    public string Value { get; set; }
}

public class IgnoreCharactersBefore
{
    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlElement("IgnoreCharacterBeforeTransliteration")]
    public List<IgnoreCharacter> Characters { get; set; }
}

public class IgnoreCharactersAfter
{
    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlElement("IgnoreCharacterAfterTransliteration")]
    public List<IgnoreCharacter> Characters { get; set; }
}

public class IgnoreCharacter
{
    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlText]
    public string Value { get; set; }
}

public class RegexChanges
{
    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlElement("RegexChange")]
    public List<RegexChange> RegexChangeList { get; set; }
}

public class RegexChange
{
    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlAttribute("pattern")]
    public string Pattern { get; set; }

    [XmlAttribute("replaceWith")]
    public string ReplaceWith { get; set; }

    [XmlAttribute("ignoreCase")]
    public bool IgnoreCase { get; set; }

    [XmlAttribute("multiline")]
    public bool Multiline { get; set; }

    [XmlAttribute("explicitCapture")]
    public bool ExplicitCapture { get; set; }

    [XmlAttribute("compiled")]
    public bool Compiled { get; set; } = true;

    [XmlAttribute("cultureInvariant")]
    public bool CultureInvariant { get; set; }

    [XmlAttribute("rightToLeft")]
    public bool RightToLeft { get; set; }
}

public class Distinctions
{
    private bool? _compareDoubleCharacters;

    [XmlAttribute("compareDoubleCharacters")]
    public bool CompareDoubleCharacters
    {
        get => _compareDoubleCharacters ?? true;
        set => _compareDoubleCharacters = value;
    }

    private bool? _ignoreCase;

    [XmlAttribute("ignoreCase")]
    public bool IgnoreCase
    {
        get => _ignoreCase ?? false;
        set => _ignoreCase = value;
    }

    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlElement("Distinction")]
    public List<Distinction> DistinctionList { get; set; }
}

public class Distinction
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlElement("NormalizeRule")]
    public List<NormalizeRule> NormalizeRules { get; set; }
}

public class NormalizeRule
{
    [XmlElement("CharactersToNormalize")]
    public CharactersToNormalize CharactersToNormalize { get; set; }

    [XmlElement("NormalizedForm")]
    public string NormalizedForm { get; set; }
}

public class CharactersToNormalize
{
    [XmlElement("CharacterToNormalize")]
    public List<CharacterToNormalize> CharacterToNormalize { get; set; }
}

public class CharacterToNormalize
{
    [XmlText]
    public string Value { get; set; }
}

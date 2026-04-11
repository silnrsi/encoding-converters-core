using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Runtime.Serialization;                 // for SerializationException
#if !SaveSoapFormatter
using System.Runtime.Serialization.Formatters.Binary;
#else
using System.Runtime.Serialization.Formatters.Soap; // for soap formatter
#endif

namespace SpellingFixer30
{
    [Serializable()]
    public class KnownGoodWordList : Hashtable
    {
        public KnownGoodWordList()
        {
        }

        public new SpellFixerWord this[object key]
        {
            // TODO: deal with index values as well (c.f. EncConverters)
            get { return (SpellFixerWord)base[key]; }
        }

        public void Add(SpellFixerWord sfw)
        {
            base.Add(sfw.Value, sfw);
        }

        /// <summary>
        /// e.g. strPath = @"C:\Program Files\Common Files\SIL\SpellFixer\Projects\Kangri"
        /// </summary>
        /// <param name="strPath">Path to the project folder</param>
        /// <returns>KnownGoodWordList</returns>
        public static KnownGoodWordList LoadTable(string strPath)
        {
            string strFilename = strPath += @"\KnownGoodWordList.sfc";
            if (!File.Exists(strFilename))
                return new KnownGoodWordList();

            FileStream fs = new FileStream(strFilename, FileMode.Open);

            // Construct a SoapFormatter and use it 
            // to serialize the data to the stream.
            KnownGoodWordList lstGoodWords = null;
            try
            {
#if !SaveSoapFormatter
                BinaryFormatter formatter = new BinaryFormatter();
#else
                SoapFormatter formatter = new SoapFormatter();
#endif
                lstGoodWords = (KnownGoodWordList)formatter.Deserialize(fs);
            }
            catch (SerializationException ex)
            {
                MessageBox.Show("Failed to open known good word list file. Reason: " + ex.Message);
            }
            finally
            {
                fs.Close();
            }

            return lstGoodWords;
        }

        public static void SaveTable(KnownGoodWordList lstGoodWords, string strGoodWordListPath)
        {
            FileStream fs = new FileStream(strGoodWordListPath, FileMode.Create);

            // Construct a SoapFormatter and use it 
            // to serialize the data to the stream.
#if !SaveSoapFormatter
            BinaryFormatter formatter = new BinaryFormatter();
#else
            SoapFormatter formatter = new SoapFormatter();
#endif
            try
            {
                formatter.Serialize(fs, lstGoodWords);
            }
            catch (SerializationException ex)
            {
                MessageBox.Show("Failed to save known good word list! Reason: " + ex.Message);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}

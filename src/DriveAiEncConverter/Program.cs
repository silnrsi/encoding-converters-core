using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ECInterfaces;
using SilEncConverters40;

namespace DriveAiEncConverter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            /*
            var theEcs = new EncConverters();
            theEcs.AutoSelect(ConvType.Unicode_to_from_Unicode);
            const string cstrFriendlyName = "Lookup in Hindi to Urdu adaptations";
            var theEcs = new EncConverters();
            theEcs.AddConversionMap(cstrFriendlyName, @"C:\Users\Bob\Documents\Adapt It Unicode Work\Hindi to Urdu adaptations\Hindi to Urdu adaptations.xml", ConvType.Unicode_to_from_Unicode,
                        EncConverters.strTypeSILadaptit, "UNICODE", "UNICODE",
                        ProcessTypeFlags.DontKnow);
            var theEc = (AdaptItEncConverter)theEcs[cstrFriendlyName];
            string strSelectedSourceWord = theEc.EditKnowledgeBase("दुन");

             * 
             * 
             * 
             * const string cstrFriendlyName = "Any-Latin";
            var theEcs = DirectableEncConverter.EncConverters;
            var assm = Assembly.LoadFile(@"C:\src\EC\output\debug\IcuECLib.dll");
            foreach (var type in assm.GetTypes())
                Console.WriteLine(type);
            theEcs.AddConversionMap(cstrFriendlyName, cstrFriendlyName, ConvType.Unicode_to_from_Unicode,
                        EncConverters.strTypeSILicuTrans, "UNICODE", "UNICODE",
                        ProcessTypeFlags.DontKnow);
            var theEc = theEcs[cstrFriendlyName];
            string m_thaiInput = "พักหลังๆนี่เวลาแก๊นจะตัดสินใจซื้ออะไรซักอย่างที่มันมีราคา จะคิดแล้วคิดอีก อย่างน้อยก็ทิ้งเวลาไว้ตั้งแต่";
            Console.WriteLine(theEc.Convert(m_thaiInput));

            Console.WriteLine(assm.FullName);
            const string cstrFriendlyName = "UnitTesting-ThaiWordBreaker";
            var theEcs = DirectableEncConverter.EncConverters;
            theEcs.AddConversionMap(cstrFriendlyName, " ", ConvType.Unicode_to_from_Unicode,
                        IcuBreakIteratorEncConverter.CstrImplementationType, "UNICODE", "UNICODE",
                        ProcessTypeFlags.DontKnow);
            var theEc = theEcs[cstrFriendlyName];
            string m_thaiInput = "พักหลังๆนี่เวลาแก๊นจะตัดสินใจซื้ออะไรซักอย่างที่มันมีราคา จะคิดแล้วคิดอีก อย่างน้อยก็ทิ้งเวลาไว้ตั้งแต่";
            string m_thaiOutput = "พัก หลังๆ นี่ เวลา แก๊น จะ ตัดสิน ใจ ซื้อ อะไร ซัก อย่าง ที่ มัน มี ราคา จะ คิด แล้ว คิด อีก อย่าง น้อย ก็ ทิ้ง เวลา ไว้ ตั้งแต่";
            
            var strOutput = theEc.Convert(m_thaiInput);
            System.Diagnostics.Debug.Assert(strOutput == m_thaiOutput);

            var theECs = new EncConverters();
            var theEc = (AdaptItEncConverter)theECs["Lookup in Hindi to English adaptations"];
            string strSelectedSourceWord = theEc.EditKnowledgeBase("दुन");
            strSelectedSourceWord = theEc.EditKnowledgeBase(strSelectedSourceWord);
            theEc.EditTargetWords(strSelectedSourceWord);
            int nBufSize = str.Length * 2;
            var abyInBuffer = new byte[nBufSize];
            fixed (byte* lpInBuffer = abyInBuffer)
            {
                int nInLen = ECNormalizeData.StringToByteStar(str, lpInBuffer, nBufSize);
                Console.WriteLine("{0}{1}", str, nInLen);
            }
    str = EncConverters.ByteArrToBytesString(Encoding.UTF8.GetBytes(str));
    Console.WriteLine(str);
            */
            /*
            string str = "TEST DATA";
            var theEc = theECs[0];
            var strO = theEc.Convert(str);
            */

            const string cstrFriendlyName = "UnitTesting-ThaiWordBreaker";
            var theEcs = DirectableEncConverter.EncConverters;
            theEcs.AddConversionMap(cstrFriendlyName, " ", ConvType.Unicode_to_from_Unicode,
                        IcuBreakIteratorEncConverter.CstrImplementationType, "UNICODE", "UNICODE",
                        ProcessTypeFlags.DontKnow);
            var theEc = theEcs[cstrFriendlyName];
            var inputText = "พักหลังๆนี่เวลาแก๊นจะตัดสินใจซื้ออะไรซักอย่างที่มันมีราคา จะคิดแล้วคิดอีก อย่างน้อยก็ทิ้งเวลาไว้ตั้งแต่";
            var outputText = "พัก หลังๆ นี่ เวลา แก๊น จะ ตัดสิน ใจ ซื้อ อะไร ซัก อย่าง ที่ มัน มี ราคา จะ คิด แล้ว คิด อีก อย่าง น้อย ก็ ทิ้ง เวลา ไว้ ตั้งแต่";
            System.Diagnostics.Debug.Assert(theEc.Convert(inputText) == outputText);
            inputText = "我喜欢吃苹果。";
            outputText = "我 喜欢 吃 苹果 。";
            System.Diagnostics.Debug.Assert(theEc.Convert(inputText) == outputText);
        }
    }
}

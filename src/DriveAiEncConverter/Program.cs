using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilEncConverters40;

namespace DriveAiEncConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var theECs = new EncConverters();
            var theEc = (AdaptItEncConverter)theECs["Lookup in Hindi to English adaptations"];
            string strSelectedSourceWord = theEc.EditKnowledgeBase("दुन");
            strSelectedSourceWord = theEc.EditKnowledgeBase(strSelectedSourceWord);
            theEc.EditTargetWords(strSelectedSourceWord);
            /*
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
        }
    }
}

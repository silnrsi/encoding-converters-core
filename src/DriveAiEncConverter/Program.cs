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
        }
    }
}

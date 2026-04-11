using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SpellingFixer30
{
    [Serializable()]
    internal class AmbiguityListMaps : Hashtable
    {
        /// <summary>
        /// strKey = "a"
        /// astrValues = { "e", "ai", "a" };
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="astrValues"></param>
        public AmbiguityListMaps()
        {
        }

        public new ArrayList this[object key]
        {
            get
            {
                return (ArrayList)base[key];
            }
            set
            {
                base[key] = value;
            }
        }
    }
}

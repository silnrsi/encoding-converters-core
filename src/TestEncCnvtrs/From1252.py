# sample encoding converter script: from cp1252 to Unicode
# the input is supposed to be 8-bit characters in Windows 1252 codepage encoding
# the output should be in UTF-8 encoding

import codecs

def Convert(bytes):
    cdc1252 = codecs.lookup('cp1252')
    uresult, ulen = cdc1252.decode(bytes)
    cdcUtf8 = codecs.lookup('utf_8')
    result, len = cdcUtf8.encode(uresult)
    return result

# sample encoding converter script: from cp1252 to Unicode
# the input should be in UTF-8 encoding
# the output is supposed to be 8-bit characters in Windows 1252 codepage encoding

import codecs

def Convert(bytes):
    # a codec wants 8-bit codes as input for decoding and produces UTF-16
    cdcUtf8 = codecs.lookup('utf_8')
    uresult, ulen = cdcUtf8.decode(bytes)
    # a codec wants UTF-16 as input for encoding and produces 8-bit codes
    cdc1252 = codecs.lookup('cp1252')
    result, len = cdc1252.encode(uresult)
    return result

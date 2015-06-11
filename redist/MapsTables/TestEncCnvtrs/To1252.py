# sample encoding converter script: from cp1252 to Unicode
# input should be a Python unicode string (UCS2 on Windows, UCS4 on Linux)
# output is supposed to be 8-bit characters in Windows 1252 codepage encoding

def Convert(unicodeInput):
    return unicodeInput.encode('cp1252');

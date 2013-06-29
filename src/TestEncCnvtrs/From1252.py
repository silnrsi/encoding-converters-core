# sample encoding converter script: from cp1252 to Unicode
# input is supposed to be 8-bit characters in Windows 1252 codepage encoding
# output should be a Python unicode string (UCS2 on Windows, UCS4 on Linux)

def Convert(bytesInput):
    return bytesInput.decode('cp1252');

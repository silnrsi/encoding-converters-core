# -*- coding: utf-8 -*-
#-------------------------------------------------------------------------------
# ToNfc.py
#-------------------------------------------------------------------------------
from unicodedata import normalize

# input:  NFD string
# output: NFC string
def Convert(u):
    return normalize('NFC', u)

if __name__ == '__main__':
    ## Warning: The following testing code will not work on Python 3.
    uInput = u"Ę" # NFD string
    print u"sending: %s" % repr(uInput)
    result = Convert(uInput)
    print u"result: %s" % repr(result)
    expected = u"Ę" # NFC string
    if result == expected:
        print "ok"
    else:
        print u"unexpected result; expected %s" % repr(expected)


# -*- coding: utf-8 -*-
#-------------------------------------------------------------------------------
# ToNfd.py
#-------------------------------------------------------------------------------
from unicodedata import normalize

# input:  NFC string
# output: NFD string
def Convert(u):
    return normalize('NFD', u)

if __name__ == '__main__':
    ## Warning: The following testing code will not work on Python 3.
    uInput = u"क़" # NFC string
    print(u"sending: %s" % repr(uInput))
    result = Convert(uInput)
    print(u"result: %s" % repr(result))
    expected = u"क़" # NFD string
    if result == expected:
        print("ok")
    else:
        print(u"unexpected result; expected %s" % repr(expected))


# -*- coding: utf-8 -*-
#-------------------------------------------------------------------------------
# ToNfc.py
#
# Warning: This code will not work on Python 3.
#-------------------------------------------------------------------------------
from unicodedata import normalize

# input:  NFD string
# output: NFC string
def Convert(u):
    return normalize('NFC', u)

if __name__ == '__main__':
    ui = u"Ę"
    print repr(ui)
    u = Convert(ui)  # NFD string
    print "result: " + repr(u)
    print "expecting: " + repr(u"Ę")  # NFC string
    if u == u"Ę": # NFC string
        print "ok"
    else:
        print "unexpected result"


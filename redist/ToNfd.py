# -*- coding: utf-8 -*-
#-------------------------------------------------------------------------------
# ToNfd.py
#
# Warning: This code will not work on Python 3.
#-------------------------------------------------------------------------------
from unicodedata import normalize

# input:  NFC string
# output: NFD string
def Convert(u):
    return normalize('NFD', u)

if __name__ == '__main__':
    ui = u"क़"
    print repr(ui)
    u = Convert(ui)  # NFC bytes
    print "result: " + repr(u)
    print "expecting: " + repr(u"क़")  # NFD string
    if u == u"क़": # NFD string
        print "ok"
    else:
        print "unexpected result"


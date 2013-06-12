# -*- coding: utf-8 -*-
#-------------------------------------------------------------------------------
# ToUnicodeNames.py
#
# Warning: This code will not work on Python 3.
#-------------------------------------------------------------------------------
import unicodedata

# input:  Unicode string
# output: The Unicode names of the individual Unicode characters
def Convert(u):
    if not isinstance(u, unicode):
        raise UnicodeError(u'Input Data is not a Unicode string! (%s)' % u)
    else:
        r = u''
        for ch in u:
            r += '%s; ' % unicodedata.name(ch)
        return r

if __name__ == '__main__':
    ui = u"क़"
    print repr(ui)
    u = Convert(ui)  # NFC bytes
    print "result: " + repr(u)
    print "expecting: " + repr(u"DEVANAGARI LETTER KA; DEVANAGARI SIGN NUKTA; ")  # NFD string
    if u == u"DEVANAGARI LETTER KA; DEVANAGARI SIGN NUKTA; ": # NFD string
        print "ok"
    else:
        print "unexpected result"

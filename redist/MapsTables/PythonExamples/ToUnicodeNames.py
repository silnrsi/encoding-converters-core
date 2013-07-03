# -*- coding: utf-8 -*-
#-------------------------------------------------------------------------------
# ToUnicodeNames.py
#-------------------------------------------------------------------------------
import unicodedata

# input:  Unicode string
# output: bytes -- names of individual Unicode characters
def Convert(u):
    if not isinstance(u, unicode):
        raise UnicodeError(u"Input Data is not a Unicode string! (%s)" % u)
    else:
        r = b""
        for ch in u:
            r += b"%s; " % unicodedata.name(ch)
        return r

if __name__ == '__main__':
    ## Warning: The following testing code will not work on Python 3.
    uInput = u"क़"
    print u"sending: %s" % repr(uInput)
    result = Convert(uInput)
    print "result: %s" % repr(result)
    expected = "DEVANAGARI LETTER KA; DEVANAGARI SIGN NUKTA; "
    if result == expected:
        print "ok"
    else:
        print "unexpected result; expected %s" % repr(expected)

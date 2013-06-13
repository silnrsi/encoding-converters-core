#-------------------------------------------------------------------------------
# ReverseString.py
#
# Warning: This code will not work on Python 3.
#-------------------------------------------------------------------------------

# input:  Unicode string
# output: the reverse of the Unicode string
def Convert(u):
    return u[::-1]

if __name__ == '__main__':
    ru = Convert(u"abcd")  # unicode
    print "result: " + repr(ru)
    print "expecting: " + repr(u"dcba")  # reversed string
    if ru == u"dcba": # reversed string
        print "ok"
    else:
        print "unexpected result"

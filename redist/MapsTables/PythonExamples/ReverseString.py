#-------------------------------------------------------------------------------
# ReverseString.py
#
# To test all of the scripts in this directory:
# ls *.py | xargs -i python {}
#-------------------------------------------------------------------------------

# input:  Unicode String or bytes
# output: The same type as input
def Convert(s):
    return s[::-1];

if __name__ == '__main__':
    ## Warning: The following testing code will not work on Python 3.
    result = Convert(b'mop')
    expected = b'pom'
    print(b"result: %s" % repr(result))
    if result != expected:
        print(b"unexpected result; expected %s" % repr(expected))
        exit()

    ui = u"மோப"  # Tamil Ma + Oo + Pa
    print(u"sending: %s " % repr(ui))
    result = Convert(ui)
    print(u"result: %s" % repr(result))
    expected = u"போம" # Tamil Pa + Oo + Ma
    if result == expected:
        print("ok")
    else:
        print(u"unexpected result; expected %s" % repr(expected))


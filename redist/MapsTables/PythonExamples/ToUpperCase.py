#-------------------------------------------------------------------------------
# ToUpperCase.py
#-------------------------------------------------------------------------------
def Convert(bytesInput):
    return bytesInput.upper()

if __name__ == '__main__':
    ## Warning: The following testing code will not work on Python 3.
    result = Convert(b'asdg')
    expected = b'ASDG'
    print(b"result: %s" % repr(result))
    if result == expected:
        print("ok")
    else:
        print(b"unexpected result; expected %s" % repr(expected))

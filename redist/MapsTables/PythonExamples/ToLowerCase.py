#-------------------------------------------------------------------------------
# ToLowerCase.py
#-------------------------------------------------------------------------------
def Convert(bytesInput):
    return bytesInput.lower()

if __name__ == '__main__':
    ## Warning: The following testing code will not work on Python 3.
    result = Convert(b'ASdG')
    expected = b'asdg'
    print(b"result: %s" % repr(result))
    if result == expected:
        print("ok")
    else:
        print(b"unexpected result; expected %s" % repr(expected))


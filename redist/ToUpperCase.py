import unicodedata

# SILConverters will execute the function called "Convert", but below that
# are some other examples of Python script functions. If you want to call any
# of them, put them in a different file with the signature 'def Convert(s)'
def Convert(u):
    return u.upper()

if __name__ == '__main__':
    u = Convert('asdg')
    print u
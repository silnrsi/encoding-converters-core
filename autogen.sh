# !/bin/sh
# Run this before configure (it runs configure itself, actually)

rm -f autogen.err
srcdir=`dirname $0`
test -z "$srcdir" && srcdir=.
olddir=`pwd`
cd $srcdir

# Create aclocal.m4, so autoconf gets the additional macros it needs
echo "Creating aclocal.m4: aclocal -I ac-helpers $ACLOCAL_FLAGS"
aclocal -I ac-helpers $ACLOCAL_FLAGS 2>> autogen.err

# If we have an existing configure script, save a copy for comparison.
if [ -f config.cache ] && [ -f configure ]; then
    cp configure configure.old
fi

# Create ./configure
echo "Creating configure..."
autoconf 2>> autogen.err || {
    echo ""
    echo "*** warning: possible errors while running automake - check autogen.err"
    echo ""
}

cd $olddir
$srcdir/configure "$@"
echo
echo "Now type 'make' to compile the encConverter libraries."

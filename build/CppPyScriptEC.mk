# Makefile for CppPlugins
# Created by Jim Kornelsen on Jan 16 2012.
#
# make -f CppPyScriptEC.mk all

PROJDIR=../
SRCDIR  =$(PROJDIR)src/PyScriptEC/
BUILDDIR=$(PROJDIR)build/Obj/
OUTDIR  =$(PROJDIR)output/Debug/
FILE1   =PyScriptEncConverter
ECHEADERDIR=$(PROJDIR)src/EncCnvtrs/lib
ECHEADER=$(ECHEADERDIR)/CEncConverter.h

all: buildPy

buildPy:$(BUILDDIR)$(FILE1).o
	g++ -shared -Wl,-soname,lib$(FILE1).so.1 \
        -o $(OUTDIR)lib$(FILE1).so.1.0 \
        $(BUILDDIR)$(FILE1).o -lpython2.7

$(BUILDDIR)$(FILE1).o: $(SRCDIR)$(FILE1).cpp $(SRCDIR)$(FILE1).h $(ECHEADER)
	g++ -Wall -fPIC -c $(SRCDIR)$(FILE1).cpp -o $(BUILDDIR)$(FILE1).o \
        -I/usr/include/python2.7/ -I$(ECHEADERDIR)

clean:
	rm -f $(BUILDDIR)*.*
	rm -f $(OUTDIR)$(FILE1).so.1.0

# Makefile for ICU EncConverter C++ component.
# Created by Jim Kornelsen on Jan 16 2012.
#
# make -f CppIcuEC.mk all

PROJDIR=../
SRCDIR  =$(PROJDIR)src/IcuEC/
BUILDDIR=$(PROJDIR)build/Obj/
OUTDIR  =$(PROJDIR)output/Debug/
FILE1   =IcuTranslitEC
FILE2   =IcuRegexEC
ECHEADERDIR=$(PROJDIR)src/EncCnvtrs/lib
ECHEADER=$(ECHEADERDIR)/CEncConverter.h

all: buildIcuTranslit buildIcuRegex

buildIcuTranslit:$(BUILDDIR)$(FILE1).o
	g++ -shared -Wl,-soname,lib$(FILE1).so.1 \
        -o $(OUTDIR)lib$(FILE1).so.1.0 \
        $(BUILDDIR)$(FILE1).o \
        -L/usr/local/lib -licuuc -licui18n

buildIcuRegex:$(BUILDDIR)$(FILE2).o
	g++ -shared -Wl,-soname,lib$(FILE2).so.1 \
        -o $(OUTDIR)lib$(FILE2).so.1.0 \
        $(BUILDDIR)$(FILE2).o \
        -L/usr/local/lib -licuuc -licui18n

$(BUILDDIR)$(FILE1).o: $(SRCDIR)$(FILE1).cpp $(SRCDIR)$(FILE1).h $(ECHEADER)
	g++ -Wall -fPIC -c $(SRCDIR)$(FILE1).cpp -o $(BUILDDIR)$(FILE1).o \
        -I/usr/local/include/ -I$(ECHEADERDIR)

$(BUILDDIR)$(FILE2).o: $(SRCDIR)$(FILE2).cpp $(SRCDIR)$(FILE2).h $(ECHEADER)
	g++ -Wall -fPIC -c $(SRCDIR)$(FILE2).cpp -o $(BUILDDIR)$(FILE2).o \
        -I/usr/local/include/ -I$(ECHEADERDIR)

clean:
	rm -f $(BUILDDIR)*.*
	rm -f $(OUTDIR)$(FILE1).so.1.0
	rm -f $(OUTDIR)$(FILE2).so.1.0


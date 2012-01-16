# Makefile for CppPlugins
# Created by Jim Kornelsen on Nov 16 2011.
#
# make -f CppPlugins.mk all

PROJDIR=../
SRCDIR  =$(PROJDIR)src/CppPlugins/
BUILDDIR=$(PROJDIR)build/Obj/
OUTDIR  =$(PROJDIR)output/Debug/
FILE1   =PyScriptEncConverter
FILE2   =IcuTranslitEC
FILE3   =IcuRegexEC
ECHEADER=CEncConverter.h

all: buildIcuTranslit buildIcuRegex buildPy

buildIcuTranslit:$(BUILDDIR)$(FILE2).o
	g++ -shared -Wl,-soname,lib$(FILE2).so.1 \
        -o $(OUTDIR)lib$(FILE2).so.1.0 \
        $(BUILDDIR)$(FILE2).o \
        -L/usr/local/lib -licuuc -licui18n

buildIcuRegex:$(BUILDDIR)$(FILE3).o
	g++ -shared -Wl,-soname,lib$(FILE3).so.1 \
        -o $(OUTDIR)lib$(FILE3).so.1.0 \
        $(BUILDDIR)$(FILE3).o \
        -L/usr/local/lib -licuuc -licui18n

$(BUILDDIR)$(FILE2).o: $(SRCDIR)$(FILE2).cpp $(SRCDIR)$(FILE2).h $(SRCDIR)$(ECHEADER)
	g++ -Wall -fPIC -c $(SRCDIR)$(FILE2).cpp -o $(BUILDDIR)$(FILE2).o \
        -I/usr/local/include/

$(BUILDDIR)$(FILE3).o: $(SRCDIR)$(FILE3).cpp $(SRCDIR)$(FILE3).h $(SRCDIR)$(ECHEADER)
	g++ -Wall -fPIC -c $(SRCDIR)$(FILE3).cpp -o $(BUILDDIR)$(FILE3).o \
        -I/usr/local/include/

buildPy:$(BUILDDIR)$(FILE1).o
	g++ -shared -Wl,-soname,lib$(FILE1).so.1 \
        -o $(OUTDIR)lib$(FILE1).so.1.0 \
        $(BUILDDIR)$(FILE1).o -lpython2.7

$(BUILDDIR)$(FILE1).o: $(SRCDIR)$(FILE1).cpp $(SRCDIR)$(FILE1).h $(SRCDIR)$(ECHEADER)
	g++ -Wall -fPIC -c $(SRCDIR)$(FILE1).cpp -o $(BUILDDIR)$(FILE1).o \
        -I/usr/include/python2.7/

clean:
	rm -f $(BUILDDIR)*.*
	rm -f $(OUTDIR)$(FILE1).so.1.0
	rm -f $(OUTDIR)$(FILE2).so.1.0
	rm -f $(OUTDIR)$(FILE3).so.1.0


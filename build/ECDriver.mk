# Makefile for ECDriver for linux.
# Created by Jim Kornelsen on 22-Oct-2011.
#
# To run the "all" target, type:
#   make -f ECDriver.mk build

PROJDIR=../
SRCDIR  =$(PROJDIR)src/ECDriver/linux/
BUILDDIR=$(PROJDIR)build/Obj/
OUTDIR  =$(PROJDIR)output/Debug/

build:$(BUILDDIR)ECDriver.o
	g++ -shared -Wl,-soname,libecdriver.so.1 \
        -o $(OUTDIR)libecdriver.so.1.0 $(BUILDDIR)ECDriver.o \
        -L/usr/lib -lmono-2.0 -lpthread

$(BUILDDIR)ECDriver.o: $(SRCDIR)ECDriver.cpp $(SRCDIR)ecdriver.h
	g++ -Wall -fPIC -c $(SRCDIR)ECDriver.cpp -o $(BUILDDIR)ECDriver.o \
        -I/usr/include/mono-2.0

test: $(SRCDIR)testDynamic.cpp
	g++ -Wall $(SRCDIR)testDynamic.cpp -ldl -o $(OUTDIR)testECDriverDyn.exe

testShared: $(BUILDDIR)testShared.o
	g++ -g -o $(OUTDIR)testECDriverShared.exe $(BUILDDIR)testShared.o -L$(OUTDIR) -lecdriver

$(BUILDDIR)testShared.o: $(SRCDIR)testShared.cpp
	g++ -Wall -g -c $(SRCDIR)testShared.cpp -o $(BUILDDIR)testShared.o

clean:
	rm -f $(BUILDDIR)*.*
	rm -f $(OUTDIR)libecdrive.so.1.0
	rm -f $(OUTDIR)testECDriver.exe


#!/bin/sh
mv debian debian-orig
mv debian-fieldworks debian

debuild -S

mv debian debian-fieldworks
mv debian-orig debian

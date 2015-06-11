#!/bin/sh
mv debian debian-orig
mv debian-fieldworks debian
mv values.xml values.xml-orig
mv values.xml-fieldworks values.xml

debuild -S

mv debian debian-fieldworks
mv debian-orig debian
mv values.xml values.xml-fieldworks
mv values.xml-orig values.xml

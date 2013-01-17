#!/bin/bash
# $1 is the value of the MONO variable set by configure
# $2 is the value of the MONO_LDFLAGS variable set by configure
# any following arguments are passed on to xbuild proper
if [ "$1" != $(which mono) ]; then
	echo adjusting PATH and LD_LIBRARY_PATH
	export PATH=$(dirname $1):$PATH
	# note that MONO_LDFLAGS starts with -L, which we don't want
	export LD_LIBRARY_PATH=$(echo $2|cut -c3-):$LD_LIBRARY_PATH
fi
echo PATH=$PATH
echo LD_LIBRARY_PATH=$LD_LIBRARY_PATH
shift 2
xbuild "$@"

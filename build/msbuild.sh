#!/bin/bash
# Run msbuild
#
# $1 is the value of the MONO variable set by configure
# $2 is the value of the MONO_LDFLAGS variable set by configure
# Any following arguments are passed on.

log() {
  echo msbuild.sh[$$]: "$@"
}

# Build with /usr/local/bin/mono on developer machines if it's the default
if [ "$(which mono)" = "/usr/local/bin/mono" ]; then
	log Adjusting PATH and LD_LIBRARY_PATH
	export PATH=/usr/local/bin:$PATH
	export LD_LIBRARY_PATH=/usr/local/lib:$LD_LIBRARY_PATH
elif [ "$1" != "$(which mono)" ]; then
	log Adjusting PATH and LD_LIBRARY_PATH
	export PATH=$(dirname $1):$PATH
	# note that MONO_LDFLAGS starts with -L, which we don't want
	export LD_LIBRARY_PATH=$(echo $2|cut -c3-):$LD_LIBRARY_PATH
fi
log PATH=$PATH
log LD_LIBRARY_PATH=$LD_LIBRARY_PATH
shift 2
msbuild "$@"

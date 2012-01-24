dnl @synopsis AC_MONO_DEVEL
dnl
dnl This macro checks for mono, and tries to get the include path to jit.h for
dnl mono-2.0, and the library path to libmono-2.0.a
dnl It provides the $(MONO_CPPFLAGS) and $(MONO_LDFLAGS) output variables.
dnl
dnl @category InstalledPackages
dnl @author Stephen McConnel <stephen_mcconnel@sil.org>
dnl @version 2012-01-23
dnl @license GPLWithACException

AC_DEFUN([EC_MONO_DEVEL],[
	#
	# Check for an installation of mono
	#
	AC_PATH_PROG([MONO],[mono])
	if test -z "$MONO"; then
		AC_MSG_ERROR([Cannot find mono.  Please install a version of mono >= 2.8.0])
	fi
	ec_origmono=$MONO
	if test -n "$MONO"; then
		if `dpkg-query -S "$MONO"`; then true; else
			#
			# This can happen on developer machines with a custom compiled version of mono in /usr/local/bin...
			# We need a version of mono that came in a package.
			#
			AC_PATH_PROG([MONO2],[mono],[$MONO],[/usr/bin:/bin:/usr/lib/fieldworks/mono/bin:/opt/mono-2.10/bin])
			MONO=$MONO2
		fi
	fi
	ec_monobase=`echo $MONO|sed s-/bin/mono--`
	if test -f $ec_monobase/include/mono-2.0/mono/jit/jit.h -a -f $ec_monobase/lib/libmono-2.0.a; then true; else
		#
		# Versions of mono prior to 2.8 don't have the needed include file and library, so look for a newer
		# version if one has been installed.
		#
		AC_PATH_PROG([MONO3],[mono],[$MONO],[/usr/lib/fieldworks/mono/bin:/opt/mono-2.10/bin:/usr/bin:/bin])
		MONO=$MONO3
		ec_monobase=`echo $MONO|sed s-/bin/mono--`
	fi
	AC_MSG_CHECKING([for mono include path])
	if test -f $ec_monobase/include/mono-2.0/mono/jit/jit.h; then
		MONO_CPPFLAGS=-I$ec_monobase/include/mono-2.0
		AC_MSG_RESULT([$MONO_CPPFLAGS])
		AC_SUBST([MONO_CPPFLAGS])
	else
		AC_MSG_ERROR([Cannot find include/mono-2.0/mono/jit/jit.h.  Please install a version of mono >= 2.8.0])
	fi
	AC_MSG_CHECKING([for mono library path])
	if test -f $ec_monobase/lib/libmono-2.0.a; then
		MONO_LDFLAGS=-L$ec_monobase/lib
		AC_MSG_RESULT([$MONO_LDFLAGS])
		AC_SUBST([MONO_LDFLAGS])
	else
		AC_MSG_ERROR([Cannot find lib/libmono-2.0.a.  Please install a version of mono >= 2.8.0])
	fi
	if test "$MONO" != "$ec_origmono"; then
		AC_MSG_WARN([You need to adjust PATH and LD_LIBRARY_PATH to include $ec_monobase/{bin,lib}])
	fi
])

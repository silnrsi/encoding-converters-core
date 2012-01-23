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
	   AC_MSG_ERROR([Cannot find mono ($MONO).  Please install a version of mono >= 2.8.0])
	fi
	ec_monobase=`echo $MONO|sed s-/bin/mono--`
	AC_MSG_CHECKING([for mono include path])
	if test -f $ec_monobase/include/mono-2.0/mono/jit/jit.h; then
		MONO_CPPFLAGS=`echo -I$ec_monobase/include/mono-2.0`
		AC_MSG_RESULT([$MONO_CPPFLAGS])
		AC_SUBST([MONO_CPPFLAGS])
	else
		AC_MSG_ERROR([Cannot find include/mono-2.0/mono/jit/jit.h.  Please install a version of mono >= 2.8.0])
	fi
	AC_MSG_CHECKING([for mono library path])
	if test -f $ec_monobase/lib/libmono-2.0.a; then
		MONO_LDFLAGS=`echo -L$ec_monobase/lib`
		AC_MSG_RESULT([$MONO_LDFLAGS])
		AC_SUBST([MONO_LDFLAGS])
	else
		AC_MSG_ERROR([Cannot find lib/libmono-2.0.a.  Please install a version of mono >= 2.8.0])
	fi
])

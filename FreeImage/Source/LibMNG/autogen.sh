# autogen.sh
#
# invoke the auto* tools to create the configureation system

# move out configure.in
if ! test -f configure.in; then
  echo "copying configure.in"
  ln -s makefiles/configure.in .
fi

# move out the macros and run aclocal
if test ! -f acinclude.m4 -a -r makefiles/acinclude.m4; then
  echo "copying configure macros"
  ln -s makefiles/acinclude.m4 .
fi

# copy up our Makefile template
if ! test -f Makefile.am; then
  echo "copying automake template"
  ln -s makefiles/Makefile.am .
fi

echo "running aclocal"
aclocal

echo "running libtoolize"
libtoolize --automake

echo "running automake"
automake --foreign --add-missing

echo "building configure script"
autoconf

# and finally invoke our new configure
./configure $*

# end

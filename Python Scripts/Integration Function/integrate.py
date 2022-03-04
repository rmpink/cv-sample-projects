#!/usr/bin/python
import sys
import math
import random

def trap_integrate(a,b,N):
  interval = (b-a)/N
  val = 0

  for idx in xrange(1,int(N)):
    p = round(interval*idx, 8)
    val += ( 2*func(p) )

  return ( (interval/2) * (func(a) + val + func(b)) )

def mc_integrate(a,b,N):
  interval = (b-a)/N
  hits = 0
  maxY = 0

  for idx in xrange(0,int(N)):
    p = round(interval*idx, 8)
    thisY = func(p)

    if ( thisY > maxY ):
      maxY = thisY

  for ctr in xrange(0,int(N)):
    r = [random.uniform(a,b), random.uniform(0,maxY)]
    if ( func(r[0]) >= r[1] ):
      hits+=1

  return ((b-a)*maxY * (hits/N))

def func(x):
  return ( x*math.sqrt( math.pow(x-1, 2) + 4) )

if ( sys.argv[1] == "auto" ):
  print sys.argv
  print "\nTRAPEZOIDAL METHOD\n=================="
  print "    N = 10 :: Integration =", trap_integrate(0,2,10.0)
  print "   N = 100 :: Integration =", trap_integrate(0,2,100.0)

  print "\nMONTE CARLO METHOD\n=================="
  print "N = 100000 :: Integration =", mc_integrate(0,2,100000.0)
  print

else:
  qFlag = True

  while qFlag:
    try:
      N = float(raw_input('With N value of: '))
      print "\n\nTRAPEZOIDAL METHOD\n=================="
      print trap_integrate(0,2,N)

      print "\n\nMONTE CARLO METHOD\n=================="
      print mc_integrate(0,2,N)
      print "\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n"

    except ValueError:
      print "Not a number. QUITTING..."
      qFlag = False

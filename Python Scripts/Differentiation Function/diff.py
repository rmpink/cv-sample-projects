#!/usr/bin/python
import math

def func(x):
  sum = 0

  for idx in xrange(0,3):
    v = pow(-1,idx) * (pow(x*math.sin(x),idx)) / math.factorial(idx+1)
    sum += v
  return sum

def diff(t, e):
  d = (func(t + e) - func(t)) / e
  return d

qFlag = True

while qFlag:
  try:
    nEpsilon = float(raw_input('To what epsilon?'))
    nTime = float(raw_input('Get f\'(x) for: '))
    print diff(nTime, nEpsilon)

  except ValueError:
    print "Not a number. QUITTING..."
    qFlag = False

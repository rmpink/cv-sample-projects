#!/usr/bin/python
import sys

def printTable( l ):
  for r in l:
    print r
  return

def parseTable( f ):
  l = [];
  for line in f:
    li = [float(x) for x in line.strip('\n').split(' ')]
    l.append(li)
  return l

def lerpPosition( t, lst ):
  timeVals = lst[0]
  posVals = lst[1]
  d = 0

  for idx in xrange(0,len(lst[1])-1):
    if timeVals[idx] > t and idx > 0:
      num = t-timeVals[idx-1]
      denom = timeVals[idx]-timeVals[idx-1]
      deltaP = posVals[idx]-posVals[idx-1]

      d = posVals[idx-1] + num/denom*deltaP
      break

  return d

def lerpVelocity( t, lst ):
  l = []
  l.append(lst[0])
  tl = []
  tl.append(0)

  for idx in xrange(1,len(lst[1])-1):
    tl.append((lst[1][idx]-lst[1][idx-1])/(lst[0][idx]-lst[0][idx-1]))

  l.append(tl)
  return lerpPosition(t, l)

inFile = open(sys.argv[1], 'r')
valList = parseTable(inFile)

qFlag = True

while qFlag:
  try:
    nTime = float(raw_input('Get position & velocity at time: '))

    if ( nTime >= valList[0][0] and nTime <= valList[0][len(valList[0])-1]):
      print 'POSITION: ', lerpPosition(nTime, valList)
      print 'VELOCITY: ', lerpVelocity(nTime, valList)

  except ValueError:
    print "Not a number. QUITTING..."
    qFlag = False

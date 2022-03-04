#!/usr/bin/python
import sys

def parsematrix( f ):
  "Reads in contents of file and turns it into an NxN matrix (list of lists) of floats"
  l = [];
  for line in f:
    li = line.strip('\n').split(' ')
    l.append([float(x) for x in li])
  return l

def parsevector( f ):
  "Reads in contents of a file and turns it into a N-sized vector of floats"
  l = [];
  for line in f:
    li = line.strip('\n')
    l.append(float(li))
  return l

def printmatrix( m ):
  for r in m:
    print r
  print
  return

def swaprows( l, r1, r2 ):
  "Swaps rows [r1] and [r2] in list [l]"
  l[r1], l[r2] = l[r2], l[r1]
  return

def swapcolumns( l, c1, c2 ):
  "Swaps columns [c1] and [c2] in list [l]"
  for r in l:
    r[c1], r[c2] = r[c2], r[c1]
  return

def addmatrixcolumn( m, v ):
  "Adds a column represented by [v] onto matrix [m]"
  for x in xrange(len(m)):
    m[x].append(v[x])
  return m

def scalarmult( s, v ):
  "Scalar multiplication of vector"
  v = [x * s for x in v]
  return v

def vectoradd( v1, v2 ):
  "Add two vectors and return the result"
  r = []
  for x in xrange(len(v1)):
    r.append(v1[x] + v2[x])
  return r

def vectorminus( v1, v2 ):
  "Subtracts vector [v2] from vector [v1]"
  return vectoradd(v1, scalarmult(-1,v2))

def rowreduce( m1, m2 ):
  "This method takes 2 matrices representing a system of linear equations and solves them"
  lineq = addmatrixcolumn(m1, m2)
  print "INITIAL INPUT"
  printmatrix(lineq)

  for x in xrange(len(lineq)):
    #####################
    # if we are past the first row, pivot the next row/column around
    if ( x > 0 ):
      print "PIVOTING around row-column", x
      swaprows(lineq, 0, x)
      swapcolumns(lineq, 0, x)
      printmatrix(lineq)

    #####################
    # check for a starting 1, swap rows if needed to get it.
    swapped = False

    if (lineq[0][0] != 1):
      for xx in xrange(len(lineq)):
        if (lineq[xx][0] == 1):
          print "SWAPPING R0 & R" + `xx`
          swaprows(lineq, 0, xx)
          printmatrix(lineq)
          swapped = True
          break
    else:
      print "R0 already starts with a 1"

    #####################
    # if needed, reduce the first element of current row to a 1
    if (swapped == False and lineq[0][0] != 1):
      print "DIVIDING R0 by", lineq[0][0]

      if (lineq[0][0] != 0):
        lineq[0] = scalarmult(1/lineq[0][0], lineq[0])
      else:
        print "BARF - Cannot divide by 0, dude."
        print "Either there are infinite solutions, or no solutions."
        break

    #####################
    # reduce all other rows to 0
    for xx in xrange(1, len(lineq)):
      print "SUBTRACTING", lineq[xx][0], "* R0 from R" + `xx`
      lineq[xx] = vectorminus(lineq[xx], scalarmult(lineq[xx][0], lineq[0]))
      printmatrix(lineq)

    #####################
    # pivot back (if necessary) around the current row/column
    if ( x > 0 ):
      print "PIVOTING BACK around row-column", x
      swaprows(lineq, 0, x)
      swapcolumns(lineq, 0, x)
      printmatrix(lineq)

  print "FINAL RESULT"
  printmatrix(lineq)
  return

###################
# Load in the input data from file
mf1 = open(sys.argv[1], 'r')
mf2 = open(sys.argv[2], 'r')

###################
# Convert file data into python objects
list1 = parsematrix( mf1 )
list2 = parsevector( mf2 )

####################
rowreduce( list1, list2 )
####################

####################
# Extract the last column for presentation
newl = [];
for l in list1:
  newl.append(round(l[len(l)-1], 4))

print 'OUTPUT: ',newl

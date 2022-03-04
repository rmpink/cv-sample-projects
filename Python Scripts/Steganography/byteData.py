#!/usr/bin/python
import re
import os
import sys

JPG_SOI = '\xff\xd8'
JPG_EOI = '\xff\xd9'
JPG_SOS = '\xff\xda'

###########################################################
def encodeMsg( jpgFilePath, msgFilePath ):
	if ( os.path.exists( msgFilePath ) and os.path.exists( jpgFilePath ) ):
		msgFile = open( msgFilePath )
		bufFile = open( jpgFilePath, 'rb' )

		msg = msgFile.read() + '\x00'
		buf = bufFile.read()

		msgFile.close()
		bufFile.close()

		pxl = list(ord(b) for b in buf)

		b = buf.find( JPG_SOS ) + 2

		for m in xrange(0,len(msg)):
			ordM = ord(msg[m])

			for n in xrange(0,8):
				p = (m*8)+n+b

				pBit = pxl[p]%2
				mBit = (ordM>>(7-n))%2

				if ( mBit == 0 and pBit == 1 ):
					pxl[p] -= 1
				elif ( mBit == 1 and pBit == 0 ):
					pxl[p] += 1

		newBuf = ''.join(chr(p) for p in pxl)

		bufFile = open( jpgFilePath, 'wb' )
		bufFile.write( newBuf )
		bufFile.close()

	else:
		print 'ERROR: Incorrect message file path'
		return 0

###########################################################
def decodeMsg( jpgFilePath ):
	if ( os.path.exists( jpgFilePath ) ):
		bufFile = open( jpgFilePath, 'rb' )
		buf = bufFile.read()
		bufFile.close()

		pxl = list(ord(b) for b in buf)
		msg = list(ord(m) for m in ('\x00'*1024))

		b = buf.find( JPG_SOS ) + 2

		for m in xrange(0,len(msg)):

			for n in xrange(0,8):
				p = (m*8)+n+b

				pBit = pxl[p]%2

				if ( pBit == 1 ):
					msg[m] += 1<<(7-n)

			if (msg[m] == 0):
				break

		newMsg = ''.join(chr(m) for m in msg)
		print newMsg

	else:
		print 'ERROR: Incorrect JPG file path'
		return 0

###########################################################
def scrapeJPGs( imgFilePath ):

	if ( os.path.exists(imgFilePath) ):
		with open( str(imgFilePath) ) as f:

			eoiCtr = 0
			imgCtr = 0
			subImgCtr = 0
			byteArray = ''

			while True:
				sec = f.read(512)
				if ( not sec ): break

				soi = sec[:2]

				if ( soi == JPG_SOI ):
					imgCtr += 1
					subImgCtr = 0
					print 'File: ', f.name, ' --- JPG'
					byteArray = ''

				i = sec.find( JPG_EOI )
				if ( i >= 0 ):
					eoiCtr += 1
					print '--- EOI [',i+2,']'

					if ( os.path.isdir('./exported/' + imgFilePath.replace('.img','') ) == False ):
						os.mkdir('./exported/' + imgFilePath.replace('.img',''))

					newFile = open( './exported/' + imgFilePath.replace('.img','') + '/recovered-' + str(imgCtr) + '_' + str(subImgCtr) + '.jpg', 'wb')
					
					newFile.write( byteArray+sec[:i+2] )
					newFile.close()
					byteArray += sec
					subImgCtr += 1
					
				else:
					byteArray += sec

			print "# of JPGs found: ", imgCtr
			print "# of files exported: ", eoiCtr

			f.close()

###########################################################
def printHelp():
	print 'usage: python byteData.py [option] [optional_input1 optional_input2]'
	print 'options:'
	print '  -s, --scrape IMG_file            Attempt to scrape JPGs from IMG_file'
	print '  -e, --encode JPG_file MSG_file   Encode message in MSG_file into JPG_file'
	print '  -d, --decode JPG_file            Decode message from JPG_file and print to terminal'
	print '  -h, --help                       Display usage information'

###########################################################

if ( str(sys.argv[1]) == '-d' or str(sys.argv[1]) == '--decode' ):
	decodeMsg( sys.argv[2] )

elif ( str(sys.argv[1]) == '-e' or str(sys.argv[1]) == '--encode' ):
	encodeMsg( sys.argv[2], sys.argv[3] )

elif ( str(sys.argv[1]) == '-s' or str(sys.argv[1]) == '--scrape' ):
	scrapeJPGs( sys.argv[2] )

elif ( str(sys.argv[1]) == '-h' or str(sys.argv[1]) == '--help' ):
	printHelp()

else:
	print "Invalid syntax!"
	printHelp()
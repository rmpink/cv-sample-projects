// JSON object that stores an array of font faces and their
// respective types.
var Fonts = [
	{
		type: 'serif',
		faces: 'Georgia, serif'
	}, {
		type: 'serif',
		faces: '"Palatino Linotype", "Book Antiqua", Palatino, serif'
	}, {
		type: 'serif',
		faces: '"Times New Roman", Times, serif'
	}, {
		type: 'sans-serif',
		faces: 'Arial, Helvetica, sans-serif'
	}, {
		type: 'sans-serif',
		faces: '"Arial Black", Gadget, sans-serif'
	}, {
		type: 'sans-serif',
		faces: '"Comic Sans MS", cursive, sans-serif'
	}, {
		type: 'sans-serif',
		faces: 'Impact, Charcoal, sans-serif'
	}, {
		type: 'sans-serif',
		faces: '"Lucida Sans Unicode", "Lucida Grande", sans-serif'
	}, {
		type: 'sans-serif',
		faces: 'Tahoma, Geneva, sans-serif'
	}, {
		type: 'sans-serif',
		faces: '"Trebuchet MS", Helvetica, sans-serif'
	}, {
		type: 'sans-serif',
		faces: 'Verdana, Geneva, sans-serif'
	}, {
		type: 'monospace',
		faces: '"Courier New", Courier, monospace'
	}, {
		type: 'monospace',
		faces: '"Lucida Console", Monaco, monospace'
	}
];

/* 
 * JSON object that stores the collision boxes of each new
 * string that is created. It will have 4 properties:
 *     x: X-coordinate of the top-left of the box
 *     y: Y-coordinate of the top-left of the box
 *     w: Width of the box
 *    h: Height of the box
 */
var Boxes = [];


/********************************************************************************************
 * FUNCTION - initCollisionBoxes()
 * PARAMETERS - N/A
 * RETURNS - N/A
 * DESCRIPTION - 
 *	 This function is called when the first string is entered, and it adds the first
 *	 collision box, which happens to belong to the input box.
 ********************************************************************************************/
function initCollisionBoxes() {

	var newX = (window.innerWidth - 400) / 2;
	var newY = (window.innerHeight - 120) / 2;
	var newBox = { x: newX, y: newY, w: 400, h: 120 };

	Boxes.push( newBox );
}


/********************************************************************************************
 * FUNCTION - updateCharsLeft()
 * PARAMETERS - event
 * RETURNS - N/A
 * DESCRIPTION - 
 *	 A twitter-esque character counter for the input field is updating using this, and if the
 *   Enter key is pressed, the field is cleared and the string is processed.
 ********************************************************************************************/
function updateCharsLeft( event ) {

	var inputString = document.getElementById("inputString").value;
	var retStr = inputString.length + "/255 characters in length";

	if ( inputString.length < 256 ) {
		if ( event.keyCode == 0x0D && inputString != "" ) {
		
			processString( inputString );
			document.getElementById("inputString").value = "";
		}

		document.getElementById("charsLeft").style.fontWeight = "normal";
		document.getElementById("charsLeft").style.color = "#000040";
		document.getElementById("inputString").style.backgroundColor = "#FFFFFF";

	} else {

		document.getElementById("charsLeft").style.fontWeight = "bold";
		document.getElementById("charsLeft").style.color = "#800000";
		document.getElementById("inputString").style.backgroundColor = "#FFDDDD";
	}

	document.getElementById("charsLeft").innerHTML = retStr;
}


/********************************************************************************************
 * FUNCTION - processString()
 * PARAMETERS - inputString
 * RETURNS - N/A
 * DESCRIPTION - 
 *	 This function randomly determines the position, font size, and face of the new string,
 *	 tests for collisions with other elements, and if all passes, it renders to the window.
 *   If the new element collides, the position is randomized again until it finds a place. It
 *	 will test for a maximum of 100 times before alerting the user.
 ********************************************************************************************/
function processString( inputString ) {

	var escStr = escapeHtml( inputString );

	do {
		var fontSize = Math.floor(Math.random()*125 + 3)+"px";
		var fontFace = Fonts[ Math.floor(Math.random() * Fonts.length) ].faces;

		var strWidth = escStr.pixelWidth(fontSize, fontFace);
		var strHeight = escStr.pixelHeight(fontSize, fontFace);

		var posX = Math.floor(Math.random() * (window.innerWidth-strWidth) );
		var posY = Math.floor(Math.random() * (window.innerHeight-strHeight) );

	} while ( checkCollision( posX, posY, strWidth, strHeight ) == true );

	var strDiv = document.createElement("div");

	strDiv.id = "strDiv"+Boxes.length;
	strDiv.className = "strDiv";
	strDiv.style.left = posX + "px";
	strDiv.style.top = posY + "px";
	strDiv.style.fontSize = fontSize;
	strDiv.style.fontFamily = fontFace;
	strDiv.style.color = "#000040";
	strDiv.innerHTML = escStr;

	var body = document.getElementById("main");
	body.appendChild(strDiv);
}


/********************************************************************************************
 * FUNCTION - checkCollision()
 * PARAMETERS - _x, _y, _w, _h
 * RETURNS - true/false whether a collision was detected
 * DESCRIPTION - 
 *	 This function is a simplified Axis-Aligned Bounding Box algorithm that steps through
 *	 all of the page elements and determines whether there are any overlaps. If there are,
 *	 it returns true, collision has been detected. Otherwise, it returns false and adds the
 *	 new bounding box to the list for later checks.
 ********************************************************************************************/
function checkCollision( _x, _y, _w, _h ) {

	var collisionFound = false;

	if ( Boxes.length == 0 ) {
		initCollisionBoxes();
	}

	for ( i = 0; i < Boxes.length; ++i ) {

		if ( ( _x < (Boxes[i].x + Boxes[i].w) ) && ( Boxes[i].x < (_x + _w) ) &&
			 ( _y < (Boxes[i].y + Boxes[i].h) ) && ( Boxes[i].y < (_y + _h) ) ) {

			collisionFound = true;
		}
	}

	if ( collisionFound == false ) {

		var newBox = { x: _x, y: _y, w: _w, h: _h };
		Boxes.push( newBox );
	}

	return collisionFound;
}


/********************************************************************************************
 * FUNCTION - escapeHtml()
 * PARAMETERS - str
 * RETURNS - Escaped string
 * DESCRIPTION - 
 *		A function that accepts a string variable and escapes any pesky HTML-relevant
 *		characters before they can begin injecting themselves into the page.
 * 
 *		Code provided by Brandon Mintern
 *		http://shebang.brandonmintern.com/foolproof-html-escaping-in-javascript/
 ********************************************************************************************/
function escapeHtml( str ) {

    var div = document.createElement("div");
    var node = document.createTextNode( str );

    div.appendChild( node );
    return div.innerHTML;
}
 

/********************************************************************************************
 * FUNCTION - unescapeHtml()
 * PARAMETERS - str
 * RETURNS - Unescaped string
 * DESCRIPTION - 
 *		A function that accepts a string variable and unescapes any previously escaped
 *		HTML-relevant characters, returning the string to its original state.
 *		NOTE: This function is unsafe with strings that have not been processed by 
 *		escapeHtml().
 * 
 *		Code provided by Brandon Mintern
 *		http://shebang.brandonmintern.com/foolproof-html-escaping-in-javascript/
 ********************************************************************************************/
function unescapeHtml( escapedStr ) {

    var div = document.createElement("div");
    div.innerHTML = escapedStr;

    var child = div.childNodes[0];
    return child ? child.nodeValue : '';
}


/********************************************************************************************
 * FUNCTION - String.pixelWidth()
 * PARAMETERS - N/A
 * RETURNS - Container's width in pixels
 * DESCRIPTION - 
 *		A prototype function applied to strings that will determine the width in pixels
 *		of the member string.
 * 
 *		Code provided by Waldek Mastykarz
 *		http://blog.mastykarz.nl/measuring-the-length-of-a-string-in-pixels-using-javascript/
 ********************************************************************************************/
String.prototype.pixelWidth = function( fontSize, fontFace ) {

 	var ruler = document.getElementById("ruler");
 	ruler.innerHTML = this;
 	ruler.style.fontSize = fontSize;
 	ruler.style.fontFamily = fontFace;

 	return ruler.offsetWidth;
}


/********************************************************************************************
 * FUNCTION - String.pixelHeight()
 * PARAMETERS - N/A
 * RETURNS - Container's height in pixels
 * DESCRIPTION - 
 *		A prototype function applied to strings that will determine the height in pixels
 *		of the member string.
 * 
 *		Code provided by Waldek Mastykarz
 *		http://blog.mastykarz.nl/measuring-the-length-of-a-string-in-pixels-using-javascript/
 ********************************************************************************************/
String.prototype.pixelHeight = function( fontSize, fontFace ) {

 	var ruler = document.getElementById("ruler");
 	ruler.innerHTML = this;
 	ruler.style.fontSize = fontSize;
 	ruler.style.fontFamily = fontFace;

 	return ruler.offsetHeight;
}
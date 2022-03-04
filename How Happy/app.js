var cvs = $('#happy-cvs')[0];
var ctx = cvs.getContext('2d');
cvs.clearCanvas = function() {
  ctx.clearRect(0,0,this.width,this.height);
};

var isBlinking = false,
    blinkCount = 5,
    touchFlag = false,
    touchPt = 0;

var moodValue = 50;
var mouthWidth = cvs.width/8;
var mouth = {
  leftCorner:  { x: cvs.width/2 - mouthWidth/2, y: cvs.height*4/7 },
  middle:      { x: cvs.width/2,                y: cvs.height*moodValue/100 },
  rightCorner: { x: cvs.width/2 + mouthWidth/2, y: cvs.height*4/7 }
};

function handleMoodSwing( evt ) {
  evt.preventDefault();
  var y_coordinate = evt.pageY;

  if (!('pageY' in evt) ){
      y_coordinate = evt.changedTouches[0].pageY;
      touchFlag = true;
  }

  var hVal = ((y_coordinate-touchPt)/cvs.height * 100);
  touchPt = y_coordinate;

  if ( touchFlag && hVal+moodValue >= 0 && hVal+moodValue <= 100 ) {
    moodValue += hVal;
    var x = 1-(moodValue/100);
    var r = x < 0.5 ? 255 - 218*x :  292 - 292*x,
        g = x < 0.5 ? 212 +  32*x :  400 - 344*x,
        b = x < 0.5 ? 20*x        : -235 + 490*x;

    cvs.style.backgroundColor = 'rgb( ' + Math.floor(r) + ', ' + Math.floor(g) + ', ' + Math.floor(b) + ' )';
    $('#happiness-score').html( Math.floor( (1-x) * 100 ) );
  }
}

function touchDown( evt ) {
  touchFlag = true;
  touchPt = evt.pageY === 0 ? evt.changedTouches[0].pageY : evt.pageY;

  $('#happiness-score').css('opacity', '1');
}

function touchUp( evt ) {
  touchFlag = false;
  $('#happiness-score').css('opacity', '0');
}

function initMoodFace() {
  if ( window.location.search !== '' ) {

    var param = $.grep(window.location.search.split(/[?=]+/),function(n){ return(n) });

    if ( param[0] === 'h' && !isNaN(parseFloat(param[1])) && isFinite(param[1]) ) {
      moodValue = parseInt( param[1] );
      var dummyEvt = { pageY: moodValue*cvs.height/100, preventDefault: function() {return true} };
      touchPt = dummyEvt.pageY;
      touchFlag = true;
      handleMoodSwing( dummyEvt );
      touchFlag = false;
    }
  }

  if ( ctx ) {
    cvs.addEventListener( 'mousedown', touchDown, false );
    cvs.addEventListener( 'touchstart', touchDown, false );
    cvs.addEventListener( 'mouseup',   touchUp, false );
    cvs.addEventListener( 'mouseout',  touchUp, false );
    cvs.addEventListener( 'touchend',   touchUp, false );
    cvs.addEventListener( 'mousemove', handleMoodSwing, false );
    cvs.addEventListener( 'touchmove', handleMoodSwing, false );

    setTimeout(setBlink, Math.random()*5000 + 2000);
    setInterval(draw, 30);
  }
}

function setBlink() {
  isBlinking = true;
  setTimeout(setBlink, Math.random()*5000 + 2000);
}

function draw() {
  mouth.middle.y = (moodValue/4 + 45)*cvs.height/100;
  cvs.clearCanvas();

  ctx.rect( 0, 0, cvs.width, cvs.height );
  ctx.fillStyle = cvs.style.backgroundColor || '#92e90a';
  ctx.lineWidth = 0;
  ctx.fill();

  drawEye( cvs.width*3/7, cvs.height*3/8, 30 ); // LEFT
  drawEye( cvs.width*4/7, cvs.height*3/8, 30 ); // RIGHT
  drawMouth();

  if ( isBlinking ) {
    blinkCount--;

    if ( !blinkCount ) {
      isBlinking = false;
      blinkCount = 5;
    }
  }
}

function drawEye( x, y, rad ) {
  ctx.save();

  if ( isBlinking ) {
    ctx.beginPath();
    ctx.moveTo( x-rad, y );
    ctx.lineTo( x+rad, y );

    ctx.strokeStyle = 'white';
    ctx.lineWidth = 10;
    ctx.stroke();

  } else {
    ctx.scale(1, 1.2);
    ctx.beginPath();
    ctx.arc( x, y/1.2, rad/1.2, 0, Math.PI*2, true );

    ctx.restore();

    ctx.fillStyle = 'white';
    ctx.lineWidth = 0;
    ctx.fill();

    var happy = moodValue/100;

    if ( happy > 0.5 ) {
      ctx.save();

      ctx.beginPath();
      ctx.arc( x+(cvs.width/2 - x)/18, y+(rad*2*(1-happy) + 30), rad, 0, Math.PI*2, false );

      ctx.restore();

      ctx.fillStyle = cvs.style.backgroundColor || '#92e90a';
      ctx.lineWidth = 0;
      ctx.fill();

    } else {
      ctx.save();

      ctx.beginPath();
      ctx.arc( x-(cvs.width/2 - x)/12, y-(rad*2*happy + 30), rad, 0, Math.PI*2, true );

      ctx.restore();

      ctx.fillStyle = cvs.style.backgroundColor || '#92e90a';
      ctx.lineWidth = 0;
      ctx.fill();
    }
  }
}

function drawMouth() {

  ctx.beginPath();
  ctx.strokeStyle = 'white';
  ctx.moveTo( mouth.leftCorner.x, mouth.leftCorner.y );
  ctx.bezierCurveTo( mouth.leftCorner.x + mouthWidth/8, mouth.middle.y,
                     mouth.rightCorner.x-mouthWidth/8, mouth.middle.y,
                     mouth.rightCorner.x, mouth.rightCorner.y );
  ctx.lineWidth = 10;
  ctx.stroke();
}

let facialRecognitionCallback = null;

function initSharedFaceRecognizer() {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('/faceworker.js', { scope: './' }).then(function (registration) {
            console.log('ServiceWorker registration successful with scope: ', registration.scope);
        }, function (err) {
            // registration failed :(
            console.log('ServiceWorker registration failed: ', err);
        })
    }
}

function initCanvas(element) {
    const canvas = document.getElementById("boxesCanvas");
    canvas.width = element.videoWidth;
    canvas.height = element.videoHeight;
    return canvas;
}

// inform c# number of faces
async function faceRecognitionSetCallback(cameraCallbackHelper) {
    facialRecognitionCallback = cameraCallbackHelper;
}

async function reportNumberOfFaces(numberOfFaces) {
    facialRecognitionCallback.invokeMethodAsync('onFrameMeta', numberOfFaces)
}

let captureCanvas = null;
async function doFaceRecognition(videoElement, overlayCanvas) {
    let canvas = captureCanvas = new OffscreenCanvas(videoElement.videoWidth, videoElement.videoHeight);
    let context = canvas.getContext('2d');
    const messageChannel = new MessageChannel();
    let overlayContext = overlayCanvas.getContext('2d');
    messageChannel.port1.onmessage = function (event) {
        if (!Array.isArray(event.data.detections)) {
            reportNumberOfFaces(0);
            return;
        }
        overlayContext.clearRect(0, 0, overlayCanvas.width, overlayCanvas.height);
        const detections = event.data.detections;
        for (let detection of detections) {
            overlayContext.beginPath();
            overlayContext.lineWidth = "1";
            overlayContext.strokeStyle = "green";
            overlayContext.rect(detection._box._x, detection._box._y, detection._box._width, detection._box._height);
            overlayContext.stroke();
        }
        if (typeof reportNumberOfFaces == 'function') {
            reportNumberOfFaces(detections.length);
        }
    }
    await navigator.serviceWorker.ready
    navigator.serviceWorker.controller.postMessage({ message: "INIT_SERVICEWORKER", width: videoElement.videoWidth, height: videoElement.videoHeight }, [messageChannel.port2]);
    return (e) => {
        context.drawImage(videoElement, 0, 0);
        const imgData = context.getImageData(0, 0, videoElement.videoWidth, videoElement.videoHeight);
        messageChannel.port1.postMessage({
            type: 'send-img-data',
            w: videoElement.videoWidth,
            h: videoElement.videoHeight,
            buffer: imgData.data.buffer
        }, [imgData.data.buffer])
    }
}

let captureResult = null;
function captureFrameForCs() {
    return new Promise((accept, reject) => {
        captureCanvas.convertToBlob().then((blob) => {
            let reader = new FileReader();
            reader.readAsArrayBuffer(blob);
            reader.onloadend = function () {
                captureResult = new Uint8Array(reader.result);
                accept(captureResult.length);
                // accept("ok")
            }
        })
    })
}

function getFrameByParts(offset, size = 8192) {
    let end = Math.min(offset + size, captureResult.length);
    let arrayToSend = captureResult.slice(offset, end);
    let json = [].slice.call(arrayToSend);
    console.log(json);
    return json;
}

async function onCameraFrameReady(element) {
    const overlayCanvas = initCanvas(element)
    element.ontimeupdate = await doFaceRecognition(element, overlayCanvas);
    // element.addEventListener('timeupdate', doFaceRecognition(element), false);
}

let _cameraStream = null;
async function faceRecognitionUi(elementSelector) {
    const videoEl = document.getElementById(elementSelector);
    _cameraStream = await navigator.mediaDevices.getUserMedia({ video: {} })
    videoEl.srcObject = _cameraStream;
}

async function disposeFacialCameraUi() {
    _cameraStream.getTracks().forEach(function (track) {
        track.stop();
    });
}
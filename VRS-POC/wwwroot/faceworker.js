
Canvas = HTMLCanvasElement = OffscreenCanvas;

global = {
    fetch: fetch
}
importScripts("/scripts/face-api.js")
faceapi.env.setEnv(faceapi.env.createNodejsEnv());
faceapi.env.monkeyPatch({
    Canvas: OffscreenCanvas,
    createCanvasElement: () => {
        return new OffscreenCanvas(480, 270);
    },
});

async function onNewChannelRequest(event) {
    const width = event.data.width;
    const height = event.data.height;
    const canvas = new HTMLCanvasElement(width, height);
    const context = canvas.getContext('2d');
    const imageData = context.createImageData(width, height);
    await faceapi.nets.tinyFaceDetector.load("/resources");
    const options = new faceapi.TinyFaceDetectorOptions({ inputSize : 160, scoreThreshold: 0.5 })
    event.ports[0].onmessage = async function (frameEvent) {
        const arr = new Uint8ClampedArray(frameEvent.data.buffer);
        imageData.data.set(arr);
        context.putImageData(imageData, 0, 0);
        let detections = await faceapi.detectAllFaces(canvas, options);
        event.ports[0].postMessage({detections})
    }
}

self.addEventListener('message', async function (event) {
    if (event.data.message == "INIT_SERVICEWORKER") {
        console.log("Worker received request to perform Facial Recognition");
        onNewChannelRequest(event);
    }
});
"use strict";
function clickElement(element) {
    element.click();
}
function downloadFromUrl(options) {
    var _a;
    var anchorElement = document.createElement('a');
    anchorElement.href = options.url;
    anchorElement.download = (_a = options.fileName) !== null && _a !== void 0 ? _a : '';
    anchorElement.click();
    anchorElement.remove();
}
function downloadFromByteArray(options) {
    // The byte array in .NET is encoded to base64 string when it passes to JavaScript.
    // So we can pass that base64 encoded string to the browser as a "data URL" directly.
    var url = "data:" + options.contentType + ";base64," + options.byteArray;
    downloadFromUrl({ url: url, fileName: options.fileName });
}

function getDownloadURL(options) {
    var url = "data:" + options.contentType + ";base64," + options.byteArray;
    return url;
}

window. Alert = function(message) {
    alert(message);
}

function initAudio(element, reference){
    element.addEventListener("ended", async e => {
        await reference.invokeMethodAsync("OnEnd");
    });
}

function playAudio(element) {
    stopAudio(element);
    element.play();
}

function stopAudio(element) {
    element.pause();
    element.currentTime = 0;
}
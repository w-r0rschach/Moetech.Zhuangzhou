// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var scheme = document.location.protocol == "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";
var url = scheme + "://" + document.location.hostname + port;
var socket;
//start()
function start() {
    socket = new WebSocket(url);
    socket.onopen = function (event) {
    }

    socket.onmessage = function (event) {
    }
}
function send() {
    var data = document.getElementById("sendtxt").value;
    socket.send(data);
}

// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var scheme = document.location.protocol == "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";
var url = scheme + "://" + document.location.hostname + port +"/WebSocketHandle";
var socket;
var warns = document.getElementById("warns");
function start() {
    socket = new WebSocket(url);
    socket.onopen = function (event) {
        socket.send(123);
        alert("连接成功")
    }

    socket.onclose = function (event) {
        socket = new WebSocket(url);//断开后重连
        alert("连接断开")
    }

    socket.onmessage = function (event) {
        var datas = event.data;   
        var jsonObj = JSON.parse(datas);
        //warns.innerHTML += '<a href="#" class="dropdown-item">'
        //    + '<div class="media"><div class="media-body">'
        //    + '<h6 class="dropdown-item-title">' + jsonObj[i].MessageTitle + '</h6>'
        //    + '<p style="font-size:12px;color:#a5a5a5">' + jsonObj[i].MessageContent + '</p>'
        //    + '<p>' + jsonObj[i].MessageWarnDate + '</p>'
        //    + '</div></div></a>';
    }
}

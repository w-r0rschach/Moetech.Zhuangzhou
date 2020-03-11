var port = document.location.port ? (":" + document.location.port) : "";
var url = "https://" + document.location.hostname + port;
var MessageWarns = document.getElementById("messageWarns");
var Warns = document.getElementById("warns");
function Init() {
    setTimeout(Init, 10 * 1000);
    $.ajax({
        url: url + "/Vmware/SelectRemain",
        type: "post",
        dataType: "json",
        success: function (data) {
            var j = data.length;
            MessageWarns.innerText = j;
            Warns.innerHTML = "";
            for (var i = 0; i < data.length; i++) {
                Warns.innerHTML += '<a href="/Vmware/RedateRemain?id=' + data[i].messageId + '" class="dropdown-item">'
                    + '<div class="media"><div class="media-body">'
                    + '<h6 class="dropdown-item-title">' + data[i].messageTitle + '</h6>'
                    + '<p style="font-size:12px;color:#a5a5a5">' + data[i].messageContent + '</p>'
                    + '<p> ' + data[i].messageWarnDate + '</p>'
                    + '</div></div></a>';
            }
        }
    });
}
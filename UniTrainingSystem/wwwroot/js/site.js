

var connectio = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connectio.on("ReceliveMessage", function (fromUser, message) {
    var msg = fromUser + ":" + message;
    var li = document.createElement("li");
    li.textContent = msg;
    $("#list").prepend(li);
});
connectio.start();
$("#btnSend").on("click", function () {
    var fromUser = $("#txtuser").val();
    var message = $("#txtMsg").val();

    connectio.invoke("SendMessage", fromUser,message);
});

"use strict";

// Initialize SignalR connection
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var sendButton = document.getElementById("sendButton");
// Check if the element exists before setting properties
if (sendButton) {
    sendButton.disabled = true;
}

// Handle incoming messages
connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = `${user} says ${message}`;
});

var notificationCount = 0;

// Handle incoming notifications
connection.on("ReceiveNotification", function (message) {
    var notificationsList = document.getElementById("notificationsList");
    var notificationCountElement = document.getElementById("notificationCount");

    var li = document.createElement("li");
    li.classList.add("dropdown-item");
    li.textContent = message;

    notificationsList.insertBefore(li, notificationsList.firstChild);

    notificationCount++;
    notificationCountElement.textContent = notificationCount;
    notificationCountElement.classList.remove("d-none");
});


// Start the connection
connection.start().catch(function (err) {
    return console.error(err.toString());
});


// Handle Like button click
document.querySelectorAll(".btn-like").forEach(function (button) {
    button.addEventListener("click", function () {
        var userId = button.getAttribute("data-user-id");
        var message = "Like your post!";
        connection.invoke("SendNotification", userId, message).catch(function (err) {
            return console.error(err.toString());
        });
    });
});

// Handle Dislike button click
document.querySelectorAll(".btn-dislike").forEach(function (button) {
    button.addEventListener("click", function () {
        var userId = button.getAttribute("data-user-id");
        var message = "Dislike your post!";
        connection.invoke("SendNotification", userId, message).catch(function (err) {
            return console.error(err.toString());
        });
    });
});



"use strict";

// Initialize SignalR connection
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

// Handle incoming messages
connection.on("ReceiveMessage", function (senderId, name, content, timestamp) {
    var messagesList = document.getElementById("messagesList");
    var li = document.createElement("li");

    var currentUserId = "@(User.FindFirstValue(ClaimTypes.NameIdentifier))"; // Get the current user ID from the server

    // Determine the display name based on the sender and receiver
    var displayName = senderId === currentUserId ? "Me" : name;

    li.innerHTML = `<strong>${displayName}:</strong> ${content} <br /><small>${new Date(timestamp).toLocaleString()}</small>`;
    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight;
});

// Handle incoming notifications
connection.on("ReceiveNotification", function (senderName, message, createdAt) {
    var notificationsList = document.getElementById("notificationsList");
    var notificationCountElement = document.getElementById("notificationCount");

    var date = new Date(createdAt);
    var formattedDate = date.toLocaleString(); // Adjust formatting as needed

    var li = document.createElement("li");
    li.classList.add("dropdown-item");
    li.textContent = `${senderName} - ${message} (Received at: ${formattedDate})`;

    notificationsList.insertBefore(li, notificationsList.firstChild);

    var notificationCount = parseInt(notificationCountElement.textContent) || 0;
    notificationCount++;
    notificationCountElement.textContent = notificationCount;
    notificationCountElement.classList.remove("d-none");
});

// Handle chat room opened
connection.on("ChatRoomOpened", function (groupName) {
    alert(`Chat room opened: ${groupName}`);
    window.location.href = `/Chat/Room?userId1=${encodeURIComponent(groupName.split('-')[0])}&userId2=${encodeURIComponent(groupName.split('-')[1])}`;
});

// Start the SignalR connection
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

// Handle sending messages in the chat room
document.getElementById("sendButton").addEventListener("click", function () {
    var messageContent = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", "@(userId2)", messageContent).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("messageInput").value = "";
});

// Scroll to the bottom of the messages list on page load
window.onload = function () {
    var messagesList = document.getElementById("messagesList");
    messagesList.scrollTop = messagesList.scrollHeight;
};

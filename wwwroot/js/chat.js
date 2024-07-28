"use strict";

// Initialize SignalR connection
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

// Handle incoming messages
connection.on("ReceiveMessage", function (senderId, content, timestamp) {
    console.log("Received message from:", senderId, "Content:", content, "Timestamp:", timestamp);
    var messagesList = document.getElementById("messagesList");
    var li = document.createElement("li");

    var currentUserId = document.getElementById("currentUserId").value; // Get the current user ID from a hidden field
    var displayName = senderId === currentUserId ? "Me" : document.getElementById("receiverName").value; // Get receiver name from a hidden field

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
    console.error(err.toString());
    alert("Failed to connect to chat server. Please try again later.");
});

// Handle Like button click
document.querySelectorAll(".btn-like").forEach(function (button) {
    button.addEventListener("click", function () {
        var userId = button.getAttribute("data-user-id");
        var message = "Like your post!";
        connection.invoke("SendNotification", userId, message).catch(function (err) {
            console.error(err.toString());
            alert("Failed to send notification. Please try again.");
        });
    });
});

// Handle Dislike button click
document.querySelectorAll(".btn-dislike").forEach(function (button) {
    button.addEventListener("click", function () {
        var userId = button.getAttribute("data-user-id");
        var message = "Dislike your post!";
        connection.invoke("SendNotification", userId, message).catch(function (err) {
            console.error(err.toString());
            alert("Failed to send notification. Please try again.");
        });
    });
});

// Handle sending messages in the chat room
document.getElementById("sendButton").addEventListener("click", function () {
    var messageContent = document.getElementById("messageInput").value;
    var receiverId = document.getElementById("receiverId").value; // Receiver ID from a hidden field
    console.log("Sending message to:", receiverId, "Content:", messageContent);

    connection.invoke("SendMessage", receiverId, messageContent).catch(function (err) {
        console.error(err.toString());
        alert("Failed to send message. Please try again.");
    });

    document.getElementById("messageInput").value = "";
});
// Scroll to the bottom of the messages list on page load
window.onload = function () {
    var messagesList = document.getElementById("messagesList");
    messagesList.scrollTop = messagesList.scrollHeight;
};

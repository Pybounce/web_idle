var http_button = document.getElementById("http_button");
http_button.addEventListener("click", function () {
  fetch("http://localhost:5042/Test/PostTest", {
    method: "POST",
    body: JSON.stringify({
      text: "hello world",
    }),
    headers: {
      "Content-type": "application/json; charset=UTF-8",
    },
  });
});

const socket = new WebSocket("ws://localhost:5042/ws");
socket.addEventListener("open", (event) => {
  socket.send("Hello World!");
});
socket.onmessage = (event) => {
  console.log("ws message from server: ", event.data);
};

socket.addEventListener("close", (event) => {
  console.log("web socket closed");
});

var ws_button = document.getElementById("ws_button");
ws_button.addEventListener("click", function () {
  if (ws_button.classList.contains("active")) {
    ws_button.classList.add("inactive");
    ws_button.classList.remove("active");
  } else {
    ws_button.classList.add("active");
    ws_button.classList.remove("inactive");
  }
  socket.send("Web Socket Ping!");
});

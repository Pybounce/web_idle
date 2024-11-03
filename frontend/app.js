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

//-----------------------------------------------------------------------------------------

var resourceNodes = document.getElementsByClassName("resourceNode");

function disableActiveResourceNodes() {
  //prettier-ignore
  var activeResourceNodes = document.getElementsByClassName("resourceNode active");

  for (var i = 0; i < activeResourceNodes.length; i++) {
    socket.send(
      createStopResourceHarvestMessage(activeResourceNodes[i].dataset.nodeId)
    );
    activeResourceNodes[i].classList.add("inactive");
    activeResourceNodes[i].classList.remove("active");
  }
}

for (var i = 0; i < resourceNodes.length; i++) {
  resourceNodes[i].addEventListener("click", function (event) {
    if (event.target.classList.contains("active")) {
      disableActiveResourceNodes();
    } else {
      disableActiveResourceNodes();
      event.target.classList.add("active");
      event.target.classList.remove("inactive");
      socket.send(
        createStartResourceHarvestMessage(event.target.dataset.nodeId)
      );
    }
  });
}

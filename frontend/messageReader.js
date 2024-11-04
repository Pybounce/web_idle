socket.onmessage = (event) => {
  handleMessage(JSON.parse(event.data));
  console.log("ws message from server: ", event.data);
};

socket.addEventListener("close", (event) => {
  console.log("web socket closed");
});

function handleMessage(message) {
  console.log("message " + message.MessageType);
  switch (message.MessageType) {
    case ReadMessageTypes.ItemCollected:
      inventory.addItem(message.ItemId, message.Amount);
      inventory.log();
      break;
    default:
      console.log("you fucked up");
      break;
  }
}

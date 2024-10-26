var btn = document.getElementById("button");
btn.addEventListener("click", function () {
  alert("test");
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

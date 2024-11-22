class AppManager {
  constructor() {
    this.loginManager = new LoginManager();
    this.gameManager = new GameManager();
  }
  showAuth() {
    this.loginManager.activate();
    this.gameManager.deactivate();
  }
  showGame() {
    this.loginManager.deactivate();
    this.gameManager.activate();
  }
}

class LoginManager {
  constructor() {
    this.container = document.getElementById("auth_login_container");
    this.deactivate();
    this.isActive = false;
  }
  deactivate() {
    if (!this.isActive) {
      return;
    }
    this.isActive = false;
    this.container.style.display = "none";

    this.isActive = false;
  }
  activate() {
    if (this.isActive) {
      return;
    }
    this.container.style.display = "block";
    document.getElementById("login_btn").addEventListener("click", (e) => {
      e.stopPropagation();
      this.login();
    });
    document
      .getElementById("create_account_btn")
      .addEventListener("click", (e) => {
        e.stopPropagation();
        this.createAccount();
      });
    this.isActive = true;
  }

  login() {
    var username = document.getElementById("login_username").value;
    var password = document.getElementById("login_password").value;
    console.log(username);
    fetch("http://localhost:5042/User/Login", {
      method: "POST",
      body: JSON.stringify({
        username: username,
        password: password,
      }),
      headers: {
        "Content-type": "application/json; charset=UTF-8",
      },
    })
      .then((response) => response.text())
      .then((body) => console.log(body))
      .catch((e) => console.log("login error"));
  }
  createAccount() {
    var username = document.getElementById("create_username").value;
    var password = document.getElementById("create_password").value;
    console.log(username);
    fetch("http://localhost:5042/User/CreateAccount", {
      method: "POST",
      body: JSON.stringify({
        username: username,
        password: password,
      }),
      headers: {
        "Content-type": "application/json; charset=UTF-8",
      },
    })
      .then((response) => response.text())
      .then((body) => console.log(body))
      .catch((e) => console.log("create error"));
  }
}

class GameManager {
  constructor() {
    this.container = document.getElementById("game_container");
    this.deactivate();
    this.isActive = false;
  }
  deactivate() {
    if (!this.isActive) {
      return;
    }
    this.container.style.display = "none";
    this.isActive = false;
  }
  activate() {
    if (this.isActive) {
      return;
    }
    this.container.style.display = "block";
    this.isActive = true;
  }
}

var app = new AppManager();
app.showGame();

//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//-----------------------------------------------------------------------------------------
// ALL OLD BELOW

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

var close_socket_btn = document.getElementById("close_socket_button");
close_socket_btn.addEventListener("click", function () {
  socket.close();
});

//-----------------------------------------------------------------------------------------

var resourceNodes = document.getElementsByClassName("resourceNode");

function disableActiveResourceNodes() {
  //prettier-ignore
  var activeResourceNodes = document.getElementsByClassName("resourceNode active");

  for (var i = 0; i < activeResourceNodes.length; i++) {
    socket.send(
      createStopResourceHarvestMessage(
        activeResourceNodes[i].dataset.resourceId
      )
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
        createStartResourceHarvestMessage(event.target.dataset.resourceId)
      );
    }
  });
}

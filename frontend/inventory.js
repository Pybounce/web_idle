class Inventory {
  constructor() {
    this.items = new Map();
  }

  addItem(itemId, amount) {
    if (!amount) {
      amount = 1;
    }
    if (this.items.has(itemId)) {
      this.items.set(itemId, this.items.get(itemId) + amount);
    } else {
      this.items.set(itemId, amount);
    }
  }
  hasItem(itemId) {
    return this.items.has(itemId);
  }
  removeItem(itemId) {
    if (this.items.has(itemId)) {
      this.items.set(itemId, this.items.get(itemId) - 1);
      if (this.items.get(itemId) <= 0) {
        this.items.delete(itemId);
      }
    }
  }
  log() {
    console.log("__Inventory__");
    this.items.keys().forEach((key) => {
      console.log("Key: " + key + " Val: " + this.items.get(key));
    });
  }
}

var inventory = new Inventory();

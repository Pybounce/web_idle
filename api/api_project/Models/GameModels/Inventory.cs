

public class Inventory {
    private Dictionary<int, int> _items;

    public Inventory() {
        _items = new Dictionary<int, int>();
    }

    public Inventory(Dictionary<int, int> items) {
        _items = items;
    }

    public void AddItem(int itemId, int count = 1) {
        if (!_items.ContainsKey(itemId)) {
            _items.Add(itemId, count);
        }
        else {
            _items[itemId] += count;
        }
    }
    public int ItemCount(int itemId) {
        if (_items.TryGetValue(itemId, out int itemCount)) {
            return itemCount;
        }
        return 0;
    }
    public void RemoveItems(int itemId, int count = 1) {
        if (_items.ContainsKey(itemId)) {
            _items[itemId] -= count;
            if (_items[itemId] < 0) {
                _items[itemId] = 0;
            }
        }
    }

    /// <summary>
    /// Returns a copy of all items in the inventory
    /// </summary>
    public Dictionary<int, int> GetItems() {
        return new Dictionary<int, int>(_items);
    }
}


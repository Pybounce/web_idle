## Tests

Architecture:

- Move api project into new folder inside api called project
- create new folder in api named tests
- create new project for tests

Actual Tests:

- Subscribe to new type
- Subscribe to existing type
- Publish to type with many subscribers (ensure each sub gets the event)
- Subscribe to type with same function twice
- Unsubscribe a function that was subscribed
- Unsubscribe without subscribing
- Subscribe 1 method each from 2 instances, unsub twice on one, ensure other instance's method is still subbed
- Publish to an event with 0 subscribers

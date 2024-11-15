#### ToDo

- [ ] Static data loading
  - Must be loaded on web server start and stay
  - Must be accessible for all other services
- [ ] Loot table service
  - Pretty simple once the static loot table data can be loaded in
  - Save different itemIds to db
- [ ] Load in player inventory when they connect
  - Probably the SaveSystem will do this
  -

#### To Think About

- Do I just make every service a singleton and handle playerIds? (probably not)

### Database

#### Needs to store:

    - Player data:
        - Name
        - Hashed password
        - Individual skill xp (levels can be calculated through code):
            - skillId
            - xp
        - Inventory
        - Name (details in general)
    - Static data: (config)
        - Resource Node Info:
            - resourceNodeId
            - skillId
            - ingredients
            - drop table (List<itemId, chance>) -> chances all run one after another so can get multiple low chances in one go
            - name?
            - duration
            - requirements (xp or other reqs?)

#### Async or Not Async

    - Some things should run in the background such as saving to the db
        - Really, the db system should track if it's currently saving, and not initiate another save until this one is finished.
    - Having everything OnTick be async means I will need locks everywhere, which is annoying
    - Even if I made everything async, the db still won't be awaited since it is likely to take longer than a tick.
    - And the code in the server will run fast so does not benefit from running async
    - Invididual players code will be on another thread since the services are scoped
    - HOWEVER: Having everything work async does not add much overhead and would make the code somewhat easier to use since you know it works

### Enchancements

    - Make messages contain the current amount so it's self correcting over time

### Bugs

    - Bug free!

### Possible Future Bugs

    - Some systems require other systems to do things before they can function
        - For example the loot system requires the static data be loaded into the web server before it can check the tables
        - May be something solved when doing auth, but there may also be a way of telling the webapp that it's not truely online until certain startup tasks are done in the singleton services
        - Bigger issue: The singleton service will only startup when another service needs it (ie loot table, ie when a player is online). Can look into making it a hosted service but those are for long running tasks, so who knows.
        - SOLUTION A: Make each service contain an event buffer?
            - If an event cannot be actioned, it stays in the buffer until the service dependancy is rectified
        - BIGGER ISSUE: Better to just have some service say whether or not the player can do stuff
            - Like a SetupService or something
            - Since there are many things that must be setup before we can begin processing events this way
            - Issue with an eventbuffer per consuming service is that the clientWrite will consume and send an ItemGained, but that may not have been written since the SaveSystem hasn't even loaded in the players current inventory

### Static Data

#### General

    - Much data for the game is static for all players, things like the loot tables for example.
    - There should be a way of getting that data into the webserver without a new player doing it every time

### Solution A: Singleton Service

    - A singleton service that will get a db client, load in the static data on startup, and then dispose of the db client
        - No point keeping a db client active if it's done fetching the data it needs

### Loot Table and System

#### Database Collection Stores:

    - ResourceNodeId
    - List<(ItemId, ChanceDenominator (int, chance = 1 / ChanceDenominator))>
    - LootSystem will have access to static data (where this table will lie)
        - Loot system will then run through each and raise events on successful chance outcome.

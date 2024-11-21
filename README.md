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

    - [ ] Make messages contain the current amount so it's self correcting over time
    - [ ] Inject container and db names as env variables
    - [ ] Tickables across users are run synchonously, each user's tickables can be run in parallel
    - [ ] Make db systems singletons
        + Would save fetching the container every request
        - Wouldn't make a huge difference to a persistent websocket
        - Less secure, probably
    - [ ] Make logic systems separate from db systems
        - Right now authAPI talks to authDb, which could itself be userDb with some authSystem for logic.

### Bugs

    - Bug free!

### Loading Saved Data

    - When the player logs in, they should load in their inventory and skillventory
    - Before this is loaded, they will not be able to harvest any resources since the ResourceHarvester won't know what level they are
    - There needs to be something similar to the setup data services, but per request
    - MORE IN AUTH BELOW

### Auth

    - Normal controller endpoint for auth login
        - Takes in username and hashed password, returns authToken
    - Then the game websocket is opened, and before it initialises anything, it will verify the auth token
    - Once the auth token is varified, it will load in user data to the webserver
    - Once the user data is loaded in, it will add the remaining services with serviceProvider
    - Once finished, it will send a message to the client that login was successful
    - Client can then send messages like StartResourceHarvest etc
    - NOTE: On each new message from the client, the auth token must be varified

    - Can use json web tokens (jwt) for auth.
        - Should mean that the webserver doesn't have to save anything additional.
        - Client is giving the JWT, and then starts sending it with every ws request (or any request)

    - If a websocket connection is made without a JWT, reject it
        - Since we need to know the user before starting up all the services so we can load in appropriate data first
    - Also, obviously if any request, barring login and few others, that isn't with an auth token, reject it.

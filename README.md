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

#### Code Architecture

    - DbWriter and DbReader
    - Writer will have access to Reader, Reader won't have access to Writer

### Fixes?

#### Possible action lag fix

    - The issue is that the client recieves a message saying they gained some xp before the server has even written it to the db. So if they level up and instantly move to a new resource, it might come back saying they are under levelled.
    - Have it so the xp requirements for each new resource in a skill, are a little lower
    - For example, they are actually -1000xp lower than they say on the frontend
    - The frontend may say Level 60 required, but it would actually be level 59, 99% of the way through
    - This way no player gains an advantage over another, and the issue of instantly moving to the new resource is LIKELY solved
    - WAIT! I AM A MORON!:
        - If I keep the players current xp in memory on the web server, I can then just check it there, instead of getting it from the db each time.
        - This way in order to sync the client and server, I can just send all the data over to the backend every x minutes?
        - This would also mean that I don't have to make writes every time an action happened, instead I would need a system to track what has changed on the webserver, and periodically send those
        - So I would have a gameplay system take the event and make a change, mark it as changed. Then either it raises events every 100 ticks, that the db writer will then read, or something else idk
        - Pros and Cons:
            + Less writes to db
            + Web server acts as a caching layer and handles all the truth
            - If webserver crashes, progress is lost (though only since last save)
            - This will only work with requests on the websocket (but I might as well make every request on the websocket now)

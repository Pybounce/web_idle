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

#### Db Code Architecture

    - DbWriter and DbReader
    - Writer will have access to Reader, Reader won't have access to Writer

### Code Architecture

#### Systems

    - Systems that store the cached info
    - PlayerLevelSystem could store all the player xp values for each skill
    - UserSystem could contain info on passtoken etc
    - These systems can subscribe to events (PlayerLevelSystem could subscribe to XpGain event etc)
    - PlayerLevelSystem can raise events for things like LevelUp(skillId, level)
    - ResourceDbSystem can hold cached resource node info (and likely be a singleton system)
    - Do I make the UserSystem be UserDb and PlayerLevelSystem be PlayerLevelDb, and then have them send write requests?
        - Since if the PLSystem reads XpGain events and increases it's cached value, then the db also reads this event and saves the new value, it's being calculated in 2 separate places
        - This makes more sense, then the dbReader/Writer are just generic db wrapper
        - The individualDb systems (Player/User etc), will keep track if they need to be saved
        - QUERY: Do these systems just raise events instead of connecting directly to dbreadwrite?
            - Or does dbwrite just read the events on things like xpGained
    - How split should the systems be?
        - Should there be an xp system that tracks all the players xp, and reads events such as GatheredResource(resourceId)
        - Then this system will work out the amount of xp and raise an event GainedXp(skillId, amount)
        - Then another system (LootSystem) will take the GatheredResource(resourceId) event, work out what drops it gives, and raise those events
        - Might need to be called something other than GatheredResource, since you're really just completing a resourceNode? kinda?

    - Okay so some updated thoughts
    - Could have SaveSystem, XpSystem, InventorySystem, LootSystem, DbReader/Writer etc
    - Now the SaveSystem contains the Xp and Inventory systems etc and will extract the changed data
        - This way the entire state can be saved in one transaction
    - ISSUE: Splitting these systems up a lot leads to different data being needed all over.
        - For example if we raise a ResourceComplete event.
        - The XpSystem will work out how much xp should be given and raise XpGained event
        - The loot system will work out what items should be given and raise ItemGained events
        - Some other system will work out what items should be removed, if recipe contained any
            - This would mean the xpsystem needs to know the xp for each item
            - Loot system needs to know the loot table for each resource
            - Some other system will need to know recipe.
        - If these are stored in SQL, they could each be separate rows/tables
            - ItemXp table
            - ResourceLoot table
            - ItemRecipe table
        - So how will these systems get the segmented data
            - On the query, it would make sense to use joins to get all this info for each item
            - However, since I will need all this info for each item, it may make sense to use NoSQL
        - POSSIBLE SOLUTION:
            - Have a system for dbState
            - This system will contain all data that goes to and from the db
            - Other systems (xpSystem, InventorySystem etc) will read from this system as if it were the db
            - Those other systems can also write to this system.
            - This dbState system will, every now and then, save back to the db

### Saving issue

    - Each system being responsible for saving it's own data could cause inconsistent data
        - For example if one system successfully saves and another doesn't
        - Player may end up with recipe resources gone but no resource gained etc
    - Possible solution to have a SaveSystem, that takes in save events, aggregates them, and saves in one transaction

### DbState Solution

    - System named DbStateSys
    - This system contains the data that is written and read from the db
    - In order to update the db, other services should mutate data in DbStateSys
    - This system will then periodically backup to the db
    - QUERY: How will analytics get the updates?
        - Given the systems just update the cached data directly, there's no way for analytics to consume it.
        - I could raise events for GainXp and such, then have the gamestate and analytics consume them
            - But do I want GameState to have such logic?
        - Also do I want more in depth analytics?
            - Using events this way will mean I gain events for each resource node completed, and each resource gathered, but not the two together.
            - Can make events more robust, to contain what caused them? Ie: an event for XpGained would contain the root event also inside it?
                - But that feels fairly messy
        - Solution A:
            - Make the subsystems such as XpSystem and LootSystem, be directly used by the ResourceNode system
            - This way the event would include all the info (xp, skill, loot)
            - ISSUE: This would make the events fairly large
        - Solution B:
            - Raise the events separately, stop being a little bitch
            - You will need to find a way to save analytics and data at the same time regardless
            - For now, just get the fucking thing working and stop being such a stupid asshole about it. Also, have fun: because otherwise what's the point.

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

### Enchancements

    - Make messages contain the current amount so it's self correcting over time

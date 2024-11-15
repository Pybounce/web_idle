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

#### Enchancements

    - Make messages contain the current amount so it's self correcting over time

#### Bugs

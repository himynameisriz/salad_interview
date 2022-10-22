# Salad Technologies Interview API

This is an example API created to add users, give them games, and compare the games. Operations are detailed below.

# Prerequisites

Please ensure before running this app that, that you have done the following:

1. Get an API key from [rawg's api docs](https://rawg.io/login?forward=developer)
2. Update `appsettings.json` or add `appsettings.Development.json` and update your `rawg` values
3. [Download](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and install the .NET 6 SDK and Runtime 

# How to run 

1. Download the source code via git, or through github directly
2. Run `dotnet run` - alternatively run `dotnet build` first, and `dotnet run --no-build`

# How to use it 
The default port, as seen on `/Properties/launchSettings.json`, is 7121. You can chane this if necessary. Once you have it running, you can access the following controllers, and methods, from the URL `https://localhost:7121`

## Games
### GET /games
Gets all games with the supplied search term. Can be ordered by various means.
#### paramters
    q : string
        The name of the game you want to search
    sort : string
        The sorting style. Must be one of the following
            [-]name
            [-]released
            [-]added
            [-]created
            [-]updated
            [-]rating
            [-]metacritic
#### valid response
    HTTP 200 OK
    [
        {
            "id": 13627,
            "name": "Undertale",
            "added": 6572,
            "metacritic": 92,
            "rating": 4.34,
            "released": "2015-09-14",
            "updated": "2022-10-21T19:25:11"
        }, 
        ...
    ]

## Users
### GET /users
Gets all users and their games
#### valid response
    HTTP 200 OK
    [
        {
            "id": 1,
            "games": [
                {
                    "id": 479694,
                    "name": "Inscryption",
                    "added": 1699,
                    "metacritic": 85,
                    "rating": 4.4,
                    "released": "2021-10-19",
                    "updated": "2022-10-21T14:14:53"
                },
                ...
            ]
        }, 
        ...
    ]

### GET /users/{userId:int}
Gets a specific user, and their games
#### parameters
    userId : int
#### valid response
    HTTP 200 OK
    {
        "id": 1,
        "games": [
            {
                "id": 479694,
                "name": "Inscryption",
                "added": 1699,
                "metacritic": 85,
                "rating": 4.4,
                "released": "2021-10-19",
                "updated": "2022-10-21T14:14:53"
            },
            ...
        ]
    }

### POST /users
Creates a new user within the cache
#### response
    HTTP 201 Created

### POST /users/{userId:int}/games
Stores a game into a specific user's games.
#### parameters
    userId: int
    gameRequestObject: object { gameId: int }
#### valid response
    HTTP 204 No Content
#### possible other responses
    HTTP 400 Bad Request
        Can be because of invalid supplied userId or invalid supplied gameId
    HTTP 409 Conflict
        Trying to a game a second time to a specified user

### DELETE /users/{userId:int}/games/{gameId:int}
Removes a game from a user's games
#### parameters
    userId: int
    gameId: int
#### valid response
    HTTP 204 No Content
#### other responses
    HTTP 404 Not Found
        Can be because of incorrectly supplied userId, gameId, or gameId that the user does not have

### POST /users/{userId:int}/comparison
#### parameters
    userId: int
    gameComparisonRequestObject : object { otherUserId: int, comparison: string }
        comparison string must be one of the following values: 
            union 
            intersection
            difference
#### valid response 
    HTTP 200 OK
    {
        "userId": 2,
        "otherUserId": 1,
        "comparison": "union",
        "games": [
            {
                "id": 58175,
                "name": "God of War",
                "added": 11215,
                "metacritic": 94,
                "rating": 4.59,
                "released": "2018-04-20",
                "updated": "2022-10-21T16:23:21"
            },
            ...
        ]
    }
#### other responses
    HTTP 400 Bad Request
        Because of improper use of comparison parameter
    HTTP 404 Not Found
        Can be because of either an invalid userId or invalid otherUserId

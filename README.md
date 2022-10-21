# In Progress 

## Current implmentations
    ### Games
        GET `/games` - Gets the first game from the results list based on a required query search, with an optional sort [unimplemented]
    ### Users
        GET `/users/{userId}` - Gets user from cache
        POST `/users` - Creates a new user and puts it into cache
        POST `/users/{userId}/games` - Adds a new game to the user, use json body
        DELETE `/users/{userId}/games/{gameId} - Deletes the specified game if it exists in the user's list of games
# ShoppingListServer
https://github.com/danielroth1/ShoppingListApp


## REST
/users/authenticate - public route that accepts HTTP POST requests with username and password in the body. If the username and password are correct then a JWT authentication token is returned.

/users - secure route restricted to "Admin" users only, it accepts HTTP GET requests and returns a list of all users if the HTTP Authorization header contains a valid JWT token and the user is in the "Admin" role. If there is no auth token, the token is invalid or the user is not in the "Admin" role then a 401 Unauthorized response is returned.

/users/{id} - secure route restricted to authenticated users in any role, it accepts HTTP GET requests and returns the user record for the specified "id" parameter if authorization is successful. Note that "Admin" users can access all user records, while other roles (e.g. "User") can only access their own user record.

/users/register/{User} - POST 

/shopping/list/{ShoppingList} - POST

..


## SignalR
http://localhost:5678/shoppingserver/update


## Docker
```bash
docker container run -d --name 'shopping-list' -p 5678:5678 sha256:6ecc3a5e1f501e6c2ea374176fd3e9f469eebae8ed6355818cd1393eb8fdd994
```


## Database
To setup the database, install mysql and run the following:
```
dotnet ef database update
```

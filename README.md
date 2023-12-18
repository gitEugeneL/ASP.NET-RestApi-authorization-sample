# ASP.NET-RestApi-authorization-sample

Authorization sample for Rest Api on ASP.NET 7.

The project implements a clean architecture, CQRS pattern, Repository pattern.

Authorization is accomplished using a JWT access token and a refresh token.
The access token is used to authorize the user, the refresh token is used to update a pair of tokens.
The refresh token is recorded in the database and allows each user to have 5 active devices at the same time.

## Main technologies

* [ASP.NET Core 7](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0)
* [Entity Framework Core 7](https://learn.microsoft.com/en-us/ef/core/)
* [MediatR](https://github.com/jbogard/MediatR)
* [SQL-Server 2022](https://www.microsoft.com/pl-pl/sql-server/sql-server-2022)
* [Docker](https://www.docker.com/)


## List of containers

* **database** - MsSQL database container.

* **app** - container for all application layers.


## How to run the server

1. Build and start Docker images based on the configuration defined in the docker-compose.yml.

        make up     // docker-compose up --build

2. Stop and remove containers.

        make down   // docker-compose down

## API documentation

1. Swagger documentation

        http://localhost:5000/swagger/index.html

## Implementation features

### Registration
<details>
<summary>
    <code>POST</code> <code><b>/api/auth/register</b></code><code>(allows to create an account)</code>
</summary>

##### Parameters
> | name     | type       | data type |                                                           
> |----------|------------|-----------|
> | email    | required   | string    |
> | password | required   | string    |


##### Responses
> | http code | content-type        | response                                         |
> |-----------|---------------------|--------------------------------------------------|
> | `201`     | `application/json`  | `"0647ce88-2e36-421a-7314-08dbffe1c4a0"`         |
> | `409`     | `application/json`  | `Entity: User (user@example.com) already exists` |
> | `400`     | `application/json`  | `Validation errors`                              |
</details>

### Login

<details>
<summary>
    <code>POST</code> <code><b>/api/auth/login</b></code><code>(allows to login)</code>
</summary>

##### Parameters
> | name     | type       | data type |                                                           
> |----------|------------|-----------|
> | email    | required   | string    |
> | password | required   | string    |

##### Responses
> | http code | content-type                             | response                                                                                                                         |
> |-----------|------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` `and HttpOnly Cookie` | `{"accessToken": "eyJhbGc...", "type": "Bearer" }` `cookie: refreshToken=Wna@3da...; Expires=...; Secure; HttpOnly; Domain=...;` |
> | `403`     | `application/json`                       | `Entity: User (user@exampe.com) doesn't exist or your password is incorrect`                                                     |
> | `400`     | `application/json`                       | `Validation errors`                                                                                                              |
</details>

### Refresh 
(*Requires refresh token in the Cookies*)
<details>
<summary>
    <code>POST</code> <code><b>/api/auth/refresh</b></code><code>(allows to refresh access and refresh tokens)</code>
</summary>

##### Parameters
> Http Only cookie<br>
> refreshToken=WnaMQ3j...; Expires=Sat, 23 Dec 2025 16:01:54 GMT; Path=/; Secure; HttpOnly; Domain=...;

##### Responses
> | http code | content-type                             | response                                                                                                                         |
> |-----------|------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` `and HttpOnly Cookie` | `{"accessToken": "eyJhbGc...", "type": "Bearer" }` `cookie: refreshToken=Wna@3da...; Expires=...; Secure; HttpOnly; Domain=...;` |
> | `401`     | `application/json`                       | `Refresh token isn't valid`                                                                                                      |
> | `401`     | `application/json`                       | `Refresh token is outdated`                                                                                                      |
</details>

### Logout 
(*Requires JWT token in the header*)
<details>
<summary>
    <code>POST</code> <code><b>/api/auth/logout</b></code><code>(allows to logout, deactivates the refresh token)</code>
</summary>

##### Parameters
> 1. Valid access JWT Bearer token in the header

##### Responses
> | http code | content-type                                    | response                           |
> |-----------|-------------------------------------------------|------------------------------------|
> | `200`     | `application/json` `and remove HttpOnly Cookie` | `No body returned for response`    |
> | `401`     | `application/json`                              | `No body returned for response`    |
</details>
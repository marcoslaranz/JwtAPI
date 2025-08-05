# JWTAPI
## Demonstrate how to use JWT, Token Refresh and a client to test it


The project 'JtwRefresh' demonstrated the use of JWT Authentication with Token Refresh.
This was based on the YouTube video: 



https://www.youtube.com/watch?v=vTXXdm44IdQ\&t=74s

Get ASP.NET Core JWT Refresh Token Working in Minutes

Coding Droplets



The additional console project 'JwtClientDemo' was created to test

Token expiration and token Refresh

The database runs in a container:
```sh
"ConnectionStrings": {
	  "DefaultConnection": "Host=192.168.56.17;Port=5432;Database=JwtAuth;Username=postgres;Password=postgres"
```


### Added the functionality to encrypt the password. This is based
on the NuGet package:

	BCrypt.Net-Next
	


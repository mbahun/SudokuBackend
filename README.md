# Sudoku backend
Backend implementation (.NET 7) supporting online Sudoku. Sudoku application (https://github.com/mbahun/Sudoku) is used for game generation.

## Features
Backend is implemented using layered (onion) architecture as explained in the book: https://code-maze.com/ultimate-aspnetcore-webapi-second-edition/ 

Sudoku backend provides restful API to play online Sudoku. Included are: Swagger documentation, authentication (MS Identity), request rate limiter, cache, Entity framework and raw database queries, DTOs, paging, inter process communication (IPC) by using shared memory (Windows) and shared file (Linux), EF migrations, Postman collection for all requests… 

MS SQL database is used to store generated games, users and high scores. 

## How to run
### Windows
- Open *appsettings.json* and input correct connection string for MS SQL server. Build Sudoku application (https://github.com/mbahun/Sudoku) and copy executable to *External* directory (or anywhere else; just remember to change the *externalAppPath* variable!).

- Run migrations: `update-database`

- Use the Swagger or import the Postman collection from *External* directory into your Postman workspace. Flow is basic: Register new user -> Login -> Create new game -> Play next available game for user.

### Linux
Running the project in Linux is similar to running it in Windows, but with some consideration. 
- The easiest way to run SQL server in Linux is by using Docker: https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker 

- Next, to run migrations, EF tool is needed (if already not globally installed): https://learn.microsoft.com/en-us/ef/core/cli/dotnet If necessary, set the env variable `export PATH=$PATH:~/.dotnet/tools`

- Run migration from project root: `dotnet ef database update --project SudokuBackend --connection "server=localhost; database=Sudoku; User Id=sa;Password=MsSqLpAsSwOrD; Encrypt=False"`

- Run the backend: `dotnet run --project SudokuBackend`

## Final notes
If you wish not to mess with C++ project from other repo, it is possible to run the backend anyway - just set flag *useTestData* to *true* in configuration file. Of course, generated games will always be the same. 

.Net Core provides IPC routines but they work best if both applications are .Net. However, combining .NET and C++ app on Windows works well. Thus, I used file as IPC mechanism to connect Sudoku (C++) and SudokuBackend (C#) on Linux. It works on Windows too, but just to make example bigger, I used shared memory on Windows. 

API is supporting simple game idea. Only registered users with role *player* can play the game. To play the game, the user (player) needs to choose next available game. If no games are available (stored in the database), external app needs to be called to generate new game (POST /api/games). Any user can generate new game and generated games are playable by other players. Potential solution can be checked or stored in the database (PATCH method). Storing the correct result will generate the score for played game. 

Online base64 binary to ASCII converter can be used to see games and solutions, e.g. https://www.multiutil.com/base64-to-ascii-converter/



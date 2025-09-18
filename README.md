# Brainbay Rick and Morty - Assessment Project
## Framework
Backend: Dot Net 8
Frontend: Vite React.js

## Setup Importer and API
1. Clone the repo.
2. Open the Solution, it should to have the following projects to start ```but``` you might need to set the solution to use Multiple Startup Project. So make sure that the following two projects are set to start:
```
- RickAndMorty.Api
- RickAndMorty.Importer
```
3. Restore NuGet Packages to make sure all used packages are installed.
4. Make sure there isn't any database in your localdb called ```RickAndMortyDb```.
5. Rebuild and then Start as http.

## Setup Frontend
1. Repo would already have been cloned.
2. Open the ```RickAndMorty.WebUI``` folder in VS Code (or similiar).
3. In a terminal, while in the ```RickAndMorty.WebUI``` folder, install the needed packages:
```(npm)
npm install
```
4. Once all packages are installed, run the following script to run the website:
```(npm)
npm run dev
```
5. Access the website by click on the link in the terminal, it should point to [http://localhost:5173/](http://localhost:5173/)

## Applications
### Rick and Morty Importer - Console App
FluentMigration is used to ensure the database and tables are created before the application imports any data.
The console application clears the Characters and Planets tables then pulls only the data for characters that's alive from the Rick and Morty API, it iterates and processes said data. 
During the processinging, the planet information (origin and location) is sent to the local RickAndMorty.Api and saved.
Once all data has been processed, the character data is sent in bulk to the local RickAndMorty.Api and also saved.

### API
A REST API is used to read and write data from/to the database. Dapper is used in the storage layer to read/save data. While a CQRS pattern is used to fetch the data from the storage layer, that way ensuring we keep seperation of concerns. I've also added Swagger so that the actions are testable directly on the API.

### Front End
The website has as home the character list page, where all available characters are shown in paginated card format. There is a search that can be used for local filtering, and you can also view characters from a specific origin by adding ```?from=x```, where x is a planet's name. This will return character who's origin contains that planet name. There is also a ```Add Character``` button, that takes you to the ```Add Character``` view, which allows you to enter data for a new character that can then be saved. The website used MUI components and is responsive. The ```Add Character``` form has some basic validation, as such a Name is mandatory and the image added should be a valid URL.

### Test
I've added tests for the RickAndMorty.Importer project that tests most of the functionality within the Importer.

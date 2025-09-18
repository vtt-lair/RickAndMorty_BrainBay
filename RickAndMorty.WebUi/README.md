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


## Application
### Front End
The website has as home the character list page, where all available characters are shown in paginated card format. There is a search that can be used for local filtering, and you can also view characters from a specific origin by adding ```?from=x```, where x is a planet's name. This will return character who's origin contains that planet name. There is also a ```Add Character``` button, that takes you to the ```Add Character``` view, which allows you to enter data for a new character that can then be saved. The website used MUI components and is responsive. The ```Add Character``` form has some basic validation, as such a Name is mandatory and the image added should be a valid URL.
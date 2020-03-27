<h1 align="center">
    <a href="https://github.com/MambaDev/Coffee-Machine">
      <img src="https://i.imgur.com/gx7rpr7.png" alt="mavic-logo" width="400">
    </a>
    <br/>
</h1>

<p align="center">
  <a href="#how-to-use">How To Use</a> •
  <a href="#notes">Notes</a> •
  <a href="#assumptions">Assumptions</a>
</p>

# How to Use

The project requires the installation of the dotnet core 3.1 SDK and a MySQL server or variant (MariaDB).

1. Update the `coffee.api` appsettings and appsettings.development JSON files with a valid `MySQL` related connection details.
2. Restore related packages `dotnet restore`
3. Execute tests to ensure correct installation and restore `dotnet test`
4. Run the application `dotnet run --project coffee.api` and go to `localhost:8080`

When the application first starts, the database will be seeded with sample data.

### Notes

- Bug related to not being able to descale had to be fixed since you could only descale in an Okay state, not as described in the task document.

- The random number assignment will never hit the value since it's not inclusive, this was just changed to 9, not 10.

### Assumptions

- Per day of the week show the time the first cup and last were made: This is over the life span of the machine and not just the last week.

- First and the last cup are relative to the lifetime and not just the latest time for that day.

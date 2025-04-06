# Tricky Trivia Trip - Work in Progress...

A simple Trivia Game that will fetch questions from Open Trivia DB via Trivia API and allow the user to answer multiple-choice questions (I'm planning to add multiplayer mode too).

The game will store player scores and answer history in an SQLite database using ADO.NET. I chose SQLite for this project because it is lightweight, easy to set up, and doesn't require a separate database server installation. This makes it convenient for running the demo without additional setup.

However, the database access logic is written using ADO.NET, so switching to Microsoft SQL Server, PostgreSQL, or MySQL would be easy enough.

The UI is not really functional yet, since I first wanted to concentrate on designing the database and implementing API interaction.

### Implemented as of now:
- Main window UI prototype
- Dependency injection infrastructure
- Normalized database design
- Model classes
- Initial database schema creation
- `BaseRepository<T>` class

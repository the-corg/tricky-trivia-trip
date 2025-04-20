# Tricky Trivia Trip - Work in Progress...

A simple Trivia Game that fetches trivia questions from Open Trivia DB via Trivia API.

The game will store player scores and answer history in an SQLite database using ADO.NET. I chose SQLite for this project because it is lightweight, easy to set up, and doesn't require a separate database server installation. This makes it convenient for running the demo without additional setup.

The database access logic uses ADO.NET, so switching from SQLite to Microsoft SQL Server, PostgreSQL, or MySQL would be easy enough.

The UI is only partly functional for now, since I first wanted to concentrate on designing the database and implementing API interaction.

### Implemented as of now:
- UI prototype with 3 independent views
- Navigation between the views
- Dependency injection infrastructure
- Normalized database design
- Model classes
- Initial database schema creation
- All Repository classes
- Getting questions from the API
- (partially) Game logic, including automated QuestionQueue

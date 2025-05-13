# Tricky Trivia Trip

A simple Trivia Game that fetches questions from Open Trivia Database API and uses a local SQLite database.

![Gameplay demo](TrickyTriviaTrip.gif)

The game stores player scores and complete answer history in an SQLite database using ADO.NET. I chose SQLite for this project because it is lightweight, easy to set up, and doesn't require a separate database server installation. This makes it convenient for running the game without additional setup.

The database access logic uses ADO.NET, so switching from SQLite to Microsoft SQL Server, PostgreSQL, or MySQL would be easy enough.

## Key Features
- Question queue with automatic background replenishment from the API, as well as the ability to fall back to the database in case of API connection problems
- Fully asynchronous API and database operations
- MVVM architecture with clean separation of concerns
- Normalized database design
- Game statistics using fairly complex SQL queries
- Logging in a background thread with communication via BlockingCollection
- Extensive use of WPF control templates to achieve non-standard UI appearance and optimal user experience

-- Initial database creation script


CREATE TABLE Player (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

CREATE TABLE Question (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Text TEXT NOT NULL
);

CREATE TABLE AnswerOption (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    QuestionId INTEGER NOT NULL,
    Text TEXT NOT NULL,
    IsCorrect BOOLEAN NOT NULL DEFAULT 0,
    FOREIGN KEY(QuestionId) REFERENCES Question(Id) ON DELETE CASCADE
);

-- Partial unique index that guarantees that there is only one correct answer per question
CREATE UNIQUE INDEX OneCorrectAnswerPerQuestion ON AnswerOption(QuestionId)
WHERE IsCorrect = 1;


CREATE TABLE AnswerAttempt (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PlayerId INTEGER NOT NULL,
    QuestionId INTEGER NOT NULL,
    AnswerOptionId INTEGER NOT NULL,
    Timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(PlayerId) REFERENCES Player(Id),
    FOREIGN KEY(QuestionId) REFERENCES Question(Id),
    FOREIGN KEY(AnswerOptionId) REFERENCES AnswerOption(Id)
);

CREATE TABLE Score (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PlayerId INTEGER NOT NULL,
    Value INTEGER NOT NULL,
    Timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(PlayerId) REFERENCES Player(Id)
);

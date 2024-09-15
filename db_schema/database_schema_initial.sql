CREATE TABLE AccountTransaction (
    TransactionId int IDENTITY(1,1) NOT NULL,
    Amount decimal(8,2) NOT NULL,
    Type nvarchar(100) NOT NULL,
    PaymentMethod nvarchar(100) NOT NULL,
    PlayerId int NOT NULL,
    CONSTRAINT AccountTransaction_pk PRIMARY KEY (TransactionId)
);

CREATE TABLE Bet (
    BetId int IDENTITY(1,1) NOT NULL,
    SpinId int NOT NULL,
    PlayerId int NOT NULL,
    GameId int NOT NULL,
    BetAmount decimal(7,2) NOT NULL,
    Result int NULL,
    BetType nvarchar(20) NOT NULL,
    CONSTRAINT Bet_pk PRIMARY KEY (BetId)
);

CREATE TABLE BetNumber (
    BetNumberId int IDENTITY(1,1) NOT NULL,
    BetId int NOT NULL,
    Number int NULL,
    CONSTRAINT BetNumber_pk PRIMARY KEY (BetNumberId)
);

CREATE TABLE Game (
    GameId int IDENTITY(1,1) NOT NULL,
    GameName nvarchar(50) NOT NULL,
    CONSTRAINT Game_pk PRIMARY KEY (GameId)
);

CREATE TABLE Player (
    PlayerId int IDENTITY(1,1) NOT NULL,
    FirstName nvarchar(100) NOT NULL,
    LastName nvarchar(100) NOT NULL,
    Email nvarchar(40) NOT NULL,
    Login nvarchar(40) NOT NULL,
    Telephone nvarchar(9) NOT NULL,
    Password nvarchar(max) NOT NULL,
    Salt nvarchar(max) NOT NULL,
    AccountBalance decimal(8,2) NOT NULL,
    RefreshToken nvarchar(max) NOT NULL,
    RefreshTokenExp datetime NOT NULL,
    CONSTRAINT Player_pk PRIMARY KEY (PlayerId)
);

CREATE TABLE Spin (
    SpinId int IDENTITY(1,1) NOT NULL,
    NumberWinner int NULL,
    GameId int NOT NULL,
    CONSTRAINT Spin_pk PRIMARY KEY (SpinId)
);

CREATE TABLE UserActivity (
    UAId int IDENTITY(1,1) NOT NULL,
    PlayerId int NOT NULL,
    StartTime datetime NOT NULL,
    EndTime datetime NULL,
    IPAddress nvarchar(max) NOT NULL,
    CONSTRAINT UserActivity_pk PRIMARY KEY (UAId)
);



ALTER TABLE BetNumber ADD CONSTRAINT BetNumber_Bet
    FOREIGN KEY (BetId)
    REFERENCES Bet (BetId);

ALTER TABLE Bet ADD CONSTRAINT Bet_Game
    FOREIGN KEY (GameId)
    REFERENCES Game (GameId);

ALTER TABLE Bet ADD CONSTRAINT Bet_Player
    FOREIGN KEY (PlayerId)
    REFERENCES Player (PlayerId);

ALTER TABLE Bet ADD CONSTRAINT Bet_Spin
    FOREIGN KEY (SpinId)
    REFERENCES Spin (SpinId);

ALTER TABLE UserActivity ADD CONSTRAINT Session_Player
    FOREIGN KEY (PlayerId)
    REFERENCES Player (PlayerId);

ALTER TABLE Spin ADD CONSTRAINT Spin_Game
    FOREIGN KEY (GameId)
    REFERENCES Game (GameId);

ALTER TABLE AccountTransaction ADD CONSTRAINT Transaction_Player
    FOREIGN KEY (PlayerId)
    REFERENCES Player (PlayerId);
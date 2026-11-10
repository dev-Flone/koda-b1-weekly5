USE EWalletDb;


GO
CREATE TABLE Users (
    Id    INT            IDENTITY (1, 1) PRIMARY KEY,
    Name  NVARCHAR (100) NOT NULL,
    Email NVARCHAR (150) NOT NULL UNIQUE,
    Phone NVARCHAR (20)  NOT NULL
);


GO
CREATE TABLE Wallets (
    Id           INT             IDENTITY (1, 1) PRIMARY KEY,
    UserId       INT             NOT NULL UNIQUE,
    WalletNumber NVARCHAR (20)   NOT NULL UNIQUE,
    Balance      DECIMAL (18, 2) DEFAULT 0 NOT NULL,
    CONSTRAINT FK_Wallets_Users FOREIGN KEY (UserId) REFERENCES Users (Id),
    CONSTRAINT Ck_Wallets_Balance CHECK (Balance >= 0)
);


GO
CREATE TABLE Transactions (
    Id               INT             IDENTITY (1, 1) PRIMARY KEY,
    SenderWalletId   INT             NULL,
    ReceiverWalletId INT             NOT NULL,
    Type             INT             NOT NULL,
    Amount           DECIMAL (18, 2) NOT NULL,
    Description      NVARCHAR (250)  NULL,
    CreatedAt        DATETIME2       DEFAULT SYSUTCDATETIME() NOT NULL,
    CONSTRAINT FK_Transactions_SenderWallet FOREIGN KEY (SenderWalletId) REFERENCES Wallets (Id),
    CONSTRAINT FK_Transactions_Receiver FOREIGN KEY (ReceiverWalletId) REFERENCES Wallets (Id),
    CONSTRAINT CK_Transactions_Amount CHECK (Amount > 0),
    CONSTRAINT CK_Transactions_Type CHECK ((Type = 1
                                            AND SenderWalletId IS NULL)
                                           OR (TYPE = 2
                                               AND SenderWalletId IS NOT NULL))
);


GO
CREATE INDEX IX_Transactions_SenderWalletId
    ON Transactions(SenderWalletId);

CREATE INDEX IX_Transactions_ReceiverWalletId
    ON Transactions(ReceiverWalletId);

CREATE INDEX IX_Transactions_CreatedAt
    ON Transactions(CreatedAt);
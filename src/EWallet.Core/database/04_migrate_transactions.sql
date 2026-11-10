USE EWalletDb;
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Transactions')
      AND name = 'Description'
      AND is_nullable = 0
)
BEGIN
    ALTER TABLE Transactions
    ALTER COLUMN Description NVARCHAR(250) NULL;
END;
GO

IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys fk
    INNER JOIN sys.foreign_key_columns fkc
        ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.columns c
        ON c.object_id = fkc.parent_object_id
       AND c.column_id = fkc.parent_column_id
    WHERE fk.name = 'FK_Transactions_Receiver'
      AND fk.parent_object_id = OBJECT_ID('dbo.Transactions')
      AND c.name <> 'ReceiverWalletId'
)
BEGIN
    ALTER TABLE Transactions
    DROP CONSTRAINT FK_Transactions_Receiver;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys fk
    INNER JOIN sys.foreign_key_columns fkc
        ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.columns c
        ON c.object_id = fkc.parent_object_id
       AND c.column_id = fkc.parent_column_id
    WHERE fk.name = 'FK_Transactions_Receiver'
      AND fk.parent_object_id = OBJECT_ID('dbo.Transactions')
      AND c.name = 'ReceiverWalletId'
)
BEGIN
    ALTER TABLE Transactions
    ADD CONSTRAINT FK_Transactions_Receiver
        FOREIGN KEY (ReceiverWalletId) REFERENCES Wallets (Id);
END;
GO

IF EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_Transactions_Type'
      AND parent_object_id = OBJECT_ID('dbo.Transactions')
)
BEGIN
    ALTER TABLE Transactions
    DROP CONSTRAINT CK_Transactions_Type;
END;
GO

ALTER TABLE Transactions
ADD CONSTRAINT CK_Transactions_Type CHECK (
    (Type = 1 AND SenderWalletId IS NULL)
    OR (Type = 2 AND SenderWalletId IS NOT NULL)
);
GO

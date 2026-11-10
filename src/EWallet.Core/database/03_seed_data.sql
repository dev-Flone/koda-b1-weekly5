USE EWalletDb;
GO

INSERT INTO Users (Name, Email, Phone)
VALUES
    ('Andi', 'andi@gmail.com', '081234567801'),
    ('Budi', 'budi@gmail.com', '081234567802'),
    ('Citra', 'citra@gmail.com', '081234567803');
GO

INSERT INTO Wallets(UserId, WalletNumber, Balance)
SELECT Id, 'WLT001', 400000.00
FROM Users
WHERE Email = 'andi@gmail.com';

INSERT INTO Wallets(UserId, WalletNumber, Balance)
SELECT Id, 'WLT002', 400000.00
FROM Users
WHERE Email = 'budi@gmail.com';

INSERT INTO Wallets(UserId, WalletNumber, Balance)
SELECT Id, 'WLT003', 750000.00
FROM Users
WHERE Email = 'citra@gmail.com';

GO

-- Top Up Andi: Rp500.000
INSERT INTO Transactions (
    SenderWalletId,
    ReceiverWalletId,
    Type,
    Amount,
    Description
)
SELECT
    NULL,
    w.Id,
    1,
    500000.00,
    'Initial top up'
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.Id
WHERE u.Email = 'andi@gmail.com';

-- Top Up Budi: Rp300.000
INSERT INTO Transactions (
    SenderWalletId,
    ReceiverWalletId,
    Type,
    Amount,
    Description
)
SELECT
    NULL,
    w.Id,
    1,
    300000.00,
    'Initial top up'
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.Id
WHERE u.Email = 'budi@gmail.com';

-- Top Up Citra: Rp750.000
INSERT INTO Transactions (
    SenderWalletId,
    ReceiverWalletId,
    Type,
    Amount,
    Description
)
SELECT
    NULL,
    w.Id,
    1,
    750000.00,
    'Initial top up'
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.Id
WHERE u.Email = 'citra@gmail.com';

-- Transfer Andi ke Budi: Rp100.000
INSERT INTO Transactions (
    SenderWalletId,
    ReceiverWalletId,
    Type,
    Amount,
    Description
)
SELECT
    sender.Id,
    receiver.Id,
    2,
    100000.00,
    'Transfer to Budi'
FROM Wallets sender
INNER JOIN Users senderUser ON sender.UserId = senderUser.Id
CROSS JOIN Wallets receiver
INNER JOIN Users receiverUser ON receiver.UserId = receiverUser.Id
WHERE senderUser.Email = 'andi@gmail.com'
  AND receiverUser.Email = 'budi@gmail.com';
GO

SELECT
    u.Id AS UserId,
    u.Name,
    u.Email,
    w.WalletNumber,
    w.Balance
FROM Users u
INNER JOIN Wallets w ON u.Id = w.UserId;

SELECT
    t.Id,
    sender.WalletNumber AS Sender,
    receiver.WalletNumber AS Receiver,
    t.Type,
    t.Amount,
    t.Description,
    t.CreatedAt
FROM Transactions t
LEFT JOIN Wallets sender
    ON t.SenderWalletId = sender.Id
INNER JOIN Wallets receiver
    ON t.ReceiverWalletId = receiver.Id
ORDER BY t.Id;

USE EWalletDb;
GO

-- -- Periksa aturan CHECK constraint
-- SELECT
--     name,
--     definition
-- FROM sys.check_constraints
-- WHERE name = 'CK_Transactions_Type';

-- -- Periksa wallet Andi dan Budi
-- SELECT
--     u.Name,
--     u.Email,
--     w.Id AS WalletId,
--     w.WalletNumber
-- FROM Users u
-- INNER JOIN Wallets w ON u.Id = w.UserId
-- WHERE u.Email IN ('andi@gmail.com', 'budi@gmail.com');
-- SELECT
--     Id,
--     SenderWalletId,
--     ReceiverWalletId,
--     Type,
--     Amount,
--     Description
-- FROM Transactions;

-- SELECT
--     Id,
--     SenderWalletId,
--     ReceiverWalletId,
--     Type,
--     Amount,
--     Description
-- FROM Transactions;

-- TRUNCATE TABLE Transactions;
-- TRUNCATE TABLE Users;
-- TRUNCATE TABLE Wallets;

-- DROP TABLE Transactions;
-- DROP TABLE Users;
-- DROP TABLE Wallets;

-- SELECT 
--     schema_name(schema_id) AS SchemaName,
--     name AS TableName
-- FROM 
--     sys.tables
-- ORDER BY 
--     SchemaName, TableName;

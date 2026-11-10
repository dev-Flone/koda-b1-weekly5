SELECT cc.name AS ConstraintName,
       cc.definition AS ConstraintDefinition
FROM   sys.check_constraints AS cc
       INNER JOIN
       sys.tables AS t
       ON cc.parent_object_id = t.object_id
WHERE  t.name = 'Wallets';

ALTER TABLE Wallets DROP CONSTRAINT Ck_Wallets_Balance;

ALTER TABLE Wallets
       ADD CONSTRAINT Ck_Wallets_Balance CHECK (Balance >= 0);

SELECT COLUMN_NAME,
       IS_NULLABLE,
       DATA_TYPE
FROM   INFORMATION_SCHEMA.COLUMNS
WHERE  TABLE_NAME = 'Transactions'
       AND COLUMN_NAME = 'Description';

ALTER TABLE Transactions ALTER COLUMN Description NVARCHAR (250) NULL;
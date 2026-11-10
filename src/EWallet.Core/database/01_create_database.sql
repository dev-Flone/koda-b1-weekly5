IF DB_ID('EWalletDb') IS NULL
    BEGIN
        CREATE DATABASE EWalletDb;
    END


GO
USE EWalletDb;
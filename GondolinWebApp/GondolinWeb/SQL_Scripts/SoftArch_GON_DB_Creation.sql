/*
Below is a script to create a new database with in local server instance.
For it to work you must ****first**** create a folder in the C: directory named 'Soft_ArchDB'.
Then connect to the local DB, you should then see 'SoftArch_DB_V1' appear after you refresh the server.
*/

CREATE DATABASE [SoftArch_DB_V1] 
    ON (NAME = N'MyDatabase', FILENAME = N'C:\SoftArch_DB\SoftArch_DB_V1.mdf', SIZE = 1024MB, FILEGROWTH = 256MB)
LOG ON (NAME = N'MyDatabase_log', FILENAME = N'C:\SoftArch_DB\SoftArch_DB_V1.ldf', SIZE = 512MB, FILEGROWTH = 125MB)
GO
/*
This script is intended to create the tables of the SoftArch_DB.
If a table is created, please add 5 items to the table for dummy data.
Please right IF Exists clause and drop and FK and Tables to make testing easier.
*/

Use [SoftArch_DB_V1]

/*
--------------This section is erasing the DB tables----------------------
*/
ALTER TABLE [Task]
DROP CONSTRAINT IF EXISTS [FK_Project_Task_ProjectTask]

ALTER TABLE [Task]
DROP CONSTRAINT IF EXISTS [FK_Category_Task_CategoryID]

ALTER TABLE [User_Task]
DROP CONSTRAINT IF EXISTS [FK_TaskID_UserTask]

ALTER TABLE [User_Task]
DROP CONSTRAINT IF EXISTS [FK_User_UserTask]


DROP TABLE IF EXISTS [dbo].[Task]
DROP TABLE IF EXISTS [dbo].[User]
DROP TABLE IF EXISTS [dbo].[Project]
DROP TABLE IF EXISTS [dbo].[Category]
DROP TABLE IF EXISTS [dbo].[User_Task]

GO

/*
-----------------This Section creates the tables and assigns primary and foreign keys-------------------
*/
CREATE TABLE [dbo].[User](
	ID			nvarChar(450)		NOT NULL,
	FirstName		nvarchar(255)		NOT NULL,
	LastName		nvarChar(255)		NOT NULL

PRIMARY KEY ([ID])
);

INSERT INTO [dbo].[User](ID,FirstName,LastName)
	SELECT Id, FirstName, LastName
	FROM [GondolinWebIdentity].[dbo].[AspNetUsers]

CREATE TABLE [dbo].[Project](
	ID		int Identity(1,1)		NOT NULL,
	CreationDate	DateTime		NOT NULL DEFAULT GETDATE(),
	CompletionDate	DateTime				,
	[Description]	nvarchar(255)	NOT NULL,
	IsComplete		bit				NOT NULL DEFAULT 0,
	[Name]			nvarchar(255)	NOT NULL,
	RequiredDate	DateTime
PRIMARY KEY ([ID])
);

CREATE TABLE [dbo].[Category](
	ID		int Identity(1,1)		NOT NULL,
	CreationDate	Datetime		NOT NULL DEFAULT GETDATE(),
	IsArchived		bit				NOT NULL DEFAULT 0,
	[Name]			nvarChar(255)	NOT NULL
PRIMARY KEY ([ID])
);

CREATE TABLE [dbo].[Task](
	ID			int Identity(1,1)		NOT NULL,
	CategoryID		int							,
	CreateionDate	DateTime			NOT NULL DEFAULT GETDATE(),
	CompletionDate	DateTime					,
	[Description]	nvarchar(255)				,
	IsComplete		bit					NOT NULL DEFAULT 0,
	[Name]			nvarchar(255)		NOT NULL,
	ProjectID		int							,
	RequiredDate	DateTime

PRIMARY KEY CLUSTERED ([ID]),

CONSTRAINT [FK_Project_Task_ProjectTask] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project]([ID]),
CONSTRAINT [FK_Category_Task_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category]([ID])
);

CREATE TABLE [dbo].[User_Task](
	ID		int Identity(1,1)			NOT NULL,
	UserID			nvarchar(450)		NOT NULL,
	TaskID			int					NOT NULL,
PRIMARY KEY ([ID]),

CONSTRAINT [FK_TaskID_UserTask]	FOREIGN KEY ([TaskID]) REFERENCES [dbo].[Task]([ID]),
CONSTRAINT [FK_User_UserTask] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User]([ID])
);
GO

/*
----------------This section creates defualt entries into to the tables.--------------
*/
INSERT INTO [Category](Name) Values ('School')
INSERT INTO [Category](Name) Values ('Home')
INSERT INTO [Category](Name) Values ('Work')
INSERT INTO [Category](Name) Values ('Fishing')
INSERT INTO [Category](Name) Values ('Cooking')

/* Inserts default values int to the project table. */
INSERT INTO [Project](Description,IsComplete,Name,RequiredDate) 
	Values ('Build lofted bed in the girls room', 0, 'Girls Bed', '20221018 6:00:00 PM')
INSERT INTO [Project](Description,IsComplete,Name,RequiredDate) 
	Values ('Finish SQL programing for project', 0, 'SQL-111', '20220618 6:00:00 PM')

/* Inserts default values into the task table. */
INSERT INTO [Task](CategoryID, Description, IsComplete, Name, ProjectID, RequiredDate) 
	Values(NULL, 'Get Lumber', 0, 'Home Depot Trip', 1, '20221018 6:00:00 PM' )
INSERT INTO [Task](CategoryID, Description, IsComplete, Name, ProjectID, RequiredDate) 
	Values(2, NULL, 0, 'Buy Milk', NULL, '20221018 6:00:00 PM' )
INSERT INTO [Task](CategoryID, Description, IsComplete, Name, ProjectID, RequiredDate) 
	Values(2, 'Get butt out of bed', 0, 'Get out of bed', 2, '20221018 6:00:00 PM' )
INSERT INTO [Task](CategoryID, Description, IsComplete, Name, ProjectID, RequiredDate) 
	Values(2, NULL, 1, 'Do the dishes', NULL, '20221018 6:00:00 PM' )
INSERT INTO [Task](CategoryID, Description, IsComplete, Name, ProjectID, RequiredDate) 
	Values(2, NULL, 1, 'Mow the grass', NULL, '20221018 6:00:00 PM' )

/* Inserts default values into the User_Task table*/
Insert INTO [User_Task](UserID,TaskID)
SELECT users.ID, tasks.ID
FROM [dbo].[User] AS users, [dbo].Task as tasks
Where users.FirstName = 'Addy'
and tasks.ID = 1
GO

Insert INTO [User_Task](UserID,TaskID)
SELECT users.ID, tasks.ID
FROM [dbo].[User] AS users, [dbo].Task as tasks
Where users.FirstName = 'Addy'
and tasks.ID = 2
GO

Insert INTO [User_Task](UserID,TaskID)
SELECT users.ID, tasks.ID
FROM [dbo].[User] AS users, [dbo].Task as tasks
Where users.FirstName = 'Addy'
and tasks.ID = 3
GO

Insert INTO [User_Task](UserID,TaskID)
SELECT users.ID, tasks.ID
FROM [dbo].[User] AS users, [dbo].Task as tasks
Where users.FirstName = 'Addy'
and tasks.ID = 4
GO

Insert INTO [User_Task](UserID,TaskID)
SELECT users.ID, tasks.ID
FROM [dbo].[User] AS users, [dbo].Task as tasks
Where users.FirstName = 'Addy'
and tasks.ID = 5
GO
	

/*
This section is for sample querys
Please label the functionality of each query you design.
*/

/*
	Selects First name from User, Project Description, Project Name, and Task Description
	Returns as a total list of all projects asigned to ALL USERS!!!!!
*/
Select FirstName, P.Description, P.Name as ProjectName, T.Description as TaskDescription
from [User] as US
Join [User_Task] as UT on UT.UserID = US.ID
Join [Task] as T on T.ID = UT.TaskID
Join [Project] as P on P.ID = T.ProjectID

/*
	Selects First name from User, Project Description, Project Name, and Task Description
	Returns as a total list of all projects asigned to CHRIS!!!!!
*/
Select FirstName, P.Description, P.Name as ProjectName, T.Description as TaskDescription
from [User] as US
Join [User_Task] as UT on UT.UserID = US.ID
Join [Task] as T on T.ID = UT.TaskID
Join [Project] as P on P.ID = T.ProjectID
Where US.FirstName = 'Addy'

/*
	Selects the Users' First Name, Task Description, and Task ID
	Return as a list with all the tasks assigned to ALL USERS
*/
Select FirstName, T.Description, T.ID
FROM [User] as US
Right Join [User_Task] as UT on UT.UserID = US.ID
Join [Task] as T on T.ID = UT.TaskID

/*
	Selects the Users' First Name, Task Description, and Task ID
	Return as a list with all the tasks assigned to ROBIN!!!!
*/
Select FirstName, T.Name, T.ID as [Task ID]
FROM [User] as US
Right Join [User_Task] as UT on UT.UserID = US.ID
Join [Task] as T on T.ID = UT.TaskID
Where US.FirstName = 'Addy'

/*
	Selects the Users' First and Task Description.
	Returns a list of all tasks for ALL USERS where the task has
	NOT been assigned to a project.
*/
Select FirstName, T.Name as [Task Name]
from [User] as US
Join [User_Task] as UT on UT.UserID = US.ID
Join [Task] as T on T.ID = UT.TaskID
Where T.ProjectID IS NULL;

/*
	Selects the Users' First and Task Description.
	Returns a list of all tasks for CHRIS where the task has
	NOT been assigned to a project.
*/
Select FirstName as [First Name], T.Description as [Task Description]
from [User] as US
Join [User_Task] as UT on UT.UserID = US.ID
Join [Task] as T on T.ID = UT.TaskID
Where T.ProjectID IS NULL and US.FirstName = 'Chris';
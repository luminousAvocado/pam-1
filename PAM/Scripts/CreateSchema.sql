IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [BureauTypes] (
    [BureauTypeId] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NOT NULL,
    [DisplayCode] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_BureauTypes] PRIMARY KEY ([BureauTypeId])
);

GO

CREATE TABLE [Employees] (
    [EmployeeId] int NOT NULL IDENTITY,
    [Username] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [MiddleName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [Title] nvarchar(max) NULL,
    [Department] nvarchar(max) NULL,
    [Service] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    [Zip] nvarchar(max) NULL,
    [Phone] nvarchar(max) NULL,
    [CellPhone] nvarchar(max) NULL,
    [SupervisorName] nvarchar(max) NULL,
    [IsAdmin] bit NOT NULL,
    [IsApprover] bit NOT NULL,
    [IsProcessor] bit NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([EmployeeId]),
    CONSTRAINT [AK_Employees_Email] UNIQUE ([Email]),
    CONSTRAINT [AK_Employees_Username] UNIQUE ([Username])
);

GO

CREATE TABLE [Locations] (
    [LocationId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    [Zip] nvarchar(max) NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY ([LocationId])
);

GO

CREATE TABLE [RequestTypes] (
    [RequestTypeId] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NOT NULL,
    [DisplayCode] nvarchar(max) NOT NULL,
    [DisplayOrder] int NULL,
    [Description] nvarchar(max) NULL,
    [Enabled] bit NOT NULL,
    CONSTRAINT [PK_RequestTypes] PRIMARY KEY ([RequestTypeId])
);

GO

CREATE TABLE [Systems] (
    [SystemId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Owner] nvarchar(max) NULL,
    [Retired] bit NOT NULL,
    CONSTRAINT [PK_Systems] PRIMARY KEY ([SystemId])
);

GO

CREATE TABLE [UnitTypes] (
    [UnitTypeId] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NOT NULL,
    [DisplayCode] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [DisplayOrder] int NULL,
    CONSTRAINT [PK_UnitTypes] PRIMARY KEY ([UnitTypeId])
);

GO

CREATE TABLE [Bureaus] (
    [BureauId] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [BureauTypeId] int NULL,
    [DisplayOrder] int NULL,
    CONSTRAINT [PK_Bureaus] PRIMARY KEY ([BureauId]),
    CONSTRAINT [FK_Bureaus_BureauTypes_BureauTypeId] FOREIGN KEY ([BureauTypeId]) REFERENCES [BureauTypes] ([BureauTypeId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Units] (
    [UnitId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [BureauId] int NOT NULL,
    [UnitTypeId] int NULL,
    [ParentId] int NULL,
    [DisplayOrder] int NULL,
    CONSTRAINT [PK_Units] PRIMARY KEY ([UnitId]),
    CONSTRAINT [FK_Units_Bureaus_BureauId] FOREIGN KEY ([BureauId]) REFERENCES [Bureaus] ([BureauId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Units_Units_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Units] ([UnitId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Units_UnitTypes_UnitTypeId] FOREIGN KEY ([UnitTypeId]) REFERENCES [UnitTypes] ([UnitTypeId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Requesters] (
    [RequesterId] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [MiddleName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [BureauId] int NULL,
    [UnitId] int NULL,
    [Title] nvarchar(max) NULL,
    [Department] nvarchar(max) NULL,
    [Service] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    [Zip] nvarchar(max) NULL,
    [Phone] nvarchar(max) NULL,
    [CellPhone] nvarchar(max) NULL,
    [SupervisorName] nvarchar(max) NULL,
    CONSTRAINT [PK_Requesters] PRIMARY KEY ([RequesterId]),
    CONSTRAINT [FK_Requesters_Bureaus_BureauId] FOREIGN KEY ([BureauId]) REFERENCES [Bureaus] ([BureauId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Requesters_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([UnitId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [UnitSystems] (
    [UnitId] int NOT NULL,
    [SystemId] int NOT NULL,
    CONSTRAINT [PK_UnitSystems] PRIMARY KEY ([UnitId], [SystemId]),
    CONSTRAINT [FK_UnitSystems_Systems_SystemId] FOREIGN KEY ([SystemId]) REFERENCES [Systems] ([SystemId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UnitSystems_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([UnitId]) ON DELETE CASCADE
);

GO

CREATE TABLE [Requests] (
    [RequestId] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [RequestTypeId] int NOT NULL,
    [RequestedById] int NOT NULL,
    [RequestedForId] int NOT NULL,
    [RequestStatus] nvarchar(max) NOT NULL,
    [CreatedOn] datetime2 NULL,
    [SubmittedOn] datetime2 NULL,
    [CompletedOn] datetime2 NULL,
    [IsContractor] bit NOT NULL,
    [IsHighProfileAccess] bit NOT NULL,
    [IsGlobalAccess] bit NOT NULL,
    [CaseloadType] nvarchar(max) NULL,
    [CaseloadFunction] nvarchar(max) NULL,
    [CaseloadNumber] nvarchar(max) NULL,
    [OldCaseloadNumber] nvarchar(max) NULL,
    [TransferredFromUnitId] int NULL,
    [DepartureReason] nvarchar(max) NULL,
    [IpAddress] nvarchar(max) NULL,
    [Notes] nvarchar(max) NULL,
    [Deleted] bit NOT NULL,
    CONSTRAINT [PK_Requests] PRIMARY KEY ([RequestId]),
    CONSTRAINT [FK_Requests_RequestTypes_RequestTypeId] FOREIGN KEY ([RequestTypeId]) REFERENCES [RequestTypes] ([RequestTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Requests_Requesters_RequestedById] FOREIGN KEY ([RequestedById]) REFERENCES [Requesters] ([RequesterId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Requests_Requesters_RequestedForId] FOREIGN KEY ([RequestedForId]) REFERENCES [Requesters] ([RequesterId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Requests_Units_TransferredFromUnitId] FOREIGN KEY ([TransferredFromUnitId]) REFERENCES [Units] ([UnitId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [RequestedSystems] (
    [RequestId] int NOT NULL,
    [SystemId] int NOT NULL,
    [InPortfolio] bit NOT NULL,
    [RemoveAccess] bit NOT NULL,
    [ProcessedById] int NULL,
    [ProcessedOn] datetime2 NULL,
    [ConfirmedById] int NULL,
    [ConfirmedOn] datetime2 NULL,
    CONSTRAINT [PK_RequestedSystems] PRIMARY KEY ([RequestId], [SystemId]),
    CONSTRAINT [FK_RequestedSystems_Employees_ConfirmedById] FOREIGN KEY ([ConfirmedById]) REFERENCES [Employees] ([EmployeeId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RequestedSystems_Employees_ProcessedById] FOREIGN KEY ([ProcessedById]) REFERENCES [Employees] ([EmployeeId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RequestedSystems_Requests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [Requests] ([RequestId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RequestedSystems_Systems_SystemId] FOREIGN KEY ([SystemId]) REFERENCES [Systems] ([SystemId]) ON DELETE CASCADE
);

GO

CREATE TABLE [Reviews] (
    [ReviewId] int NOT NULL IDENTITY,
    [RequestId] int NOT NULL,
    [ReviewerId] int NOT NULL,
    [ReviewOrder] int NOT NULL,
    [ReviewerTitle] nvarchar(max) NOT NULL,
    [Approved] bit NULL,
    [Comments] nvarchar(max) NULL,
    [Timestamp] datetime2 NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([ReviewId]),
    CONSTRAINT [FK_Reviews_Requests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [Requests] ([RequestId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_Employees_ReviewerId] FOREIGN KEY ([ReviewerId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE
);

GO

CREATE TABLE [SystemAccesses] (
    [EmployeeId] int NOT NULL,
    [SystemId] int NOT NULL,
    [RequestId] int NOT NULL,
    [RemoveAccess] bit NOT NULL,
    [SystemAccessStatus] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_SystemAccesses] PRIMARY KEY ([EmployeeId], [SystemId]),
    CONSTRAINT [FK_SystemAccesses_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SystemAccesses_Requests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [Requests] ([RequestId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SystemAccesses_Systems_SystemId] FOREIGN KEY ([SystemId]) REFERENCES [Systems] ([SystemId]) ON DELETE CASCADE
);

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'EmployeeId', N'Address', N'CellPhone', N'City', N'Department', N'Email', N'FirstName', N'IsAdmin', N'IsApprover', N'IsProcessor', N'LastName', N'MiddleName', N'Name', N'Phone', N'Service', N'State', N'SupervisorName', N'Title', N'Username', N'Zip') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] ON;
INSERT INTO [Employees] ([EmployeeId], [Address], [CellPhone], [City], [Department], [Email], [FirstName], [IsAdmin], [IsApprover], [IsProcessor], [LastName], [MiddleName], [Name], [Phone], [Service], [State], [SupervisorName], [Title], [Username], [Zip])
VALUES (1, NULL, NULL, NULL, NULL, N'pam@localhost.localdomain', N'Pam', 1, 0, 0, N'Admin', NULL, N'Pam Admin (e111111)', NULL, NULL, NULL, NULL, NULL, N'e111111', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'EmployeeId', N'Address', N'CellPhone', N'City', N'Department', N'Email', N'FirstName', N'IsAdmin', N'IsApprover', N'IsProcessor', N'LastName', N'MiddleName', N'Name', N'Phone', N'Service', N'State', N'SupervisorName', N'Title', N'Username', N'Zip') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] OFF;

GO

CREATE INDEX [IX_Bureaus_BureauTypeId] ON [Bureaus] ([BureauTypeId]);

GO

CREATE INDEX [IX_RequestedSystems_ConfirmedById] ON [RequestedSystems] ([ConfirmedById]);

GO

CREATE INDEX [IX_RequestedSystems_ProcessedById] ON [RequestedSystems] ([ProcessedById]);

GO

CREATE INDEX [IX_RequestedSystems_SystemId] ON [RequestedSystems] ([SystemId]);

GO

CREATE INDEX [IX_Requesters_BureauId] ON [Requesters] ([BureauId]);

GO

CREATE INDEX [IX_Requesters_UnitId] ON [Requesters] ([UnitId]);

GO

CREATE INDEX [IX_Requests_RequestTypeId] ON [Requests] ([RequestTypeId]);

GO

CREATE INDEX [IX_Requests_RequestedById] ON [Requests] ([RequestedById]);

GO

CREATE INDEX [IX_Requests_RequestedForId] ON [Requests] ([RequestedForId]);

GO

CREATE INDEX [IX_Requests_TransferredFromUnitId] ON [Requests] ([TransferredFromUnitId]);

GO

CREATE INDEX [IX_Reviews_RequestId] ON [Reviews] ([RequestId]);

GO

CREATE INDEX [IX_Reviews_ReviewerId] ON [Reviews] ([ReviewerId]);

GO

CREATE INDEX [IX_SystemAccesses_RequestId] ON [SystemAccesses] ([RequestId]);

GO

CREATE INDEX [IX_SystemAccesses_SystemId] ON [SystemAccesses] ([SystemId]);

GO

CREATE INDEX [IX_Units_BureauId] ON [Units] ([BureauId]);

GO

CREATE INDEX [IX_Units_ParentId] ON [Units] ([ParentId]);

GO

CREATE INDEX [IX_Units_UnitTypeId] ON [Units] ([UnitTypeId]);

GO

CREATE INDEX [IX_UnitSystems_SystemId] ON [UnitSystems] ([SystemId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190212005848_InitialSchema', N'2.2.1-servicing-10028');

GO


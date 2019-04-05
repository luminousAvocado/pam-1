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

CREATE TABLE [Files] (
    [FileId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [ContentType] nvarchar(max) NULL,
    [Length] bigint NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [Content] varbinary(max) NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY ([FileId])
);

GO

CREATE TABLE [Locations] (
    [LocationId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    [Zip] nvarchar(max) NULL,
    [Deleted] bit NOT NULL DEFAULT 0,
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

CREATE TABLE [SupportUnits] (
    [SupportUnitId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Deleted] bit NOT NULL DEFAULT 0,
    CONSTRAINT [PK_SupportUnits] PRIMARY KEY ([SupportUnitId])
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
    [DisplayOrder] int NOT NULL DEFAULT 50,
    [Deleted] bit NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Bureaus] PRIMARY KEY ([BureauId]),
    CONSTRAINT [FK_Bureaus_BureauTypes_BureauTypeId] FOREIGN KEY ([BureauTypeId]) REFERENCES [BureauTypes] ([BureauTypeId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Forms] (
    [FormId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [ForEmployeeOnly] bit NOT NULL,
    [ForContractorOnly] bit NOT NULL,
    [Deleted] bit NOT NULL DEFAULT 0,
    [FileId] int NULL,
    CONSTRAINT [PK_Forms] PRIMARY KEY ([FormId]),
    CONSTRAINT [FK_Forms_Files_FileId] FOREIGN KEY ([FileId]) REFERENCES [Files] ([FileId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [RequiredSignatures] (
    [RequiredSignatureId] int NOT NULL IDENTITY,
    [RequestTypeId] int NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Order] int NOT NULL,
    CONSTRAINT [PK_RequiredSignatures] PRIMARY KEY ([RequiredSignatureId]),
    CONSTRAINT [FK_RequiredSignatures_RequestTypes_RequestTypeId] FOREIGN KEY ([RequestTypeId]) REFERENCES [RequestTypes] ([RequestTypeId]) ON DELETE CASCADE
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
    [SupportUnitId] int NULL,
    [IsAdmin] bit NOT NULL,
    [IsApprover] bit NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([EmployeeId]),
    CONSTRAINT [AK_Employees_Email] UNIQUE ([Email]),
    CONSTRAINT [AK_Employees_Username] UNIQUE ([Username]),
    CONSTRAINT [FK_Employees_SupportUnits_SupportUnitId] FOREIGN KEY ([SupportUnitId]) REFERENCES [SupportUnits] ([SupportUnitId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Systems] (
    [SystemId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Owner] nvarchar(max) NULL,
    [Retired] bit NOT NULL DEFAULT 0,
    [SupportUnitId] int NULL,
    CONSTRAINT [PK_Systems] PRIMARY KEY ([SystemId]),
    CONSTRAINT [FK_Systems_SupportUnits_SupportUnitId] FOREIGN KEY ([SupportUnitId]) REFERENCES [SupportUnits] ([SupportUnitId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Units] (
    [UnitId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [BureauId] int NOT NULL,
    [UnitTypeId] int NULL,
    [ParentId] int NULL,
    [DisplayOrder] int NULL,
    [Deleted] bit NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Units] PRIMARY KEY ([UnitId]),
    CONSTRAINT [FK_Units_Bureaus_BureauId] FOREIGN KEY ([BureauId]) REFERENCES [Bureaus] ([BureauId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Units_Units_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Units] ([UnitId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Units_UnitTypes_UnitTypeId] FOREIGN KEY ([UnitTypeId]) REFERENCES [UnitTypes] ([UnitTypeId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AuditLog] (
    [AuditLogEntryId] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [ActionType] nvarchar(max) NOT NULL,
    [ResourceType] nvarchar(max) NOT NULL,
    [ResourceId] int NOT NULL,
    [Message] nvarchar(max) NULL,
    [Timestamp] datetime2 NOT NULL,
    [OldValue] nvarchar(max) NULL,
    [NewValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AuditLog] PRIMARY KEY ([AuditLogEntryId]),
    CONSTRAINT [FK_AuditLog_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE
);

GO

CREATE TABLE [SystemForms] (
    [SystemId] int NOT NULL,
    [FormId] int NOT NULL,
    CONSTRAINT [PK_SystemForms] PRIMARY KEY ([SystemId], [FormId]),
    CONSTRAINT [FK_SystemForms_Forms_FormId] FOREIGN KEY ([FormId]) REFERENCES [Forms] ([FormId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SystemForms_Systems_SystemId] FOREIGN KEY ([SystemId]) REFERENCES [Systems] ([SystemId]) ON DELETE CASCADE
);

GO

CREATE TABLE [Requesters] (
    [RequesterId] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [MiddleName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
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
    [RequestTypeId] int NOT NULL,
    [RequestedById] int NOT NULL,
    [RequestedForId] int NOT NULL,
    [RequestStatus] nvarchar(max) NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    [SubmittedOn] datetime2 NULL,
    [UpdatedOn] datetime2 NULL,
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

CREATE TABLE [FilledForms] (
    [FilledFormId] int NOT NULL IDENTITY,
    [RequestId] int NOT NULL,
    [FormId] int NOT NULL,
    [FileId] int NULL,
    CONSTRAINT [PK_FilledForms] PRIMARY KEY ([FilledFormId]),
    CONSTRAINT [FK_FilledForms_Files_FileId] FOREIGN KEY ([FileId]) REFERENCES [Files] ([FileId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FilledForms_Forms_FormId] FOREIGN KEY ([FormId]) REFERENCES [Forms] ([FormId]) ON DELETE CASCADE,
    CONSTRAINT [FK_FilledForms_Requests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [Requests] ([RequestId]) ON DELETE CASCADE
);

GO

CREATE TABLE [RequestedSystems] (
    [RequestId] int NOT NULL,
    [SystemId] int NOT NULL,
    [InPortfolio] bit NULL,
    [AccessType] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_RequestedSystems] PRIMARY KEY ([RequestId], [SystemId]),
    CONSTRAINT [FK_RequestedSystems_Requests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [Requests] ([RequestId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RequestedSystems_Systems_SystemId] FOREIGN KEY ([SystemId]) REFERENCES [Systems] ([SystemId]) ON DELETE CASCADE
);

GO

CREATE TABLE [Reviews] (
    [ReviewId] int NOT NULL IDENTITY,
    [RequestId] int NOT NULL,
    [ReviewerId] int NULL,
    [ReviewOrder] int NOT NULL,
    [ReviewerTitle] nvarchar(max) NOT NULL,
    [Approved] bit NULL,
    [Comments] nvarchar(max) NULL,
    [Timestamp] datetime2 NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([ReviewId]),
    CONSTRAINT [FK_Reviews_Requests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [Requests] ([RequestId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_Employees_ReviewerId] FOREIGN KEY ([ReviewerId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [SystemAccesses] (
    [SystemAccessId] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [SystemId] int NOT NULL,
    [RequestId] int NOT NULL,
    [ApprovedOn] datetime2 NOT NULL,
    [InPortfolio] bit NULL,
    [AccessType] nvarchar(max) NOT NULL,
    [ProcessedById] int NULL,
    [ProcessedOn] datetime2 NULL,
    [ConfirmedById] int NULL,
    [ConfirmedOn] datetime2 NULL,
    CONSTRAINT [PK_SystemAccesses] PRIMARY KEY ([SystemAccessId]),
    CONSTRAINT [AK_SystemAccesses_RequestId_SystemId] UNIQUE ([RequestId], [SystemId]),
    CONSTRAINT [FK_SystemAccesses_Employees_ConfirmedById] FOREIGN KEY ([ConfirmedById]) REFERENCES [Employees] ([EmployeeId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SystemAccesses_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SystemAccesses_Employees_ProcessedById] FOREIGN KEY ([ProcessedById]) REFERENCES [Employees] ([EmployeeId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SystemAccesses_Requests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [Requests] ([RequestId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SystemAccesses_Systems_SystemId] FOREIGN KEY ([SystemId]) REFERENCES [Systems] ([SystemId]) ON DELETE CASCADE
);

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'EmployeeId', N'Address', N'CellPhone', N'City', N'Department', N'Email', N'FirstName', N'IsAdmin', N'IsApprover', N'LastName', N'MiddleName', N'Name', N'Phone', N'Service', N'State', N'SupervisorName', N'SupportUnitId', N'Title', N'Username', N'Zip') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] ON;
INSERT INTO [Employees] ([EmployeeId], [Address], [CellPhone], [City], [Department], [Email], [FirstName], [IsAdmin], [IsApprover], [LastName], [MiddleName], [Name], [Phone], [Service], [State], [SupervisorName], [SupportUnitId], [Title], [Username], [Zip])
VALUES (1, NULL, NULL, NULL, NULL, N'pam@localhost.localdomain', N'Pam', 1, 0, N'Admin', NULL, N'Pam Admin (e111111)', NULL, NULL, NULL, NULL, NULL, NULL, N'e111111', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'EmployeeId', N'Address', N'CellPhone', N'City', N'Department', N'Email', N'FirstName', N'IsAdmin', N'IsApprover', N'LastName', N'MiddleName', N'Name', N'Phone', N'Service', N'State', N'SupervisorName', N'SupportUnitId', N'Title', N'Username', N'Zip') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] OFF;

GO

CREATE INDEX [IX_AuditLog_EmployeeId] ON [AuditLog] ([EmployeeId]);

GO

CREATE INDEX [IX_Bureaus_BureauTypeId] ON [Bureaus] ([BureauTypeId]);

GO

CREATE INDEX [IX_Employees_SupportUnitId] ON [Employees] ([SupportUnitId]);

GO

CREATE INDEX [IX_FilledForms_FileId] ON [FilledForms] ([FileId]);

GO

CREATE INDEX [IX_FilledForms_FormId] ON [FilledForms] ([FormId]);

GO

CREATE INDEX [IX_FilledForms_RequestId] ON [FilledForms] ([RequestId]);

GO

CREATE INDEX [IX_Forms_FileId] ON [Forms] ([FileId]);

GO

CREATE INDEX [IX_RequestedSystems_SystemId] ON [RequestedSystems] ([SystemId]);

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

CREATE INDEX [IX_RequiredSignatures_RequestTypeId] ON [RequiredSignatures] ([RequestTypeId]);

GO

CREATE INDEX [IX_Reviews_RequestId] ON [Reviews] ([RequestId]);

GO

CREATE INDEX [IX_Reviews_ReviewerId] ON [Reviews] ([ReviewerId]);

GO

CREATE INDEX [IX_SystemAccesses_ConfirmedById] ON [SystemAccesses] ([ConfirmedById]);

GO

CREATE INDEX [IX_SystemAccesses_EmployeeId] ON [SystemAccesses] ([EmployeeId]);

GO

CREATE INDEX [IX_SystemAccesses_ProcessedById] ON [SystemAccesses] ([ProcessedById]);

GO

CREATE INDEX [IX_SystemAccesses_SystemId] ON [SystemAccesses] ([SystemId]);

GO

CREATE INDEX [IX_SystemForms_FormId] ON [SystemForms] ([FormId]);

GO

CREATE INDEX [IX_Systems_SupportUnitId] ON [Systems] ([SupportUnitId]);

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
VALUES (N'20190405210744_InitialSchema', N'2.2.1-servicing-10028');

GO


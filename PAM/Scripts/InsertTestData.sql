SET IDENTITY_INSERT ProcessingUnits ON
INSERT INTO ProcessingUnits (ProcessingUnitId, Name, Email) VALUES (1, N'Processing Unit A', N'punit1@localhost.localdomain')
INSERT INTO ProcessingUnits (ProcessingUnitId, Name, Email) VALUES (2, N'Processing Unit B', N'punit2@localhost.localdomain')
SET IDENTITY_INSERT ProcessingUnits OFF

UPDATE Systems SET ProcessingUnitId = SystemId % 2 + 1

SET IDENTITY_INSERT Employees ON
INSERT INTO Employees (EmployeeId, Username, Name, FirstName, LastName, Email, IsAdmin, IsApprover, IsProcessor, ProcessingUnitId)
    VALUES (2, N'e123456', N'John Doe (e123456)', N'John', N'Doe', N'jdoe1@localhost.localdomain', 0, 0, 1, 1)
INSERT INTO Employees (EmployeeId, Username, Name, FirstName, LastName, Email, IsAdmin, IsApprover, IsProcessor, ProcessingUnitId)
    VALUES (3, N'e234567', N'Jane Doe (e234567)', N'Jane', N'Doe', N'jdoe2@localhost.localdomain', 0, 0, 1, 2)
SET IDENTITY_INSERT Employees OFF

GO

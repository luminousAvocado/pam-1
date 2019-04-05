UPDATE SupportUnits set Email = CONCAT(SUBSTRING(Email, 0, CHARINDEX('@', Email)), '@localhost.localdomain');
GO

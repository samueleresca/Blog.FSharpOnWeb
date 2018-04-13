CREATE DATABASE ContentDataDB
USE ContentDataDB

CREATE TABLE Labels (
    Id int  NOT NULL IDENTITY(1,1),
    Code varchar(255) NOT NULL,
    IsoCode varchar(3),
    Content varchar(max),
    Inactive bit
	CONSTRAINT [PK_SAMPLE] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)
);

-- Insert rows into table 'Labels'
INSERT INTO Labels
( -- columns to insert data into
Code, IsoCode, Content, Inactive
)
VALUES
( -- first row: values for the columns in the list above
'value2', 'IT', 'Hello', 0
)
GO

-- Select rows from a Table or View 'Labels'
SELECT * FROM Labels
GO

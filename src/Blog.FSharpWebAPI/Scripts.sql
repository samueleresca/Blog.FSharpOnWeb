CREATE DATABASE ContentData
USE ContentData

CREATE TABLE Labels (
    Id int,
    Code varchar(255),
    IsoCode varchar(3),
    Content varchar(max),
    Inactive bit
);

-- Insert rows into table 'Labels'
INSERT INTO Labels
( -- columns to insert data into
 Id, Code, IsoCode, Content, Inactive
)
VALUES
( -- first row: values for the columns in the list above
1, 'value2', 'IT', 'Hello', 0
)
GO

-- Select rows from a Table or View 'Labels'
SELECT * FROM Labels
GO
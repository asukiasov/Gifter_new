# 63BITS SQL Stored Procedure Standards

## Standard IUD Pattern (Insert, Update, Delete)
For every table (e.g., `Products`), create a single stored procedure named `[Table]IUD`.

### Template:
```sql
CREATE PROCEDURE [dbo].[ProductsIUD]
(
    @Action varchar(10), -- 'INSERT', 'UPDATE', 'DELETE'
    @ProductID int = NULL,
    @Title nvarchar(255) = NULL,
    @Price decimal(18,2) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- INSERT
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO Products (Title, Price)
        VALUES (@Title, @Price)
        
        SELECT SCOPE_IDENTITY() -- Must return the new ID
    END

    -- UPDATE
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE Products
        SET Title = ISNULL(@Title, Title),
            Price = ISNULL(@Price, Price)
        WHERE ProductID = @ProductID
    END

    -- DELETE
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM Products
        WHERE ProductID = @ProductID
    END
END
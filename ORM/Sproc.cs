/*
 * CREATE PROCEDURE GetOrdersWithFiltration
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @ProductId INT = NULL,
    @Status NVARCHAR(50) = NULL
AS
BEGIN
    SELECT *
    FROM Orders
    WHERE (@StartDate IS NULL OR CreatedDate >= @StartDate)
      AND (@EndDate IS NULL OR CreatedDate <= @EndDate)
      AND (@ProductId IS NULL OR ProductId = @ProductId)
      AND (@Status IS NULL OR Status = @Status)
END
 */
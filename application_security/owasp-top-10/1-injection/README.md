# Visma - LigaAC Labs - Application Security

# Sql Injection Vulnerabilities Mitigations

## Database users with least priviledges 
Do not use a user with owner rights. 
Create the user below and change the application connection string to use the new user created.
Example of user creation script:

```sql

/* Create SQL login User */
USE master;
CREATE LOGIN [1-Injection-User] WITH PASSWORD= /*Add a valid password otherwise script will fail , values should be defined like this N'CHANGEME' */

/* Create database user and link it to the sql user*/
USE AdventureWorks2019;
DROP USER IF EXISTS [1-Injection-User];
CREATE USER [1-Injection-User] FOR LOGIN [1-Injection-User] WITH DEFAULT_SCHEMA=[dbo];

USE master;
GRANT CONNECT SQL TO [1-Injection-User];

/* Create roles and assign rights to different tables*/
USE AdventureWorks2019;
CREATE ROLE [InjectionApiUsers];
ALTER ROLE [InjectionApiUsers] ADD MEMBER [1-Injection-User];

GRANT SELECT ON [Production].[Product] TO [InjectionApiUsers];
GRANT SELECT ON [Production].[ProductSubcategory] TO [InjectionApiUsers];


```
## Request input parameters validation and whitelisting
Proper validate the user input. 
When possbile let the framework reject requests that don't use correct variable types.
```C#
public IActionResult Products(int productCategory){....}
```
Email, phone numbers can be validated using regular expressions. 
## Parameterized queries
```C#
var sqlString = "SELECT * FROM Production.Product WHERE ProductSubCategoryID = @prodCat";
..
command.Parameters.Add("@prodCat",SqlDbType.Int);
command.Parameters["@prodCat"].Value = productCategory;
```
## Stored produres when written correclty
How to create a stored procedure
```SQL
CREATE PROCEDURE [dbo].[GetProducts]
	@ProductSubCategoryID int
	AS
	BEGIN
		SELECT * FROM Product WHERE @ProductSubCategoryID = @ProductSubCategoryID
	END 
GO
```
How to call the stored procedure
```C#
var sqlString = "GetProducts";
..
command.CommandType = CommandType.StoredProcedure;
command.Parameters.Add("@ProductSubCategoryID",SqlDbType.Int);
command.Parameters["@ProductSubCategoryID"].Value = productCategory;

```

## Using an ORM - EntityFramework
### Packages needed to run the application
---
These where already added to the project, hence you don't need to install them again.
```
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```
Look at the method *ConfigureServices* from *Startup* how the sql server is initialized.

The db model has been generated using the scaffolding. 

If you want to regenerate the models see the information below.

### Packages needed to regenerate the models 
---

See docs and needed packages : https://docs.microsoft.com/en-us/ef/core/cli/dotnet#installing-the-tools

Scaffolding command:

```
dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=AdventureWorks2019;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o DbModels
```

# Disclosure
None of the solutions gathered here are production ready! 

One should treat them with care and not deploy to public web sites.



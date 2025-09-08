# **Test Submission (Back-end)**
This is my test submission.
## Directory structure
- **Controllers**: Handles HTTP requests and responses. Defines API endpoints.
- **Data**: Contains database-related code to manage DbContext class to manage database connection and schemas.
- **DTOs**: Used to transfer data between client and server or between layers of the application.
- **Models**: Defines entities representing tables in the database.
- **Repositories**: Implements data access logics and operations.
- **Services**: Contains JWT service to handle the token generation logic.

## appsetting.json folder**
Please add the below line of code inside the appsetting.json in order to make the application work locally (replace the information inside the "{}", using SQL Server).

    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=NETAPI;Trusted_Connection=True;TrustServerCertificate=True;"
    },
    "Jwt": {
        "Secret": "{yoursecretkey}", // Secret keys must be over 32 characters
        "Issuer": "NETAPI",
        "Audience": "NETAPIClient",
        "AccessTokenExpr": "60"
    }

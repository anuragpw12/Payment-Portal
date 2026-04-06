🚀 Payments Portal – Full Stack Application
A full-stack Payments Management System built using Angular 15 and .NET 8 Web API, following Clean Architecture, Dapper, and SQL Server Stored Procedures.

📌 Summary
This project implements a complete payment lifecycle system (Create, Read, Update, Delete) with a strong focus on scalability, clean architecture, and real-world reliability.
The system prevents duplicate transactions using an idempotent API design with clientRequestId, ensuring safe retries and consistent data.

🏗️ Architecture
The backend follows Clean Architecture principles:
Domain → Core business entities and rules
Application → Business logic and services
Infrastructure → Dapper + Stored Procedures
API → Controllers and request handling

Key Design Decisions
Dapper for high-performance data access
Stored Procedures for critical logic
Separation of concerns for maintainability
Idempotent API design for reliability

🔁 Idempotency (Duplicate Prevention)
To prevent duplicate transactions:
Each request includes a unique clientRequestId (GUID)
If the same request is sent again:
✅ Existing record is returned
❌ No duplicate record is created

Why this matters:
Safe retry mechanism
Prevents financial inconsistencies
Production-ready behavior

🏷️ Payment Reference Generation
Each payment gets a unique reference:

Format:
PAY-YYYYMMDD-####

Example:
PAY-20250910-0001

Sequence resets daily
Generated at database level
Ensures uniqueness and traceability

🧩 Features
View all payments
Create new payment
Edit existing payment
Delete payment with confirmation
Duplicate transaction prevention
Responsive UI

🏗️ Tech Stack
🔹 Frontend
Angular 15
TypeScript
Bootstrap 5
Reactive Forms
HttpClient
ngx-toastr
SweetAlert2

🔹 Backend
.NET 8 Web API
Clean Architecture
Dapper ORM
SQL Server
Stored Procedures
Swagger



📂 Project Structure
Backend
Payments.Api → Controllers, Swagger
Payments.Application → Business Logic, DTOs
Payments.Domain → Entities
Payments.Infrastructure → Dapper + DB Access
sql/ → Database scripts

Frontend
src/app/payments/
├── pages/
│ ├── payments-list
│ └── payment-form
├── services
└── models



⚙️ Setup Instructions
🔹 Backend Setup
Create Database:
CREATE DATABASE PaymentsDb;
GO
Run SQL Script:
Backend\Scripts\sql\Stored Procedures
Backend\Scripts\sql\Tables and Index

Update Connection String:

"ConnectionStrings": {
  "PaymentsDb": "Server=.;Database=PaymentsDb;Trusted_Connection=True;TrustServerCertificate=True;"
}

Run API:
dotnet restore Payments.sln
dotnet build Payments.sln
dotnet run --project Payments.Api

🔹 Frontend Setup
Install dependencies:
npm install
Run app:
npm start

Open:
http://localhost:4200

🔗 API Endpoints
POST /api/payments → Create payment
GET /api/payments → Get all payments
PUT /api/payments/{id} → Update payment
DELETE /api/payments/{id} → Delete payment

🖥️ UI Features
Payments List
Displays Reference, Amount, Currency, CreatedAt
Edit and Delete actions

Payment Form
Create & Edit mode
Validation (Amount > 0, Currency required)
Auto clientRequestId generation

UX Enhancements
Toast notifications
Confirmation dialogs
Responsive layout



📸 Screenshots
Payments List
<img width="1920" height="1080" alt="Frontend - PaymentsList" src="https://github.com/user-attachments/assets/ecb692ef-6aeb-4702-9fa2-889febf60f94" /> 

Create Payment
<img width="1920" height="1080" alt="Frontend - Add" src="https://github.com/user-attachments/assets/67f26096-c804-47e8-ac76-e8e9401a943b" />

Edit Payment
<img width="1920" height="1080" alt="Frontend - Edit" src="https://github.com/user-attachments/assets/dd45a6a8-061d-4efc-921e-accebe2cf1eb" /> 

Delete
<img width="1920" height="1080" alt="Frontend - Delete" src="https://github.com/user-attachments/assets/13264e4d-850f-4737-b0a4-390811fe8c77" />

Swagger API 
<img width="1920" height="1080" alt="Backend - Swagger" src="https://github.com/user-attachments/assets/1ca846d9-5840-41e2-8228-71f104e2901a" />

🧪 Swagger
https://localhost:<port>/swagger
Used for API testing and validation.

🚀 Highlights
Idempotent API design (prevents duplicate transactions)
Clean Architecture implementation
High-performance data access using Dapper
Business logic handled at DB level
Responsive Angular UI
Production-ready structure

🔮 Future Improvement
Add JWT Authentication
Backend pagination support
Unit & integration testing
Caching (Redis)
Logging & monitoring

📌 Submission
Public GitHub repository
Includes:
Source code
Screenshots
Swagger
Database schema

💡 Key Takeaway
This project demonstrates how to build a scalable, reliable payment system using:
Idempotent APIs
Clean Architecture
Efficient database design

Ensuring no duplicate transactions even in retry scenarios.

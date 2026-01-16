# Insult Generator – Lab 2

**Short description**  
A WPF application that lets the user generate, view, favourite, edit, and delete insults. All data is stored in a local SQL Server database using Entity Framework Code-First with migrations.

**Features**
- Full CRUD operations using Entity Framework:
  - Create: Add new insults
  - Read: List all insults + generate a random one
  - Update: Edit existing insults
  - Delete: Remove insults
- At least two related tables:
  - `Insults` (Id, Text, Rating)
  - `Categories` (Id, Name) + many-to-many relationship
- Both tables are actively used in the app

**Database**
- Created automatically on first run (via EF migrations)
- Uses SQL Server (localhost) – database name: `insulter`

**How to run the project (tested on a clean machine)**

Release 1.0 is available in Releases

![Login](Screenshots/LoginScreenshot.png)
![GermanInsult](Screenshots/GermanInsultScreenshot.png)
![EnglishInsult](Screenshots/EnglishInsultScreenshot.png)


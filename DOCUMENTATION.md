# QueueApp Documentation

Welcome to the QueueApp project! This is a modern, web-based queue management system designed to handle ticket generation, counter assignment, and real-time status displays.

---

## 1. Technology Stack
This application is built using the following technologies:
- **Framework:** .NET (ASP.NET Core / Blazor)
- **UI Architecture:** Blazor Hybrid/Web (Razor components)
- **Styling:** Tailwind CSS (via utility classes)
- **Database:** SQLite (Relational database)
- **Language:** C#

---

## 2. Project Architecture
The project is structured into clear functional layers:

- **`/Pages`**: Contains the Blazor Razor components that make up the user interface.
    - `Kiosk.razor`: Customer interface for generating tickets.
    - `CounterPage.razor`: Admin/Staff dashboard for managing the queue and counters.
    - `DisplayPage.razor`: Public view for customers to see live calling status.
    - `Admin.razor`: Management page for counter staff setup.
- **`/Core`**: Contains the business logic and services.
    - `QueueManager.cs`: The central engine that handles ticket generation, calling, serving, and updating data.
- **`/Models`**: Defines the data structures (C# classes) used throughout the app.
    - `Ticket.cs`, `Counter.cs`, `User.cs`.
- **`/Data`**: Manages data persistence.
    - `DatabaseContext.cs`: Initializes the SQLite database and manages connections.
- **`/wwwroot`**: Static assets like CSS and HTML files.

---

## 3. Database Details
- **Type:** SQLite (`queue.db`)
- **Connection:** Managed via `Microsoft.Data.Sqlite` in `Data/DatabaseContext.cs`.
- **Access:** The `QueueManager` class acts as the interface between the UI and the database. It uses standard SQL commands (wrapped in C# methods) to CRUD (Create, Read, Update, Delete) tickets and counters.
- **Schema:** 
    - `Users`: Stores administrator credentials.
    - `Counters`: Defines stations and staff assignments.
    - `Tickets`: Tracks the lifecycle of every generated ticket (Waiting -> Calling -> Served).

---

## 4. Key Workflows
1. **Ticket Generation:** Customers visit the `/kiosk` page to enter their name/phone. `QueueManager.GenerateTicket` adds the entry to the `Tickets` table with status `Waiting`.
2. **Queue Management:** Staff visit the `/counter` dashboard. This dashboard shows all available counters and the waiting queue.
3. **Calling Tickets:** Staff click "Call" on a waiting ticket. This updates the ticket status in the database to `Calling` and assigns it to a `CounterId`.
4. **Displaying Status:** The `/display` page polls the database every 3 seconds to show which tickets are currently being called and which counters are active.

---

## 5. How to Run the Project
1. **Prerequisites:** Ensure you have the [.NET SDK](https://dotnet.microsoft.com/) installed (version 9.0 or 10.0 recommended).
2. **Navigate:** Open your terminal to the project root directory (`C:\Users\TEST\QueueApp`).
3. **Run:** Execute the following command:
   ```bash
   dotnet run
   ```
4. **Access:** The application will launch a window or provide a local URL (e.g., `http://localhost:5000`) to view the application in your browser.

---

## 6. Development Tips
- **Modifying UI:** Edit the `.razor` files in the `/Pages` folder. Changes will reflect after a restart or hot-reload.
- **Adding Logic:** Add new business rules to `Core/QueueManager.cs`.
- **Database Changes:** If you add a new table or column, update `Data/DatabaseContext.cs`. Remember to use `PRAGMA table_info` checks if you need to perform migrations on an existing `queue.db`.

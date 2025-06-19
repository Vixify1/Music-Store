# 🎵 Music Store

A full-featured e-commerce web application for online music store management built with ASP.NET Core MVC and Entity Framework Core.

The application provides a complete solution for managing music albums, artists, genres, and customer orders with secure authentication and administrative features.

## ✨ Features

### Customer Features
- 🔐 **User Authentication & Registration** - Secure login and registration system
- 🛒 **Shopping Cart** - Add, edit, and remove items from cart
- 📀 **Album Catalog** - Browse and search music albums
- 🎨 **Artist & Genre Management** - Organized music categorization
- 📦 **Order Management** - Place and track orders
- 👤 **User Profile** - Manage personal information and order history

### Administrative Features
- 🔧 **Admin Panel** - Complete administrative control
- 📊 **Customer Management** - View and manage customer accounts
- 🎵 **Album Management** - Add, edit, and delete albums
- 🎭 **Artist Management** - Manage artist information
- 🏷️ **Genre Management** - Organize music categories
- 📈 **Order Tracking** - Monitor and manage customer orders

### Technical Features
- 🔒 **ASP.NET Core Identity** - Secure authentication and authorization
- 💾 **Entity Framework Core** - Robust data access and management
- 🎨 **Responsive Design** - Mobile-friendly user interface
- 📱 **Session Management** - Persistent shopping cart experience
- 🔄 **Database Migrations** - Version-controlled database schema

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8.0 (MVC)
- **Database**: SQL Server with Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap
- **Architecture**: Repository Pattern with Dependency Injection
- **ORM**: Entity Framework Core with Code-First approach


## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or Full)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   
3. **Restore dependencies**

4. **Update connection string**
   
   Edit `MusicStore/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=MusicStoreDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
     }
   }
   ```

5. **Run database migrations**

6. **Run and access the application**
   
## 🗃️ Database Setup

The application uses Entity Framework Core with SQL Server. The database schema includes:

- **Users & Authentication** (AspNetUsers, AspNetRoles, etc.)
- **Albums** - Music album information
- **Artists** - Artist details
- **Genres** - Music categories
- **Customers** - Customer profiles
- **Cart & CartItems** - Shopping cart functionality
- **Orders & OrderItems** - Order management

## 🎮 Usage

### For Customers
1. Register a new account or login
2. Browse albums by category or search
3. Add items to your shopping cart
4. Proceed to checkout and place orders
5. View your order history in the user profile

### For Administrators
1. Login with administrative credentials
2. Access admin panel for managing:
   - Albums (Add, Edit, Delete)
   - Artists and Genres
   - Customer accounts
   - Order management

## Developer
This project was developed as a collaborative effort by our development team.


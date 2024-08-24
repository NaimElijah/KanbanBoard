# Kanban
Instructions: 
* Replace "ID1_ID2_ID3" with the correct ID numbers
* Replace the "00" number at the end with your group's number


## Group members
206001018_317971752_313351439

## Group number (from Moodle)
A

# **Kanban Project**

## **Overview**

This project is a Kanban board application designed to help users manage tasks and workflows efficiently. The application is built with a multi-layered architecture, including separate frontend and backend components. The frontend is developed using WPF with an MVVM (Model-View-ViewModel) architecture, while the backend handles the core business logic, data persistence, and services.

## **Features**

• **Task Management**: Create, update, and manage tasks on a Kanban board.

• **MVVM Architecture**: Ensures a clear separation of concerns between the UI and business logic.

• **Data Persistence**: Uses SQLite for database management to store tasks and related data.

• **Logging**: Integrated logging using log4net for tracking application activities and debugging.

• **Custom Exceptions**: Handles errors gracefully with custom exception handling.

  
## **Project Structure**

### **Frontend**

• **App.xaml & App.xaml.cs**: Application-level resources and startup logic.

• **AssemblyInfo.cs**: Metadata about the frontend assembly.

• **Model, View, ViewModel**: Implements the MVVM pattern for a clean separation of UI and business logic.

• **NotifiableObject.cs**: Base class for data binding, implementing INotifyPropertyChanged.

  
### **Backend**

• **BusinessLayer**: Contains the core business logic.

• **DataAccessLayer**: Manages interactions with the SQLite database.

• **ServiceLayer**: Provides services that the frontend consumes.

• **Exceptions**: Custom exceptions used throughout the application.

• **Logger.cs**: Manages logging using log4net.

  
### **Getting Started**

##### **Prerequisites**

• **.NET Framework 6.0 or later**

• **SQLite**: Ensure SQLite is installed and accessible.

##### **Installation**

1. Clone the repository:
   
   Open the folder you want to clone to in the terminal/git bash and enter:
   
    `git clone https://github.com/BGU-SE-Courses/kanban-2024-m3-2024-a.git`

3. Open the solution file Kanban.sln in Visual Studio.

  
**Running the Application**

1. Build the solution in Visual Studio.

2. Start the application by running the Frontend project.


**Logging**

The application uses log4net for logging. Configuration is managed in the log4net.config file located in the Backend directory.


**Database**

The application uses SQLite for data storage. The database template file kanban.db is included in the root directory of the project.
**The database needs to be copied to the bin directory of the Frontend after compilation!**


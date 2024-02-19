Marketplace
=============

Overview
--------

This project is a .NET application designed to manage orders and user registrations for a marketplace platform. It features a RESTful API for order functionality, allowing users to create, retrieve, update, and delete orders. Additionally, it supports user creation based on external user IDs. The project utilizes a PostgreSQL database to store user and order information.

### Key Features

*   **Order Management**: Create, retrieve, update, and delete orders.
*   **User Registration**: Register users based on external IDs.
*   **Dockerized PostgreSQL Database**: Ensures easy setup and consistency across development environments.
*   **Background Service Integration**: Includes a background service for specific functionalities, such as cleaning up unpaid orders.

Getting Started
---------------

### Prerequisites

*   .NET 8.0
*   Docker and Docker Compose
*   Liquibase for database migrations

### Setting Up the Database

The project uses a Dockerized PostgreSQL database. To start the database:

1.  Ensure Docker is installed and running on your machine.
2.  Navigate to the root directory of the project where the `docker-compose.yml` file is located.
3.  Run the following command to build and start the database container:

`docker-compose up --build`

This command builds the necessary Docker image for the PostgreSQL database and starts a container running the database. The database is configured automatically based on the settings specified in the `docker-compose.yml` file.

### Running Migrations

To apply database migrations, navigate to the migrations folder and use Liquibase:

1.  Ensure Liquibase is installed on your machine.
2.  Open a terminal and navigate to the `migrations` folder within the project.
3.  Run the following command to apply migrations:

`liquibase update --changelog-file=dbchangelog.sql`

This command applies the necessary database schema changes specified in the `dbchangelog.sql` file.

Current Working State
---------------------

*   The database is fully Dockerized and can be started with `docker-compose`.
*   The .NET application itself is not Dockerized. A known issue with the background service leads to the application being shut down. This is under investigation.

### Running the Application

After setting up the database, you can run the .NET application locally by navigating to the project root directory and using the following command:

`dotnet run`

This command starts the application, making the API endpoints available for interaction.

Contributing
------------

Contributions are welcome. Please feel free to submit pull requests or open issues to discuss proposed changes or report bugs.

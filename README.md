# ChatGPT_CodeGen
Demo-Projects for the article "Code Generation Revolution from models to the natural requirements" using different programming languages

## Car Rental System in C# (CarRentalSystem)
This project is a console-based car rental system built with C# using .NET Framework. It allows users to register customers, add and delete cars, and rent and return cars. The system uses a file-based data storage system to persist data between application runs.

###Installation
To run the Car Rental System, you need to have the following installed on your system:

- .NET Core 3.1 or later
- Newtonsoft Json (13.0.3)
- Visual Studio (recommended) or any other C# compiler
- Once you have installed the required dependencies, you can clone this repository and open the solution file in Visual Studio to build and run the application.

### Usage
The Car Rental System is a console-based application that takes input as command-line arguments. Here are the available commands and their usage:

- `cmd=register_customer customer="ID,Name,PhoneNumber,Address,Email"`: Register a new customer
- `cmd=delete_customer customer="ID"`: Delete an existing customer
- `cmd=add_car car="ID,Make,Model,Year,PricePerDay"`: Add a new car to the system
- `cmd=delete_car car="ID"`: Delete an existing car from the system
- `cmd=rent_car car="ID" customer="ID"`: Rent a car to a customer
- `cmd=return_car customer="ID"`: Return a rented car from a customer

##Contributing
Contributions are welcome! If you find a bug or want to add a new feature, please open an issue or submit a pull request.

##License
This project is licensed under the MIT License. See the LICENSE file for more details.

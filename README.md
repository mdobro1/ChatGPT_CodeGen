# ChatGPT_CodeGen
Demo-Projects for the article "Code Generation Revolution from models to the natural requirements" using different programming languages. 

## Car Rental System 
This project is a console-based car rental system. It allows users to register customers, add and delete cars, and rent and return cars. 
The system uses a file-based data storage system to persist data between application runs in CSV- or JSON-format.

## Business perspective
From a business perspective, the code represents a car rental system where customers can register and rent cars. 
The system allows for adding and deleting cars and customers, as well as renting and returning cars. 
The code also includes test commands to test the system.

## Technical perspective
From a technical perspective, the code includes several classes and methods:

- ***MainProgram*** class includes the main method which handles the program execution and calls other methods to perform specific commands. 
- ***CarRentalSystem*** class represents the car rental system and includes methods to add and delete cars and customers, as well as renting and returning cars. 
- ***CarRentalCommands*** class includes methods to load and save data, as well as perform the specific commands such as adding and deleting cars and customers and renting and returning cars.
- ***CarRentalContext*** class is a data structure that holds information about the current command being executed and the specific car or customer involved. 

The code uses command-line arguments to pass information to the program and includes error handling to ensure that the correct arguments are provided. 
The data is loaded/saved from/to the ***data***-sub-directory. 
The code also includes logging and messaging functionality to provide feedback to the user.

### C# implementation (CarRentalSystem)
The models and skeleton code for this project have been generated by [Chat-GPT 3.5](https://chat.openai.com/) see *"Experiment_3_?.txt"* files in [Experiments](Experiments) and then was extended manually.
Using ChatGPT as an assistance tool has saved a lot of time especially in initial prototyping and test development phase. 
**However, the output of a LLM as ChatGPT is non-deterministic and therefore must be critically checked!**

#### Installation
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

## Contributing
Contributions are welcome! If you find a bug or want to add a new feature, please open an issue or submit a pull request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
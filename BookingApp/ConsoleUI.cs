using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace BookingApp
{
    internal class ConsoleUI
    {
        bool inMenu = true;
        int userSelection;
        List<IRoom> rooms = new List<IRoom>(); //this list stores both Room and _3DRoom objects.
        BinaryFormatter formatter = new BinaryFormatter();
        static string[] movieTitles = { "Avengers: Endgame", "The Dark Knight", "Interstellar", "The Matrix" };
        static int currentTitleIndex = 0;
        TimeTracker tracker = new TimeTracker();
        Thread movieThread = new Thread(new ThreadStart(MovieThread)); //create a thread to update the movie title


        public void Run()
        {
            Console.Title = "Cinema booking app";

            movieThread.Start(); //start the thread that updates the currently playing movie
            tracker.StartTracking(); //start the thread that tracks the timer spent in app
            MovieThread();
            LoadData(); //load the list of rooms from the file rooms.dat
            Menu(); //display the menu to the user and links all the available options the user has
            ExportData(); //save the list of rooms to the file rooms.dat

            int timeSpentInSeconds = tracker.GetTimeInSeconds(); //update the time spent in app into the timeSpentInSeconds variable

            Console.Clear();
            Console.WriteLine($"\n\nThank you for using the booking app.\nYou have spent {timeSpentInSeconds} seconds in the booking app. \nI hope you enjoyed your experience.");
            Thread.Sleep(1000);
        }

        static void MovieThread()
        {
            // Start a background thread to update the movie title every 5 seconds
            Thread backgroundThread = new Thread(new ThreadStart(UpdateMovieTitle));
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
        }

        static void UpdateMovieTitle()
        {
            while (true)
            {
                // Wait for 5 seconds
                Thread.Sleep(5000);

                // Increment the title index, and wrap around if necessary
                currentTitleIndex = (currentTitleIndex + 1) % movieTitles.Length;
            }
        }
            public void LoadData()
        {
            // check if file exists and is not empty
            if (File.Exists("rooms.dat") && new FileInfo("rooms.dat").Length > 0)
            {
                // create file stream to read serialized data from
                FileStream fileStream = new FileStream("rooms.dat", FileMode.Open);

                #pragma warning disable SYSLIB0011
                // deserialize data from file stream
                List<IRoom> loadedRooms = (List<IRoom>)formatter.Deserialize(fileStream);

                // close file stream
                fileStream.Close();

                // add loaded rooms to the current list
                rooms.AddRange(loadedRooms);
                rooms.Add(new _3DRoom("3D", 5, 7));
            }
            else
            {
                // create new rooms with the default settings if file does not exist or is empty
                rooms.Add(new Room("Room 1", 6, 10));
            }
        }

        public void ExportData()
        {
            // create file stream to write serialized data to
            FileStream fileStream2 = new FileStream("rooms.dat", FileMode.Create);

            // serialize data to file stream
            formatter.Serialize(fileStream2, rooms);

            // close file stream
            fileStream2.Close();
        }

        public void Menu()
        {
            while (inMenu) // Loop while inMenu is true (initially set to true)
            {

                Console.Clear();

                // Display the cinema title and the menu options
                DisplayMenu();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);  // Read the user's key input without showing it on the console
                switch (keyInfo.Key) // Switch statement based on the key pressed by the user
                {
                    case ConsoleKey.D1:
                        // If there is at least one room in the list, book seats in the first room (this should be the default room unless the default room is deleted)
                        if (rooms.Count() > 0)
                        {
                            BookSeats((Room)rooms[0]);
                        }

                        // If there are no rooms in the list, display a message and wait for user input to get back into the menu selection
                        if (rooms.Count() == 0)
                        {
                            Console.Clear();
                            Console.WriteLine("No room is currently available. Press any key to return to the menu.");
                            Console.ReadKey();
                            break;
                        }

                        break;

                    case ConsoleKey.D2:

                        Console.Clear();
                        DisplayAllRooms(); //show the list of available rooms (if there is any) to the user

                        // If there is at least one room in the list, prompt the user to select a room
                        if (rooms.Count() > 0)
                        {
                            Console.WriteLine("Select a room.");

                        }

                        userSelection = GetIntUserInput() - 1;// Get user input and subtract 1 to account for zero-based indexing

                        if (userSelection < 0 || userSelection >= rooms.Count) // If the user's selection is invalid, display an error message and wait for user input
                        {
                            Console.WriteLine($"Invalid selection: {userSelection}. Please select a valid option from the list. Press any key to get back to the menu.");
                            Console.ReadKey();
                        }
                        else
                        {
                            // Otherwise, if all the checks are passed succesfully, book seats in the selected room
                            Room selectedRoom = (Room)rooms[userSelection];
                            BookSeats(selectedRoom);
                        }

                        break;

                    case ConsoleKey.D3:
                        AddNewRoom(); // Add a new room to the list of rooms with the parameters specified by the user
                        break;

                    case ConsoleKey.D4:
                        RemoveRoom(); //Remove a room form the list
                        break;

                    case ConsoleKey.D5:
                        // Display all available rooms to the user if any exists
                        Console.Clear();
                        DisplayAllRooms();

                        if (rooms.Count() > 0)// If there is at least one room in the list, prompt the user to select a room
                        {
                            Console.WriteLine("Select a room.");

                        }

                        userSelection = GetIntUserInput() - 1; // Get user input and subtract 1 to account for zero-based indexing

                        if (userSelection < 0 || userSelection >= rooms.Count)// If the user's selection is invalid, display an error message and wait for user input
                        {
                            Console.WriteLine($"Invalid selection: {userSelection}. Please select a valid option from the list. Press any key to get back to the menu.");
                            Console.ReadKey();
                        }
                        else
                        {
                            //If everything has been checked succesfully, allow the user to book seats in the selected room
                            Room selectedRoom = (Room)rooms[userSelection];
                            BookCustomSeat(selectedRoom);
                        }

                        break;

                    case ConsoleKey.D6:

                        Console.Clear();
                        DisplayAllRooms(); //Show the available rooms to the user

                        if (rooms.Count() > 0) //If at least one room exists, promt user to select a room
                        {
                            Console.WriteLine("Select a room.");

                        }

                        userSelection = GetIntUserInput() - 1; //Get user input -1 to account for zero-based indexing

                        if (userSelection < 0 || userSelection >= rooms.Count) //If the selection is invalid, display an error message
                        {
                            Console.WriteLine($"Invalid selection: {userSelection}. Please select a valid option from the list. Press any key to get back to the menu.");
                            Console.ReadKey();
                        }
                        else
                        {
                            Room selectedRoom = (Room)rooms[userSelection]; //if the checks are passed succesfully, allow the user to unbook seats
                            UnbookCustomSeats(selectedRoom);
                        }

                        break;

                        case ConsoleKey.D7: //book multiple seats from the selected row (this allows you to chose any seats you want to book)
                        Console.Clear();
                        DisplayAllRooms(); //Show the available rooms to the user

                        if (rooms.Count() > 0) //If at least one room exists, promt user to select a room
                        {
                            Console.WriteLine("Select a room.");

                        }

                        userSelection = GetIntUserInput() - 1; //Get user input -1 to account for zero-based indexing

                        if (userSelection < 0 || userSelection >= rooms.Count) //If the selection is invalid, display an error message
                        {
                            Console.WriteLine($"Invalid selection: {userSelection}. Please select a valid option from the list. Press any key to get back to the menu.");
                            Console.ReadKey();
                        }
                        else
                        {
                            Room selectedRoom = (Room)rooms[userSelection]; //if the checks are passed succesfully, allow the user to unbook seats
                            BookCustomSeats(selectedRoom);
                        }
                        break;

                    case ConsoleKey.D8: //unbook multiple seats at once
                        Console.Clear();
                        DisplayAllRooms(); //Show the available rooms to the user

                        if (rooms.Count() > 0) //If at least one room exists, promt user to select a room
                        {
                            Console.WriteLine("Select a room.");

                        }

                        userSelection = GetIntUserInput() - 1; //Get user input -1 to account for zero-based indexing

                        if (userSelection < 0 || userSelection >= rooms.Count) //If the selection is invalid, display an error message
                        {
                            Console.WriteLine($"Invalid selection: {userSelection}. Please select a valid option from the list. Press any key to get back to the menu.");
                            Console.ReadKey();
                        }
                        else
                        {
                            Room selectedRoom = (Room)rooms[userSelection]; //if the checks are passed succesfully, allow the user to unbook seats
                            UnbookMultipleSeats(selectedRoom);
                        }
                        break;

                    case ConsoleKey.D9: //Display all available rooms until the user presses a key
                        Console.Clear();
                        DisplayAllRooms();
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();

                        break;


                    case ConsoleKey.D0: //Switch inMenu variable to false to allow the user to exit the menu and stop the program
                        inMenu = false;
                        break;

                    default: //If anything else than the options available in the menu is pressed then show an error message
                        Console.WriteLine("Invalid key pressed");
                        break;
                }
            }
        }
        public string GetStringUserInput() //Get user input checking for null values
        {

            string input = Console.ReadLine();

            return input;
        }

        public void DisplayMenu() //Display the available options and the warning message for the correct way to save the progress of the app
        {
            CinemaTitle();

            Console.WriteLine("Press 1 to use the default room settings (10 rows of 6 seats each).");
            Console.WriteLine("Press 2 to book multiple seats on the same row (last available row will be used).");
            Console.WriteLine("Press 3 to create a new room.");
            Console.WriteLine("Press 4 to remove a room.");
            Console.WriteLine("Press 5 to book a specific seat.");
            Console.WriteLine("Press 6 to unbook a specific seat.");
            Console.WriteLine("Press 7 to book multiple seats. (the seats can be selected by the user, they don't have to be next to each other)");
            Console.WriteLine("Press 8 to unbook multiple seats. (the seats can be selected by the user, they don't have to be next to each other)");
            Console.WriteLine("Press 9 to display all existing rooms.");
            Console.WriteLine("Press 0 to exit the program.");
            Console.WriteLine($"\nNow playing: " + movieTitles[currentTitleIndex]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(0, Console.WindowHeight - 4);
            Console.WriteLine("Please close the app using the 0 key to save the rooms created and the bookings.\n\nYOUR PROGRESS SINCE THE LAST SAVE WILL BE LOST OTHERWISE");
            Console.ForegroundColor = ConsoleColor.White;

        }


        public void AddNewRoom() //Add a room
        {
            Console.Clear();

            Console.WriteLine("Input name/ID of the room you wish to create.");
            string roomName = GetStringUserInput();

            Console.WriteLine("Input number of seats per row.");
            int seatsPerRow = GetIntUserInput();

            Console.WriteLine("Input number of rows the room has.");
            int rowsOfSeats = GetIntUserInput();

            rooms.Add(new Room(roomName, seatsPerRow, rowsOfSeats));
        }

        public void RemoveRoom()
        {
            Console.Clear();
            DisplayAllRooms();
            Console.WriteLine("\nSelect the number of the room to remove.");

            int userSelection = GetIntUserInput() - 1;

            if (userSelection < 0 || userSelection >= rooms.Count)
            {
                Console.WriteLine($"Invalid selection: {userSelection}. Please select a valid option from the list. Press any key to get back to the menu.");
                Console.ReadKey();
            }
            else if (userSelection == 0 || userSelection == 1)
            {
                Console.WriteLine("Sorry, the first and second rooms cannot be deleted.");
                Thread.Sleep(2000);
            }
            else
            {
                Console.WriteLine($"Are you sure you want to remove room {rooms[userSelection].roomName}? \nPress \"Y\" to confirm or any other key to cancel.)");
                string confirm = GetStringUserInput().ToUpper();

                if (confirm == "Y")
                {
                    Console.WriteLine($"{rooms[userSelection].roomName} has been removed. Press any key to return to the menu.");
                    rooms.RemoveAt(userSelection);
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"Cancelling removal of {rooms[userSelection].roomName}. Press any key to return to the menu.");
                    Console.ReadKey();
                }
            }
        }

        public void BookCustomSeats(Room selectedRoom) // Book one seat wherever the user whishes to as long as it exists and it is not booked
        {
            bool booking = true;

            while (booking) // Loop until user exits the booking process
            {
                Console.Clear();
                DisplayRoom(selectedRoom);

                int maxRow = selectedRoom.roomSeats.Count;
                int maxSeat = selectedRoom.roomSeats[0].Count;

                Console.WriteLine($"Input row number to book seats on (1-{maxRow}):\nPress 0 to exit the booking process. "); // Prompt user for row number
                int rowToBook = GetIntUserInput();

                if (rowToBook == 0) // Exit booking process if user input is 0
                {
                    booking = false;
                    break;
                }
                 
                Console.WriteLine($"Input seat numbers to book on row {rowToBook} (1-{maxSeat}), separated by commas (e.g. 1,3,4):\nPress 0 to exit the booking process. "); // Prompt user for seat numbers
                string seatNumbers = GetStringUserInput();
                if (seatNumbers == "0") // Exit booking process if user input is 0
                {
                    booking = false;
                    break;
                }

                string[] seats = seatNumbers.Split(','); // Split seat numbers into array of strings. This is used to book multiple seats.

                bool validSeats = true;
                List<Seat> seatsToBook = new List<Seat>();

                foreach (string seatStr in seats) // Iterate through seat numbers to check if they are valid and available
                {
                    if (!int.TryParse(seatStr, out int seatNum) || seatNum < 1 || seatNum > maxSeat)
                    {
                        validSeats = false;
                        break;
                    }

                    Seat seat = selectedRoom.roomSeats[rowToBook - 1][seatNum - 1];
                    if (seat.isBooked)
                    {
                        validSeats = false;
                        break;
                    }

                    seatsToBook.Add(seat);
                }

                if (!validSeats) // If any of the seats are invalid or booked, notify user and restart booking process
                {
                    Console.WriteLine($"Invalid input. Seat numbers must be between 1 and {maxSeat} and must not be already booked. \nPress any key to continue.");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Are you sure you want to book these seats? (Y/N)"); // Confirm booking with user
                    string confirmation = GetStringUserInput().ToUpper();

                    if (confirmation == "Y") // If user confirms booking, mark seats as booked and notify user
                    {
                        foreach (Seat seat in seatsToBook)
                        {
                            seat.isBooked = true;
                        }

                        Console.WriteLine($"Successfully booked seats on row {rowToBook}.");
                    }
                    else // If user presses any other key, then the user is sent back to the booking process and the "Booking cancelled" message is displayed.
                    {
                        Console.WriteLine("Booking cancelled.");
                    }
                }
            }
        }

        public void UnbookMultipleSeats(Room selectedRoom)
        {
            Console.Clear();
            DisplayRoom(selectedRoom);

            int maxRow = selectedRoom.roomSeats.Count;
            int maxSeat = selectedRoom.roomSeats[0].Count;

            Console.WriteLine($"Input row number to unbook seats on (1-{maxRow}):\nPress 0 to exit the unbooking process. "); // Prompt the user to enter the row number to unbook seats from
            int rowToUnbook = GetIntUserInput();

            if (rowToUnbook == 0)
            {
                return;
            }

            Console.WriteLine($"Input seat numbers to unbook on row {rowToUnbook} (1-{maxSeat}) separated by commas (e.g. 1,3,5):\nPress 0 to exit the unbooking process. "); // Prompt the user to enter the seat numbers to unbook, separated by commas
            string seatsToUnbookString = GetStringUserInput();

            if (seatsToUnbookString == "0")
            {
                return;
            }

            string[] seatNumbers = seatsToUnbookString.Split(',');
            List<int> seatsToUnbook = new List<int>();

            foreach (string seatNumber in seatNumbers) // Parse the user input for seat numbers and add valid seat numbers to a list
            {
                if (int.TryParse(seatNumber, out int seat) && seat >= 1 && seat <= maxSeat)
                {
                    seatsToUnbook.Add(seat);
                }
            }

            bool unbookedSeats = false;

            foreach (int seat in seatsToUnbook) // Iterate through the list of seats to unbook and prompt the user for confirmation before unbooking
            {
                if (selectedRoom.roomSeats[rowToUnbook - 1][seat - 1].isBooked)
                {
                    Console.WriteLine($"Are you sure you want to unbook seat {seat} in row {rowToUnbook}? (Y/N)");
                    string confirmation = GetStringUserInput().ToUpper();

                    if (confirmation == "Y")
                    {
                        // Unbook the seat if the user confirms
                        selectedRoom.roomSeats[rowToUnbook - 1][seat - 1].isBooked = false;
                        Console.WriteLine($"Successfully unbooked seat {seat} in row {rowToUnbook}.");
                        unbookedSeats = true;
                    }
                    else
                    {
                        Console.WriteLine($"Seat {seat} in row {rowToUnbook} was not unbooked.");
                    }
                }
                else
                {
                    Console.WriteLine($"Seat {seat} in row {rowToUnbook} is not booked.");
                }
            }

            if (!unbookedSeats) // If no seats were unbooked, inform the user
            {
                Console.WriteLine($"The requested seats cannot be unbooked. Please select other seats.");
            }

            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey();
        }

        public void DisplayAllRooms() //Display a numbered list of all rooms in the "room" list
        {
            int i = 0;
            foreach (Room room in rooms)
            {
                i++;
                Console.WriteLine($"Selection {i} = Room: {room.roomName}");

            }

            if (rooms.Count == 0) // If there are no rooms in the list, inform the user
            {
                Console.WriteLine("No room available. Press any key to exit.");
            }
        }

        public void BookSeats(Room selectedRoom) //Book a certain number of seats in the last available row
        {
            bool booking = true;

            while (booking) //loop until the user finishes booking
            {
                // Clear the console and display the selected room
                Console.Clear();
                DisplayRoom(selectedRoom);

                Console.WriteLine("Input number of seats to book:\nPress 0 to exit the booking process. "); // Prompt the user to input the number of seats to book
                int seatsToBook = GetIntUserInput();

                bool foundSeats = false;

                if (seatsToBook == 0) // If the user entered 0, set the booking to false to exit the loop
                {
                    booking = false;
                }

                // loop through all rows in the room in reverse order
                for (int i = selectedRoom.roomSeats.Count - 1; i >= 0; i--)
                {
                    int rowNumber = i + 1; // Get the row number
                    int seatsAvailableInRow = GetAvailableSeatsInRow(selectedRoom, i); // Get the number of available seats in the row

                    
                    if (seatsAvailableInRow >= seatsToBook && seatsToBook != 0) // check if the current row has enough available seats
                    {
                        // find the first group of available seats in the row
                        for (int j = 0; j < selectedRoom.seatsPerRow - seatsToBook + 1; j++) //j represents the index of the first seat in a potential group of seats.
                        {
                            //checks if the seats in the potential group are available or not. This is done by setting a boolean flag seatsAvailable to true at the beginning of the loop.
                            //For each seat in the group, the code checks if it is booked or not. If any seat in the group is already booked, the seatsAvailable flag is set to false
                            bool seatsAvailable = true;
                            for (int k = 0; k < seatsToBook; k++)
                            {
                                if (selectedRoom.roomSeats[i][j + k].isBooked)
                                {
                                    seatsAvailable = false;
                                    break;
                                }
                            }
                            if (seatsAvailable)
                            {
                                // book the seats if they are available
                                for (int k = 0; k < seatsToBook; k++)
                                {
                                    selectedRoom.roomSeats[i][j + k].isBooked = true;
                                }
                                foundSeats = true;
                                Console.WriteLine($"Successfully booked {seatsToBook} seats in row {rowNumber}.");
                                break;
                            }
                        }
                    }
                    if (foundSeats) // Exit the loop if seats have been found
                    {
                        break;
                    }
                }

                if (!foundSeats && seatsToBook > 0) // If the user attempts to book more seats than the length of one row, ask the user to split the request.
                {
                    Console.WriteLine($"Currently you can book up to {selectedRoom.seatsPerRow} seats per booking request. Please split your request to book multiple seats.\nPress any key to continue.");
                    Console.ReadKey();
                }
            }
        }

        public void BookCustomSeat(Room selectedRoom)
        {
            bool booking = true;

            while (booking)
            {
                // Clear the console and display the selected room
                Console.Clear();
                DisplayRoom(selectedRoom);
                
                // Get the maximum row and seat numbers for the selected room
                int maxRow = selectedRoom.roomSeats.Count;
                int maxSeat = selectedRoom.roomSeats[0].Count;

                // Prompt the user to input the row number to book seats on
                Console.WriteLine($"Input row number to book seats on (1-{maxRow}):\nPress 0 to exit the booking process. ");
                int rowToBook = GetIntUserInput();

                if (rowToBook == 0) // Exit if selection is 0
                {
                    break;
                }

                // Prompt the user to input the seat number to book on the selected row
                Console.WriteLine($"Input seat number to book on row {rowToBook} (1-{maxSeat}):\nPress 0 to exit the booking process. ");
                int seatToBook = GetIntUserInput();

                bool foundSeats = false;

                if (seatToBook == 0) // Exit if selection is 0
                {
                    break;
                }
                else if (rowToBook < 1 || rowToBook > maxRow || seatToBook < 1 || seatToBook > maxSeat)             // Notify the user of an invalid input

                {
                    Console.WriteLine($"Invalid input. Row number must be between 1 and {maxRow}, and seat number must be between 1 and {maxSeat}. Press any key to continue.");
                    Console.ReadKey();
                }
                else if (!selectedRoom.roomSeats[rowToBook - 1][seatToBook - 1].isBooked)
                {
                    // Ask for confirmation before booking the seat
                    Console.WriteLine($"Do you want to book seat {seatToBook} in row {rowToBook}? Press Y to confirm or any other key to cancel the booking.");
                    string bookingConfirmation = GetStringUserInput().ToUpper();
                    if (bookingConfirmation == "Y")
                    {
                        // Book the seat
                        selectedRoom.roomSeats[rowToBook - 1][seatToBook - 1].isBooked = true;
                        Console.WriteLine($"Successfully booked seat {seatToBook} in row {rowToBook}.");
                        Thread.Sleep( 1000 );
                        foundSeats = true;
                    }

                    else
                    {
                        continue; // Restart the loop if the user cancels the booking
                    }
                }
                else
                {
                    Console.WriteLine($"Seat {seatToBook} in row {rowToBook} is already booked.");
                }

                if (!foundSeats && rowToBook > 0 && seatToBook > 0) // Notify the user of an unsuccessful booking attempt
                {
                    Console.WriteLine($"Booking attempt unsuccessful. Please select another seat.\nPress any key to continue.");
                    Console.ReadKey();
                }
            }
        }


        public void UnbookCustomSeats(Room selectedRoom)
        {
            bool unBooking = true;

            while (unBooking) // loop until the user is finished unbooking seats
            {
                Console.Clear();
                DisplayRoom(selectedRoom);

                int maxRow = selectedRoom.roomSeats.Count;
                int maxSeat = selectedRoom.roomSeats[0].Count;

                // ask the user to input the row number of the seats they want to unbook
                Console.WriteLine($"Input row number to unbook seats on (1-{maxRow}):\nPress 0 to exit the unbooking process. "); 
                int rowToUnbook = GetIntUserInput();

                if (rowToUnbook == 0) // if the user inputs 0, exit the process
                {
                    unBooking = false;
                    break;
                }

                // ask the user to input the seat number of the seats they want to unbook
                Console.WriteLine($"Input seat number to unbook on row {rowToUnbook} (1-{maxSeat}):\nPress 0 to exit the unbooking process. ");
                int seatToUnbook = GetIntUserInput();

                bool foundSeats = false;

                if (seatToUnbook == 0) // if the user inputs 0, exit the process
                {
                    unBooking = false;
                    break;
                }

                // if the user inputs an invalid row or seat number, display an error message
                else if (rowToUnbook < 1 || rowToUnbook > maxRow || seatToUnbook < 1 || seatToUnbook > maxSeat)
                {
                    Console.WriteLine($"Invalid input. Row number must be between 1 and {maxRow}, and seat number must be between 1 and {maxSeat}. \nPress any key to continue.");
                    Console.ReadKey();
                }
                else
                {
                    int maxSeatsInSelectedRow = selectedRoom.roomSeats[rowToUnbook - 1].Count;

                    // if the user inputs a seat number that doesn't exist in the selected row, display an error message
                    if (seatToUnbook > maxSeatsInSelectedRow)
                    {
                        Console.WriteLine($"Invalid input. Row {rowToUnbook} only has {maxSeatsInSelectedRow} seats. Please select a seat between 1 and {maxSeatsInSelectedRow}.\nPress any key to continue.");
                        Console.ReadKey();
                    }

                    // if the selected seat is already booked, confirm with the user that they want to unbook it and unbook it if they confirm
                    else if (selectedRoom.roomSeats[rowToUnbook - 1][seatToUnbook - 1].isBooked)
                    {
                        Console.WriteLine($"Are you sure you want to unbook seat {seatToUnbook} in row {rowToUnbook}? Press Y to confirm or any other key to cancel the process."); // ask for confirmation
                        string confirmation = GetStringUserInput().ToUpper();

                        if (confirmation == "Y")
                        {
                            // unbook the seat
                            selectedRoom.roomSeats[rowToUnbook - 1][seatToUnbook - 1].isBooked = false;
                            Console.WriteLine($"Successfully unbooked seat {seatToUnbook} in row {rowToUnbook}.");
                            foundSeats = true;
                        }
                        else // if the user does not confirm, cancel the process
                        {
                            Console.WriteLine("Process cancelled.");
                            Thread.Sleep( 1000 );
                        }
                    }
                    if (!foundSeats) // if the selected seat can not be unbooked, display an error message
                    {
                        Console.WriteLine($"The requested seat can not be unbooked. Please select another seat.\nPress any key to continue.");
                        Console.ReadKey();
                    }
                }
            }
        }

        private int GetAvailableSeatsInRow(Room room, int rowIndex)
        {
            int availableSeats = 0;

            for (int j = 0; j < room.seatsPerRow; j++) // iterate through each seat in the row
            {
                if (!room.roomSeats[rowIndex][j].isBooked) // check if the seat is not booked
                {
                    availableSeats++; // increment availableSeats if the seat is not booked
                }
            }

            // return the number of available seats in the row
            return availableSeats;
        }

        private void DisplayRoom(Room selectedRoom)
        {
            Console.WriteLine($"Room {selectedRoom.roomName}"); // display the room number

            for (int i = 0; i < selectedRoom.roomSeats.Count; i++) // iterate through each row in the room
            {
                int rowNumber = i + 1;
                Console.Write($"Row {rowNumber.ToString("D2")}: "); // display the row number in two characters. this assumes that the room will never have over 99 rows
                for (int j = 0; j < selectedRoom.roomSeats[i].Count; j++) // iterate through each seat in the row
                {
                    if (selectedRoom.roomSeats[i][j].isBooked) // if the seat is booked, display it in red
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"[{j + 1}]");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else // if the seat is not booked, display it in green
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"[{j + 1}]");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                Console.WriteLine(); // move to the next line
            }

            // add a blank line at the end
            Console.WriteLine();
        }

        public int GetIntUserInput() //get user input and check if the input is null
        {
            bool validatingInput = true;
            int result = 0;

            while (validatingInput)
            {
                string userInput = GetStringUserInput();

                if (int.TryParse(userInput, out result))
                {
                    validatingInput = false;
                }
                else
                {
                    Console.WriteLine($"Invalid input, please try again.");
                }
            }

            return result;
        }

        public void CinemaTitle()
        {
            //reference: ASCII art from - https://ascii.co.uk/art/cinema
            Console.WriteLine("      _                            \r\n     (_)                           \r\n  ___ _ _ __   ___ _ __ ___   __ _ \r\n / __| | '_ \\ / _ \\ '_ ` _ \\ / _` |\r\n| (__| | | | |  __/ | | | | | (_| |\r\n \\___|_|_| |_|\\___|_| |_| |_|\\__,_|\n\n");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp
{
    [Serializable]
    internal class Room : IRoom
    {

        public List<List<Seat>> roomSeats = new List<List<Seat>>(); // this list stores the seats and rows in the room, with each row being represented by a list of Seat objects.

        public string roomName { get; set; }
        public int seatsPerRow { get; set; }
        public int rows { get; set; }



        public Room(string roomName, int seatsPerRow, int rows)
        {
            this.roomName = roomName;
            this.seatsPerRow = seatsPerRow;
            this.rows = rows;
            GenerateLayout(); //generate the room layout based on the number of seats and rows
        }

        public void GenerateLayout() //generate the room layout based on the number of seats and rows
        {
            for (int i = 0; i < rows; i++) //generate a rows of seats up to the value specified 
            {
                List<Seat> rowOfSeats = new List<Seat>(); //generate a list of seats for a single row in the room

                for (int x = 0; x < seatsPerRow; x++) //generate a new seat in the row up to the number of seats per row specified 
                {
                    Seat newSeat = new Seat();
                    rowOfSeats.Add(newSeat);
                }

                roomSeats.Add(rowOfSeats); // add the row to the roomSeats list
            }
        }

    }
}

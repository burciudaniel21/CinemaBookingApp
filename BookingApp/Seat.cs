using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp
{

    [Serializable]
    internal class Seat : ISeat
    {
        public bool isBooked { get; set; }
        public Seat() //represent a single seat in the room
        {
            isBooked = false; //indicate if the seat is booked or not
        }

        public bool Available() //returns false to the isBooked variable if the seat is available
        {
            return isBooked = false;
        }

        public bool Unavailable() //returns true to the isBooked variable if the seat is booked
        {
            return isBooked = true;
        }
    }
}

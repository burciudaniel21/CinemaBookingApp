using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp
{
    [Serializable]
    internal class _3DRoom : Room //In this particular project, the _3DRoom is not offering any benefit as the ScreenType variable could be part of the room class (I would have created it as an Enum) and edited as required.
                                  //It has been created to demonstrate polymorphism by storing different types of objects in the list in ConsoleUI.cs class.
    {
        public string ScreenType { get; set; }
        public _3DRoom(string roomName, int seatsPerRow, int rows) : base(roomName, seatsPerRow, rows)
        {
            this.ScreenType = "3D";

        }

    }
}

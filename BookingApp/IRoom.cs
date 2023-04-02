using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp
{
    internal interface IRoom
    {
        public string roomName { get; set; }
        public int seatsPerRow { get; set; }
        public int rows { get; set; }

        public void GenerateLayout();
    }


}
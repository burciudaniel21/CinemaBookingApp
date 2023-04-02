using System;
using System.Threading;

namespace BookingApp
{

    public class TimeTracker
    {
        private int _timeInSeconds; //_ is used to highlight that the field is private

        public TimeTracker()
        {
            _timeInSeconds = 0;
        }

        public void StartTracking()
        {
            Thread trackingThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000); // Sleep for 1 second
                    _timeInSeconds++; //Then add 1 to the timer
                }
            });

            trackingThread.IsBackground = true; // Set the thread to run in the background
            trackingThread.Start(); //start the thread.
        }

        public int GetTimeInSeconds()
        {
            return _timeInSeconds;
        }
    }
}
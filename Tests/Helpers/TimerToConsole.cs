#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: TimerToConsole.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.Diagnostics;

namespace Tests.Helpers
{
    class TimerToConsole : IDisposable
    {
        private readonly string _message;

        private readonly Stopwatch _timer;

        public TimerToConsole(string message)
        {
            _message = message;
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Dispose()
        {
            Console.WriteLine("{0} took {1:f2} ms", _message, 1000.0 * _timer.ElapsedTicks / Stopwatch.Frequency);
        }
    }
}

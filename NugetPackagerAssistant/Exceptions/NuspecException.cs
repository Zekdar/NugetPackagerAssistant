using System;

namespace NugetPackagerAssistant.Exceptions
{
    public class NuspecException : Exception
    {
        public NuspecException(string message) : base(message) { }
        public NuspecException(string message, Exception ex) : base(message, ex) { }
    }
}

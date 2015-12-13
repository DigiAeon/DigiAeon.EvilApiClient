using System;

namespace DigiAeon.EvilApiClient.Services
{
    public class ServiceDependencyException : Exception
    {
        public ServiceDependencyException(string message) : base(message)
        { }
    }
}
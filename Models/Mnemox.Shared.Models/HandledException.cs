using System;

namespace Mnemox.Shared.Models
{
    public class HandledException: Exception
    {
        public HandledException(Exception exception) : base(exception.Message, exception.InnerException)
        {

        }
    }
}

using System;
using System.Runtime.CompilerServices;

namespace Mnemox.Logs.Models
{
    public class ErrorLogStructure
    {
        private readonly Exception _exception;

        private string _errorSource;

        public ErrorLogStructure(Exception exception)
        {
            _exception = exception;
        }

        public void WithErrorSource([CallerMemberName] string callerMemberName = null)
        {

        }
    }
}

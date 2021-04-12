using System;

namespace Mnemox.Logs.Models
{
    public class ErrorLogStructure
    {
        private readonly Exception _exception;

        public string ErrorSource { get; set; }

        public ErrorLogStructure(Exception exception)
        {
            _exception = exception;
        }

        public void WithErrorSource()
        {

        }
    }
}

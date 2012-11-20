using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NScript.Template.Compiler
{
    public interface ILog
    {
        void LogError(string message);
        void LogError(ErrorInfo errorInfo);
        void LogWarning(string message);
        void LogWarning(ErrorInfo errorInfo);
    }
}

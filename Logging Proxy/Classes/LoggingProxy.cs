using System;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Logging_Proxy
{
    public class LoggingProxy<T> : DynamicProxy<T> where T : class
    {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var types = args.Select(x => x.GetType()).ToArray();
            var methodInfo = typeof(T).GetMethod(binder.Name, types);
            if (methodInfo == null)
            {
                result = null;
                return false;
            }

            var stringBuilder = new StringBuilder();
            var pars = methodInfo.GetParameters();

            for (int i = 0; i < args.Length; i++)
            {
                stringBuilder.Append($"{types[i]} {pars[i].Name} = \"{args[i]}\"");
                stringBuilder.Append(",");
            }

            if (stringBuilder.Length > 0)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

            Console.WriteLine($"Invoking method {methodInfo.Name} ( {stringBuilder.ToString()} )");
            result = methodInfo.Invoke(base.obj, args);
            return true;
        }
    }
}

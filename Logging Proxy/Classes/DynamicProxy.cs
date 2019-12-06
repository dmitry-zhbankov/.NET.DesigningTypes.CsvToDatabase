using ImpromptuInterface;
using System.Dynamic;

namespace Logging_Proxy
{
    public class DynamicProxy<T> : DynamicObject where T : class
    {
        protected T obj;

        public T CreateInstance(T obj)
        {
            this.obj = obj;
            return this.ActLike<T>();
        }
    }
}

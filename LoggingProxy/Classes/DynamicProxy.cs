﻿using ImpromptuInterface;
using System.Dynamic;

namespace LoggingProxy
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

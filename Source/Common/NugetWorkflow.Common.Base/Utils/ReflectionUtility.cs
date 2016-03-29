using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.Base.Utils
{
    public static class ReflectionUtility
    {
        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        public static string GetTypeName<Type>()
        {
            return typeof(Type).Name;
        }

        public static string GetTypeName(object obj)
        {
            string ret = string.Empty;
            if (obj == null)
            {
                ret = "NULL";
            }
            else
            {
                ret = obj.GetType().Name;
            }
            return ret;
        }

        public static string GetMethodName<T>(Expression<Action<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentException("Action provided to get method name was NULL");
            }
            MethodCallExpression methodCall = action.Body as MethodCallExpression;
            return methodCall.Method.Name;
        }

        public static string GetPropFieldName<T, P>(System.Linq.Expressions.Expression<Func<T, P>> action)
        {
            var expression = (System.Linq.Expressions.MemberExpression)action.Body;
            string name = expression.Member.Name;

            return name;
        }
    }
}

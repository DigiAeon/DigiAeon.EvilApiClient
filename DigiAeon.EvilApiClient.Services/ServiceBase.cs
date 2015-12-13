using System;
using System.Linq.Expressions;
using DigiAeon.Core;

namespace DigiAeon.EvilApiClient.Services
{
    public abstract class ServiceBase
    {
        protected void ThrowServiceDependencyException<T>(Type classType, Expression<Func<T>> propertyLambda, string message)
        {
            throw new ServiceDependencyException(string.Format("Class: {0}\nProperty: {1} {2}", classType, ReflectionHelper.GetPropertyName(propertyLambda), message));
        }
    }
}
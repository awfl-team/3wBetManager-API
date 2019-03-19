using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Proxy
{
    public class Proxy<T> : RealProxy
    {
        private readonly T _decorator;

        public Proxy(T decorator) : base(typeof(T))
        {
            _decorator = decorator;
        }

        public override IMessage Invoke(IMessage msg)
        {
            if (!(msg is IMethodCallMessage methodCall)) return null;

            var time = Stopwatch.StartNew();
            var methodInfo = methodCall.MethodBase as MethodInfo;
            var collection = methodInfo.GetGenericArguments().Length > 0
                ? methodInfo.GetGenericArguments()[0].GetType().Name
                : methodInfo.GetType().Name;

            Log($"Execute {methodCall.MethodName}");
            try
            {
                using (new ElasticsSearchDatabaseContext(methodCall.MethodName, collection))
                {
                    var result = methodInfo.Invoke(_decorator, methodCall.InArgs);
                    Log($"{methodCall.MethodName} executed in {time.ElapsedMilliseconds}ms");

                    return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
                }
            }
            catch (Exception e)
            {
                Log($"Exception in method {methodCall.MethodName} : {e.Message}");
                return new ReturnMessage(e, methodCall);
            }
        }

        private void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now:s} - {message}");
        }
    }
}
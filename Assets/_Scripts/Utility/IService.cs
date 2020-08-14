using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.Services
{
    interface IService
    {
        void Init();
        void Takedown();
    }

    abstract class ServiceBase<T> where T : IService
    {
        public static T Get() { return mImpl; }
        public static void Register(T impl) { mImpl = impl; }
        public static void UnRegister() { mImpl = default; }

        private static T mImpl;

        protected static string mServiceName = "";
    }
}

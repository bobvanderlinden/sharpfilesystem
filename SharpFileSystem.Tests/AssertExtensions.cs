using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SharpFileSystem.Tests
{
    public static class EAssert
    {
        public static void Throws<T>(Action a)
            where T: Exception
        {
            try
            {
                a();
            }
            catch (T)
            {
                return;
            }
            Assert.Fail(string.Format("The exception '{0}' was not thrown.", typeof(T).FullName));
        }
    }
}

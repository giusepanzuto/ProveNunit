using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace ProveNunit
{
    [TestFixture]
    public class GenericsTests
    {
        [TestCaseGeneric]
        public void SomeGenericTest<T1, T2>()
        {
            typeof(T1).Should().NotBe(typeof(T2));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseGenericAttribute : TestCaseAttribute, ITestBuilder
    {
        readonly List<(Type T1, Type T2)> _typeList = new List<(Type T1, Type T2)>
        {
            (typeof(int),typeof(string)),
            (typeof(int),typeof(long)),
            (typeof(int),typeof(int)),
            (typeof(byte),typeof(string)),
        };

        public TestCaseGenericAttribute(params object[] arguments)
            : base(arguments)
        {
        }

        IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
        {
            if (!method.IsGenericMethodDefinition)
                return base.BuildFrom(method, suite);

            var methods = _typeList.Select(t => method.MakeGenericMethod(t.T1, t.T2));

            return methods.SelectMany(m => base.BuildFrom(m, suite));
        }
    }
}

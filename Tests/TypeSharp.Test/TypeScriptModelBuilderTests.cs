using Ajax;
using NStandard;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace TypeSharp.Test
{
    public class TypeScriptModelBuilderTests
    {
        [Fact]
        public void SimpleTest()
        {
            var types = Assembly.GetExecutingAssembly().GetTypesWhichMarkedAs<TypeScriptModelAttribute>();
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<RootClass>();
            builder.CacheType<JSend>();

            var tscode = builder.Compile();
            var expectedCode = $"{TestUtil.DeclareContent}\r\n\r\n{File.ReadAllText($"{nameof(SimpleTest)}.ts")}";
            Assert.Equal(expectedCode, tscode);
        }

        [Fact]
        public void GenericTest1()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType(typeof(GenericClass<int>));
            var tscode = builder.Compile();
            var expectedCode = $"{TestUtil.DeclareContent}\r\n\r\n{File.ReadAllText($"{nameof(GenericTest1)}.ts")}";
            Assert.Equal(expectedCode, tscode);
        }

        [Fact]
        public void GenericTest2()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType(Type.GetType("TypeSharp.Test.GenericClass`1"));
            var tscode = builder.Compile(new CompileOptions
            {
                OutputNames = true,
            });
            var expectedCode = $"{TestUtil.DeclareContent}\r\n\r\n{File.ReadAllText($"{nameof(GenericTest2)}.ts")}";
            Assert.Equal(expectedCode, tscode);
        }

        [Fact]
        public void LiveAccountTest()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType(typeof(LiveAccount.LiveUser));
            var tscode = builder.Compile();
            var expectedCode = $"{TestUtil.DeclareContent}\r\n\r\n{File.ReadAllText($"{nameof(LiveAccountTest)}.ts")}";
            Assert.Equal(expectedCode, tscode);
        }

        [Fact]
        public void JSendTest()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType(typeof(JSend));
            var tscode = builder.Compile();
            var expectedCode = $"{TestUtil.DeclareContent}\r\n\r\n{File.ReadAllText($"{nameof(JSendTest)}.ts")}";
            Assert.Equal(expectedCode, tscode);
        }

    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CSharp;
using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace RuntimeCompilerApi.Services
{
    public static class CompilerService
    {
        public static object Compile(string fileToCompile, string resultFileName)
        {
            var source = File.ReadAllText(fileToCompile);
            var parsedSyntaxTree = Parse(source, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp5));


            var compilation
                = CSharpCompilation.Create(resultFileName, new SyntaxTree[] { parsedSyntaxTree }, DefaultReferences, DefaultCompilationOptions);
            string projectPath = @"C:\Users\Z6PLT\source\repos\MyProjects\Test_Projects\RuntimeCompilerApi\";
            string fullPath = projectPath + resultFileName;
            try
            {
                var result = compilation.Emit(fullPath);

                if (!result.Success)
                {
                    throw new CompilerException("I think that compiling has failed.");
                }
            }
            catch (CompilerException cp)
            {
                return cp.Message;
            }
            catch (Exception ex)
            {
                throw new CompilerException("I think that compiling has failed.", ex);
            }
            return InvokeDll(fullPath);
        }
        public static object Compile(string code)
        {
            var parsedSyntaxTree = Parse(code, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp5));
            var resultFileName = "Test" + DateTime.Now.Millisecond + ".dll";

            var compilation
                = CSharpCompilation.Create(resultFileName, new SyntaxTree[] { parsedSyntaxTree }, DefaultReferences, DefaultCompilationOptions);
            string projectPath = @"C:\Users\Z6PLT\source\repos\MyProjects\Test_Projects\RuntimeCompilerApi\";
            string fullPath = projectPath + resultFileName;
            try
            {
                var result = compilation.Emit(fullPath);

                if (!result.Success)
                {
                    throw new CompilerException("I think that compiling has failed.");
                }

            }
            catch (CompilerException cp)
            {
                return cp.Message;
            }
            catch (Exception ex)
            {
                throw new CompilerException("I think that compiling has failed.", ex);
            }
            return InvokeDll(fullPath);
        }
        private static readonly IEnumerable<string> DefaultNamespaces =
            new[]
            {
                "System",
                "System.IO",
                "System.Net",
                "System.Linq",
                "System.Text",
                "System.Text.RegularExpressions",
                "System.Collections.Generic"
            };

        private static string runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\{0}.dll";

        private static readonly IEnumerable<MetadataReference> DefaultReferences =
            new[]
            {
                MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core"))
            };

        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(DefaultNamespaces);

        public static SyntaxTree Parse(string text, string filename = "", CSharpParseOptions options = null)
        {
            var stringText = SourceText.From(text, Encoding.UTF8);
            return SyntaxFactory.ParseSyntaxTree(stringText, options, filename);
        }
        private static object InvokeDll(string pathToTheDll)
        {

            Assembly assembly = Assembly.LoadFile(pathToTheDll);
            Type type = assembly.GetType("Program");

            object result = null;
            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod("Main");

                if (methodInfo != null)
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    object classInstance = Activator.CreateInstance(type, null);

                    if (parameters.Length == 0)
                    {
                        // This works fine
                        result = methodInfo.Invoke(classInstance, null);
                    }
                    else
                    {
                        object[] parametersArray = new object[] { };

                        // The invoke does NOT work;
                        // it throws "Object does not match target type"             
                        result = methodInfo.Invoke(classInstance, parametersArray);
                    }
                }
            }
            return result;
        }

    }
}

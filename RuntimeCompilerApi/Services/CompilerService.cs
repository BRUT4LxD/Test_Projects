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
        private static readonly string path = "testcode.cs";
        public static void CompileCSharp(string code, string path)
        {
            try
            {
                var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, path, true)
                {
                    GenerateExecutable = true
                };
                CompilerResults results = csc.CompileAssemblyFromSource(parameters,code);
                if (results.Errors.HasErrors)
                {
                    results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
                }
            }
            catch (Exception compilerException)
            {
                throw new CompilerException("Compiler failed to compiler the file",
                                            compilerException);
            }

        }

        public static void CompileRoslyn(string code, string fileName)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location)
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = File.Create(fileName))
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType("RoslynCompileSample.Writer");
                    var instance = assembly.CreateInstance("RoslynCompileSample.Writer");
                    var meth = type.GetMember("Write").First() as MethodInfo;
                    meth.Invoke(instance, new[] { "joel" });
                }
            }
        }

        public static void CompileRoslyn2(string code, string fileName)
        {
            // intentionally introduce errors to see the compiler reporting code issues
            code = @"using System; 
                using System.IO; 
                namespace MathFunctions 
                { 
                 public static class MathHelper
                 { 
                    public static void CalculateCircleArea() 
                    { 
                        double radius = 10;
                        double result = radius * radius * System.Math.PI;
                        Console.WriteLine(result.ToString()); 
                    } 
                  } 
                }";

            code = File.ReadAllText("testcode2.txt");

            // Get a SyntaxTree
            var tree = SyntaxFactory.ParseSyntaxTree(code);

            Console.WriteLine(tree);
            PrintDiagnostics(tree);

            // Create a compilation for the syntax tree
            var compilation = CSharpCompilation.Create("mylib.dll").
                WithOptions(
                    new CSharpCompilationOptions(OutputKind.ConsoleApplication)).
                AddReferences(MetadataReference.CreateFromFile(typeof(object).
                GetTypeInfo().Assembly.Location)).
                AddSyntaxTrees(tree);

            fileName = "mylib.dll";
            var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            // Emit an Assembly that contains the result of the Roslyn code generation
            compilation.Emit(path);

           var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            asm.GetType("MathFunctions.MathHelper").GetMethod("CalculateCircleArea").Invoke(null, new object[] { });
            Console.ReadLine();
        }
        private static void PrintDiagnostics(SyntaxTree tree)
        {
            // detects diagnostics in the source code
            var diagnostics = tree.GetDiagnostics();

            if (diagnostics.Any())
            {
                foreach (var diag in diagnostics)
                {
                    // if any, prints diagnostic message and line/row position
                    Console.WriteLine($"{diag.GetMessage()} {diag.Location.GetLineSpan()}");
                }
            }
        }

        public static object Roslyn3(string path)
        {
            var fileToCompile = "testcode.txt";
            var source = File.ReadAllText(fileToCompile);
            var parsedSyntaxTree = Parse(source, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp5));

            var compilation
                = CSharpCompilation.Create("Test.dll", new SyntaxTree[] { parsedSyntaxTree }, DefaultReferences, DefaultCompilationOptions);
            try
            {
                var result = compilation.Emit(@"C:\Users\Z6PLT\source\repos\MyProjects\Test_Projects\RuntimeCompilerApi\Test.dll");

                Console.WriteLine(result.Success ? "Sucess!!" : "Failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return InvokeDll(@"C:\Users\Z6PLT\source\repos\MyProjects\Test_Projects\RuntimeCompilerApi\Test.dll");
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

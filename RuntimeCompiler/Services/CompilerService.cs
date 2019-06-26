using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RuntimeCompilerApi.Services
{
    public class CompilerService
    {
        public static void CompileCSharp(string code, string path)
        {
            try
            {
                var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, path, true)
                {
                    GenerateExecutable = true
                };
                CompilerResults results = csc.CompileAssemblyFromSource(parameters,
                    @"using System.Linq;
                using System;
            class Program {
              public static void Main(string[] args) {
                int i = 1144;
                Console.WriteLine(i);
                Console.ReadKey();
              }
            }");
                if (results.Errors.HasErrors)
                {
                    results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
                }
            }
            catch (Exception compilerException)
            {
                throw new CompilerException("Compiler failed to compiler the file", compilerException);
            }

        }

        public static void CompileRoslyn()
        {
            Console.Write("Let's compile!");

            string codeToCompile = @"
            using System;
            namespace RoslynCompileSample
            {
                public class Writer
                {
                    public void Write(string message)
                    {
                        Console.WriteLine($""you said '{message}!'"");
                    }
                }
            }";

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);

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

            using (var ms = new MemoryStream())
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
                //else
                //{
                //    ms.Seek(0, SeekOrigin.Begin);
                //    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                //    var type = assembly.GetType("RoslynCompileSample.Writer");
                //    var instance = assembly.CreateInstance("RoslynCompileSample.Writer");
                //    var meth = type.GetMember("Write").First() as MethodInfo;
                //    meth.Invoke(instance, new[] { "joel" });
                //}
            }
        }
    }
}

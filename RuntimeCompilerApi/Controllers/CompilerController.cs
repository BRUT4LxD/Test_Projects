using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using RuntimeCompilerApi.Requests;
using RuntimeCompilerApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace RuntimeCompilerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompilerController : Controller
    {
        // POST: api/Compiler
        [HttpPost]
        public IActionResult Post([FromBody] CompileCodeRequest codeRequest)
        {
            string path = "compiledFile.exe";
            try
            {
                Sth();
            }
            catch (Exception)
            {
                Console.WriteLine("Error during file compilation");
                throw;
            }
            return Ok($"File has been successfully compiled. Check [SolutionDirectory]/bin/Debug/{path}");
        }
        [HttpGet]
        public IActionResult Get()
        {
            string path = "testcode.txt";
            string resultFilePath = "result.exe";
            string code = System.IO.File.ReadAllText(path);
            //CompilerService.CompileRoslyn(code, resultFilePath);
            //CompilerService.CompileRoslyn2(code, resultFilePath);
            object result = CompilerService.Roslyn3(path);

            return Ok(result);
        }
        private void Sth()
        {
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
    }
}

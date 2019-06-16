using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Compiling in runtime");
        var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
        var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, "foo.exe", true)
        {
            GenerateExecutable = true
        };
        CompilerResults results = csc.CompileAssemblyFromSource(parameters,
            @"using System.Linq;
                using System;
            class Program {
              public static void Main(string[] args) {
                int i = 1111;
                Console.WriteLine(i);
                Console.ReadKey();
              }
            }");
        if (results.Errors.HasErrors)
        {
            results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
        }
        else
        {
            Console.WriteLine("Everything is ok");
        }

        Console.ReadKey();
    }
}
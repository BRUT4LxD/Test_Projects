using Microsoft.CSharp;
using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

internal class Program
{
    private static readonly string path = "testcode.cs";

    private static void Main(string[] args)
    {
        Console.WriteLine("Compiling in runtime");
        var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
        var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, "foo.exe", true)
        {
            GenerateExecutable = true
        };
        string text = File.ReadAllText(path);
        Console.WriteLine(text);
        CompilerResults results = csc.CompileAssemblyFromSource(parameters,
            text);
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

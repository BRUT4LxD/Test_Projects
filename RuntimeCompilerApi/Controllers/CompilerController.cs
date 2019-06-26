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
            object result = CompilerService.Compile(codeRequest.Code);

            return Ok(result);
        }
        [HttpGet]
        public IActionResult Get(string codeFileName, string resultFileName)
        {
            object result = CompilerService.Compile(codeFileName, resultFileName);

            return Ok(result);
        }
    }
}

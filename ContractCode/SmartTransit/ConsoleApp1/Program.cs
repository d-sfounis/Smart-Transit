using System;
using System.IO;
using System.Linq;
using Neo;
using Neo.VM;
using Neo.Cryptography;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new ExecutionEngine(null, Crypto.Default);
            engine.LoadScript(File.ReadAllBytes("C:\\Users\\Dimitris\\source\\repos\\ConsoleApp1\\TestContractSfounis\\bin\\Debug\\TestContractSfounis.avm"));

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(2); // corresponds to the parameter c
                sb.EmitPush(3); // corresponds to the parameter b
                sb.EmitPush(5); // corresponds to the parameter a
                engine.LoadScript(sb.ToArray());
            }

            engine.Execute(); // start execution

            byte[] result = engine.EvaluationStack.Peek().GetByteArray(); // set the return value here
            string str =  Convert.ToBase64String(result);
            Console.WriteLine($"Execution result: {str}");
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using XNAContentCompiler;

namespace XNA_CC_CLI
{
    public class Options
    {
        [Option('o', "out", Required = true, HelpText = "Output directory")]
        public string OutputFolder { get; set; }

        [Option('i', "input", Required = true, HelpText = "Input file path")]
        public string Inputs { get; set; }

        [Option('c', "compress", Required = false, HelpText = "Compress output")]
        public bool Compress { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var inputs = Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                // TODO: param validation
            }).Value;

            var outputPath = inputs.OutputFolder.Trim();
            if (!Directory.Exists(outputPath))
            {
                Console.WriteLine("output must be a directory!");
                return;
            }

            var item = inputs.Inputs;
            ContentBuilder contentBuilder = new ContentBuilder(inputs.Compress);

            //Remove os arquivos anteriormente adicionados.
            contentBuilder.Clear();

            contentBuilder.Add(new ComboItem(Path.GetFileName(item), item));
            //Aplica o Build
            string error = contentBuilder.Build();
            //Se houve algum erro, informa-o
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(string.Format("{0}: {1}", item, error));
                return;
            }
            //Recupera os arquivos criados
            string tempPath = contentBuilder.OutputDirectory;
            string[] files = Directory.GetFiles(tempPath, "*.xnb");
            //Copia os arquivos para a saída
            foreach (string file in files)
            {
                var fullPath = Path.Combine(outputPath, Path.GetFileName(file));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                File.Move(file, fullPath);
            }
            Console.WriteLine("Files compiled !!");
        }
    }
}

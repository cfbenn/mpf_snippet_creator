using System;
using System.IO;
using System.Reflection;
using mpfConfig;

namespace mpf_snippet_creator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            string config_file;

            // Check and verify which directory that config_spec.yaml is located
            if (args.Length == 0)
            {
                config_file = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";
            }
            else
            {
                config_file = args[0];
                if (config_file.Substring(config_file.Length - 1) != @"\")
                    config_file += @"\";
            }
            string outPath = config_file + "snippets.txt";
            config_file += "config_spec.yaml";

            try
            {
                config_spec cs = new config_spec(config_file);
                StreamWriter sw = new StreamWriter(outPath);
                sw.Write(cs.ToString());
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Completed.");
            }

        }
    }
}

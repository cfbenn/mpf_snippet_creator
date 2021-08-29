using System;
using System.Collections.Generic;
using System.IO;

namespace mpfConfig
{
    public class config_spec
    {
        private SortedDictionary<string, Snippet> _snippets = new SortedDictionary<string, Snippet>();
        private string currentSection = string.Empty;
        private Snippet _currentSnippet;

        public config_spec(string config_spec_file)
        {
            string line = string.Empty;
            StreamReader sr = new StreamReader(config_spec_file);
            while (line != null)
            {
                ProcessLine(line);
                line = sr.ReadLine();
            }
        }

        
        /// <summary>
        /// Process a single line from the config_spec file.
        /// </summary>
        /// <param name="line">The raw line to parse</param>
        private void ProcessLine(string line)
        {
            // todo - Error trapping
            if (line != string.Empty)
            {
                if (line.Substring(0, 1) != " ")
                {
                    //todo - Check if snippit has anything added, then place back in dictionary
                    if (_currentSnippet != null)
                    {
                        _snippets[currentSection] = _currentSnippet;
                    }

                    // Remove anything to the right of the colon
                    if (line.Contains(":"))
                        currentSection = line.Remove(line.IndexOf(":") - 1);
                    
                    if (_snippets.TryGetValue(currentSection, out _currentSnippet))
                    {
                        // todo ????  Reload an existing snippet...but this should never happen?
                    }
                    else
                    {
                        _currentSnippet = new Snippet(currentSection, currentSection);
                    }
                }
                else // we are updating existing snippet
                {
                    int index = line.IndexOf(':');
                    string config_key = line.Substring(0, index);
                    string config_value = line.Substring(index + 1);
                    _currentSnippet.AddBody(config_key, config_value);
                }
            }
        }

        public override string ToString() 
        {
            string fulllConfig = string.Empty;
            if (_snippets.Count > 0)
            {
                foreach (Snippet s in _snippets.Values)
                {
//                    fulllConfig += s.ToString() + Environment.NewLine;
                    if (fulllConfig != string.Empty)
                    {
                        fulllConfig += "," + Environment.NewLine;
                    }
                    fulllConfig += s.ToString();
                }
                fulllConfig = "{" + Environment.NewLine + fulllConfig + Environment.NewLine + "}";
            }
            return fulllConfig;
        }
    }
}

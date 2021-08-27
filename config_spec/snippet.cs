using System;
using System.Linq;

namespace mpfConfig
{
    public class Snippet
    {
        const string quote = "\"";
        const string tab = "\t";

        private string _name;
        private string _body;

        enum MpfValueType : int
        {
            unknown = 0,
            boolean = 1,
            enumeration = 2,
            boolean_or_token = 3
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Snippet name</param>
        /// <param name="description">Snippet description</param>
        public Snippet(string name, string description)
        {
            Name = name;
            Description = description;
        }

        #region properties

        private bool _isDirty = false;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

       public string Name
            {
                get { return _name; }
                set 
                { 
                    _name = value;
                    _isDirty = true;
                }
            }

        private string _description;
        public string Description
            {
                get { return _description; }
                set 
                { 
                    _description = value;
                    _isDirty = true;
                }
            }

        #endregion

        #region Helper Functions

        private int bodyCount = 0;
        /// <summary>
        /// Add an entry to the body of the snippet.
        /// </summary>
        /// <param name="key">Left side value</param>
        /// <param name="value">Right side value(s)</param>
        public void AddBody(string key, string value)
        {
            bool validKey = true;
            try
            {
                int indentlevel = key.TakeWhile(Char.IsWhiteSpace).Count();
                key = key[indentlevel..];

                // Ignore invalid lines
                if (key.Length > 1)
                {
                    validKey = (key.Substring(0, 2) != "__");  //underscore definitions are to be ignored
                    validKey = validKey && (key.Substring(0,1) != "#"); //Remove if line is a pure comment
                }

                // Strip comments off end of lines
                int commentPosition = value.IndexOf("#");
                if (commentPosition > 1)
                    value = value.Substring(0, commentPosition - 1); // Strip off comment line

                
                if (validKey)
                {
                    indentlevel /= 4;
                    string tabs = new String('\t', indentlevel + 1);

                    bodyCount++;
                    _body += Environment.NewLine + tabs;
                    if (bodyCount == 1)
                    {
                        _body += string.Format("{0}{1}:{0}", quote, _name);
                        _body += Environment.NewLine + tabs;
                    }
                    _body += string.Format(",{0}\\t{1}: ${{{2}}}{0}", quote, key, GetValue(bodyCount, value));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                System.Diagnostics.Debugger.Break();
                throw;
            }
        }

        /// <summary>
        /// Creates the snippet portion before the 'body' tag.
        /// 
        ///     Example:
        ///         "widget_player": {
        ///             "prefix": ["wp"],
        /// 
        /// </summary>
        /// <returns>string</returns>
        private string CreateHeader()
        {
            string head = string.Format("{0}{1}-mpf{0}: {{", quote, _name) + Environment.NewLine;
            head += string.Format("{0}{1}prefix{1}: [{1}{2}{1}],", tab, quote, _name);
            return head;
        }

        /// <summary>
        /// Creates the snippet portion afer the 'body' tag.
        /// 
        ///     Example:
        ///     
        /// 		    "description": "Widget Player"
        ///         },
        ///
        /// </summary>
        /// <returns>string</returns>
        private string CreateTail()
        {
            string tail = "";
            tail = Environment.NewLine + string.Format("{0}{1}description{1}: {1}{2}{1}", tab, quote, _description);
            tail += Environment.NewLine + "}";
            return tail;
        }

        /// <summary>
        /// Creates teh body portion of the snippet.
        /// 
        ///     Example:
        ///     
        /// 		"body": [
		///	            "dual_wound_coils:"
		///	            ,"\tmain_coil: ${1: single|machine(coils)|}"
		///	            ,"\thold_coil: ${2: single|machine(coils)|}"
		///	            ,"\teos_switch: ${3: single|machine(switches)|None}"
		///	            ],
        /// 
        /// </summary>
        /// <returns>string</returns>
        private string CreateBody()
        {
            string body;
            body = Environment.NewLine + string.Format("{0}{1}body{1}: [", tab, quote);
            body += _body; 
            body += Environment.NewLine + tab + "\t],";
            return body;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Convert the complete snippet into a single string.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            string finalSnippet;
            finalSnippet = CreateHeader();
            finalSnippet += CreateBody();
            finalSnippet += CreateTail();

            return finalSnippet;
        }

        #endregion


        private string GetValue(int bodyCount, string value)
        {
            int valueType = (int)MpfValueType.unknown;
            string v = string.Format("{0}:?", bodyCount);

            try
            {
                string[] values = value.Split('|');
                if (value.IndexOf('|') != -1) // Determine if it is a special type
                {
                    if (values[1] == "bool")
                        valueType = (int)MpfValueType.boolean;
                    else if (values[1] == "bool_or_token")
                        valueType = (int)MpfValueType.boolean_or_token;
                    else if (values[1].Length > 4)
                        if (values[1].Substring(0, 4) == "enum")
                            valueType = (int)MpfValueType.enumeration;
                }

                if (valueType == (int)MpfValueType.boolean)
                {
                    v = string.Format("{0}|true,false|", bodyCount); // True and false choice
                }
                else if (valueType == (int)MpfValueType.boolean_or_token)
                {
                    v = string.Format("{0}|true,false,[token]|", bodyCount); // True and false choice
                }
                else if (valueType == (int)MpfValueType.enumeration)
                {
                    v = string.Format("{0}|{1}|", bodyCount, parseEnum(values[1]));
                }
                else
                {
                    v = string.Format("{0}:{1}", bodyCount, value); // No processing
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);  // Bad form 
                System.Diagnostics.Debugger.Break();
                throw;
            }
            return v;


        }

        private string parseEnum(string enumValue)
        {
            string v;
            v = enumValue.Substring(5, enumValue.Length - 6);
            return v;
        }

    }
}

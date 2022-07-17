using System.Collections.Generic;
using Microsoft.Build.Utilities;
using System.Xml;
using Microsoft.Build.Framework;
using System;
using System.Reflection;
using System.Linq;

namespace SoupSoftware
{
    public class BuildAssemblyGenerator : Task
    {


        [Required]
        public string year { get; set; }

        [Required]
        public string FileName { get; set; }
     
        public override bool Execute()
        {
            bool cont = true;
            Log.LogMessage(MessageImportance.High, "Starting Assembly Version Generation", null);
            try
            {
                int yearasInt = Convert.ToInt32(year);
                string assyversion = (yearasInt - 2007).ToString() + ".0";
                string[] CodeStr = new string[] { };

                if (FileName.ToLower().Contains("vb")){ 
                CodeStr = APIStravb(assyversion);
                }
                else if (FileName.ToLower().Contains("cs"))
                {
                    
                CodeStr = APIStravc(assyversion);
                }
                // some times possible to include /r/n depending how proj XML is formatted, cleanup filename
                foreach (string c in System.IO.Path.GetInvalidFileNameChars().Intersect(System.IO.Path.GetInvalidPathChars()).Select(x => x.ToString()).ToArray())
                {
                    FileName = FileName.Replace(c, "");
                }
                string Path = System.IO.Path.GetDirectoryName(FileName);
                string fn = System.IO.Path.GetFileName(FileName).TrimStart().TrimEnd();
                FileName = System.IO.Path.Combine(Path, fn);

                if (System.IO.Path.IsPathRooted(FileName))
                {
                    
                        System.IO.File.WriteAllLines(
                           FileName, CodeStr

                           );
                    
                }
                else
                {
                    throw new Exception("Invalid FilePath for Assembly Attribute Source Code");
                }

            }


            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }
             return cont;
        }

        private static string[] APIStravc(string assyversion)
        {
            List<string> line = new List<string>();
            line.Add(string.Format("[assembly: Autodesk.Connectivity.Extensibility.Framework.ApiVersion({0})]",
               new string[] { char.ConvertFromUtf32(34) + assyversion + char.ConvertFromUtf32(34) }));
            return line.ToArray();
        }
        private static string[] APIStravb(string assyversion)
        {
            List<string> line = new List<string>();
            
            line.Add(string.Format("<Assembly: Autodesk.Connectivity.Extensibility.Framework.ApiVersion({0})>",
               new string[] { char.ConvertFromUtf32(34) + assyversion + char.ConvertFromUtf32(34)}));
            return line.ToArray();
        }
    }
}


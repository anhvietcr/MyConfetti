using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Question
{
    public class Data
    {
        
        public string[] readFile(String fileName)
        {
            string[] questions = File.ReadAllLines(fileName);
            return questions;
        }
        
    }
}

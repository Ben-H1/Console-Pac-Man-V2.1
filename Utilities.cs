using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace PacManV2._1
{
    class Utilities
    {
        public Utilities()
        {

        }

        public void resetCursorPosition()
        {
            Console.SetCursorPosition(0, 0);
        }

        public void setConsoleColours(ConsoleColor fgCol, ConsoleColor bgCol)
        {
            Console.ForegroundColor = fgCol;
            Console.BackgroundColor = bgCol;
        }

        public void setConsoleDimensions(int width, int height)
        {
            Console.WindowWidth = width;
            Console.WindowHeight = height;
        }

        public string createPath(string file)
        {
            return Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName, file);
        }

        public dynamic readFile(string path)
        {
            using (StreamReader sr = new StreamReader(path)) {
                string json = sr.ReadToEnd();

                dynamic array = JsonConvert.DeserializeObject(json);
                return array;
            }
        }
    }
}

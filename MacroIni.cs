using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Macro
{
    public class MacroIni
    {
        private static readonly string FILE_NAME = "MacroConf.ini";
        public static int MinWaitTime { get; private set; }
        public static int MaxWaitTime { get; private set; }

        public static void Read()
        {
            var parser = new FileIniDataParser();

            IniData data = null;
            try
            {
                data = parser.ReadFile(FILE_NAME);
            }
            catch(Exception e)
            {
                return;
            }

            string _MinWaitTime = data["WaitTime"]["MinWaitTime"];            
            if(_MinWaitTime != null)
                MinWaitTime = int.Parse(_MinWaitTime);

            string _MaxWaitTime = data["WaitTime"]["MaxWaitTime"];
            if (_MaxWaitTime != null)
                MaxWaitTime = int.Parse(_MaxWaitTime);
        }
        public static void Write()
        {
            var parser = new FileIniDataParser();

            IniData data = null;
            try
            {
                data = parser.ReadFile(FILE_NAME);
            }
            catch (Exception e)
            {
                data = new IniData();
            }

            data["WaitTime"]["MinWaitTime"] = MinWaitTime.ToString();
         

            data["WaitTime"]["MaxWaitTime"] = MaxWaitTime.ToString();
            parser.WriteFile(FILE_NAME, data);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWorkCore.Core
{
    class Test
    {
        public void Log(string logstring)
        {
            RandomValue randomvalue = new RandomValue();
            
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(randomvalue.Nest());
            }

            randomvalue.SetRange(1,100);
            Console.WriteLine("==============");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(randomvalue.Nest());
            }

            //Console.WriteLine(new NumberFactory().GetCentCout(19.97923f));
            //Console.WriteLine(logstring);
        }
    }
}

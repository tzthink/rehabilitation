using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRehabilitation8
{
    public static class Class1
    {
        public static int counterOfTwoTextbox = 0;
        public struct RehabilicationType
        {
            public string typeOfPose;
            public int timeOfPose;
        }
        public static RehabilicationType rT = new RehabilicationType();
        public static List<RehabilicationType> listRT = new List<RehabilicationType>();
    }
}

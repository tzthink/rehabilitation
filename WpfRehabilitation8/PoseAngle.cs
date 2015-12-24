using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace WpfRehabilitation8
{
    public class PoseAngle
    {
        public PoseAngle(JointType firstJoint, JointType secondJoint, JointType thirdJoint, double angle, double threshold)
        {
            FirstJoint = firstJoint;
            SecondJoint = secondJoint;
            ThirdJoint = thirdJoint;
            Angle = angle;
            Threshold = threshold;
        }

        public JointType FirstJoint { get; private set; }
        public JointType SecondJoint { get; private set; }
        public JointType ThirdJoint { get; private set; }
        public double Angle { get; private set; }
        public double Threshold { get; private set; }
    }
}

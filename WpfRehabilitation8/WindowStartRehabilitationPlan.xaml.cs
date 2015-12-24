using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Windows.Threading;
using System.IO;

namespace WpfRehabilitation8
{
    /// <summary>
    /// WindowStartRehabilitationPlan.xaml 的交互逻辑
    /// </summary>
    public partial class WindowStartRehabilitationPlan : Window
    {
        #region Member Variables
        private KinectSensor kinectDevice;
        private Skeleton[] frameSkeletons;
        private List<Pose> poseLibrary = new List<Pose>();//important
        private DispatcherTimer poseTimer;  //WPF中System.Windows.Threading命名空间下的DispatcherTimer类可以简单的完成计时器的功能。
        private int numberOfPoseLibrary = 0;
        int ii = 0;
        int k = 0;
        #endregion Member Variables

        #region Constructor

        public WindowStartRehabilitationPlan()
        {
            InitializeComponent();
            PopulatePoseLibrary();

            poseTimer = new System.Windows.Threading.DispatcherTimer();
            poseTimer.Interval = new TimeSpan(0, 0, Class1.listRT[ii].timeOfPose);
            poseTimer.Tick += new EventHandler(poseTimer_Tick);
            poseTimer.Start();

            for (int j = 0; j < numberOfPoseLibrary; j++)
            {
                if (Class1.listRT[ii].typeOfPose == poseLibrary[j].nameOfPose)
                {
                    k = j;
                    break;
                }
            }

            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            this.KinectDevice = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);

            //Time== class1.listrt.timeofpose+++
            //计时器
            //从第一个动作开始，kinect得到的数据和标准库中得到的数据进行比较，对的不做反应，错的给出提示
            //到时间移向下一个动作，知道计时器时间结束
            //关闭窗口
        }
        #endregion Constructor

        #region Methods
        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Initializing:
                case KinectStatus.Connected:
                case KinectStatus.NotPowered:
                case KinectStatus.NotReady:
                case KinectStatus.DeviceNotGenuine:
                    this.KinectDevice = e.Sensor;
                    break;
                case KinectStatus.Disconnected:
                    //TODO: Give the user feedback to plug-in a Kinect device.                    
                    this.KinectDevice = null;
                    break;
                default:
                    //TODO: Show an error state
                    break;
            }
        }

        private void KinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    frame.CopySkeletonDataTo(this.frameSkeletons); //必须

                    Skeleton skeleton = GetPrimarySkeleton(this.frameSkeletons);

                    if (ii < Class1.counterOfTwoTextbox)
                    {
                        if (skeleton == null)
                        {
                            tbPoseRequirement.Text = Class1.listRT[ii].typeOfPose.ToString() + "  " + Class1.listRT[ii].timeOfPose.ToString() + "seconds";
                            tbWhetherIsPose.Text = "No skeleton";
                        }
                        else
                        {
                            tbPoseRequirement.Text = Class1.listRT[ii].typeOfPose.ToString() + "  " + Class1.listRT[ii].timeOfPose.ToString() + "seconds";

                            //从typeOfPose选出对应的Pose,即找到poseLibrary对应的下标
                            if(IsPose(skeleton, poseLibrary[k]))
                                tbWhetherIsPose.Text ="Your pose is right.";
                            else
                                tbWhetherIsPose.Text ="Your pose is wrong.";
                            //tbWhetherIsPose.Text = Convert.ToString(IsPose(skeleton, poseLibrary[k]));
                        }
                    }
                    else
                    {
                        tbPoseRequirement.Text = "";
                        tbWhetherIsPose.Text = "";
                        tbRehabilitationState.Text = "Rehabilitation OVER";
                        Class1.counterOfTwoTextbox = 0;
                        Class1.listRT = new List<Class1.RehabilicationType>();//集合不用clear，引用类型！！！！！！！其他部分引用了他会出错
                    }
           
                }
            }
        }

        void poseTimer_Tick(object sender, EventArgs e)
        {
            ii++;
            if (ii < Class1.counterOfTwoTextbox)
            {
                poseTimer.Interval = new TimeSpan(0, 0, Class1.listRT[ii].timeOfPose);
                poseTimer.Start();
                for (int j = 0; j < numberOfPoseLibrary; j++)
                {
                    if (Class1.listRT[ii].typeOfPose == poseLibrary[j].nameOfPose)
                    {
                        k = j;
                        break;
                    }
                }
            }
        }

        private Point GetJointPoint(Joint joint)
        {
            DepthImagePoint point = this.KinectDevice.MapSkeletonPointToDepth(joint.Position, this.KinectDevice.DepthStream.Format);
            point.X = point.X * (int)this.LayoutRoot.ActualWidth / KinectDevice.DepthStream.FrameWidth / 2;
            point.Y = point.Y * (int)this.LayoutRoot.ActualHeight / KinectDevice.DepthStream.FrameHeight / 2;

            return new Point(point.X, point.Y);
        }


        private double GetJointAngle(Joint firstJoint, Joint secondJoint, Joint thirdJoint)//求secondJoint对应的角度
        {
            Point firstPoint = GetJointPoint(firstJoint);//和PoseAngle.cs中定义的有点乱？？？？？？？？？？？
            Point secondPoint = GetJointPoint(secondJoint);
            Point thirdPoint = GetJointPoint(thirdJoint);

            double a;
            double b;
            double c;

            a = Math.Sqrt(Math.Pow(firstPoint.X - secondPoint.X, 2) + Math.Pow(firstPoint.Y - secondPoint.Y, 2));
            b = Math.Sqrt(Math.Pow(thirdPoint.X - secondPoint.X, 2) + Math.Pow(thirdPoint.Y - secondPoint.Y, 2));
            c = Math.Sqrt(Math.Pow(firstPoint.X - thirdPoint.X, 2) + Math.Pow(firstPoint.Y - thirdPoint.Y, 2));

            double angleRad = Math.Acos((a * a + b * b - c * c) / (2 * a * b));
            double angleDeg = angleRad * 180 / Math.PI;

            /*if (primaryPoint.Y < anglePoint.Y)
            {
                angleDeg = 360 - angleDeg;
            }*///????????????????????????????????????????????????????????????

            return angleDeg;
        }


        private void PopulatePoseLibrary()
        {
            string fileOfPose;
            fileOfPose = File.ReadAllText(@"e:\pose.txt", Encoding.ASCII);

            string testString;
            string setContentOfPose = null;

            Pose onePose = new Pose();//////////////////////////////////////////////////
            onePose.Angles = new List<PoseAngle>();
            PoseAngle onePoseAngle;
            JointType firstJoint = new JointType();
            JointType secondJoint = new JointType();
            JointType thirdJoint = new JointType();
            double angle = 0.0;
            double threshold = 0.0;


            for (int i = 0; i < fileOfPose.Length; i++)
            {
                testString = fileOfPose.Substring(i, 1);

                if (testString == "#")
                {
                    onePose.numberOfAngle = Convert.ToInt32(setContentOfPose, 10);
                    setContentOfPose = null;
                }
                else if (testString == "%")
                {
                    onePose.nameOfPose = setContentOfPose;
                    setContentOfPose = null;
                }
                else if (testString == "$")
                {
                    firstJoint = evaluateJointType(setContentOfPose);
                    setContentOfPose = null;
                }
                else if (testString == "~")
                {
                    secondJoint = evaluateJointType(setContentOfPose);
                    setContentOfPose = null;
                }
                else if (testString == "|")
                {
                    thirdJoint = evaluateJointType(setContentOfPose);
                    setContentOfPose = null;
                }
                else if (testString == "@")
                {
                    angle = Convert.ToDouble(setContentOfPose);
                    setContentOfPose = null;
                }
                else if (testString == "/")
                {
                    threshold = Convert.ToDouble(setContentOfPose);
                    onePoseAngle = new PoseAngle(firstJoint, secondJoint, thirdJoint, angle, threshold);
                    onePose.Angles.Add(onePoseAngle);
                    setContentOfPose = null;
                }
                else if (testString == ")")
                {
                    poseLibrary.Add(onePose);
                    //onePose.Angles.RemoveRange(0, 4);集合是引用类型！！！！！！！！！！！
                    onePose = new Pose();
                    onePose.Angles = new List<PoseAngle>();
                    numberOfPoseLibrary++;
                }
                else
                {
                    setContentOfPose = setContentOfPose + testString;
                }

            }

        }

        private JointType evaluateJointType(string st)
        {
            JointType eJT;
            if (st.Substring(0, 1) == "H")
            {
                if (st.Length == 4) { eJT = JointType.Head; return eJT; }
                else if (st.Length == 12) { eJT = JointType.HipLeft; return eJT; }
                else if (st.Substring(4, 1) == "e") { eJT = JointType.HipCenter; return eJT; }
                else if (st.Substring(4, 1) == "L") { eJT = JointType.HandLeft; return eJT; }
                else if (st.Substring(4, 1) == "R") { eJT = JointType.HandRight; return eJT; }
                else { eJT = JointType.HipRight; return eJT; }
            }
            else if (st.Substring(0, 1) == "S")
            {
                if (st.Length == 5) { eJT = JointType.Spine; return eJT; }
                else if (st.Length == 14) { eJT = JointType.ShoulderCenter; return eJT; }
                else if (st.Length == 12) { eJT = JointType.ShoulderLeft; return eJT; }
                else { eJT = JointType.ShoulderRight; return eJT; }
            }
            else if (st.Substring(0, 1) == "E")
            {
                if (st.Length == 9) { eJT = JointType.ElbowLeft; return eJT; }
                else { eJT = JointType.ElbowRight; return eJT; }
            }
            else if (st.Substring(0, 1) == "W")
            {
                if (st.Length == 9) { eJT = JointType.WristLeft; return eJT; }
                else { eJT = JointType.WristRight; return eJT; }
            }
            else if (st.Substring(0, 1) == "K")
            {
                if (st.Length == 8) { eJT = JointType.KneeLeft; return eJT; }
                else { eJT = JointType.KneeRight; return eJT; }
            }
            else if (st.Substring(0, 1) == "A")
            {
                if (st.Length == 9) { eJT = JointType.AnkleLeft; return eJT; }
                else { eJT = JointType.AnkleRight; return eJT; }
            }
            else
            {
                if (st.Length == 8) { eJT = JointType.FootLeft; return eJT; }
                else { eJT = JointType.FootRight; return eJT; }
            }
        }

        private bool IsPose(Skeleton skeleton, Pose pose)
        {
            bool isPose = true;
            double angle;
            double poseAngle;
            double poseThreshold;
            double loAngle;
            double hiAngle;
            

            for (int i = 0; i < pose.numberOfAngle && isPose; i++)
            {
                poseAngle = pose.Angles[i].Angle;
                poseThreshold = pose.Angles[i].Threshold;
                hiAngle = poseAngle + poseThreshold;
                loAngle = poseAngle - poseThreshold;
                angle = GetJointAngle(skeleton.Joints[pose.Angles[i].FirstJoint], skeleton.Joints[pose.Angles[i].SecondJoint], skeleton.Joints[pose.Angles[i].ThirdJoint]);

                if (hiAngle >= 360 || loAngle < 0)
                {
                    loAngle = (loAngle < 0) ? 360 + loAngle : loAngle;
                    hiAngle = hiAngle % 360;
                    
                    isPose = !(loAngle > angle && angle > hiAngle);
                }
                else
                {
                    isPose = (loAngle <= angle && hiAngle >= angle);
                }
            }

            return isPose;
        }

        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;

            if (skeletons != null)
            {
                //Find the closest skeleton       
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }

            return skeleton;
        }

        #endregion Methods

        #region Properties
        public KinectSensor KinectDevice
        {
            get { return this.kinectDevice; }
            set
            {
                if (this.kinectDevice != value)
                {
                    //Uninitialize
                    if (this.kinectDevice != null)
                    {
                        this.kinectDevice.Stop();
                        this.kinectDevice.SkeletonFrameReady -= KinectDevice_SkeletonFrameReady;
                        this.kinectDevice.SkeletonStream.Disable();
                        SkeletonViewerElement.KinectDevice = null;
                        this.frameSkeletons = null;
                    }

                    this.kinectDevice = value;

                    //Initialize
                    if (this.kinectDevice != null)
                    {
                        if (this.kinectDevice.Status == KinectStatus.Connected)
                        {
                            this.kinectDevice.SkeletonStream.Enable();
                            this.frameSkeletons = new Skeleton[this.kinectDevice.SkeletonStream.FrameSkeletonArrayLength];
                            this.kinectDevice.Start();

                            SkeletonViewerElement.KinectDevice = this.KinectDevice;
                            this.KinectDevice.SkeletonFrameReady += KinectDevice_SkeletonFrameReady;
                        }
                    }
                }
            }
        }

        #endregion Properties
    }
}

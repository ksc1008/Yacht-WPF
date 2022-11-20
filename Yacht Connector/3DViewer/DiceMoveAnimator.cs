using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Quaternion = System.Windows.Media.Media3D.Quaternion;

namespace Yacht_Connector._3DViewer
{
     public class DiceMoveAnimator
    {
        public enum AnimationPhase {OnBoard, BoardToFloat, FloatToKeep, KeepToFloat, Float, Keep, BackToHand}

        Vector3D fromPos;
        Vector3D toPos;
        public Vector3D currentPos { get; private set; }

        public bool OnRunning { get; private set; }
        AxisAngleRotation3D initialFacingFace;
        float initialYRotation = 0;
        float targetYRotation = 0;

        public AnimationPhase currentPhase { get; private set; } = AnimationPhase.Keep;
        public AnimationPhase targetPhase { get; private set; } = AnimationPhase.OnBoard;

        long startTime;
        long curTime;
        long targetTime;
        int durationMilliseconds;
        long intervalMilliseconds;

        Func<double,double> EaseMethod = (t)=> -(Math.Cos(Math.PI * t) - 1) / 2;

        MeshGeometryModel3D model;

        public DiceMoveAnimator(MeshGeometryModel3D _model, int intervalMilliseconds)
        {
            model = _model;
            this.intervalMilliseconds = intervalMilliseconds;
        }

        public void SetAnimation(Vector3D fromPos, Vector3D toPos, AxisAngleRotation3D initialFace = null, int durationMilliseconds = 300)
        {
            if(initialFace != null)
                initialFacingFace = initialFace.Clone();
            this.fromPos = fromPos;
            this.toPos = toPos;
            this.durationMilliseconds = durationMilliseconds;
        }

        public void StartAnimation(long timeStamp, long frequency, AnimationPhase targetPhase)
        {
            this.targetPhase = targetPhase;
            currentPos = fromPos;
            if(targetPhase == AnimationPhase.Float)
            {
                if (currentPhase == AnimationPhase.OnBoard)
                    currentPhase = AnimationPhase.BoardToFloat;
                else if(currentPhase == AnimationPhase.Float)
                    currentPhase= AnimationPhase.Float;
                else
                    currentPhase = AnimationPhase.KeepToFloat;
            }
            else if(targetPhase == AnimationPhase.Keep)
            {
                currentPhase = AnimationPhase.FloatToKeep;
            }
            else if(targetPhase == AnimationPhase.BackToHand)
            {
                currentPhase = AnimationPhase.BackToHand;
                this.targetPhase = AnimationPhase.BackToHand;
            }

            OnRunning = true;
            startTime = timeStamp;
            curTime = timeStamp;
            targetTime = durationMilliseconds * frequency / 1000 + timeStamp ;
        }

        public void ChangeDestination(Vector3D toPos)
        {
            var tr = GetCurrentTimeRatio();
            var curPos = GetCurrentPosition(tr);
            tr = EaseMethod(tr);
            if (tr > 0)
            {
                this.fromPos = currentPos + (curPos - toPos) / (1 - tr) * tr;
            }
            this.toPos = toPos;
        }

        Matrix3D GetCurrentRotation(double timeRatio)
        {
            double rotation = (timeRatio * targetYRotation + (1 - timeRatio) * initialYRotation);

            var mat = new RotateTransform3D(initialFacingFace).ToMatrix().ToMatrix3D();
            mat.Rotate(new Quaternion(new Vector3D(0, 1, 0), rotation));
            return mat;
        }

        Vector3D GetCurrentPosition(double timeRatio)
        {

            double eased = EaseMethod(timeRatio);
            return (eased * toPos + (1 - eased) * fromPos);
        }
               

        double GetCurrentTimeRatio()
        {
            return  1- ((double)targetTime - curTime) / (targetTime - startTime);
        }

        public bool Update(long timeStamp, long frequency)
        {
            if ( ((float)(timeStamp - curTime) / frequency * 1000) < intervalMilliseconds )
                return true;
            curTime = timeStamp;
            if (curTime > targetTime)
                curTime = targetTime;
            var tr = GetCurrentTimeRatio();
            var trans = GetCurrentRotation(tr);
            currentPos = GetCurrentPosition(tr);
            trans.Translate(currentPos);
            model.Transform = new MatrixTransform3D(trans);

            if (curTime >= targetTime)
            {
                switch (targetPhase)
                {
                    case AnimationPhase.Keep:
                        currentPhase = AnimationPhase.Keep;
                        break;
                    case AnimationPhase.Float:
                        currentPhase = AnimationPhase.Float;
                        break;
                    case AnimationPhase.BackToHand:
                        currentPhase = AnimationPhase.OnBoard;
                        break;
                }
                OnRunning = false;
                return false;
            }
            return true;
        }
    }
}

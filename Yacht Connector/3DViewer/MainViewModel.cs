using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using Yacht_Connector._3DViewer;

namespace Yacht_Connector
{
    using Vector3D = System.Windows.Media.Media3D.Vector3D;


    public partial class MainViewModel : BaseViewModel
    {
        readonly int FLOAT_KEEP_DURATION = 150;
        readonly int DICE_HIDE_DURATION = 1000;
        readonly int DICE_PICKING_DURATION = 300;

        public enum State { Cleared, RollingAnimation, RollingToPicking, PickingDice, DiceToHand, ClearingBoard, LastStand}

        State _curentState = State.Cleared;

        public State CurrentState { get => _curentState; private set { _curentState = value; CheckButtonActivision(); } }

        private bool isLastRoll;

        int _floatNum = 5;
        int floatNum { get => _floatNum; set { _floatNum = value; CheckButtonActivision(); }}
        int[] diceResult = { -1, -1, -1, -1, -1 };
        int[] floatings = {0, 1, 2, 3, 4};
        int[] keep = { -1, -1, -1, -1, -1 };

        DiceRoller mainWindow;


        public delegate void ReturnDiceDelegate(int[] dice);
        public delegate void ButtonDelegate(bool state);
        public delegate void GeneralCallbackDelegate();

        public ReturnDiceDelegate DiceRollCallback;
        public ButtonDelegate ButtonStateCallback;
        public GeneralCallbackDelegate FirstRenderCallback = null;

        private bool _initialized = true;
        public bool Initialized { get => _initialized; private set => _initialized = value; }


        public Vector3D UpDirection { set; get; } = new Vector3D(0, 1, 0);

        List<float> simulationResult;


        TextureModel DiceTexture;
        TextureModel DiceNormalTexture;

        void CheckButtonActivision()
        {
            ButtonStateCallback(DiceRollReady());
        }

        private void Render(object? sender, System.Windows.Media.RenderingEventArgs e)
        {
            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;
            bool end = false;
            for (int i = 0; i < floatNum; i++)
            {
                if (!animators[floatings[i]].Update(ts, fq))
                    end = true;
            }

            if (end)
            {
                CurrentState = State.RollingToPicking;
                compositeHelper.Rendering -= Render;
                if (isLastRoll)
                    SetLastMove();
                else
                    SetMove();
                SetDiceTable();
            }
        }

        public void HideDice()
        {
            CurrentState = State.ClearingBoard;

            var ts = Stopwatch.GetTimestamp();
            var fq = Stopwatch.Frequency;
            for (int i = 0; i < 5; i++)
            {
                if (keep[i] == -1)
                    continue;
                moveAnimators[keep[i]].SetAnimation(GetKeepVector(i), new Vector3D(0, 1.5, -40), null, DICE_HIDE_DURATION);
                moveAnimators[keep[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.BackToHand);
            }
            for (int i = 0; i < floatNum; i++)
            {
                moveAnimators[floatings[i]].SetAnimation(moveAnimators[floatings[i]].currentPos, new Vector3D(0, 1.5, -40), null, DICE_HIDE_DURATION);
                moveAnimators[floatings[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.BackToHand);
            }
            for (int i = 0; i < 5; i++)
            {
                floatings[i] = i;
                keep[i] = -1;
            }
            floatNum = 5;
        }

        public bool DiceRollReady()
        {
            if (isLastRoll)
                return false;
            if (floatNum == 0 && CurrentState != State.Cleared)
                return false;
            if (CurrentState != State.PickingDice && CurrentState != State.Cleared)
                return false;
            return true;
        }

        public void AnimateDiceRoll(List<float> v,bool isLast)
        {
            isLastRoll = isLast;
            if (CurrentState == State.Cleared)
            {
                CurrentState = State.RollingAnimation;
                StartDiceRollAnimation(v);
            }
            else
            {
                simulationResult = v;
                CurrentState = State.DiceToHand;
                var ts = Stopwatch.GetTimestamp();
                var fq = Stopwatch.Frequency;
                for (int i = 0; i < floatNum; i++)
                {
                    moveAnimators[floatings[i]].SetAnimation(moveAnimators[floatings[i]].currentPos, new Vector3D(0, 2, 40), null, DICE_HIDE_DURATION);
                    moveAnimators[floatings[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.BackToHand);
                }
            }
        }

        void StartDiceRollAnimation(List<float> v)
        {
            long t = Stopwatch.GetTimestamp();
            for (int i = 0; i < floatNum; i++)
            {
                animators[floatings[i]].SetAnimation(v, floatNum, i, diceResult[floatings[i]]);
                animators[floatings[i]].StartAnimation(t);
            }
            compositeHelper.Rendering += Render;
        }

        public int[] GetDiceValues()
        {
            int[] newArr = (int[])diceResult.Clone();
            return newArr;
        }

        public void SetDiceResult(List<int> dr)
        {
            if (CurrentState == State.Cleared)
            {
                Debug.Assert(dr.Count == 5);
                for (int i = 0; i < dr.Count; i++)
                {
                    diceResult[i] = dr[i];
                }
            }
            else
            {
                Debug.Assert(dr.Count == floatNum);
                for (int i = 0; i < dr.Count; i++)
                {
                    diceResult[floatings[i]] = dr[i];
                }
            }
        }

        public bool ClickFloating(int floatIndex)
        {
            Debug.Assert(floatIndex >= 0 && floatIndex < 5);
            if (floatings[floatIndex] == -1 || CurrentState != State.PickingDice)
                return false;
            if (moveAnimators[floatings[floatIndex]].currentPhase != DiceMoveAnimator.AnimationPhase.Float)
                return false;

            FloatingToKeep(floatIndex);
            return true;
        }

        public bool ClickKeep(int keepIndex)
        {
            Debug.Assert(keepIndex >= 0 && keepIndex < 5);
            if (keep[keepIndex] == -1|| CurrentState != State.PickingDice)
                return false;
            if (moveAnimators[keep[keepIndex]].currentPhase != DiceMoveAnimator.AnimationPhase.Keep)
                return false;

            KeepToFloating(keepIndex);
            return true;
        }

        void FloatingToKeep(int floatIndex)
        {
            int keepIndex = -1;
            for (int i = 0; i < 5; i++)
            {
                if (keep[i] == -1)
                {
                    keepIndex = i;
                    break;
                }
            }

            Debug.Assert(keepIndex >= 0,"Keep is Full.");

            keep[keepIndex] = floatings[floatIndex];
            floatings[floatIndex] = -1;
            RearrangeFloatings();
            floatNum--;


            moveAnimators[keep[keepIndex]].SetAnimation(moveAnimators[keep[keepIndex]].currentPos, GetKeepVector(keepIndex),null,FLOAT_KEEP_DURATION);
            moveAnimators[keep[keepIndex]].StartAnimation(Stopwatch.GetTimestamp(), Stopwatch.Frequency, DiceMoveAnimator.AnimationPhase.Keep);
            ProcessArrangeAnimation();
        }

        void KeepToFloating(int keepIndex)
        {
            Debug.Assert(floatNum < 5, "Floating is Full");

            floatings[floatNum++] = keep[keepIndex];
            keep[keepIndex] = -1;

            moveAnimators[floatings[floatNum - 1]].SetAnimation(moveAnimators[floatings[floatNum - 1]].currentPos, GetFloatVector(floatNum, floatNum - 1),null, FLOAT_KEEP_DURATION);
            moveAnimators[floatings[floatNum - 1]].StartAnimation(Stopwatch.GetTimestamp(), Stopwatch.Frequency, DiceMoveAnimator.AnimationPhase.Float);
            ProcessArrangeAnimation();
        }

        void RearrangeFloatings()
        {
            for(int i = 0; i < floatNum; i++)
            {
                if (floatings[i] != -1)
                    continue;
                for (int j = i + 1; j < floatNum; j++)
                {
                    if (floatings[j] == -1)
                        continue;

                    floatings[i] = floatings[j];
                    floatings[j] = -1;
                    break;
                }
            }
        }

        void ProcessArrangeAnimation()
        {
            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;
            for (int i = 0; i < floatNum; i++)
            {
                if (moveAnimators[floatings[i]].currentPhase == DiceMoveAnimator.AnimationPhase.Float)
                {
                    moveAnimators[floatings[i]].SetAnimation(moveAnimators[floatings[i]].currentPos, GetFloatVector(floatNum, i));
                    moveAnimators[floatings[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.Float);
                }
                else if (moveAnimators[floatings[i]].currentPhase == DiceMoveAnimator.AnimationPhase.KeepToFloat)
                {
                    moveAnimators[floatings[i]].ChangeDestination(GetFloatVector(floatNum, i));
                }
            }
        }

        private Vector3D GetFloatVector(int floatNum, int floatIndex)
        {
            double span = 1.4 + (5 - floatNum) * 0.3;
            return new Vector3D(floatIndex * span - (float)(floatNum - 1) / 2 * span, 4, 5.5);

        }
        private Vector3D GetKeepVector(int keepIndex)
        {
            double span = 2; 
            return new Vector3D(keepIndex * span - 2 * span, 1.5, 0);
        }


        private void SetMove()
        {
            int[] order = new int[floatNum];
            for (int i = 0; i < floatNum; i++)
                order[i] = i;
            var sortedOrder = order.OrderBy(x => animators[floatings[x]].LastPos.X).ToArray();
            for (int i = 0; i < floatNum; i++)
                order[i] = floatings[sortedOrder[i]];
            for (int i = 0; i < floatNum; i++)
                floatings[i] = order[i];
            int index;

            for (int i = 0; i < floatNum; i++)
            {
                index = floatings[i];
                moveAnimators[index].SetAnimation(animators[index].LastPos, GetFloatVector(floatNum, i), animators[index].LastAngle, DICE_PICKING_DURATION);
            }
            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;

            for (int i = 0; i < floatNum; i++)
            {
                moveAnimators[floatings[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.Float);
            }
        }

        private void SetLastMove()
        {
            int[] order = new int[floatNum];
            for (int i = 0; i < floatNum; i++)
                order[i] = i;
            var sortedOrder = order.OrderBy(x => animators[floatings[x]].LastPos.X).ToArray();
            for (int i = 0; i < floatNum; i++)
                order[i] = floatings[sortedOrder[i]];
            for (int i = 0; i < floatNum; i++)
                floatings[i] = order[i];
            int index;

            int j = -1;

            for (int i = 0; i < floatNum; i++)
            {
                for (++j; keep[j] != -1; j++) ;
                index = floatings[i];
                moveAnimators[index].SetAnimation(animators[index].LastPos, GetKeepVector(j), animators[index].LastAngle, FLOAT_KEEP_DURATION);
            }
            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;

            for (int i = 0; i < floatNum; i++)
            {
                moveAnimators[floatings[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.Float);
            }
        }

        private void SetDiceTable()
        {
            DiceRollCallback(diceResult);
        }

        private void RenderMove(object? sender, System.Windows.Media.RenderingEventArgs e)
        {
            if (firstRun)
            {
                firstRun = false;
                if(FirstRenderCallback != null)
                    FirstRenderCallback();
            }
            if (CurrentState == State.RollingAnimation || CurrentState == State.Cleared || CurrentState == State.LastStand)
                return;
            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;
            for (int i = 0; i < 5; i++)
            {
                if (moveAnimators[i].OnRunning) {
                    if(!moveAnimators[i].Update(ts, fq))
                    {
                        if (CurrentState == State.RollingToPicking)
                        {
                            if (!isLastRoll)
                                CurrentState = State.PickingDice;
                            else
                                CurrentState = State.LastStand;
                        }
                        if (CurrentState == State.DiceToHand)
                            CurrentState = State.RollingAnimation;
                        else if (CurrentState == State.ClearingBoard)
                            CurrentState = State.Cleared;
                    }
                }
            }

            if (CurrentState == State.RollingAnimation)
            {
                StartDiceRollAnimation(simulationResult);
            }
        }

    }
}

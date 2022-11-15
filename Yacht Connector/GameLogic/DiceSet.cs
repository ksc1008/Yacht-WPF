using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yacht
{
    internal class DiceSet
    {
        public enum category
        {
            Ones, Twos, Threes, Fours, Fives, Sixes,
            Choice, FoK, FullHouse, LStraight, BStraight, Yacht
        }

        private int[] points = new int[Enum.GetValues(typeof(category)).Length];

        private int[] dices = {0, 0, 0, 0, 0};

        private int[] diceCount = new int[6];

        public int[] Dices { get { return dices; } }

        static private Random rnd = new Random();

        public void Roll(bool[] diceRollMask)
        {
            if (diceRollMask.Length != 5)
            {
                throw new ArgumentException("Invalid Dice Roll Mask.");
            }

            MakeDices(diceRollMask);
            CalculateDices();
        }

        public int GetPoint(category category)
        {
            return points[(int)category];
        }

        void MakeDices(bool[] diceRollMask)
        {
            for (int i = 0; i < dices.Length; i++)
            {
                if (diceRollMask[i])
                {
                    dices[i] = rnd.Next() % 6;
                }
            }
        }

        void CalculateDices()
        {
            CountDices();
            CalcSingles();
            CalcChoice();
            CalcFoK();
            CalcFullHouse();
            CalcLittleStraight();
            CalcBigStraight();
            CalcYacht();
        }

        void CountDices()
        {
            for (int i = 0; i < 6; i++)
                diceCount[i] = 0;

            for (int i = 0; i < 5; i++)
                diceCount[dices[i]]++;
        }

        void CalcSingles()
        {            
            for(int i = 0; i < 6; i++)
                points[i] = diceCount[i] * (i+1);
        }

        void CalcFoK()
        {
            for(int i = 0; i < 6; i++)
            {
                if (diceCount[i] >= 4)
                {
                    points[(int)category.FoK] = points[(int)category.Choice];
                    return;
                }
            }
            points[(int)category.FoK] = 0;
        }

        void CalcFullHouse()
        {
            int threeDice = -1;
            int twoDice = -1;
            for(int i = 0;i < 6; i++)
            {
                if (diceCount[i] == 3)
                    threeDice = i;
                else if (diceCount[i] == 2)
                    twoDice = i;
            }
            if(threeDice == -1 || twoDice == -1)
            {
                points[(int)category.FullHouse] = 0;
                return;
            }
            points[(int)category.FullHouse] = points[(int)category.Choice];
        }

        void CalcLittleStraight()
        {
            if (diceCount[2] == 0 || diceCount[3] == 0)
            {
                points[(int)category.LStraight] = 0;
                return;
            }
            if(diceCount[1] >= 1)
            {
                if (diceCount[0]>= 1 || diceCount[4] >= 1)
                {
                    points[(int)category.LStraight] = 15;
                    return;
                }
            }
            else if (diceCount[4]>=1 && diceCount[5] >= 1)
            {
                points[(int)category.LStraight] = 15;
                return;

            }
            points[(int)category.LStraight] = 0;
            return;
        }

        void CalcBigStraight()
        {
            if (diceCount[1] == 0 || diceCount[2] == 0 || diceCount[3] == 0|| diceCount[4] == 0)
            {
                points[(int)category.BStraight] = 0;
                return;
            }
            if (diceCount[0] >=1 || diceCount[5] >= 1)
            {
                points[(int)category.BStraight] = 30;
                return;
            }
        }

        void CalcYacht()
        {
            for(int i = 0; i < 6; i++)
            {
                if (diceCount[i] == 5)
                {
                    points[(int)category.Yacht] = 50;
                    return;
                }
            }
            points[(int)category.Yacht] = 0;
        }

        void CalcChoice()
        {
            points[(int)category.Choice] = 0;
            for(int i = 0; i < 6; i++)
            {
                points[(int)category.Choice] += diceCount[i] * (i + 1);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yacht
{
    internal class ScoreBoard
    {
        private bool[] isChecked = new bool[Enum.GetValues(typeof(DiceSet.category)).Length];
        private int[] scores = new int[Enum.GetValues(typeof(DiceSet.category)).Length];
        private int subTotal = 0;
        private int total = 0;
        private bool bonusEnabled = false;

        /** Return if the type of score is already checked  */
        public bool GetChecked(DiceSet.category category) => isChecked[(int)category];
        public int GetScores(DiceSet.category category) => scores[(int)category];

        public int SetScore(DiceSet.category category, int score)
        {
            if (GetChecked(category))
                throw new ArgumentException("The type of score is already checked.");
            scores[(int)category] = score;
            isChecked[(int)category] = true;

            if((int)category <= (int)DiceSet.category.Sixes)
            {
                CalcSingles(score);
            }
            else
            {
                CalcNonSingles(score);
            }
            return score;
        }

        public int GetSubTotal() => subTotal;
        public int GetTotal() => total;
        public bool GetBonusEnabled() => bonusEnabled;

        private void CalcSingles(int score)
        {
            subTotal += score;
            total += score;
            if(!bonusEnabled && subTotal >= 63)
            {
                bonusEnabled = true;
                total += 35;
            }
        }

        private void CalcNonSingles(int score)
        {
            total += score;
        }
    }
}

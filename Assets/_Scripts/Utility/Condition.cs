using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.Utility
{
    enum Operator
    {
        Less,
        Equal,
        Greater,
        LessEqual,
        GreaterEqual,

        NUM
    }

    static class Condition
    {
        public static bool Compare<T>(T lhs, T rhs, Operator op) where T : IComparable
        {
            bool retVal = false;

            int compareValue = lhs.CompareTo(rhs);
            switch (op)
            {
                case Operator.Less:
                    retVal = compareValue < 0;
                    break;

                case Operator.Greater:
                    retVal = compareValue > 0;
                    break;

                case Operator.Equal:
                    retVal = compareValue == 0;
                    break;

                case Operator.LessEqual:
                    retVal = compareValue <= 0;
                    break;

                case Operator.GreaterEqual:
                    retVal = compareValue >= 0;
                    break;
            }

            return retVal;
        }
    }
}

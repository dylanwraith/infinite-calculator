using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// Name: Dylan Wraith
/// Date: 09/24/2018

namespace InfInt
{
    public class InfInt : IComparable
    {
        private const int DIGITS = 40; //max size is 40
        private int[] Integer { get; set; } //array containing our infint
        private bool Positive { get; set; } //is it positive

        //default value constructor
        public InfInt()
        {
            Integer = new int[DIGITS];
            Positive = true;
        }

        //parametrized constructor
        public InfInt(string input)
        {
            //accepts a string value of your infint and initialize the fields
            //remember you are allowed a max length of 40 and it can be a negative number
            //if you have a negative number the length could be 41 to account for the '-' in front
            Integer = new int[DIGITS];
            Positive = true;
            int i = 0;
            string temp;
            if (input[i] == '-') {
                Positive = false;
                i++;
            }
            while (i < input.Length)
            {
                temp = Convert.ToString(input[i]);
                Integer[DIGITS - input.Length + i] = Int32.Parse(temp);
                i++;
            }
        }

        //freebie add courtesy of professor amack
        public InfInt Add(InfInt addValue)
        {
            InfInt temp = new InfInt();
            if (Positive == addValue.Positive)
            {
                temp = AddPositives(addValue);
            }
            //addvalue is negative
            else if (Positive && (!addValue.Positive))
            {
                addValue.Positive = true;
                if (IsGreaterThan(addValue))
                {
                    temp = SubtractPositives(addValue);
                }
                else
                {
                    temp = addValue.SubtractPositives(this);
                    temp.Positive = false;
                }

                addValue.Positive = false;
            }
            else if (!Positive && addValue.Positive)
            {
                addValue.Positive = false;

                if (IsGreaterThan(addValue))
                {
                    temp = addValue.SubtractPositives(this);
                }
                else
                {
                    temp = SubtractPositives(addValue);
                    temp.Positive = false;
                }

                addValue.Positive = true;
            }
            return temp;
        }

        //Subtracts the absolute values of two InfInt's
        private InfInt SubtractPositives(InfInt addValue)
        {
            InfInt temp = new InfInt();
            InfInt tempThis = new InfInt();
            tempThis.EqualTo(this);
            //iterate the infint
            for (int i = DIGITS - 1; i >= 0; i--)
            {
                int j = i;
                //Borrow
                if (tempThis.Integer[i] < addValue.Integer[i])
                {
                    j--;
                    while (tempThis.Integer[j] == 0)
                    {
                        tempThis.Integer[j] -= 1;
                        j--;
                    }
                    tempThis.Integer[j] -= 1;
                    j++;
                    while (j <= i)
                    {
                        tempThis.Integer[j] += 10;
                        j++;
                    }
                }
                temp.Integer[i] = tempThis.Integer[i] - addValue.Integer[i];
            }
            return temp;
        }

        //Checks to see which InfInt is greater
        private bool IsGreaterThan(InfInt addValue)
        {
            int i = 0;
            if (Positive && !addValue.Positive)
                return true;
            else if (!Positive && addValue.Positive)
                return false;
            while(i < DIGITS)
            {
                if (this.Integer[i] > addValue.Integer[i])
                    return true;
                else if (this.Integer[i] < addValue.Integer[i])
                    return false;
                i++;
            }
            return false;
        }

        //Adds absolute values of two InfInt's
        private InfInt AddPositives(InfInt addValue)
        {
            InfInt temp = new InfInt();
            int carry = 0;
            //iterate the infint
            for (int i = DIGITS - 1; i >= 0; i--)
            {
                temp.Integer[i] = Integer[i] + addValue.Integer[i] + carry;
                //determine if we need to carry the 1 
                if (temp.Integer[i] > 9)
                {
                    temp.Integer[i] %= 10; //reduce to 0-9
                    carry = 1;
                }
                else
                {
                    carry = 0;
                }
            }
            if (carry != 0)
            {
                Console.WriteLine($"Oops, your input produced a value larger than {DIGITS} digits long!");
                InfInt zero = new InfInt();
                return zero;
            }
            if (!Positive)
            {
                temp.Positive = false;
            }

            return temp;
        }

        //Figures out what function needs to be done in order to subtract 2 InfInt's
        public InfInt Subtract(InfInt subtractValue)
        {
            InfInt temp = new InfInt(); // temporary result

            // subtractValue is negative
            if (Positive && (!subtractValue.Positive))
            {
                temp = AddPositives(subtractValue);
            }
            // this InfInt is negative
            else if (!Positive && subtractValue.Positive)
            {
                temp = AddPositives(subtractValue);
            }
            // at this point, both InfInts have the same sign
            else
            {
                bool isPositive = Positive; // original sign
                bool resultPositive = Positive; // sign of the result

                // set both to positive so we can compare absolute values
                Positive = true;
                subtractValue.Positive = true;
                if (this.IsGreaterThan(subtractValue))
                {
                    temp = this.SubtractPositives(subtractValue);
                }
                else
                {
                    temp = subtractValue.SubtractPositives(this);
                    resultPositive = !isPositive; // flip the sign
                }

                Positive = isPositive;
                subtractValue.Positive = isPositive;
                temp.Positive = resultPositive;
            }

            return temp;
        }

        //Multiplies 2 InfInt's
        public InfInt Multiply(InfInt multValue)
        {
            int lengthInput = DIGITS;
            int lengthMultValue = DIGITS;
            InfInt zero = new InfInt("0");
            if (this.CompareTo(zero) == 0 || multValue.CompareTo(zero) == 0)
                return zero;
            //Find lengths of both multiplied values
            while(Integer[DIGITS - lengthInput] == 0)
            {
                if (lengthInput == 0)
                    break;
                lengthInput--;
            }
            while (multValue.Integer[DIGITS - lengthMultValue] == 0)
            {
                if (lengthMultValue == 0)
                    break;
                lengthMultValue--;
            }
            InfInt returnValue = new InfInt();
            //Iterate through each value in one array, multiplying it by each value in the other array
            for (int i = 0; i < lengthMultValue; i++)
            {
                returnValue = returnValue.Add(this.MultiplyThese(multValue.Integer[DIGITS - 1 - i], lengthInput, i));
            }
            //If the multiplied numbers have different signs, the result is negative
            if (this.Positive != multValue.Positive)
                returnValue.Positive = false;
            return returnValue;
        }

        //Continuation of Multiply function dealing with absolute values
        private InfInt MultiplyThese(int multValue, int lengthInput, int column)
        {
            InfInt returnValue = new InfInt();
            int i = 0;
            int carry = 0;
            try
            {
                while (i < lengthInput)
                {
                    int temp = Integer[DIGITS - 1 - i] * multValue;
                    temp += carry;
                    carry = temp / 10;
                    temp %= 10;
                    returnValue.Integer[DIGITS - 1 - i - column] = temp;
                    i++;
                }
                if (carry != 0)
                    returnValue.Integer[DIGITS - 1 - i - column] = carry;
            }
            catch(IndexOutOfRangeException)
            {
                Console.WriteLine($"Oops! The product of these two numbers is outside of the bounds");
                InfInt zero = new InfInt();
                return zero;
            }
            return returnValue;
        }

        //Sets values in InfInt equal to those of the inpt
        private void EqualTo(InfInt input)
        {
            int i = 0;
            Positive = input.Positive;
            while (i < DIGITS)
            {
                Integer[i] = input.Integer[i];
                i++;
            }
        }

        //Divides two InfInt's
        public InfInt Divide(InfInt divValue)
        {
            InfInt zero = new InfInt();
            InfInt one = new InfInt("1");
            InfInt ten = new InfInt("10");
            //Numerator smaller than denominator
            if (this.CompareTo(divValue) == -1)
                return zero;
            //Numerator == Denominator
            if (this.CompareTo(divValue) == 0)
                return one;
            //Divide by zero
            if (divValue.CompareTo(zero) == 0)
            {
                Console.WriteLine("Oops, you tried to divide by zero!");
                return zero;
            }
            //Numerator larger than denominator
            int i = 0;
            InfInt returnValue = new InfInt();
            InfInt tempThis = new InfInt();
            while (i < DIGITS)
            {
                //Drop down a value
                int tempNumber = this.Integer[i];
                string tempString = Convert.ToString(tempNumber);
                InfInt tempInfInt = new InfInt(tempString);
                for (int l = 0; l < DIGITS; l++)
                {
                    tempThis = tempThis.Add(tempInfInt);
                    //divValue divides into tempThis
                    if (tempThis.CompareTo(divValue) >= 0)
                    //See how many times divValue goes into tempThis
                    {
                        InfInt tempReturnValue = new InfInt("1");
                        while (divValue.CompareTo(tempThis.Subtract(divValue.Multiply(tempReturnValue))) <= 0)
                        {
                            tempReturnValue = tempReturnValue.Add(one);
                        }
                        returnValue.Integer[i] = tempReturnValue.Integer[DIGITS - 1];
                        tempThis = tempThis.Subtract(tempReturnValue.Multiply(divValue));
                    }
                    tempThis.Positive = true;
                    tempThis = tempThis.Multiply(ten);
                    i++;
                }
            }
            if (Positive != divValue.Positive)
                returnValue.Positive = false;
            return returnValue;
        }

        //Converts InfInt to string
        public override string ToString()
        {
            int i = 0;
            InfInt zero = new InfInt();
            zero.Positive = false;
            if (zero.CompareTo(this) == 0)
            {
                return "0";
            }
            string returnString = "";
            if (!Positive) {
                returnString += '-';
            }
            while (Integer[i] == 0 && i < DIGITS - 1)
                i++;
            while (i < DIGITS) {
                returnString += Convert.ToString(Integer[i]);
                i++;
            }
            return returnString;
        }

        //-1 == Less than, 0 == Equal to, 1 == Greater than, -2 == Invalid Input
        public int CompareTo(object obj)
        {
            if (obj is InfInt) {
                InfInt input = (InfInt)obj;
                if (Positive != input.Positive)
                {
                    if (Positive)
                        return 1;
                    return -1;
                }
                int i = 0;
                while (i < DIGITS)
                {
                    if (Integer[i] > input.Integer[i])
                    {
                        if (Positive)
                            return 1;
                        return -1;
                    }
                    else if (Integer[i] < input.Integer[i])
                    {
                        if (Positive)
                            return -1;
                        return 1;
                    }
                    i++;
                }
                return 0;
            }
            //Return -2 if input is not of type InfInt
            Console.WriteLine("Oops! Input was not of type InfInt!");
            return -2;
        }
    }
}

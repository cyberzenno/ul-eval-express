namespace ul.eval.express.data
{
    public class EvalModel
    {
        public float? Number { get; set; }
        public string Operator { get; set; }

        public string Rawvalue { get; set; }

        public bool Executed { get; set; }

        public EvalModel(float number)
        {
            Number = number;

            Rawvalue = number.ToString();
        }
        public EvalModel(string operatorOrNumber)
        {
            if (float.TryParse(operatorOrNumber, out float number))
            {
                Number = number;
            }
            else
            {
                Operator = operatorOrNumber;
            }

            Rawvalue = operatorOrNumber;
        }

        public bool IsNumber()
        {
            if (Executed)
            {
                return false;
            }

            if (Number.HasValue)
            {
                return true;
            }

            return false;
        }
        public bool IsOperator()
        {
            if (Executed)
            {
                return false;
            }

            switch (Operator)
            {
                case "*":
                case "/":
                case "+":
                case "-":
                    return true;

                default:
                    return false;
            }
        }

        public bool IsMultiply()
        {
            if (Executed)
            {
                return false;
            }

            return Operator == "*";
        }

        public bool IsDivide()
        {
            if (Executed)
            {
                return false;
            }

            return Operator == "/";
        }

        public bool IsAddition()
        {
            if (Executed)
            {
                return false;
            }

            return Operator == "+";
        }

        public bool IsSubtraction()
        {
            if (Executed)
            {
                return false;
            }

            return Operator == "-";
        }
    }
}

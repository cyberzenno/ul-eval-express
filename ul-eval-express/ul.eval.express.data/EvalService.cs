using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ul.eval.express.data
{
    public class EvalService
    {
        //todo: move cleanup logic to CleanupService

        public EvalResult Evaluate(string expression)
        {
            //0. clean the expression from everything but non-negative integers and the + - / *operators only
            var cleanExpression = CleanExpression(expression);

            //1. split the expression in Numbers and Operators
            var numbersAndOperators = SplitToNumbersAndOperators(cleanExpression);

            var evalModels = ConvertToEvalModel(numbersAndOperators);

            //2. execute evaluation
            var resultEval = ExecuteEvaluationModels(evalModels);

            //3. return result

            return resultEval;
        }

        /// <summary>
        /// Removes any chars not in [0-9, + - / *]
        /// </summary>
        public string CleanExpression(string expression)
        {
            var regEx = new Regex(@"[^0-9,+\-*\/]");

            var cleanExpression = regEx.Replace(expression, "");

            return cleanExpression;
        }

        /// <summary>
        /// Given a clean expression, returns a list of distinct integers or operators.
        /// </summary>
        public List<string> SplitToNumbersAndOperators(string cleanExpression)
        {
            var regEx = new Regex(@"\d+|[+\-\/*]");

            var matches = regEx.Matches(cleanExpression);

            var numbersAndOperators = matches.Select(x => x.Value).ToList();

            return numbersAndOperators;
        }

        /// <summary>
        /// Parses string Number and Operators into typed EvalModels
        /// </summary>
        public List<EvalModel> ConvertToEvalModel(List<string> numbersAndOperators)
        {
            var evalModels = numbersAndOperators.Select(x => new EvalModel(x)).ToList();

            var cleanedEvalModels = CleanEvalModels(evalModels);

            return cleanedEvalModels;
        }

        /// <summary>
        /// Ideally should take care of the logical cleaning of the Evaluation Models (i.e. multiple operators in a row, start with sequence with operator, etc)
        /// </summary>
        public List<EvalModel> CleanEvalModels(List<EvalModel> evalModels)
        {
            //1. make sure it doesn't start with operator
            while (evalModels.First().IsOperator())
            {
                evalModels.RemoveAt(0);
            }

            //2. make sure it doesn't ends with operator
            while (evalModels.Last().IsOperator())
            {
                evalModels.RemoveAt(evalModels.Count - 1);
            }

            //3. make sure it doesn't have multiple operators in a row
            var previous = "operator";

            var cleanModels = new List<EvalModel>();
            for (int i = 0; i < evalModels.Count; i++)
            {
                var em = evalModels[i];

                if (em.IsNumber() && previous == "operator")
                {
                    cleanModels.Add(em);

                    previous = "number";

                    continue;
                }

                if (em.IsOperator() && previous == "number")
                {
                    cleanModels.Add(em);

                    previous = "operator";

                    continue;
                }
            }

            return cleanModels;
        }


        /// <summary>
        /// Execute the actual operation returning the final result as an Evaluation Model
        /// </summary>
        public EvalResult ExecuteEvaluationModels(List<EvalModel> evalList)
        {
            var finalResult = new EvalResult();
            finalResult.ComputedExpressionAfterCleanup = string.Join("", evalList.Select(x => x.Rawvalue));

            //execute Multiplication and Division
            while (evalList.Any(x => x.Operator == "*" || x.Operator == "/"))
            {
                for (int i = 0; i < evalList.Count; i++)
                {
                    var eval = evalList[i];
                    if (eval.IsOperator())
                    {
                        var a = evalList[i - 1];
                        var b = evalList[i + 1];

                        if (eval.IsMultiply() || eval.IsDivide())
                        {
                            if (eval.IsMultiply())
                            {
                                a.Number *= b.Number;
                            }
                            else
                            {
                                a.Number /= b.Number;
                            }

                            eval.Executed = true;
                            b.Executed = true;

                            break;
                        }


                    }
                }

                evalList = evalList.Where(x => !x.Executed).ToList();
            }

            //execute addition and subtraction
            while (evalList.Count > 1)
            {
                for (int i = 0; i < evalList.Count; i++)
                {
                    var eval = evalList[i];
                    if (eval.IsOperator())
                    {
                        var a = evalList[i - 1];
                        var b = evalList[i + 1];

                        if (eval.IsAddition() || eval.IsSubtraction())
                        {
                            if (eval.IsAddition())
                            {
                                a.Number += b.Number;
                            }
                            else
                            {
                                a.Number -= b.Number;
                            }

                            eval.Executed = true;
                            b.Executed = true;

                            break;
                        }
                    }
                }

                evalList = evalList.Where(x => !x.Executed).ToList();
            }


            var evaluatedResult = evalList.SingleOrDefault(x => !x.Executed);

            finalResult.Number = evaluatedResult.Number;

            return finalResult;
        }

    }
}

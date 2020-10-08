using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using ul.eval.express.data;

namespace ul.eval.express.test
{
    public class EvalServiceFixture
    {
        private EvalService _evalService;

        [SetUp]
        public void Setup()
        {
            _evalService = new EvalService();
        }

        [Test]
        [TestCase("4+5*2", 14f)]
        [TestCase("4+5/2", 6.5f)]
        [TestCase("4+5/2-1", 5.5f)]
        [TestCase("1+2+6/2+2*2/4", 7f)]
        public void Evaluate(string expression, float expectedResult)
        {
            //arrange

            //act
            var actual = _evalService.Evaluate(expression);

            //assert
            Assert.IsTrue(actual.Number == expectedResult);
        }

        [Test]
        [TestCase("1+a2", "1+2")]
        [TestCase("1+!£$%^&()2", "1+2")]
        [TestCase("1+2", "1+2")]
        [TestCase("1+a+++++2", "1+2", Ignore = "Multiple operators cleanup, not implemented yet")]
        public void CleanExpression(string expression, string expectedResult)
        {
            //arrange

            //act
            var actual = _evalService.CleanExpression(expression);

            //assert
            Assert.IsTrue(actual == expectedResult);
        }

        [Test]
        [TestCase("1+2", "1", "+", "2")]
        public void SplitIntoNumbersAndOperators(string expression, string a, string op, string b)
        {
            //arrange

            //act
            var actual = _evalService.SplitToNumbersAndOperators(expression);

            //assert
            Assert.IsTrue(actual[0] == a);
            Assert.IsTrue(actual[1] == op);
            Assert.IsTrue(actual[2] == b);
        }

        [Test]
        public void ConvertToEvalModel()
        {
            //arrange
            var expressionThatStartsAndEndsWithOperators = new List<string> { "-", "*", "2", "/", "+" };
            var expectedNumber = 2;

            //act
            var actual = _evalService
                .ConvertToEvalModel(expressionThatStartsAndEndsWithOperators)
                .Single();

            //assert
            Assert.IsTrue(actual.Number == expectedNumber);
        }


        [Test]
        public void CleanEvalModels()
        {
            //arrange
            var modelWithMultipleOperatorsInRow = new List<EvalModel> {
                new EvalModel(2),
                new EvalModel("+"),
                new EvalModel("/"),
                new EvalModel("*"),
                new EvalModel(2),
            };

            var expected = new List<EvalModel> {
                new EvalModel(2),
                new EvalModel("+"),
                new EvalModel(2),
            };

            //act
            var actual = _evalService
                .CleanEvalModels(modelWithMultipleOperatorsInRow);

            //assert
            Assert.IsTrue(actual.Count == expected.Count);
            Assert.IsTrue(actual[0].Rawvalue == expected[0].Rawvalue);
            Assert.IsTrue(actual[1].Rawvalue == expected[1].Rawvalue);
            Assert.IsTrue(actual[2].Rawvalue == expected[2].Rawvalue);
        }



        [Test]
        [TestCase(1, "+", 1, 2)]
        [TestCase(2, "/", 2, 1)]
        public void Calculate(float a, string _operator, float b, float expectedResult)
        {
            //arrange
            var evalList = new List<EvalModel> {

                new EvalModel(a),
                new EvalModel(_operator),
                new EvalModel(b)
            };

            //act
            var actual = _evalService.ExecuteEvaluationModels(evalList);

            //assert
            Assert.IsTrue(actual.Number == expectedResult);
        }

        [Test]
        public void Calculate()
        {
            //arrange
            var evalList = new List<EvalModel> {

                new EvalModel(1),
                new EvalModel("-"),
                new EvalModel(2),
                new EvalModel("*"),
                new EvalModel(2),
            };

            //act
            var actual = _evalService.ExecuteEvaluationModels(evalList);

            //assert
            Assert.IsTrue(actual.Number == -3);
        }
    }
}
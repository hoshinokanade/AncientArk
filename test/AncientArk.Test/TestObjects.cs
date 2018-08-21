using AgileObjects.ReadableExpressions;
using AncientArk.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AncientArk.Test
{
    public class TestObject
    {
        public int Property { get; set; }
        public int Field;
        public TestEmbededObjectA EmbededObjectA { get; set; }
        
        public static TestObject GetTestObject()
        {
            return new TestObject
            {
                Property = 2018,
                EmbededObjectA = new TestEmbededObjectA
                {
                    EmbededPropertyA = 100,
                    EmbededObjectB = new TestEmbededObjectB
                    {
                        EmbededPropertyB = 200,
                    }
                }
            };
        }

        public static LambdaExpression GetAccessLambda<TIn, TOut>(Expression<Func<TIn, TOut>> expression)
        {
            var hierarchy = ExpressionHelper.GetMemberHierarchy(expression.Body);
            var result = ExpressionHelper.VisitMembers(hierarchy.Item2);
            var lambda = Expression.Lambda(result, expression.Parameters);
            return lambda;
        }

        public class Map: ProfileMap<TestObject>
        {
            public Map()
            {
                Map(x => x.Property);
                Map(x => x.EmbededObjectA.EmbededObjectB.EmbededPropertyB).As<short>();
            }
        }
    }

    public class TestEmbededObjectA
    {
        public int EmbededPropertyA { get; set; }
        public int EmbededFieldA;
        public TestEmbededObjectB EmbededObjectB { get; set; }
    }

    public class TestEmbededObjectB
    {
        public int EmbededPropertyB { get; set; }
        public int EmbededFieldB;
    }
}

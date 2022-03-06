using FastExpressionCompiler;
using SecurityLogin.Redis.Converters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SecurityLogin.Redis
{
    public class ExpressionRedisOperator: ComplexRedisOperator
    {
        private static readonly Dictionary<Type, ExpressionRedisOperator> defaultRedisOpCache = new Dictionary<Type, ExpressionRedisOperator>();

        public static ExpressionRedisOperator GetRedisOperator(Type type)
        {
            if (!defaultRedisOpCache.TryGetValue(type, out var @operator))
            {
                @operator = new ExpressionRedisOperator(type, SharedAnalysis);
                defaultRedisOpCache[type] = @operator;
                @operator.Build();
            }
            return @operator;
        }

        private static readonly MethodInfo TryGetValueMethod = typeof(IDictionary<string, RedisValue>).GetMethod("TryGetValue");
        private static readonly ConstructorInfo HashEntryConstructorInfo = typeof(HashEntry).GetConstructor(new Type[] { typeof(RedisValue), typeof(RedisValue) });
        private static readonly MethodInfo ConvertMethod = typeof(IRedisValueConverter).GetMethod("Convert");
        private static readonly MethodInfo ConvertBackMethod = typeof(IRedisValueConverter).GetMethod("ConvertBack");

        public ExpressionRedisOperator(Type target, IRedisColumnAnalysis columnAnalysis) : base(target, columnAnalysis)
        {
        }
        
        private Action<object, IDictionary<string, RedisValue>> writeMethod;
        private Func<object, HashEntry[]> asMethod;

        protected override void OnBuild()
        {
            writeMethod = AotCompileWrite();
            asMethod = AotCompileAs();
        }
        private IEnumerable<Expression> AotWriteAll(Expression instance, IEnumerable<IRedisColumn> columns, Expression map)
        {
            foreach (var column in columns)
            {
                var val = Expression.Variable(typeof(RedisValue));
                var value = Expression.Variable(typeof(object));
                var tryGet = Expression.Call(map, TryGetValueMethod, Expression.Constant(column.Path), val);
                Expression assignValue = Expression.Assign(value, Expression.Convert(val, typeof(object)));
                if (column.Converter != null)
                {
                    assignValue = Expression.Assign(value, Expression.Call(
                        Expression.Constant(column.Converter),
                        ConvertBackMethod,
                        val,
                        Expression.Constant(column)));
                }
                var doNothingCheck = Expression.IfThen(
                    Expression.NotEqual(value, Expression.Constant(RedisValueConverterConst.DoNothing)),
                    Expression.Call(instance, column.Property.SetMethod, Expression.Convert(value, column.Property.PropertyType)));
                var ifThen = Expression.IfThen(
                    Expression.Equal(tryGet, Expression.Constant(true)),
                    Expression.Block(assignValue, doNothingCheck));
                yield return Expression.Block(new ParameterExpression[] { val, value }, new Expression[] { ifThen });
                if (column.Nexts != null && column.Nexts.Count != 0)
                {
                    yield return Expression.Call(instance, column.Property.SetMethod, Expression.New(column.Property.PropertyType));
                    var nextInst = Expression.Call(instance, column.Property.GetMethod);
                    foreach (var item in AotWriteAll(nextInst, column.Nexts, map))
                    {
                        yield return item;
                    }
                }
            }
        }

        private Action<object,IDictionary<string, RedisValue>> AotCompileWrite()
        {
            var map = Expression.Parameter(typeof(IDictionary<string, RedisValue>));
            var inst = Expression.Parameter(typeof(object));
            var exps = AotWriteAll(Expression.Convert(inst,Target), RedisColumns, map);
            var allExps = new List<Expression>();
            allExps.AddRange(exps);
            allExps.Add(inst);
            var body = Expression.Block(allExps);
            return Expression.Lambda<Action<object, IDictionary<string, RedisValue>>>(body, inst,map)
                .CompileSys();
        }
        private Func<object, HashEntry[]> AotCompileAs()
        {
            var inst = Expression.Parameter(typeof(object));
            var exps = CompileGetEntities(Expression.Convert(inst, Target), RedisColumns);
            var listExp = Expression.NewArrayInit(typeof(HashEntry), exps);
            return Expression.Lambda<Func<object, HashEntry[]>>(listExp, inst)
                .CompileSys();
        }
        private IEnumerable<Expression> CompileGetEntities(Expression instance, IEnumerable<IRedisColumn> columns)
        {
            foreach (var column in columns)
            {
                var val = Expression.Call(instance, column.Property.GetMethod);
                if (column.Nexts != null && column.Nexts.Count != 0)
                {
                    foreach (var item in CompileGetEntities(val, column.Nexts))
                    {
                        yield return item;
                    }
                    continue;
                }
                if (column.Converter == null)
                {
                    yield return Expression.Constant(new HashEntry(column.Path, RedisValue.EmptyString));
                }
                else
                {
                    var call = Expression.Call(Expression.Constant(column.Converter), ConvertMethod,
                        instance,
                        Expression.Convert(Expression.Call(instance, column.Property.GetMethod), typeof(object)),
                        Expression.Constant(column));
                    yield return Expression.New(HashEntryConstructorInfo,
                        Expression.Constant(new RedisValue(column.Path)),
                        call);
                }
            }
        }

        public override void Write(ref object instance, HashEntry[] entries)
        {
            writeMethod(instance, entries.ToDictionary(x => x.Name.ToString(), x => x.Value));
        }

        public override HashEntry[] As(object value)
        {
            return asMethod(value);
        }
    }
}

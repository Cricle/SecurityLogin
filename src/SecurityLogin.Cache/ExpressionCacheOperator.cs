﻿using FastExpressionCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastExpressionCompiler.LightExpression;
using SecurityLogin.Cache.Converters;

namespace SecurityLogin.Cache
{
    public class ExpressionCacheOperator: ComplexCacheOperator
    {
        private static readonly Dictionary<Type, ExpressionCacheOperator> defaultRedisOpCache = new Dictionary<Type, ExpressionCacheOperator>();

        public static ExpressionCacheOperator GetRedisOperator(Type type)
        {
            if (!defaultRedisOpCache.TryGetValue(type, out var @operator))
            {
                @operator = new ExpressionCacheOperator(type, SharedAnalysis);
                defaultRedisOpCache[type] = @operator;
                @operator.Build();
            }
            return @operator;
        }

        private static readonly MethodInfo TryGetValueMethod = typeof(IDictionary<string, BufferValue>).GetMethod("TryGetValue");
        private static readonly ConstructorInfo HashEntryConstructorInfo = typeof(BufferEntry).GetConstructor(new Type[] { typeof(BufferValue), typeof(BufferValue) });
        private static readonly MethodInfo ConvertMethod = typeof(ICacheValueConverter).GetMethod("Convert");
        private static readonly MethodInfo ConvertBackMethod = typeof(ICacheValueConverter).GetMethod("ConvertBack");

        public ExpressionCacheOperator(Type target, ICacheColumnAnalysis columnAnalysis) : base(target, columnAnalysis)
        {
        }
        
        private Action<object, IDictionary<string, BufferValue>> writeMethod;
        private Func<object, BufferEntry[]> asMethod;
        private Func<IDictionary<string, BufferValue>,object> writeWithObjectMethod;

        protected override void OnBuild()
        {
            writeMethod = AotCompileWrite();
            asMethod = AotCompileAs();
            writeWithObjectMethod = AotCompileWithInstanceWrite();
        }
        private IEnumerable<Expression> AotWriteAll(Expression instance, IEnumerable<ICacheColumn> columns, Expression map)
        {
            foreach (var column in columns)
            {
                var val = Expression.Variable(typeof(BufferValue));
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
                    Expression.NotEqual(value, Expression.Constant(CacheValueConverterConst.DoNothing)),
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
        private Func<IDictionary<string, BufferValue>,object> AotCompileWithInstanceWrite()
        {
            var map = Expression.Parameter(typeof(IDictionary<string, BufferValue>));
            var inst = Expression.Variable(Target);
            var assign = Expression.Assign(inst, Expression.New(Target));
            var exps = AotWriteAll(inst, RedisColumns, map);
            var allExps = new List<Expression> { assign };
            allExps.AddRange(exps);
            allExps.Add(Expression.Convert(inst, Target));
            var body = Expression.Block(new ParameterExpression[] {inst},allExps);
            return Expression.Lambda<Func<IDictionary<string, BufferValue>, object>>(body, map)
                .CompileSys();
        }
        private Action<object,IDictionary<string, BufferValue>> AotCompileWrite()
        {
            var map = Expression.Parameter(typeof(IDictionary<string, BufferValue>));
            var inst = Expression.Parameter(typeof(object));
            var exps = AotWriteAll(Expression.Convert(inst,Target), RedisColumns, map);
            var allExps = new List<Expression>();
            allExps.AddRange(exps);
            allExps.Add(inst);
            var body = Expression.Block(allExps);
            return Expression.Lambda<Action<object, IDictionary<string, BufferValue>>>(body, inst,map)
                .CompileSys();
        }
        private Func<object, BufferEntry[]> AotCompileAs()
        {
            var inst = Expression.Parameter(typeof(object));
            var exps = CompileGetEntities(Expression.Convert(inst, Target), RedisColumns);
            var listExp = Expression.NewArrayInit(typeof(BufferEntry), exps);
            return Expression.Lambda<Func<object, BufferEntry[]>>(listExp, inst)
                .CompileFast();
        }
        private IEnumerable<Expression> CompileGetEntities(Expression instance, IEnumerable<ICacheColumn> columns)
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
                    yield return Expression.Constant(new BufferEntry(column.Path, BufferValue.EmptyString));
                }
                else
                {
                    var call = Expression.Call(Expression.Constant(column.Converter), ConvertMethod,
                        instance,
                        Expression.Convert(Expression.Call(instance, column.Property.GetMethod), typeof(object)),
                        Expression.Constant(column));
                    yield return Expression.New(HashEntryConstructorInfo,
                        Expression.Constant(new BufferValue(column.Path)),
                        call);
                }
            }
        }
        public object Write(BufferEntry[] entries)
        {
            return writeWithObjectMethod(ToMap(entries));
        }
        private static Dictionary<string, BufferValue> ToMap(BufferEntry[] entries)
        {
            var len = entries.Length;
            var d = new Dictionary<string, BufferValue>(len);
            for (int i = 0; i < len; i++)
            {
                var item = entries[i];
                d.Add(item.Name.ToString(), item.Value);
            }
            return d;
        }
        public override void Write(ref object instance, BufferEntry[] entries)
        {
            var map = ToMap(entries);
            writeMethod(instance, map);
        }

        public override BufferEntry[] As(object value)
        {
            return asMethod(value);
        }
    }
}

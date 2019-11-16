using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using MathLiberator.Syntax;
using MathLiberator.Syntax.Expressions;
using static System.Reflection.Emit.OpCodes;

namespace MathLiberator.Analysis
{
    static class ILGeneratorCache
    {
        public static AssemblyBuilder Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("MathLiberator Dynamic Code"), AssemblyBuilderAccess.Run);
        public static ModuleBuilder Module = Assembly.DefineDynamicModule("MathLiberator Dynamic Code");
        public static Int32 Count = 0;

        public static int IncrementCount()
        {
            return Interlocked.Increment(ref Count);
        }
    }

    public struct ILGenerator<TAdapter, TNumber>
        where TAdapter : INumericAdapter<TNumber>
        where TNumber : unmanaged
    {
        readonly TAdapter adapter;
        Dictionary<String, (FieldBuilder Field, TNumber? ConstantValue)> symbols;

        MethodBuilder step;
        ILGenerator il;

        ConstructorBuilder constructor;
        ILGenerator constructorIL;

        TypeBuilder stepper;
        
        public ILGenerator(TAdapter adapter)
        {
            this.adapter = adapter;
            symbols = new Dictionary<String, (FieldBuilder Field, TNumber? ConstantValue)>();
            step = default;
            il = default;
            constructor = default;
            constructorIL = default;
            stepper = default;
        }
        
        public Type ReturnEnumerator(CompilationUnit<TNumber> compilation)
        {
            stepper = ILGeneratorCache.Module.DefineType($"MathLiberator Submission {ILGeneratorCache.IncrementCount()}",
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoLayout, typeof(ValueType));

            step = stepper.DefineMethod("Step", MethodAttributes.Public | MethodAttributes.HideBySig, typeof(void), null);
            il = step.GetILGenerator();
            
            il.Emit(Ret);

            constructor = stepper.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            constructorIL = constructor.GetILGenerator();

            foreach (var expr in compilation.Statements)
            {
                AnalyzeExpression(expr);
            }

            return stepper.CreateType();
        }
        
        void AnalyzeExpression(ExpressionSyntax<TNumber> expression)
        {
            switch (expression)
            {
                case BinaryExpression<TNumber> binary:
                    AnalyzeBinaryExpression(binary);
                    break;
                case ModelExpression<TNumber> model:
                    AnalyzeModelExpression(model);
                    break;
            }
        }

        void AnalyzeModelExpression(ModelExpression<TNumber> model)
        {
            
        }
        
        void AnalyzeBinaryExpression(BinaryExpression<TNumber> binary)
        {
            switch (binary.Kind)
            {
                case SyntaxKind.Constant:
                {
                    var expr = (IdentifierExpression<TNumber>) binary.Left;
                    var builder = stepper.DefineField(expr.ToString(), typeof(TNumber), FieldAttributes.Public | FieldAttributes.Literal);
                    var value = ReadConstant(binary.Right);
                    builder.SetConstant(value);
                    symbols.Add(expr.ToString(), (builder, value));
                    break;
                }
                case SyntaxKind.State:
                {
                    var expr = (IdentifierExpression<TNumber>) binary.Left;
                    var builder = stepper.DefineField(expr.ToString(), typeof(TNumber), FieldAttributes.Public);
                    var value = ReadConstant(binary.Right);
                    
                    constructorIL.Emit(Ldarg_0);
                    constructorIL.EmitNumber(Ldc_R8, value);
                    constructorIL.Emit(Stfld, builder);
                    
                    symbols.Add(expr.ToString(), (builder, value));
                    break;
                }
            }
        }
        
        TNumber ReadConstant(ExpressionSyntax<TNumber> expr)
        {
            switch (expr)
            {
                case ConstantExpression<TNumber> constant:
                    return constant.Value;
                case UnaryExpression<TNumber> unary:
                    return unary.Operator switch
                    {
                        SyntaxKind.Minus => adapter.Minus(ReadConstant(unary.Operand)),
                        // unary plus does nothing
                        SyntaxKind.Plus => ReadConstant(unary.Operand),
                        SyntaxKind.Parenthesized => ReadConstant(unary.Operand),
                        _ => default
                    };
                
                case BinaryExpression<TNumber> binary:
                    var left = ReadConstant(binary.Left);
                    var right = ReadConstant(binary.Right);

                    return binary.Operator switch
                    {
                        SyntaxKind.Plus => adapter.Add(left, right),
                        SyntaxKind.Minus => adapter.Subtract(left, right),
                        SyntaxKind.Asterisk => adapter.Multiply(left, right),
                        SyntaxKind.Slash => adapter.Divide(left, right),
                        _ => default
                    };
                
                case IdentifierExpression<TNumber> identifier:
                    return symbols[identifier.ToString()].ConstantValue.Value;
                default:
                    return default;
            }
        }
    }
}
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
        public static Int32 Count;

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

        MethodBuilder start;
        ILGenerator startIL;

        TypeBuilder stepper;
        
        public ILGenerator(TAdapter adapter)
        {
            this.adapter = adapter;
            symbols = new Dictionary<String, (FieldBuilder Field, TNumber? ConstantValue)>();
            step = default;
            il = default;
            start = default;
            startIL = default;
            stepper = default;
        }
        
        public Type ReturnEnumerator(CompilationUnit<TNumber> compilation)
        {
            stepper = ILGeneratorCache.Module.DefineType($"MathLiberator Submission {ILGeneratorCache.IncrementCount()}",
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoLayout, typeof(ValueType), new[] { typeof(IStepper) });

            step = stepper.DefineMethod(nameof(IStepper.Step),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                typeof(Boolean), null);
            
            stepper.DefineMethodOverride(step, typeof(IStepper).GetMethod(nameof(IStepper.Step)));
            il = step.GetILGenerator();

            start = stepper.DefineMethod(nameof(IStepper.Start),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, null,
                null);
            
            stepper.DefineMethodOverride(start, typeof(IStepper).GetMethod(nameof(IStepper.Start)));
            startIL = start.GetILGenerator();
            
            foreach (var expr in compilation.Statements)
            {
                AnalyzeExpression(expr);
            }
            
            startIL.Emit(Ret);

            return stepper.CreateType();
        }
        
        void AnalyzeExpression(ExpressionSyntax<TNumber> expression)
        {
            switch (expression)
            {
                case BinaryExpression<TNumber> binary:
                    EmitBinaryExpression(binary);
                    break;
                case ModelExpression<TNumber> model:
                    AnalyzeModelExpression(model);
                    break;
            }
        }

        void AnalyzeModelExpression(ModelExpression<TNumber> model)
        {
            // Start
            var t = stepper.DefineField("t", typeof(TNumber), FieldAttributes.Public);
            symbols.Add("t", (t, null));
            startIL.Emit(Ldarg_0);
            startIL.EmitNumber(Ldc_R8, ReadConstant(model.Start));
            startIL.Emit(Stfld, t);

            // Body
            foreach (var statement in model.ModelStatements)
            {
                EmitExpression(statement);
            }
            
            // Step
            var field = symbols["t"].Field;
            il.Emit(Ldarg_0);
            il.Emit(Ldarg_0);
            il.Emit(Ldfld, field); 
            il.EmitNumber(Ldc_R8, 1.AsTNumber<TNumber>());
            il.Emit(Add);
            il.Emit(Stfld, field);
            
            // Condition 
            EmitExpression(model.Condition.Left);
            EmitExpression(model.Condition.Right);

            switch (model.Condition.Operator)
            {
                case SyntaxKind.GreaterThan:
                    il.Emit(Cgt);
                    break;
                case SyntaxKind.LessThan:
                    il.Emit(Clt);
                    break;
                case SyntaxKind.GreaterThanEquals:
                    il.Emit(Clt);
                    il.Emit(Ldc_I4_0);
                    il.Emit(Ceq);
                    break;
                case SyntaxKind.LessThanEquals:
                    il.Emit(Cgt);
                    il.Emit(Ldc_I4_0);
                    il.Emit(Ceq);
                    break;
            }
            
            il.Emit(Ret);
        }

        void EmitExpression(ExpressionSyntax<TNumber> value)
        {
            switch (value)
            {
                case ConstantExpression<TNumber> constant:
                    il.EmitNumber(Ldc_R8, constant.Value);
                    break;
                case IdentifierExpression<TNumber> id:
                    var symbol = symbols[id.ToString()];
                    if (symbol.ConstantValue.HasValue)
                    {
                        // do not use ldfld; instead copy paste constant value
                        il.EmitNumber(Ldc_R8, symbol.ConstantValue.GetValueOrDefault());
                    }
                    else
                    {
                        il.Emit(Ldarg_0);
                        il.Emit(Ldfld, symbols[id.ToString()].Field);
                    }
                    break;
                case UnaryExpression<TNumber> unary:
                    EmitUnaryExpression(unary);
                    break;
                case BinaryExpression<TNumber> binary:
                    EmitBinaryExpression(binary);
                    break;
            }
        }

        void EmitUnaryExpression(UnaryExpression<TNumber> unary)
        {
            switch (unary.Operator)
            {
                case SyntaxKind.Minus:
                    EmitExpression(unary.Operand);
                    il.Emit(Neg);
                    break;
                case SyntaxKind.Plus:
                case SyntaxKind.Parenthesized:
                    EmitExpression(unary.Operand);
                    break;
            }
        }

        void EmitBinaryExpression(BinaryExpression<TNumber> binary)
        {
            switch (binary.Kind)
            {
                case SyntaxKind.Constant:
                {
                    var expr = (IdentifierExpression<TNumber>) binary.Left;
                    var builder = stepper.DefineField(expr.ToString(), typeof(TNumber),
                        FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal);
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
                    
                    startIL.Emit(Ldarg_0);
                    startIL.EmitNumber(Ldc_R8, value);
                    startIL.Emit(Stfld, builder);
                    
                    symbols.Add(expr.ToString(), (builder, null));
                    break;
                }
                // TODO: Vectorize
                case SyntaxKind.Arithmetic:
                {
                    EmitExpression(binary.Left);
                    EmitExpression(binary.Right);
                    
                    il.Emit(binary.Operator switch
                    {
                        SyntaxKind.Plus => Add,
                        SyntaxKind.Minus => Sub,
                        SyntaxKind.Asterisk => Mul,
                        SyntaxKind.Slash => Div
                    });
                    break;
                }
                case SyntaxKind.Mutation:
                {
                    var left = (IdentifierExpression<TNumber>) binary.Left;
                    var field = symbols[left.Identifier.ToString()].Field;

                    il.Emit(Ldarg_0);
                    il.Emit(Ldarg_0);
                    il.Emit(Ldfld, field);
                    EmitExpression(binary.Right);

                    il.Emit(binary.Operator switch
                    {
                        SyntaxKind.PlusEquals => Add,
                        SyntaxKind.MinusEquals => Sub,
                        SyntaxKind.AsteriskEquals => Mul,
                        SyntaxKind.SlashEquals => Div,
                    });
                    
                    il.Emit(Stfld, field);
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
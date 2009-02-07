#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Linq.Expressions;

namespace SampleSolrApp.Helpers {
    /// <summary>
    /// Based on http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
    /// </summary>
    public class ExpressionVisitorV {
        public virtual void Visit(Expression exp) {
            if (exp == null)
                return;

            switch (exp.NodeType) {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    Visit((UnaryExpression) exp);
                    break;

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    Visit((BinaryExpression) exp);
                    break;

                case ExpressionType.TypeIs:
                    Visit((TypeBinaryExpression) exp);
                    break;

                case ExpressionType.Conditional:
                    Visit((ConditionalExpression) exp);
                    break;

                case ExpressionType.Constant:
                    Visit((ConstantExpression) exp);
                    break;

                case ExpressionType.Parameter:
                    Visit((ParameterExpression) exp);
                    break;

                case ExpressionType.MemberAccess:
                    Visit((MemberExpression) exp);
                    break;

                case ExpressionType.Call:
                    Visit((MethodCallExpression) exp);
                    break;

                case ExpressionType.Lambda:
                    Visit((LambdaExpression) exp);
                    break;

                case ExpressionType.New:
                    Visit((NewExpression) exp);
                    break;

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    Visit((NewArrayExpression) exp);
                    break;

                case ExpressionType.Invoke:
                    Visit((InvocationExpression) exp);
                    break;

                case ExpressionType.MemberInit:
                    Visit((MemberInitExpression) exp);
                    break;

                case ExpressionType.ListInit:
                    Visit((ListInitExpression) exp);
                    break;

                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        public Action<UnaryExpression> VisitUnary { get; set; }
        public virtual void Visit(UnaryExpression exp) {
            if (VisitUnary != null)
                VisitUnary(exp);
        }

        public Action<ConditionalExpression> VisitConditional { get; set; }
        public virtual void Visit(ConditionalExpression expression) {
            if (VisitConditional != null)
                VisitConditional(expression);
        }

        public Action<TypeBinaryExpression> VisitTypeBinary { get; set; }
        public virtual void Visit(TypeBinaryExpression expression) {
            if (VisitTypeBinary != null)
                VisitTypeBinary(expression);
        }

        public Action<ConstantExpression> VisitConstant { get; set; }
        public virtual void Visit(ConstantExpression expression) {
            if (VisitConstant != null)
                VisitConstant(expression);
        }

        public Action<ParameterExpression> VisitParameter { get; set; }
        public virtual void Visit(ParameterExpression expression) {
            if (VisitParameter != null)
                VisitParameter(expression);
        }

        public Action<MemberExpression> VisitMember { get; set; }
        public virtual void Visit(MemberExpression expression) {
            if (VisitMember != null)
                VisitMember(expression);
        }

        public Action<MethodCallExpression> VisitMethodCall { get; set; }
        public virtual void Visit(MethodCallExpression expression) {
            if (VisitMethodCall != null)
                VisitMethodCall(expression);
        }

        public Action<LambdaExpression> VisitLambda { get; set; }
        public virtual void Visit(LambdaExpression expression) {
            if (VisitLambda != null)
                VisitLambda(expression);
        }

        public Action<NewExpression> VisitNew { get; set; }
        public virtual void Visit(NewExpression expression) {
            if (VisitNew != null)
                VisitNew(expression);
        }

        public Action<NewArrayExpression> VisitNewArray { get; set; }
        public virtual void Visit(NewArrayExpression expression) {
            if (VisitNewArray != null)
                VisitNewArray(expression);
        }

        public Action<InvocationExpression> VisitInvocation { get; set; }
        public virtual void Visit(InvocationExpression expression) {
            if (VisitInvocation != null)
                VisitInvocation(expression);
        }

        public Action<MemberInitExpression> VisitMemberInit { get; set; }
        public virtual void Visit(MemberInitExpression expression) {
            if (VisitMemberInit != null)
                VisitMemberInit(expression);
        }

        public Action<ListInitExpression> VisitListInit { get; set; }
        public virtual void Visit(ListInitExpression expression) {
            if (VisitListInit != null)
                VisitListInit(expression);
        }

        public Action<BinaryExpression> VisitBinary { get; set; }
        public virtual void Visit(BinaryExpression expression) {
            if (VisitBinary != null)
                VisitBinary(expression);
        }
    }
}
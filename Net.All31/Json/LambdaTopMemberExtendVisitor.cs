using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Net.Json
{
    class LambdaTopMemberExtendVisitor : ExpressionVisitor
    {
        private bool IsTopMember = true;
        private Func<Expression, Expression> ExtendFn;
        public LambdaTopMemberExtendVisitor(Func<Expression,Expression> extendFn)
        {
            this.ExtendFn = extendFn;
        }

     
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            
            var newExpression = Visit(node.Body);
            return Expression.Lambda(newExpression, node.Parameters);
        }

        public override Expression Visit(Expression node)
        {
            if (!IsTopMember) return base.Visit(node);
            this.IsTopMember = false;
            var result = this.ExtendFn(node);
            return result;
        }

    }
}

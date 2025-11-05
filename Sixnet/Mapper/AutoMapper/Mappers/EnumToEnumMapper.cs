// "Company © 2025. All rights reserved."

using System.Reflection;

using AutoMapper.Execution;

namespace AutoMapper.Internal.Mappers;
public sealed class EnumToEnumMapper : IObjectMapper
{
    private static readonly MethodInfo TryParseMethod = typeof(Enum).StaticGenericMethod("TryParse", parametersCount: 3);
    public bool IsMatch(TypePair context) => context.IsEnumToEnum();
    public Expression MapExpression(IGlobalConfiguration configuration, ProfileMap profileMap,
        MemberMap memberMap, Expression sourceExpression, Expression destExpression)
    {
        var destinationType = destExpression.Type;
        var sourceToString = Expression.Call(sourceExpression, ExpressionBuilder.ObjectToString);
        var result = Expression.Variable(destinationType, "destinationEnumValue");
        var ignoreCase = ExpressionBuilder.True;
        var tryParse = Expression.Call(TryParseMethod.MakeGenericMethod(destinationType), sourceToString, ignoreCase, result);
        var (variables, statements) = configuration.Scratchpad();
        variables.Add(result);
        statements.Add(Expression.Condition(tryParse, result, Expression.Convert(sourceExpression, destinationType)));
        return Expression.Block(variables, statements);
    }
}
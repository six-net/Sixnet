// "Company © 2025. All rights reserved."

using System.ComponentModel;

using AutoMapper.Internal;

namespace AutoMapper.QueryableExtensions.Impl;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class StringProjectionMapper : IProjectionMapper
{
    public bool IsMatch(TypePair context) => context.DestinationType == typeof(string);
    public Expression Project(IGlobalConfiguration configuration, in ProjectionRequest request, Expression resolvedSource, LetPropertyMaps letPropertyMaps)
        => Call(resolvedSource, ObjectToString);
}
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Tests
{
    [TypeMatcher]
    public sealed class IsAnyToken : ITypeMatcher, IEquatable<IsAnyToken>
    {
        public bool Matches(Type typeArgument) => true;
        public bool Equals(IsAnyToken? other) => throw new NotImplementedException();
    }
}

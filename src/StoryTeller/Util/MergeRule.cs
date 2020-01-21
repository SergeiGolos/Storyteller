using System.Collections.Generic;

namespace StoryTeller.Util
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public abstract class MergeRule<TType>
        where TType : IDecoratable<TType>
    {
        public abstract IEnumerable<IDecorator<TType>> Apply(
            IEnumerable<IDecorator<TType>> list, 
            IDecorator<TType> decorator);
    }
}

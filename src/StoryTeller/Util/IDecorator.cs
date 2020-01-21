namespace StoryTeller.Util
{
    /// <summary>
    /// Defines the interface for objects that decorate <see cref="IDecoratable{TType}"/> classes.
    /// </summary>
    /// <typeparam name="TType">The type of the object being decorated.</typeparam>
    public interface IDecorator<TType> 
        where TType : IDecoratable<TType>
    {
        string Type { get; }
        void Apply(TType item);
        MergeRule<TType> Rule { get; }
    }
}

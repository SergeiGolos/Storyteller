namespace StoryTeller.Util
{
    /// <summary>
    /// Defines the interface for object that can be modified by the <see cref="IDecorator{TType}"/> classes.
    /// </summary>
    /// <typeparam name="TType">Describes the object being decorated.</typeparam>
    public interface IDecoratable<TType>
        where TType : IDecoratable<TType>
    {
        TType Apply(IDecorator<TType> decorator);
    }
}

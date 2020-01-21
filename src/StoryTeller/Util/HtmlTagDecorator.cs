namespace StoryTeller.Util
{
    /// <summary>
    /// Abstract implementation for decorators for <see cref="HtmlTag" /> element. 
    /// </summary>
    /// <typeparam name="TRule">The merge rule for this rule on a element stack.</typeparam>
    public abstract class HtmlTagDecorator<TRule> : HtmlTagDecorator
        where TRule : MergeRule<HtmlTag>, new()
    {
        protected HtmlTagDecorator(string type)
        {
            this.Type = type;
            this.Rule = new TRule();
        }
    }
    
    /// <summary>
    /// Abstract implementation for decorators for <see cref="HtmlTag" /> element. 
    /// </summary>
    public abstract class HtmlTagDecorator : IDecorator<HtmlTag>
    {
        
        public string Type { get; protected set; }
        
        public MergeRule<HtmlTag> Rule { get; protected set; }
        
        public abstract void Apply(HtmlTag tag);
    }
}

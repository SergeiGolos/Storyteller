using System;
using StoryTeller.Util;


namespace StorytellerDocGen.Samples
{
    public class SampleTag : HtmlTag
    {
        public SampleTag(Sample sample) : base("pre")
        {
            Add("code").AddClass("language-" + sample.Language).Text(Environment.NewLine + sample.Text);
        }
    }
}
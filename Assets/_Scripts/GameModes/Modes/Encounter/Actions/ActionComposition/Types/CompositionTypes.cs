using System.Collections.Generic;


namespace MAGE.GameModes.Encounter
{
    interface IComposer
    {
        CompositionElement Compose();
    }

    class TimelineElement : ITimelineEvent
    {
        public int StartPointOffset { get; set; }
        public int DurationFrames { get; set; }

        public CompositionElement CompositionElement;
        public void Trigger()
        {
            CompositionElement.Trigger();
        }
    }

    class CompositionLink<T> 
    {
        public T Child;
        public AllignmentPosition ParentAllignment;
        public AllignmentPosition ChildAllignment;

        public CompositionLink(T child)
        {
            Child = child;
            ParentAllignment = AllignmentPosition.Start;
            ChildAllignment = AllignmentPosition.Start;
        }

        public CompositionLink(AllignmentPosition parentAllignment, AllignmentPosition childAllignment, T child)
        {
            Child = child;
            ParentAllignment = parentAllignment;
            ChildAllignment = childAllignment;
        }
    }

    abstract class CompositionNode
    {
        public CompositionElement mComposition = null;
        public List<CompositionLink<CompositionNode>> ChildComposers = new List<CompositionLink<CompositionNode>>();

        protected abstract CompositionElement OnCompose();
        public virtual List<TimelineElement> Compose()
        {
            mComposition = OnCompose();

            List<TimelineElement> timeline = ConvertCompositionElementToTimeline(mComposition);
            timeline.AddRange(ComposeChildren());

            return timeline;
        }

        protected static List<TimelineElement> ConvertCompositionElementToTimeline(CompositionElement compositionElement)
        {
            List<TimelineElement> timeline = new List<TimelineElement>();

            TimelineElement rootElement = new TimelineElement();
            rootElement.CompositionElement = compositionElement;
            rootElement.DurationFrames = compositionElement.NumFrames;
            rootElement.StartPointOffset = 0;
            timeline.Add(rootElement);

            foreach (CompositionLink<CompositionElement> childLink in compositionElement.Children)
            {
                TimelineElement childElement = new TimelineElement();
                childElement.CompositionElement = childLink.Child;
                childElement.DurationFrames = childLink.Child.NumFrames;
                childElement.StartPointOffset = SyncPoint.GetStartPointOffset(rootElement.CompositionElement, childLink.ParentAllignment, childLink.Child, childLink.ChildAllignment, 0);

                timeline.Add(childElement);
            }

            return timeline;
        }


        protected virtual List<TimelineElement> ComposeChildren()
        {
            List<TimelineElement> timeline = new List<TimelineElement>();

            foreach (CompositionLink<CompositionNode> childLink in ChildComposers)
            {
                List<TimelineElement> childTimeline = childLink.Child.Compose();
                int childOffset = SyncPoint.GetStartPointOffset(mComposition, childLink.ParentAllignment, childLink.Child.mComposition, childLink.ChildAllignment, 0);

                foreach (TimelineElement timelineElement in childTimeline)
                {
                    timelineElement.StartPointOffset += childOffset;
                }

                timeline.AddRange(childTimeline);
            }

            return timeline;
        }
    }

    class EmptyNode : CompositionNode
    {
        protected override CompositionElement OnCompose()
        {
            return new EmptyElement();
        }
    }
}
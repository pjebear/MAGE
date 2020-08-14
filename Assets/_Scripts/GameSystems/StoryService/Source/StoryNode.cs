using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Story.Internal
{
    class StoryNode
    {
        public string Name = "NAME";
        public string Description = "DESCRIPTION";
        public StoryCondition Requirement;
        public List<StoryMutatorParams> ChangesOnActivation = new List<StoryMutatorParams>();
        public List<StoryMutatorParams> ChangesOnCompletion = new List<StoryMutatorParams>();
    }
}


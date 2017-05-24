using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBehaviourTree
{
    /// <summary>
    /// Decorator node that repeats the execution of a child node until it fails
    /// </summary>
    public class RepeaterNode : IParentBehaviourTreeNode
    {
        /// <summary>
        /// Name of the node.
        /// </summary>
        private string name;

        /// <summary>
        /// The child to be repeated until failure
        /// </summary>
        private IBehaviourTreeNode childNode;

        public RepeaterNode(string name)
        {
            this.name = name;
        }

        public BehaviourTreeStatus Tick(TimeData time)
        {
            if (childNode == null)
            {
                throw new ApplicationException("RepeaterNode must have a child node!");
            }

            var result = childNode.Tick(time);
            while (result == BehaviourTreeStatus.Success)
            {
                result = childNode.Tick(time);
            }

            return result;
        }

        /// <summary>
        /// Add a child to the parent node.
        /// </summary>
        public void AddChild(IBehaviourTreeNode child)
        {
            if (this.childNode != null)
            {
                throw new ApplicationException("Can't add more than a single child to RepeaterNode!");
            }

            this.childNode = child;
        }
    }
}

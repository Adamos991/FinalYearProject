using System.Collections.Generic;
namespace FixedSizeQueueNamespace
{

    public class FixedSizeQueue
    {
        private Queue<int> queue = new Queue<int>();
        private int size;

        public FixedSizeQueue(int size)
        {
            this.size = size;
        }

        public void Enqueue(int value)
        {
            if (queue.Count == size)
            {
                queue.Dequeue();
            }

            queue.Enqueue(value);
        }

        public int[] ToArray()
        {
            return queue.ToArray();
        }
    }
}
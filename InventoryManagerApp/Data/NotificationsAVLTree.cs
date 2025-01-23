namespace InventoryManagerApp.Data
{
    // Define a node in the AVL Tree
    public class TreeNode
    {
        public Item Item { get; set; }
        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }
        public int Height { get; set; }

        public TreeNode(Item item)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Height = 1;
        }
    }

    // An AVL Tree for managing low stock notifications
    public class NotificationsAVLTree
    {
            private readonly object _lock = new();
            public TreeNode? Root { get; private set; }

            //  Insert an item to the tree
            // PARAM = item : The item to insert
            public void Insert(Item item)
            {
                if (item == null) throw new ArgumentNullException(nameof(item));
                lock (_lock)
                {
                    Root = Insert(Root, item);
                }
            }
            
            // Recursively insert items into the tree
            private TreeNode Insert(TreeNode? node, Item item)
            {
                if (node == null) return new TreeNode(item);

                if (item.Quantity <= node.Item.Quantity)
                {
                    node.Left = Insert(node.Left, item);
                }
                else
                {
                node.Right = Insert(node.Right, item);
                }

                // Update height and rebalance tree
                UpdateHeight(node);
                return Rebalance(node);
            }

            // Remove an item from the tree
            // PARAM = item : The item to remove
            public void Remove(Item item)
            {
                if (item == null) throw new ArgumentNullException(nameof(item));
                lock (_lock)
                {
                    Root = Remove(Root, item);
                }
            }

            // Recursively remove items from the tree
            private TreeNode? Remove(TreeNode? node, Item item)
            {
                if (node == null) return null;

                if (item.Quantity < node.Item.Quantity)
                {
                    node.Left = Remove(node.Left, item);
                }
                else if (item.Quantity > node.Item.Quantity)
                {
                    node.Right = Remove(node.Right, item);
                }
                else
                {
                     // A node has been found to delete
                     if (node.Left == null) return node.Right;
                     if (node.Right == null) return node.Left;

                    // Replace with smallest node in the right subtree
                    TreeNode minLargerNode = GetMin(node.Right);
                    node.Item = minLargerNode.Item;
                    node.Right = Remove(node.Right, minLargerNode.Item);
                }

                UpdateHeight(node);
                return Rebalance(node);
            }

            // Get all items with a quantity less than or equal to the threshold
            public List<Item> GetLowStockItems(int lowStockThreshold)
            {
                lock (_lock)
                {
                    var result = new List<Item>();
                    TraverseInOrder(Root, lowStockThreshold, result);
                    return result;
                }
            }

            // Traverse the tree in order to collate low stock item list
            private void TraverseInOrder(TreeNode? node, int threshold, List<Item> result)
            {
                if (node == null) return;
                
                TraverseInOrder(node.Left, threshold, result);
                
                if (node.Item.Quantity <= threshold)
                {
                    result.Add(node.Item);
                }
                else
                {
                    return;
                }

                TraverseInOrder(node.Right, threshold, result);
            }
            
            // Get node with smallest value
            private TreeNode GetMin(TreeNode node)
            {
                while (node.Left != null)
                {
                    node = node.Left;
                }
                return node;
            }

             // Updates the height of the given tree node
             private void UpdateHeight(TreeNode node)
             {
                 node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
             }

             // Gets the height of the given tree node or returns 0 if null
             private int GetHeight(TreeNode? node)
             {
                 return node?.Height ?? 0;
             }

             // Gets the balance factor of the AVL
             private int GetBalanceFactor(TreeNode node)
             {
                 return GetHeight(node.Left) - GetHeight(node.Right);
             }

             // Rebalances the AVL tree
             private TreeNode Rebalance(TreeNode node)
             {
                 int balanceFactor = GetBalanceFactor(node);

                 // Left-heavy
                 if (balanceFactor > 1)
                 {
                     // Left-Right case
                     if (GetBalanceFactor(node.Left!) < 0)
                     {
                         node.Left = RotateLeft(node.Left!);
                     }
                     // Left-Left case
                     return RotateRight(node);
                 }

                 // Right-heavy
                 if (balanceFactor < -1)
                 {
                     // Right-Left case
                     if (GetBalanceFactor(node.Right!) > 0)
                     {
                         node.Right = RotateRight(node.Right!);
                     }
                     // Right-Right case
                     return RotateLeft(node);
                 }

                 // Return if no rebalancing needed
                 return node;
             }

             // Rotates the tree node right
             private TreeNode RotateRight(TreeNode y)
             {
                 TreeNode x = y.Left!;
                 TreeNode? T2 = x.Right;

                 // Perform rotation
                 x.Right = y;
                 y.Left = T2;

                 // Update heights
                 UpdateHeight(y);
                 UpdateHeight(x);

                 // New root
                 return x;
             }

             // Rotates the tree node left
             private TreeNode RotateLeft(TreeNode x)
             {
                 TreeNode y = x.Right!;
                 TreeNode? T2 = y.Left;

                 // Perform rotation
                 y.Left = x;
                 x.Right = T2;

                 // Update heights
                 UpdateHeight(x);
                 UpdateHeight(y);
                
                 // New root
                 return y;
             }
    }
}

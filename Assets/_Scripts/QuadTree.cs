using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts {
    public class QuadTree {
        public Bounds bounds;
        public int minNodeSize;

        // Quadrant children
        public QuadTree ne;
        public QuadTree nw;
        public QuadTree sw;
        public QuadTree se;

        // Constructors
        public QuadTree(Vector2Int min, Vector2Int max, int minNodeSize) {
            bounds = new Bounds(min, max);
            this.minNodeSize = minNodeSize;
        }

        public QuadTree(Vector2Int center, int halfWidth, int minNodeSize) {
            bounds = new Bounds(center, halfWidth);
            this.minNodeSize = minNodeSize;
        }

        /// <summary>
        /// Inserts a position into the tree
        /// </summary>
        /// <param name="position">Position to insert</param>
        public void Insert(Vector2 position) {
            Insert(this, position);
        }

        /// <summary>
        /// Recursively generates nodes for a given position, until the node is too small
        /// </summary>
        /// <param name="node">Root node</param>
        /// <param name="position">Position to insert</param>
        private void Insert(QuadTree node, Vector2 position) {
            var distToChild = Vector2.Distance(node.bounds.center, position);
            
            // If object is within bounds and the nodes are big enough
            if (distToChild < node.bounds.width && node.bounds.width > minNodeSize) {
                // Create children
                CreateChildren(this);
                
                // Recurse
                Insert(ne, position);
                Insert(nw, position);
                Insert(sw, position);
                Insert(se, position);
            }
        }

        /// <summary>
        /// Creates new children for this
        /// </summary>
        /// <param name="node"></param>
        private void CreateChildren(QuadTree node) {
            var midpoint = node.bounds.center;

            // Quadrant I
            node.ne = new QuadTree(
                midpoint, 
                node.bounds.max, 
                minNodeSize
            );
            // Quadrant II
            node.nw = new QuadTree(
                new Vector2Int(node.bounds.min.x, midpoint.y), 
                new Vector2Int(midpoint.x, node.bounds.max.y),
                minNodeSize
            );
            // Quadrant III
            node.sw = new QuadTree(
                node.bounds.min, 
                midpoint, 
                minNodeSize
            );
            // Quadrant IV
            node.se = new QuadTree(
                new Vector2Int(midpoint.x, node.bounds.min.y),
                new Vector2Int(node.bounds.max.x, midpoint.y),
                minNodeSize
            );
        }

        /// <summary>
        /// Gets a list of all leaf nodes starting from the root
        /// </summary>
        /// <returns></returns>
        public List<Vector2Int> GetChildren() {
            var children = new List<Vector2Int>();
            GetChildren(this, children);
            return children;
        }

        /// <summary>
        /// Recursively gets a list of all leaf nodes
        /// </summary>
        /// <param name="node">Root node</param>
        /// <param name="children">List to be modified</param>
        private void GetChildren(QuadTree node, List<Vector2Int> children) {
            if (node == null) return;
            
            // If this node is leaf node, add to list
            if (node.ne == null && node.nw == null && node.sw == null && node.se == null) {
                children.Add(node.bounds.center);
            }

            // Recurse
            GetChildren(node.ne, children);
            GetChildren(node.nw, children);
            GetChildren(node.sw, children);
            GetChildren(node.se, children);
        }
    }

    /// <summary>
    /// Custom bounds helper class containing int vector information
    /// </summary>
    public class Bounds {
        public Vector2Int center;
        public Vector2Int min;
        public Vector2Int max;
        public int width;

        public Bounds(Vector2Int center, int halfWidth) {
            this.center = center;
            min = new Vector2Int(center.x - halfWidth, center.y - halfWidth);
            max = new Vector2Int(center.x + halfWidth, center.y + halfWidth);
            width = halfWidth * 2;
        }

        public Bounds(Vector2Int min, Vector2Int max) {
            this.min = min;
            this.max = max;

            width = max.x - min.x;
            center = (min + max) / 2;
        }
    }
}
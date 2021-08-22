using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts {
    /// <summary>
    /// Implementation of a quadtree datastructure
    /// </summary>
    public class QuadTree {
        public Bounds bounds;
        public static int minNodeSize = 64;

        // Children
        public QuadTree ne; // Quadrant I
        public QuadTree nw; // Quadrant II
        public QuadTree sw; // Quadrant III
        public QuadTree se; // Quadrant IV

        // Constructors
        public QuadTree(Vector2Int min, Vector2Int max) {
            bounds = new Bounds(min, max);
        }

        public QuadTree(Vector2Int center, int halfWidth) {
            bounds = new Bounds(center, halfWidth);
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
        private static void Insert(QuadTree node, Vector2 position) {
            var distToChild = Vector2.Distance(node.bounds.center, position);
            
            // If object is within bounds and the nodes are big enough
            if (distToChild < node.bounds.width && node.bounds.width > minNodeSize) {
                // Create children
                CreateChildren(node);
                
                // Recurse
                Insert(node.ne, position);
                Insert(node.nw, position);
                Insert(node.sw, position);
                Insert(node.se, position);
            }
        }

        /// <summary>
        /// Creates new children for this node
        /// </summary>
        /// <param name="node"></param>
        private static void CreateChildren(QuadTree node) {
            var midpoint = node.bounds.center;

            // Quadrant I
            node.ne = new QuadTree(
                midpoint, 
                node.bounds.max
            );
            // Quadrant II
            node.nw = new QuadTree(
                new Vector2Int(node.bounds.min.x, midpoint.y), 
                new Vector2Int(midpoint.x, node.bounds.max.y)
            );
            // Quadrant III
            node.sw = new QuadTree(
                node.bounds.min, 
                midpoint
            );
            // Quadrant IV
            node.se = new QuadTree(
                new Vector2Int(midpoint.x, node.bounds.min.y),
                new Vector2Int(node.bounds.max.x, midpoint.y)
            );
        }

        public Dictionary<Vector2Int, QuadTree> GetChildNodes() {
            var children = new Dictionary<Vector2Int, QuadTree>();
            GetChildren(this, children);
            return children;
        }

        private void GetChildren(QuadTree node, Dictionary<Vector2Int, QuadTree> children) {
            if (node == null) return;

            // If this node is leaf node, add to list
            if (node.ne == null && node.nw == null && node.sw == null && node.se == null) {
                children.Add(node.bounds.center, node);
            }

            // Recurse
            GetChildren(node.ne, children);
            GetChildren(node.nw, children);
            GetChildren(node.sw, children);
            GetChildren(node.se, children);
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
        private static void GetChildren(QuadTree node, List<Vector2Int> children) {
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

        /// <summary>
        /// Constructs a Bounds with a center and half of the Bound's width
        /// </summary>
        /// <param name="center">Center point</param>
        /// <param name="halfWidth">Half width</param>
        public Bounds(Vector2Int center, int halfWidth) {
            this.center = center;
            min = new Vector2Int(center.x - halfWidth, center.y - halfWidth);
            max = new Vector2Int(center.x + halfWidth, center.y + halfWidth);
            width = halfWidth * 2;
        }

        /// <summary>
        /// Constructs a Bounds with a min and max position (bottom-left, top-right)
        /// </summary>
        /// <param name="min">Axis-aligned minimum position</param>
        /// <param name="max">Axis-aligned maximum position</param>
        public Bounds(Vector2Int min, Vector2Int max) {
            this.min = min;
            this.max = max;

            width = max.x - min.x;
            // Get midpoint
            center = (min + max) / 2;
        }
    }
}
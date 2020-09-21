using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    class PathNode
    {
        public static TileIdx InvalidIdx = new TileIdx(-1, -1);
        public TileIdx Previous = InvalidIdx;
        public TileIdx Location = InvalidIdx;
        public float CostFromPrevious = float.MaxValue;
        public float HorzCostFromPrevious = float.MaxValue;
        public float VertCostFromPrevious = float.MaxValue;
        public bool CanStopOnNode = true;

        public void SetPrevious(TileIdx previous, float horzChange, float vertChange)
        {
            Previous = previous;
            HorzCostFromPrevious = DistanceHueristic(horzChange);
            VertCostFromPrevious = ElevationHueristic(vertChange);
            CostFromPrevious = HorzCostFromPrevious + VertCostFromPrevious;
        }

        private float DistanceHueristic(float distanceDifference)
        {
            return distanceDifference;
        }

        private float ElevationHueristic(float elevationDifference)
        {
            return Math.Abs(elevationDifference);
        }
    }

    class MapPathFinder
    {
        List<List<PathNode>> mPathNodes = new List<List<PathNode>>();
        float mMaxHorzDistance = 0;
        float mMaxElevationDistance = 0;

        public MapPathFinder()
        {

        }

        public List<TileIdx> GetPossibleTiles()
        {
            List<TileIdx> inRange = new List<TileIdx>();

            foreach (List<PathNode> row in mPathNodes)
            {
                foreach (PathNode node in row)
                {
                    if (node != null && node.CanStopOnNode)
                    {
                        
                        inRange.Add(node.Location);    
                    }
                }
            }

            return inRange;
        }

        public List<TileIdx> GetPathTo(TileIdx tileIdx)
        {
            List<TileIdx> path = new List<TileIdx>();

            PathNode current = NodeAt(tileIdx);
            while (current != null)
            {
                if (current.Previous != PathNode.InvalidIdx)
                {
                    path.Add(current.Location);
                    current = NodeAt(current.Previous);
                }
                else
                {
                    current = null;
                }
            }

            path.Reverse();
            return path;
        }

        public void CalculatePaths(Map map, TileIdx fromLocation, TeamSide forTeamSide, float maxHorz, float maxVert)
        {
            mPathNodes = new List<List<PathNode>>();
            mMaxHorzDistance = maxHorz;
            mMaxElevationDistance = maxVert;

            for (int z = 0; z < map.Length; ++z)
            {
                List<PathNode> nodeRow = new List<PathNode>();
                for (int x = 0; x < map.Width; ++x)
                {
                    nodeRow.Add(null);
                }
                mPathNodes.Add(nodeRow);
            }

            PathNode startingNode = new PathNode() { CanStopOnNode = false, Location = fromLocation };
            
            Dictionary<int, PathNode> potentialNodes = new Dictionary<int, PathNode>();
            potentialNodes.Add(FlattenIdx(fromLocation), startingNode);

            int safteyBreak = 0;
            while (potentialNodes.Count > 0 && safteyBreak++ < 1000)
            {
                // Find node with the lowest cost to progress next
                List<int> possibleNodes = potentialNodes.Keys.ToList();
                possibleNodes.Sort((x, y) => CostTo(potentialNodes[x]).CompareTo(CostTo(potentialNodes[y])));
                int nextNode = possibleNodes[0];
                
                PathNode node = potentialNodes[nextNode];
                TileIdx tileIdx = UnFlatten(nextNode);
                potentialNodes.Remove(nextNode);

                UnityEngine.Debug.Assert(!IsNodeSet(tileIdx));
                UpdateNode(node);

                // End

                // Add all neighbours to potential nodes
                for (int i = 0; i < (int)Orientation.NUM; ++i)
                {
                    Orientation neighbourOrientation = (Orientation)i;
                    TileIdx neighbourIdx = tileIdx.GetTileToThe(neighbourOrientation);
                    if (map.IsValidIdx(neighbourIdx))
                    {
                        bool obstructedDestination = map.TileAt(neighbourIdx).IsObstructed;
                        bool isEnemyInTheWay = !(map.TileAt(neighbourIdx).OnTile == null || map.TileAt(neighbourIdx).OnTile.TeamSide == forTeamSide);
                        bool isPathBetweenTilesObstructed =
                            map.IsPathObstructed(tileIdx, neighbourOrientation)
                            || map.IsPathObstructed(neighbourIdx, OrientationUtil.Flip(neighbourOrientation));

                        if (!obstructedDestination 
                            && !isEnemyInTheWay
                            && !isPathBetweenTilesObstructed)
                        {
                            float horzDifference = 1; // cost to move 1 tile
                            float vertDifference = map.TileAt(neighbourIdx).Elevation - map.TileAt(tileIdx).Elevation;

                            PathNode neighbourNode = new PathNode() { CanStopOnNode = map.TileAt(neighbourIdx).OnTile == null, Location = neighbourIdx };
                            neighbourNode.SetPrevious(node.Location, horzDifference, vertDifference);

                            float horzCost = 0;
                            float vertCost = 0;
                            HorzVertCostTo(neighbourNode, ref horzCost, ref vertCost);
                            if (horzCost <= mMaxHorzDistance && vertCost <= mMaxElevationDistance)
                            {
                                // check if we have a different solution to get to this tile
                                int flattenedNeighbourIdx = FlattenIdx(neighbourIdx);

                                // If there's a potential node that this is better than
                                if (potentialNodes.ContainsKey(flattenedNeighbourIdx))
                                {
                                    if (CostTo(potentialNodes[flattenedNeighbourIdx]) > CostTo(neighbourNode))
                                    {
                                        potentialNodes[flattenedNeighbourIdx] = neighbourNode;
                                    }
                                }
                                else if (IsNodeSet(neighbourIdx))
                                {
                                    // Neighbours have allready been calculated, just need to update this node if applicable
                                    if (CostTo(NodeAt(neighbourIdx)) > CostTo(neighbourNode))
                                    {
                                        UpdateNode(neighbourNode);
                                    }
                                }
                                else
                                {
                                    potentialNodes.Add(flattenedNeighbourIdx, neighbourNode);
                                }
                            }
                        }
                    }
                }
            }
        }

        private int FlattenIdx(TileIdx tileIdx) { return tileIdx.y * mPathNodes.Count + tileIdx.x; }
        private TileIdx UnFlatten(int flattened)
        {
            return new TileIdx(flattened % mPathNodes.Count, flattened / mPathNodes.Count);
        }

        private bool CanNavigateToNeighbour(TileIdx index)
        {
            return index.y >= 0 && index.y < mPathNodes.Count
                && index.x >= 0 && index.x < mPathNodes[0].Count;
        }

        private bool IsNodeSet(TileIdx atIdx)
        {
            return NodeAt(atIdx) != null;
        }

        private PathNode NodeAt(TileIdx atIdx)
        {
            return mPathNodes[atIdx.y][atIdx.x];
        }

        private void UpdateNode(PathNode node)
        {
            mPathNodes[node.Location.y][node.Location.x] = node;
        }

        private float CostTo(PathNode node)
        {
            float cost = node.CostFromPrevious;
            if (node.Previous != PathNode.InvalidIdx)
            {
                cost += CostTo(NodeAt(node.Previous));
            }
            return cost;
        }

        private void HorzVertCostTo(PathNode pathNode, ref float horzCost, ref float vertCost)
        {
            if (pathNode.Previous != PathNode.InvalidIdx)
            {
                horzCost += pathNode.HorzCostFromPrevious;
                vertCost += pathNode.VertCostFromPrevious;

                HorzVertCostTo(NodeAt(pathNode.Previous), ref horzCost, ref vertCost);
            }
        }
    }
}

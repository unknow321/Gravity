﻿using System.Collections.Generic;
using Gravity.Server.ProcessingNodes.Transform;
using Gravity.Server.Ui.Shapes;

namespace Gravity.Server.Ui.Nodes
{
    internal class TransformDrawing: NodeDrawing
    {
        private readonly DrawingElement _drawing;
        private readonly TransformNode _transform;

        public TransformDrawing(
            DrawingElement drawing, 
            TransformNode transform) 
            : base(drawing, "Transform", "transform", transform.Offline, 2, transform.Name)
        {
            _drawing = drawing;
            _transform = transform;
        }

        public override void AddLines(IDictionary<string, NodeDrawing> nodeDrawings)
        {
            if (string.IsNullOrEmpty(_transform.OutputNode))
                return;

            NodeDrawing nodeDrawing;
            if (nodeDrawings.TryGetValue(_transform.OutputNode, out nodeDrawing))
            {
                _drawing.AddChild(new ConnectedLineDrawing(TopRightSideConnection, nodeDrawing.TopLeftSideConnection)
                {
                    CssClass = _transform.Offline ? "connection_none" : "connection_unknown"
                });
            }
        }
    }
}
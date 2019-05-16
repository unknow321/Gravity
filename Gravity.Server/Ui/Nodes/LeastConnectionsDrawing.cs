﻿using System.Linq;
using System.Collections.Generic;
using Gravity.Server.DataStructures;
using Gravity.Server.ProcessingNodes;
using Gravity.Server.Ui.Shapes;

namespace Gravity.Server.Ui.Nodes
{
    internal class LeastConnectionsDrawing: NodeDrawing
    {
        private readonly DrawingElement _drawing;
        private readonly LeastConnectionsNode _leastConnections;
        private readonly OutputDrawing[] _outputDrawings;

        public LeastConnectionsDrawing(
            DrawingElement drawing, 
            LeastConnectionsNode leastConnections) 
            : base(drawing, "Least connections", 2, leastConnections.Name)
        {
            _drawing = drawing;
            _leastConnections = leastConnections;
            
            SetCssClass(leastConnections.Disabled ? "disabled" : "least_connections");

            if (leastConnections.Outputs != null)
            {
                _outputDrawings = new OutputDrawing[leastConnections.Outputs.Length];

                for (var i = 0; i < leastConnections.Outputs.Length; i++)
                {
                    var outputNodeName = leastConnections.Outputs[i];
                    var output = leastConnections.OutputNodes[i];
                    _outputDrawings[i] = new OutputDrawing(drawing, outputNodeName, output);
                }

                foreach (var outputDrawing in _outputDrawings)
                    AddChild(outputDrawing);
            }
        }

        public override void AddLines(IDictionary<string, NodeDrawing> nodeDrawings)
        {
            if (_leastConnections.Outputs == null) return;

            for (var i = 0; i < _leastConnections.Outputs.Length; i++)
            {
                var output = _leastConnections.Outputs[i];
                var outputDrawing = _outputDrawings[i];

                NodeDrawing nodeDrawing;
                if (nodeDrawings.TryGetValue(output, out nodeDrawing))
                {
                    _drawing.AddChild(new ConnectedLineDrawing(outputDrawing.TopRightSideConnection, nodeDrawing.TopLeftSideConnection)
                    {
                        CssClass = _leastConnections.Disabled ? "connection_disabled" : "connection_light"
                    });
                }
            }
        }

        private class OutputDrawing : NodeDrawing
        {
            public OutputDrawing(
                DrawingElement drawing,
                string label,
                NodeOutput output)
                : base(drawing, "Output", 3, label)
            {
                CssClass = "least_connections_output";

                var details = new List<string>();

                if (output != null)
                {
                    details.Add(output.RequestCount + " requests");
                    details.Add(output.ConnectionCount + " connections");
                }

                AddDetails(details);
            }
        }
    }
}
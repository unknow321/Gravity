﻿using System.Collections.Generic;
using System.Linq;
using Gravity.Server.Configuration;
using Gravity.Server.ProcessingNodes.Routing;
using Gravity.Server.Ui.Shapes;

namespace Gravity.Server.Ui.Nodes
{
    internal class RouterTile: NodeTile
    {
        private readonly DrawingElement _drawing;
        private readonly RoutingNode _router;
        private readonly RouterOutputDrawing[] _outputDrawings;
        private readonly double[] _trafficIndicatorThresholds;

        public RouterTile(
            DrawingElement drawing, 
            RoutingNode router,
            DashboardConfiguration.NodeConfiguration nodeConfiguration,
            TrafficIndicatorConfiguration trafficIndicatorConfiguration)
            : base(
                drawing,
                nodeConfiguration?.Title ?? "Router", 
                "router", 
                router.Offline, 
                2, 
                router.Name)
        {
            _drawing = drawing;
            _router = router;
            _trafficIndicatorThresholds = trafficIndicatorConfiguration.Thresholds;

            LinkUrl = "/ui/node?name=" + router.Name;

            var details = new List<string>();

            if (router.Outputs != null)
            {
                _outputDrawings = router.Outputs
                    .Select(o => new RouterOutputDrawing(drawing, o, o.RouteTo, null, router.Offline))
                    .ToArray();

                foreach (var outputDrawing in _outputDrawings)
                    AddChild(outputDrawing);
            }

            AddDetails(details, null, router.Offline ? "disabled" : string.Empty);
        }

        public override void AddLines(IDictionary<string, NodeTile> nodeDrawings)
        {
            if (_router.Outputs == null) return;

            for (var i = 0; i < _router.Outputs.Length; i++)
            {
                var outputConfiguration = _router.Outputs[i];
                var outputNode = _router.OutputNodes[i];
                var outputDrawing = _outputDrawings[i];

                NodeTile nodeDrawing;
                if (nodeDrawings.TryGetValue(outputConfiguration.RouteTo, out nodeDrawing))
                {
                    var css = "connection_none";

                    if (!outputNode.Disabled)
                    {
                        var requestsPerMinute = outputNode.TrafficAnalytics.RequestsPerMinute;
                        if (requestsPerMinute < _trafficIndicatorThresholds[0]) css = "connection_none";
                        else if (requestsPerMinute < _trafficIndicatorThresholds[1]) css = "connection_light";
                        else if (requestsPerMinute < _trafficIndicatorThresholds[2]) css = "connection_medium";
                        else if (requestsPerMinute < _trafficIndicatorThresholds[3]) css = "connection_heavy";
                    }

                    _drawing.AddChild(new ConnectedLineDrawing(outputDrawing.TopRightSideConnection, nodeDrawing.TopLeftSideConnection)
                    {
                        CssClass = css
                    });
                }
            }
        }

        private class RouterOutputDrawing : NodeTile
        {
            public RouterOutputDrawing(
                DrawingElement drawing,
                RouterOutputConfiguration routerOutput,
                string label,
                string title,
                bool disabled)
                : base(
                    drawing, 
                    title ?? "Output", 
                    "router_output", 
                    disabled, 
                    3, 
                    label)
            {
                var details = new List<string>();

                if (routerOutput.Conditions != null && routerOutput.Conditions.Length > 0)
                {
                    details.Add("If " + routerOutput.ConditionLogic.ToString().ToLower());

                    foreach (var rule in routerOutput.Conditions)
                    {
                        if (!rule.Disabled)
                        {
                            if (rule.Negate)
                                details.Add("Not " + rule.Condition);
                            else
                                details.Add(rule.Condition);
                        }
                    }
                }

                AddDetails(details, null, disabled ? "disabled" : string.Empty);
            }
        }
    }
}
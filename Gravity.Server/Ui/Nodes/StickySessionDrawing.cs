﻿using System.Collections.Generic;
using Gravity.Server.ProcessingNodes;
using Gravity.Server.Ui.Shapes;

namespace Gravity.Server.Ui.Nodes
{
    internal class StickySessionDrawing: NodeDrawing
    {
        private readonly DrawingElement _drawing;
        private readonly StickySessionNode _stickySession;

        public StickySessionDrawing(
            DrawingElement drawing, 
            StickySessionNode stickySession) 
            : base(drawing, "Sticky session", 2, stickySession.Name)
        {
            _drawing = drawing;
            _stickySession = stickySession;
            
            SetCssClass(stickySession.Disabled ? "disabled" : "sticky_session");

            var details = new List<string>();

            if (stickySession.Outputs != null)
                details.Add("To: " + string.Join(", ", stickySession.Outputs));

            details.Add("Cookie: " + stickySession.SessionCookie);
            details.Add("Lifetime: " + stickySession.SessionDuration);

            AddDetails(details);
        }

        public override void AddLines(IDictionary<string, NodeDrawing> nodeDrawings)
        {
            foreach (var output in _stickySession.Outputs)
            {
                NodeDrawing nodeDrawing;
                if (nodeDrawings.TryGetValue(output, out nodeDrawing))
                {
                    _drawing.AddChild(new ConnectedLineDrawing(TopRightSideConnection, nodeDrawing.TopLeftSideConnection)
                    {
                        CssClass = _stickySession.Disabled ? "connection_disabled" : "connection_light"
                    });
                }
            }
        }
    }
}
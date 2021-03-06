﻿using System;
using Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Interfaces;
using Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Interfaces.Actions;
using Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Interfaces.Conditions;
using Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Interfaces.Rules;

namespace Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Actions
{
    internal class Insert : Action, IInsertAction
    {
        private Scope _scope;
        private string _scopeIndex;
        private int _scopeIndexValue;
        private IValueGetter _valueGetter;
        private Action<IRuleExecutionContext, string> _action;

        public IInsertAction Initialize(Scope scope, string scopeIndex, IValueGetter valueGetter)
        {
            _scope = scope;
            _scopeIndex = scopeIndex;
            _valueGetter = valueGetter;

            if (string.IsNullOrEmpty(scopeIndex))
                throw new Exception("When inserting into the path the index of the element to insert before must be provided");
            
            if (scope != Scope.PathElement && scope != Scope.HostElement)
                throw new Exception("You can only insert into the path scope or host scope");

            if (!int.TryParse(scopeIndex, out _scopeIndexValue))
                throw new Exception("The index of the path element to insert must be a number");
            
            // TODO: Insert into host elements

            if (_scopeIndexValue == 0)
                _action = (requestInfo, value) =>
                {
                    if (requestInfo.NewPathString == "/")
                        requestInfo.NewPathString = "/" + value;
                    else
                        requestInfo.NewPathString = "/" + value + requestInfo.NewPathString;
                };

            else if (_scopeIndexValue > 0)
            {
                _action = (requestInfo, value) =>
                {
                    var maxIndex = requestInfo.NewPath.Count;
                    if (string.IsNullOrEmpty(requestInfo.NewPath[requestInfo.NewPath.Count - 1]))
                        maxIndex--;
                    if (_scopeIndexValue < maxIndex)
                    {
                        requestInfo.NewPath.Insert(_scopeIndexValue, value);
                        requestInfo.PathChanged();
                    }
                };
            }

            else
            {
                _action = (requestInfo, value) =>
                {
                    var index = requestInfo.NewPath.Count + _scopeIndexValue;
                    if (string.IsNullOrEmpty(requestInfo.NewPath[requestInfo.NewPath.Count - 1]))
                        index--;
                    if (index > 0)
                    {
                        requestInfo.NewPath.Insert(index, value);
                        requestInfo.PathChanged();
                    }
                };
            }

            return this;
        }

        public override void PerformAction(
            IRuleExecutionContext requestInfo,
            IRuleResult ruleResult,
            out bool stopProcessing,
            out bool endRequest)
        {
            var value = _valueGetter.GetString(requestInfo, ruleResult);
            _action(requestInfo, value);

            stopProcessing = _stopProcessing;
            endRequest = _endRequest;
        }

        public override string ToString()
        {
            var text = "Insert " + _valueGetter + " into " + _scope;
            if (!string.IsNullOrEmpty(_scopeIndex))
                text += " before " + _scopeIndex;
            return text;
        }

        public override string ToString(IRuleExecutionContext request)
        {
            var text = "insert " + _valueGetter + " into " + _scope;
            if (!string.IsNullOrEmpty(_scopeIndex))
                text += " before " + _scopeIndex;
            return text;
        }
    }
}

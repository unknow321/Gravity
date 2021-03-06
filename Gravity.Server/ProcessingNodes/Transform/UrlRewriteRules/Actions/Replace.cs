﻿using System;
using System.Collections.Generic;
using Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Interfaces;
using Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Interfaces.Actions;
using Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Interfaces.Conditions;
using Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Interfaces.Rules;
using Microsoft.Owin;

namespace Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Actions
{
    internal class Replace : Action, IReplaceAction
    {
        private Scope _scope;
        private string _scopeIndex;
        private int _scopeIndexValue;
        private IValueGetter _valueGetter;

        public IReplaceAction Initialize(Scope scope, string scopeIndex, IValueGetter valueGetter)
        {
            _scope = scope;
            _scopeIndex = scopeIndex;
            _valueGetter = valueGetter;

            if (string.IsNullOrEmpty(scopeIndex))
            {
                switch (scope)
                {
                    case Scope.Header:
                        throw new Exception("When replacing the request headers you must specify the name of the header to replace");
                    case Scope.ServerVariable:
                        throw new Exception("When replacing server variables you must specify the name of the server variable to replace");
                    case Scope.Parameter:
                        _scope = Scope.QueryString;
                        break;
                    case Scope.PathElement:
                        _scope = Scope.Path;
                        break;
                }
            }
            else
            {
                if (int.TryParse(scopeIndex, out _scopeIndexValue))
                {
                    if (_scopeIndexValue == 0)
                    {
                        if (scope == Scope.PathElement)
                            _scope = Scope.Path;
                        else if (scope == Scope.HostElement)
                            _scope = Scope.Host;
                    }
                }
                else
                {
                    if (scope == Scope.PathElement) 
                        _scope = Scope.Path;
                    else if (scope == Scope.HostElement)
                        _scope = Scope.Host;
                }
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

            switch (_scope)
            {
                case Scope.Url:
                    requestInfo.NewUrlString = value;
                    break;
                case Scope.Host:
                    requestInfo.NewHost = value;
                    break;
                case Scope.Path:
                    requestInfo.NewPathString = value;
                    break;
                case Scope.QueryString:
                    requestInfo.NewParametersString = value;
                    break;
                case Scope.Header:
                    requestInfo.SetHeader(_scopeIndex, value);
                    break;
                case Scope.Parameter:
                    requestInfo.NewParameters[_scopeIndex] = new List<string> { value };
                    requestInfo.ParametersChanged();
                    break;
                case Scope.PathElement:
                    if (_scopeIndexValue == 0)
                        requestInfo.NewPathString = value;
                    else if (_scopeIndexValue > 0)
                    {
                        var count = requestInfo.NewPath.Count;
                        if (string.IsNullOrEmpty(requestInfo.NewPath[count - 1]))
                            count--;
                        if (_scopeIndexValue < count)
                        {
                            requestInfo.NewPath[_scopeIndexValue] = value;
                            requestInfo.PathChanged();
                        }
                    }
                    else
                    {
                        var count = requestInfo.NewPath.Count;
                        if (string.IsNullOrEmpty(requestInfo.NewPath[count - 1]))
                            count--;
                        var index = count + _scopeIndexValue;
                        if (index > 0)
                        {
                            requestInfo.NewPath[index] = value;
                            requestInfo.PathChanged();
                        }
                    }
                    break;
                case Scope.HostElement:
                    if (_scopeIndexValue == 0)
                        requestInfo.NewHost = value;
                    else if (_scopeIndexValue > 0)
                    {
                        var hostElements = requestInfo.NewHost.Split('.');
                        var count = hostElements.Length;
                        if (string.IsNullOrEmpty(hostElements[count - 1])) count--;
                        if (_scopeIndexValue < count)
                        {
                            hostElements[_scopeIndexValue - 1] = value;
                            requestInfo.NewHost = string.Join(".", hostElements);
                        }
                    }
                    else
                    {
                        var hostElements = requestInfo.NewHost.Split('.');
                        var count = hostElements.Length;
                        if (string.IsNullOrEmpty(hostElements[count - 1])) count--;
                        var index = count + _scopeIndexValue;
                        if (index > 0)
                        {
                            requestInfo.NewPath[index] = value;
                            requestInfo.NewHost = string.Join(".", hostElements);
                        }
                    }
                    break;
                case Scope.ServerVariable:
                    requestInfo.SetServerVariable(_scopeIndex, value);
                    break;
            }

            stopProcessing = _stopProcessing;
            endRequest = _endRequest;
        }

        public override string ToString()
        {
            var text = "Replace " + _scope;
            if (!string.IsNullOrEmpty(_scopeIndex))
                text += "[" + _scopeIndex + "]";
            text += " with " + _valueGetter;
            return text;
        }

        public override string ToString(IRuleExecutionContext request)
        {
            var text = "replace " + _scope;
            if (!string.IsNullOrEmpty(_scopeIndex))
                text += "[" + _scopeIndex + "]";
            text += " with " + _valueGetter;
            return text;
        }
    }
}

// Copyright (c) FireGiant.  All Rights Reserved.  Licensed under the BSD License.  See LICENSE.txt in the project root for license information.

using System;
using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace FireGiant.BuildTasks.VSSdkMitigations
{
    internal class FilteredBuildEngine : IBuildEngine
    {
        internal FilteredBuildEngine(IBuildEngine realEngine, string excludeMessagePattern, string excludeWarningPattern, string excludeErrorPattern)
        {
            if (realEngine == null) throw new ArgumentNullException(nameof(realEngine));

            this.RealEngine = realEngine;

            if (!String.IsNullOrEmpty(excludeMessagePattern))
            {
                this.ExcludeMessageRegex = new Regex(excludeMessagePattern);
            }

            if (!String.IsNullOrEmpty(excludeWarningPattern))
            {
                this.ExcludeWarningRegex = new Regex(excludeWarningPattern);
            }

            if (!String.IsNullOrEmpty(excludeErrorPattern))
            {
                this.ExcludeErrorRegex = new Regex(excludeErrorPattern);
            }
        }

        private IBuildEngine RealEngine { get; }

        private Regex ExcludeMessageRegex { get; }

        private Regex ExcludeWarningRegex { get; }

        private Regex ExcludeErrorRegex { get; }

        bool IBuildEngine.BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return this.RealEngine.BuildProjectFile(projectFileName, targetNames, globalProperties, targetOutputs);
        }

        int IBuildEngine.ColumnNumberOfTaskNode => this.RealEngine.ColumnNumberOfTaskNode;

        bool IBuildEngine.ContinueOnError => this.RealEngine.ContinueOnError;

        int IBuildEngine.LineNumberOfTaskNode => this.RealEngine.LineNumberOfTaskNode;

        void IBuildEngine.LogCustomEvent(CustomBuildEventArgs e)
        {
            this.RealEngine.LogCustomEvent(e);
        }

        void IBuildEngine.LogErrorEvent(BuildErrorEventArgs e)
        {
            if (this.ExcludeErrorRegex == null)
            {
                this.RealEngine.LogErrorEvent(e);
            }
            else if (!this.ExcludeErrorRegex.IsMatch(e.Message))
            {
                this.RealEngine.LogErrorEvent(e);
            }
        }

        void IBuildEngine.LogMessageEvent(BuildMessageEventArgs e)
        {
            if (this.ExcludeMessageRegex == null)
            {
                this.RealEngine.LogMessageEvent(e);
            }
            else if (!this.ExcludeMessageRegex.IsMatch(e.Message))
            {
                this.RealEngine.LogMessageEvent(e);
            }
        }

        void IBuildEngine.LogWarningEvent(BuildWarningEventArgs e)
        {
            if (this.ExcludeWarningRegex == null)
            {
                this.RealEngine.LogWarningEvent(e);
            }
            else if (!this.ExcludeWarningRegex.IsMatch(e.Message))
            {
                this.RealEngine.LogWarningEvent(e);
            }
        }

        string IBuildEngine.ProjectFileOfTaskNode => this.RealEngine.ProjectFileOfTaskNode;
    }
}

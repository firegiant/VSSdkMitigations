// Copyright (c) FireGiant.  All Rights Reserved.  Licensed under the BSD License.  See LICENSE.txt in the project root for license information.

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VsSDK.Build.Tasks;

namespace FireGiant.BuildTasks.VSSdkMitigations
{
    public class FilteredGetVSTemplateItems : Task
    {
        public FilteredGetVSTemplateItems()
        {
            this.GetVSTemplateItemsTask = new GetVSTemplateItems();
        }

        public override bool Execute()
        {
            this.GetVSTemplateItemsTask.BuildEngine = new FilteredBuildEngine(base.BuildEngine, this.ExcludeMessagePattern, this.ExcludeWarningPattern, this.ExcludeErrorPattern);

            this.GetVSTemplateItemsTask.HostObject = base.HostObject;
            this.GetVSTemplateItemsTask.TargetPath = this.TargetPath;
            this.GetVSTemplateItemsTask.TemplateFiles = this.TemplateFiles;

            bool outcome = this.GetVSTemplateItemsTask.Execute();

            this.ZipItems = this.GetVSTemplateItemsTask.ZipItems;
            this.ZipProjects = this.GetVSTemplateItemsTask.ZipProjects;

            return outcome;
        }

        private GetVSTemplateItems GetVSTemplateItemsTask { get; }

        /// <summary>
        /// Optional.  Indicates the regular expression that should be used to filter out informational messages.
        /// </summary>
        public string ExcludeMessagePattern { get; set; }

        /// <summary>
        /// Optional.  Indicates the regular expression that should be used to filter out warning messages.
        /// </summary>
        public string ExcludeWarningPattern { get; set; }

        /// <summary>
        /// Optional.  Indicates the regular expression that should be used to filter out error messages.
        /// </summary>
        public string ExcludeErrorPattern { get; set; }

        // The following properties echo what the real GetVSTemplateItems build task exposes,
        // and are passed through as-is.
        //
        [Required]
        public string TargetPath { get; set; }

        [Required]
        public ITaskItem[] TemplateFiles { get; set; }

        [Output]
        public ITaskItem[] ZipItems { get; private set; }

        [Output]
        public ITaskItem[] ZipProjects { get; private set; }
    }
}

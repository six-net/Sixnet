﻿using System;
using System.Collections.Generic;
using System.IO;

namespace EZNEW.Application
{
    /// <summary>
    /// Application file match options
    /// </summary>
    public class FileMatchOptions
    {
        /// <summary>
        /// Gets or sets the file match pattern
        /// </summary>
        public FileMatchPattern FileMatchPattern { get; set; } = FileMatchPattern.Convention;

        /// <summary>
        /// File search option
        /// </summary>
        public SearchOption FileSearchOption { get; set; } = SearchOption.TopDirectoryOnly;

        /// <summary>
        /// Gets or sets the file name keywords
        /// </summary>
        public List<string> FileNameKeywords { get; set; }

        /// <summary>
        /// Gets or sets the regex expression
        /// </summary>
        public string RegexExpression { get; set; }

        /// <summary>
        /// Additional convention file patterns
        /// </summary>
        internal List<string> AdditionalConventionFilePatterns = new List<string>();

        /// <summary>
        /// Add additional convention file match patterns
        /// </summary>
        /// <param name="patterns"></param>
        public void AddConventionFileMatchPatterns(params string[] patterns)
        {
            if (patterns.IsNullOrEmpty())
            {
                return;
            }
            AdditionalConventionFilePatterns.AddRange(patterns);
        }
    }
}
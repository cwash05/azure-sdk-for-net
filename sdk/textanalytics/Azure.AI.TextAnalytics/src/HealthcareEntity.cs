﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Azure.AI.TextAnalytics
{
    /// <summary>
    /// .
    /// </summary>
    public class HealthcareEntity
    {
        internal HealthcareEntity(HealthcareEntityInternal entity, IReadOnlyDictionary<HealthcareEntity, HealthcareEntityRelationType> relatedEntities)
        {
            Category = entity.Category;
            Text = entity.Text;
            SubCategory = entity.Subcategory;
            ConfidenceScore = entity.ConfidenceScore;
            Offset = entity.Offset;
            DataSources = entity.Links;
            RelatedEntities = relatedEntities;
        }
        /// <summary>
        /// Gets the entity text as it appears in the input document.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the entity category inferred by the Text Analytics service's
        /// healthcare model.  The list of available categories is
        /// described at
        /// <a href="https://docs.microsoft.com/en-us/azure/cognitive-services/text-analytics/named-entity-types?tabs=health"/>.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the sub category of the entity inferred by the Text Analytics service's
        /// healthcare model.  This property may not have a value if
        /// a sub category doesn't exist for this entity.  The list of available categories and
        /// subcategories is described at
        /// <a href="https://docs.microsoft.com/en-us/azure/cognitive-services/text-analytics/named-entity-types?tabs=health"/>.
        /// </summary>
        public string SubCategory { get; }

        /// <summary>
        /// Gets a score between 0 and 1, indicating the confidence that the
        /// text substring matches this inferred entity.
        /// </summary>
        public double ConfidenceScore { get; }

        /// <summary>
        /// Gets the starting position (in UTF-16 code units) for the matching text in the input document.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the length of input document.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Get the list of data sources for the entity.
        /// </summary>
        public IReadOnlyCollection<EntityDataSource> DataSources { get; }

        /// <summary>
        /// Gets the dictionary for related entity with mapped relation type for each.
        /// </summary>
        public IReadOnlyDictionary<HealthcareEntity, HealthcareEntityRelationType> RelatedEntities { get; }
    }
}

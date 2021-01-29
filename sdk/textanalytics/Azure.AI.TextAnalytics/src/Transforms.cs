﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Azure.AI.TextAnalytics.Models;

namespace Azure.AI.TextAnalytics
{
    internal static class Transforms
    {
        #region Common

        internal static TextAnalyticsError ConvertToError(TextAnalyticsErrorInternal error)
        {
            string errorCode = error.Code;
            string message = error.Message;
            string target = error.Target;
            InnerError innerError = error.Innererror;

            if (innerError != null)
            {
                // Return the innermost error, which should be only one level down.
                return new TextAnalyticsError(innerError.Code, innerError.Message, innerError.Target);
            }

            return new TextAnalyticsError(errorCode, message, target);
        }

        internal static List<TextAnalyticsError> ConvertToErrors(IReadOnlyList<TextAnalyticsErrorInternal> internalErrors)
        {
            var errors = new List<TextAnalyticsError>();

            if (internalErrors == null)
            {
                return errors;
            }

            foreach (TextAnalyticsErrorInternal error in internalErrors)
            {
                errors.Add(ConvertToError(error));
            }
            return errors;
        }

        internal static List<TextAnalyticsWarning> ConvertToWarnings(IReadOnlyList<TextAnalyticsWarningInternal> internalWarnings)
        {
            var warnings = new List<TextAnalyticsWarning>();

            if (internalWarnings == null)
            {
                return warnings;
            }

            foreach (TextAnalyticsWarningInternal warning in internalWarnings)
            {
                warnings.Add(new TextAnalyticsWarning(warning));
            }
            return warnings;
        }

        #endregion

        #region DetectLanguage

        internal static DetectedLanguage ConvertToDetectedLanguage(DocumentLanguage documentLanguage)
        {
            return new DetectedLanguage(documentLanguage.DetectedLanguage, ConvertToWarnings(documentLanguage.Warnings));
        }

        internal static DetectLanguageResultCollection ConvertToDetectLanguageResultCollection(LanguageResult results, IDictionary<string, int> idToIndexMap)
        {
            var detectedLanguages = new List<DetectLanguageResult>();

            //Read errors
            foreach (DocumentError error in results.Errors)
            {
                detectedLanguages.Add(new DetectLanguageResult(error.Id, ConvertToError(error.Error)));
            }

            //Read languages
            foreach (DocumentLanguage language in results.Documents)
            {
                detectedLanguages.Add(new DetectLanguageResult(language.Id, language.Statistics ?? default, ConvertToDetectedLanguage(language)));
            }

            detectedLanguages = SortHeterogeneousCollection(detectedLanguages, idToIndexMap);

            return new DetectLanguageResultCollection(detectedLanguages, results.Statistics, results.ModelVersion);
        }

        #endregion

        #region AnalyzeSentiment

        internal static AnalyzeSentimentResultCollection ConvertToAnalyzeSentimentResultCollection(SentimentResponse results, IDictionary<string, int> idToIndexMap)
        {
            var analyzedSentiments = new List<AnalyzeSentimentResult>();

            //Read errors
            foreach (DocumentError error in results.Errors)
            {
                analyzedSentiments.Add(new AnalyzeSentimentResult(error.Id, ConvertToError(error.Error)));
            }

            //Read sentiments
            foreach (DocumentSentimentInternal docSentiment in results.Documents)
            {
                analyzedSentiments.Add(new AnalyzeSentimentResult(docSentiment.Id, docSentiment.Statistics ?? default, new DocumentSentiment(docSentiment)));
            }

            analyzedSentiments = SortHeterogeneousCollection(analyzedSentiments, idToIndexMap);

            return new AnalyzeSentimentResultCollection(analyzedSentiments, results.Statistics, results.ModelVersion);
        }

        #endregion

        #region KeyPhrases

        internal static KeyPhraseCollection ConvertToKeyPhraseCollection(DocumentKeyPhrases documentKeyPhrases)
        {
            return new KeyPhraseCollection(documentKeyPhrases.KeyPhrases.ToList(), ConvertToWarnings(documentKeyPhrases.Warnings));
        }

        internal static ExtractKeyPhrasesResultCollection ConvertToExtractKeyPhrasesResultCollection(KeyPhraseResult results, IDictionary<string, int> idToIndexMap)
        {
            var keyPhrases = new List<ExtractKeyPhrasesResult>();

            //Read errors
            foreach (DocumentError error in results.Errors)
            {
                keyPhrases.Add(new ExtractKeyPhrasesResult(error.Id, ConvertToError(error.Error)));
            }

            //Read Key phrases
            foreach (DocumentKeyPhrases docKeyPhrases in results.Documents)
            {
                keyPhrases.Add(new ExtractKeyPhrasesResult(docKeyPhrases.Id, docKeyPhrases.Statistics ?? default, ConvertToKeyPhraseCollection(docKeyPhrases)));
            }

            keyPhrases = SortHeterogeneousCollection(keyPhrases, idToIndexMap);

            return new ExtractKeyPhrasesResultCollection(keyPhrases, results.Statistics, results.ModelVersion);
        }

        #endregion

        #region Recognize Entities

        internal static List<CategorizedEntity> ConvertToCategorizedEntityList(List<Entity> entities)
            => entities.Select((entity) => new CategorizedEntity(entity)).ToList();

        internal static CategorizedEntityCollection ConvertToCategorizedEntityCollection(DocumentEntities documentEntities)
        {
            return new CategorizedEntityCollection(ConvertToCategorizedEntityList(documentEntities.Entities.ToList()), ConvertToWarnings(documentEntities.Warnings));
        }

        internal static RecognizeEntitiesResultCollection ConvertToRecognizeEntitiesResultCollection(EntitiesResult results, IDictionary<string, int> idToIndexMap)
        {
            var recognizeEntities = new List<RecognizeEntitiesResult>();

            //Read errors
            foreach (DocumentError error in results.Errors)
            {
                recognizeEntities.Add(new RecognizeEntitiesResult(error.Id, ConvertToError(error.Error)));
            }

            //Read document entities
            foreach (DocumentEntities docEntities in results.Documents)
            {
                recognizeEntities.Add(new RecognizeEntitiesResult(docEntities.Id, docEntities.Statistics ?? default, ConvertToCategorizedEntityCollection(docEntities)));
            }

            recognizeEntities = SortHeterogeneousCollection(recognizeEntities, idToIndexMap);

            return new RecognizeEntitiesResultCollection(recognizeEntities, results.Statistics, results.ModelVersion);
        }

        #endregion

        #region Recognize PII Entities

        internal static List<PiiEntity> ConvertToPiiEntityList(List<Entity> entities)
            => entities.Select((entity) => new PiiEntity(entity)).ToList();

        internal static PiiEntityCollection ConvertToPiiEntityCollection(PiiDocumentEntities documentEntities)
        {
            return new PiiEntityCollection(ConvertToPiiEntityList(documentEntities.Entities.ToList()), documentEntities.RedactedText, ConvertToWarnings(documentEntities.Warnings));
        }

        internal static RecognizePiiEntitiesResultCollection ConvertToRecognizePiiEntitiesResultCollection(PiiEntitiesResult results, IDictionary<string, int> idToIndexMap)
        {
            var recognizeEntities = new List<RecognizePiiEntitiesResult>();

            //Read errors
            foreach (DocumentError error in results.Errors)
            {
                recognizeEntities.Add(new RecognizePiiEntitiesResult(error.Id, ConvertToError(error.Error)));
            }

            //Read document entities
            foreach (PiiDocumentEntities docEntities in results.Documents)
            {
                recognizeEntities.Add(new RecognizePiiEntitiesResult(docEntities.Id, docEntities.Statistics ?? default, ConvertToPiiEntityCollection(docEntities)));
            }

            recognizeEntities = SortHeterogeneousCollection(recognizeEntities, idToIndexMap);

            return new RecognizePiiEntitiesResultCollection(recognizeEntities, results.Statistics, results.ModelVersion);
        }

        #endregion

        #region Recognize Linked Entities

        internal static LinkedEntityCollection ConvertToLinkedEntityCollection(DocumentLinkedEntities documentEntities)
        {
            return new LinkedEntityCollection(documentEntities.Entities.ToList(), ConvertToWarnings(documentEntities.Warnings));
        }

        internal static RecognizeLinkedEntitiesResultCollection ConvertToRecognizeLinkedEntitiesResultCollection(EntityLinkingResult results, IDictionary<string, int> idToIndexMap)
        {
            var recognizeEntities = new List<RecognizeLinkedEntitiesResult>();

            //Read errors
            foreach (DocumentError error in results.Errors)
            {
                recognizeEntities.Add(new RecognizeLinkedEntitiesResult(error.Id, ConvertToError(error.Error)));
            }

            //Read document linked entities
            foreach (DocumentLinkedEntities docEntities in results.Documents)
            {
                recognizeEntities.Add(new RecognizeLinkedEntitiesResult(docEntities.Id, docEntities.Statistics ?? default, ConvertToLinkedEntityCollection(docEntities)));
            }

            recognizeEntities = SortHeterogeneousCollection(recognizeEntities, idToIndexMap);

            return new RecognizeLinkedEntitiesResultCollection(recognizeEntities, results.Statistics, results.ModelVersion);
        }

        #endregion

        #region Healthcare

        internal static List<HealthcareEntity> ConvertToHealthcareEntityCollection(IEnumerable<HealthcareEntityInternal> healthcareEntities, IEnumerable<HealthcareRelationInternal> healthcareRelations)
        {
            return healthcareEntities.Select((entity) => new HealthcareEntity(entity, ResolveRelatedEntities(entity, healthcareEntities, healthcareRelations))).ToList();
        }

        internal static AnalyzeHealthcareEntitiesResultCollection ConvertToRecognizeHealthcareEntitiesResultCollection(HealthcareResult results, IDictionary<string, int> idToIndexMap)
        {
            var healthcareEntititesResults = new List<AnalyzeHealthcareEntitiesResult>();

            //Read errors
            foreach (DocumentError error in results.Errors)
            {
                healthcareEntititesResults.Add(new AnalyzeHealthcareEntitiesResult(error.Id, ConvertToError(error.Error)));
            }

            //Read entities
            foreach (DocumentHealthcareEntitiesInternal documentHealthcareEntities in results.Documents)
            {
                healthcareEntititesResults.Add(new AnalyzeHealthcareEntitiesResult(
                    documentHealthcareEntities.Id,
                    documentHealthcareEntities.Statistics ?? default,
                    ConvertToHealthcareEntityCollection(documentHealthcareEntities.Entities, documentHealthcareEntities.Relations),
                    ConvertToWarnings(documentHealthcareEntities.Warnings)));
            }

            healthcareEntititesResults = healthcareEntititesResults.OrderBy(result => idToIndexMap[result.Id]).ToList();

            return new AnalyzeHealthcareEntitiesResultCollection(healthcareEntititesResults, results.Statistics, results.ModelVersion);
        }

        private static Dictionary<HealthcareEntity, HealthcareEntityRelationType> ResolveRelatedEntities(HealthcareEntityInternal entity,
            IEnumerable<HealthcareEntityInternal> healthcareEntities,
            IEnumerable<HealthcareRelationInternal> healthcareRelations)
        {
            Dictionary<HealthcareEntity, HealthcareEntityRelationType> dictionary = new Dictionary<HealthcareEntity, HealthcareEntityRelationType>();

            if (!healthcareRelations.Any())
            {
                return dictionary;
            }

            foreach (HealthcareRelationInternal relation in healthcareRelations)
            {
                int entityIndex = healthcareEntities.ToList().FindIndex(e => e.Text.Equals(entity.Text));

                if (IsEntitySource(relation, entityIndex))
                {
                    string targetRef = relation.Target;

                    HealthcareEntityInternal relatedEntity = ResolveHealthcareEntity(healthcareEntities, targetRef);

                    dictionary.Add(new HealthcareEntity(relatedEntity, ResolveRelatedEntities(relatedEntity, healthcareEntities, healthcareRelations)), relation.RelationType);
                }
            }

            return dictionary;
        }

        private static HealthcareEntityInternal ResolveHealthcareEntity(IEnumerable<HealthcareEntityInternal> entities, string reference)
        {
            Match healthcareEntityMatch = _healthcareEntityRegex.Match(reference);
            if (healthcareEntityMatch.Success)
            {
                int entityIndex = int.Parse(healthcareEntityMatch.Groups["entityIndex"].Value, CultureInfo.InvariantCulture);

                if (entityIndex < entities.Count())
                {
                    return entities.ElementAt(entityIndex);
                }
            }

            throw new InvalidOperationException($"Failed to parse element reference: {reference}");
        }

        private static Regex _healthcareEntityRegex = new Regex(@"\#/results/documents\/(?<documentIndex>\d*)\/entities\/(?<entityIndex>\d*)$", RegexOptions.Compiled, TimeSpan.FromSeconds(2));

        private static bool IsEntitySource(HealthcareRelationInternal healthcareRelation, int entityIndex)
        {
            string sourceRef = healthcareRelation.Source;

            var healthcareEntityMatch = _healthcareEntityRegex.Match(sourceRef);

            if (healthcareEntityMatch.Success)
            {
                int index = int.Parse(healthcareEntityMatch.Groups["entityIndex"].Value, CultureInfo.InvariantCulture);

                if (index == entityIndex)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Analyze Operation

        internal static AnalyzeOperationResult ConvertToAnalyzeOperationResult(AnalyzeJobState jobState, IDictionary<string, int> map)
        {
            return new AnalyzeOperationResult(jobState, map);
        }

        internal static IReadOnlyList<KeyPhraseExtractionTasksItem> ConvertToKeyPhraseExtractionTasks(IReadOnlyList<KeyPhraseExtractionTasksItem> keyPhraseExtractionTasks, IDictionary<string, int> idToIndexMap)
        {
            var collection = new List<KeyPhraseExtractionTasksItem>();
            foreach (KeyPhraseExtractionTasksItem task in keyPhraseExtractionTasks)
            {
                collection.Add(new KeyPhraseExtractionTasksItem(task, idToIndexMap));
            }

            return collection;
        }

        internal static IReadOnlyList<EntityRecognitionPiiTasksItem> ConvertToEntityRecognitionPiiTasks(IReadOnlyList<EntityRecognitionPiiTasksItem> entityRecognitionPiiTasks, IDictionary<string, int> idToIndexMap)
        {
            var collection = new List<EntityRecognitionPiiTasksItem>();
            foreach (EntityRecognitionPiiTasksItem task in entityRecognitionPiiTasks)
            {
                collection.Add(new EntityRecognitionPiiTasksItem(task, idToIndexMap));
            }

            return collection;
        }

        internal static IReadOnlyList<EntityRecognitionTasksItem> ConvertToEntityRecognitionTasks(IReadOnlyList<EntityRecognitionTasksItem> entityRecognitionTasks, IDictionary<string, int> idToIndexMap)
        {
            var collection = new List<EntityRecognitionTasksItem>();
            foreach (EntityRecognitionTasksItem task in entityRecognitionTasks)
            {
                collection.Add(new EntityRecognitionTasksItem(task, idToIndexMap));
            }

            return collection;
        }

        #endregion

        private static List<T> SortHeterogeneousCollection<T>(List<T> collection, IDictionary<string, int> idToIndexMap) where T : TextAnalyticsResult
        {
            return collection.OrderBy(result => idToIndexMap[result.Id]).ToList();
        }
    }
}

﻿using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser
{
    internal class KeywordsHelper : IKeywordsHelper
    {
        public const string SYMBOL_IT = "$";
        public const string SYMBOL_PARENT = "^";
        public const string SYMBOL_ROOT = "~";

        public const string KEYWORD_IT = "it";
        public const string KEYWORD_PARENT = "parent";
        public const string KEYWORD_ROOT = "root";

        public const string FUNCTION_IIF = "iif";
        public const string FUNCTION_ISNULL = "isnull";
        public const string FUNCTION_NEW = "new";
        public const string FUNCTION_NULLPROPAGATION = "np";
        public const string FUNCTION_IS = "is";
        public const string FUNCTION_AS = "as";
        public const string FUNCTION_CAST = "cast";

        private int _keywordsSize = 74;

        private readonly IDictionary<string, object> _keywords;

        public KeywordsHelper(ParsingConfig config)
        {
            IDictionary<string, Type> predefinedEnumerationTypes = EnumerationsFromMscorlib.PredefinedEnumerationTypes;
            HashSet<Type> customTypes = config.CustomTypeProvider.GetCustomTypes();

            _keywordsSize += predefinedEnumerationTypes.Count + (2 * customTypes.Count);

            _keywords = new Dictionary<string, object>(_keywordsSize, StringComparer.OrdinalIgnoreCase);

            _keywords.Add("true", Expression.Constant(true));
            _keywords.Add("false", Expression.Constant(false));
            _keywords.Add("null", Expression.Constant(null));

            if (config.AreContextKeywordsEnabled)
            {
                _keywords.Add(KEYWORD_IT, KEYWORD_IT);
                _keywords.Add(KEYWORD_PARENT, KEYWORD_PARENT);
                _keywords.Add(KEYWORD_ROOT, KEYWORD_ROOT);
            }

            _keywords.Add(SYMBOL_IT, SYMBOL_IT);
            _keywords.Add(SYMBOL_PARENT, SYMBOL_PARENT);
            _keywords.Add(SYMBOL_ROOT, SYMBOL_ROOT);

            _keywords.Add(FUNCTION_IIF, FUNCTION_IIF);
            _keywords.Add(FUNCTION_ISNULL, FUNCTION_ISNULL);
            _keywords.Add(FUNCTION_NEW, FUNCTION_NEW);
            _keywords.Add(FUNCTION_NULLPROPAGATION, FUNCTION_NULLPROPAGATION);
            _keywords.Add(FUNCTION_IS, FUNCTION_IS);
            _keywords.Add(FUNCTION_AS, FUNCTION_AS);
            _keywords.Add(FUNCTION_CAST, FUNCTION_CAST);

            foreach (Type type in PredefinedTypesHelper.PredefinedTypes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
            {
                if (!string.IsNullOrEmpty(type.FullName))
                {
                    _keywords[type.FullName] = type;
                }
                _keywords[type.Name] = type;
            }

            foreach (var pair in PredefinedTypesHelper.PredefinedTypesShorthands)
            {
                _keywords.Add(pair.Key, pair.Value);
            }

            if (config.SupportEnumerationsFromSystemNamespace)
            {
                foreach (var pair in predefinedEnumerationTypes)
                {
                    _keywords.Add(pair.Key, pair.Value);
                }
            }

            if (config.CustomTypeProvider != null)
            {
                foreach (Type type in customTypes)
                {
                    _keywords[type.FullName] = type;
                    _keywords[type.Name] = type;
                }
            }
        }

        public bool TryGetValue(string name, out object type)
        {
            return _keywords.TryGetValue(name, out type);
        }
    }
}

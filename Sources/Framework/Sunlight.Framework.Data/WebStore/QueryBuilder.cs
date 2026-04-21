namespace Sunlight.Framework.Data.WebStore
{
    using System;
    using System.Web.Html.Data.IndexedDB;

    /// <summary>
    /// Fluent builder that composes a <see cref="Query"/> from
    /// equality, range, skip/limit, and ordering constraints.
    /// </summary>
    public class QueryBuilder
    {
        private readonly string[] _keyPaths;
        private bool _excludeFrom;
        private bool _excludeTo;
        private object[] _lowerBound;
        private object[] _strictMatch;
        private object[] _upperBound;
        private bool _isDescending;

        public QueryBuilder(string[] keyPaths)
        {
            _keyPaths = keyPaths;
        }

        protected int? LimitCount { get; private set; }

        protected object[] LowerBound
        {
            get { return _lowerBound; }
            set
            {
                this.CheckNoStrictMatch();

                if (_lowerBound != null)
                {
                    throw new Exception("Lower bound already set");
                }

                _lowerBound = value;
            }
        }

        protected int? SkipCount { get; private set; }

        protected object[] StrictMatch
        {
            get { return _strictMatch; }
            set
            {
                this.CheckNoRangeMatch();

                if (_strictMatch != null)
                {
                    throw new Exception("Strict match already set");
                }

                _strictMatch = value;
            }
        }

        protected object[] UpperBound
        {
            get { return _upperBound; }
            set
            {
                this.CheckNoStrictMatch();

                if (_upperBound != null)
                {
                    throw new Exception("Upper bound already set");
                }

                _upperBound = value;
            }
        }

        /// <summary>Requests descending (reverse) cursor iteration.</summary>
        public QueryBuilder Descending()
        {
            _isDescending = true;
            return this;
        }

        public QueryBuilder Equal<TKey>(TKey key)
        {
            this.StrictMatch = new object[] {
                Type.AS<TKey, object>(key),
            };

            return this;
        }

        public QueryBuilder Equal<TKey, TKey2>(TKey key, TKey2 key2)
        {
            this.CheckKeyCount(2);
            this.StrictMatch = new object[] {
                Type.AS<TKey, object>(key),
                Type.AS<TKey2, object>(key2),
            };
            return this;
        }

        public QueryBuilder Equal<TKey, TKey2, TKey3>(TKey key, TKey2 key2, TKey3 key3)
        {
            this.CheckKeyCount(3);
            this.StrictMatch = new object[] {
                Type.AS<TKey, object>(key),
                Type.AS<TKey2, object>(key2),
                Type.AS<TKey3, object>(key3),
            };

            return this;
        }

        /// <summary>Excludes the lower bound from the range (defaults to inclusive).</summary>
        public QueryBuilder ExcludeFrom()
        {
            _excludeFrom = true;
            return this;
        }

        /// <summary>Excludes the upper bound from the range (defaults to inclusive).</summary>
        public QueryBuilder ExcludeTo()
        {
            _excludeTo = true;
            return this;
        }

        public QueryBuilder From<TKey>(TKey key)
        {
            this.LowerBound = new object[] { Type.AS<TKey, object>(key) };
            return this;
        }

        public QueryBuilder From<TKey, TKey2>(TKey key, TKey2 key2)
        {
            this.CheckKeyCount(2);
            this.LowerBound = new object[] {
                Type.AS<TKey, object>(key),
                Type.AS<TKey2, object>(key2),
            };

            return this;
        }

        public QueryBuilder From<TKey, TKey2, TKey3>(TKey key, TKey2 key2, TKey3 key3)
        {
            this.CheckKeyCount(3);
            this.LowerBound = new object[] {
                Type.AS<TKey, object>(key),
                Type.AS<TKey2, object>(key2),
                Type.AS<TKey3, object>(key3),
            };

            return this;
        }

        public QueryBuilder Limit(int count)
        {
            LimitCount = count;
            return this;
        }

        public QueryBuilder Skip(int count)
        {
            SkipCount = count;
            return this;
        }

        public QueryBuilder To<TKey>(TKey key)
        {
            this.UpperBound = new object[] {
                Type.AS<TKey, object>(key),
            };

            return this;
        }

        public QueryBuilder To<TKey, TKey2>(TKey key, TKey2 key2)
        {
            this.CheckKeyCount(2);
            this.UpperBound = new object[] {
                Type.AS<TKey, object>(key),
                Type.AS<TKey2, object>(key2),
            };

            return this;
        }

        public QueryBuilder To<TKey, TKey2, TKey3>(TKey key, TKey2 key2, TKey3 key3)
        {
            this.CheckKeyCount(3);
            this.UpperBound = new object[] {
                Type.AS<TKey, object>(key),
                Type.AS<TKey2, object>(key2),
                Type.AS<TKey3, object>(key3),
            };

            return this;
        }

        /// <summary>Compose the accumulated constraints into an immutable <see cref="Query"/>.</summary>
        public Query Build()
        {
            IDBKeyRange singleColumnRange = null;
            IDBKeyRange range;
            if (_lowerBound != null && _upperBound != null)
            {
                range = IDBKeyRange.Bound(
                    (NativeArray<object>)_lowerBound,
                    (NativeArray<object>)_upperBound,
                    _excludeFrom,
                    _excludeTo);

                if (_lowerBound.Length == 1)
                {
                    singleColumnRange = IDBKeyRange.Bound(
                        _lowerBound[0],
                        _upperBound[0],
                        _excludeFrom,
                        _excludeTo);
                }
            }
            else if (_lowerBound != null)
            {
                range = IDBKeyRange.LowerBound(
                    (NativeArray<object>)_lowerBound,
                    _excludeFrom);

                if (_lowerBound.Length == 1)
                {
                    singleColumnRange = IDBKeyRange.LowerBound(
                        _lowerBound[0],
                        _excludeFrom);
                }
            }
            else if (_upperBound != null)
            {
                range = IDBKeyRange.UpperBound(
                    (NativeArray<object>)_upperBound,
                    _excludeTo);

                if (_upperBound.Length == 1)
                {
                    singleColumnRange = IDBKeyRange.UpperBound(
                        _upperBound[0],
                        _excludeTo);
                }
            }
            else if (_strictMatch != null)
            {
                range = IDBKeyRange.Only(
                    (NativeArray<object>)_strictMatch);

                if (_strictMatch.Length == 1)
                {
                    singleColumnRange = IDBKeyRange.Only(
                        _strictMatch[0]);
                }
            }
            else
            {
                if (SkipCount == null
                    && LimitCount == null
                    && !_isDescending)
                {
                    throw new Exception("Incorrect use of QueryBuild, use Query.All");
                }

                return new Query(
                    new string[0],
                    null,
                    null,
                    _isDescending,
                    SkipCount,
                    LimitCount);
            }

            return new Query(
                _keyPaths,
                range,
                singleColumnRange,
                _isDescending,
                SkipCount,
                LimitCount);
        }

        private void CheckNoRangeMatch()
        {
            if (_lowerBound != null || _upperBound != null)
            { throw new Exception("Query already associated with Strict Match"); }
        }

        private void CheckNoStrictMatch()
        {
            if (_strictMatch != null)
            { throw new Exception("Query already associated with Strict Match"); }
        }

        private void CheckKeyCount(int keys)
        {
            if (keys > _keyPaths.Length)
            { throw new Exception("Can't use more keys then key paths"); }
        }
    }
}

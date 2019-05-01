﻿using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.Contracts
{
    public static class FilterExtensions
    {
        private readonly static object[] EmptyObjectArray = new object[0];
        public static string Key(this FilterLog log)
        {
            if (log.TransactionHash == null || log.LogIndex == null)
                return log.GetHashCode().ToString();

            return $"{log.TransactionHash}{log.LogIndex.HexValue}";
        }

        public static Dictionary<string, FilterLog> Merge(this Dictionary<string, FilterLog> masterList, FilterLog[] candidates)
        {
            foreach (var log in candidates)
            {
                var key = log.Key();

                if (!masterList.ContainsKey(key))
                {
                    masterList.Add(key, log);
                }
            }

            return masterList;
        }

        public static void SetBlockRange(this NewFilterInput filter, BlockRange range)
        {
            filter.FromBlock = new BlockParameter(range.From);
            filter.ToBlock = new BlockParameter(range.To);
        }

        public static bool IsTopicFiltered(this NewFilterInput filter, uint topicNumber)
        {
            var filterValue = filter.GetFirstTopicValue(topicNumber);
            return filterValue != null;
        }

        public static string GetFirstTopicValueAsString(this NewFilterInput filter, uint topicNumber)
        {
            var filterValue = filter.GetFirstTopicValue(topicNumber);
            return filterValue?.ToString();
        }

        public static object GetFirstTopicValue(this NewFilterInput filter, uint topicNumber)
        {
            var topicValues = filter.GetTopicValues(topicNumber);
            return topicValues.FirstOrDefault();
        }

        public static object[] GetTopicValues(this NewFilterInput filter, uint topicNumber)
        {
            var allTopics = filter.Topics;

            if (allTopics == null) return EmptyObjectArray;
            if (allTopics.Length < 2) return EmptyObjectArray;
            if (topicNumber > allTopics.Length) return EmptyObjectArray;

            if (allTopics[topicNumber] is object[] topicValues)
                return topicValues;

            if (allTopics[topicNumber] is object val)
                return new [] {val};

            return EmptyObjectArray;
        }
    }
}

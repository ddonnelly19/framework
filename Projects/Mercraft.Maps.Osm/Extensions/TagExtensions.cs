﻿
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Extensions
{
    /// <summary>
    /// Contains extensions that aid in interpreting some of the OSM-tags.
    /// </summary>
    public static class TagExtensions
    {
        private static string[] BooleanTrueValues = { "yes", "true", "1" };
        private static string[] BooleanFalseValues = { "no", "false", "0" };

        private const string REGEX_DECIMAL = @"\s*(\d+(?:\.\d*)?)\s*";

        private const string REGEX_UNIT_TONNES = @"\s*(t|to|tonnes|tonnen)?\s*";
        private const string REGEX_UNIT_METERS = @"\s*(m|meters|metres|meter)?\s*";
        private const string REGEX_UNIT_KILOMETERS = @"\s*(km)?\s*";
        //private const string REGEX_UNIT_KILOMETERS_PER_HOUR = @"\s*(kmh|km/h|kph)?\s*";
        //private const string REGEX_UNIT_MILES_PER_HOUR = @"\s*(mph)?\s*";

        /// <summary>
        /// Returns true if the given tags key has an associated value that can be interpreted as true.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="tagKey"></param>
        /// <returns></returns>
        public static bool IsTrue(this ICollection<Tag> tags, string tagKey)
        {
            if (tags == null || IsNullOrWhiteSpace(tagKey))
                return false;

            // TryGetValue tests if the 'tagKey' is present, returns true if the associated value can be interpreted as true.
            //                                               returns false if the associated value can be interpreted as false.
            string tagValue;
            return tags.TryGetValue(tagKey, out tagValue) && 
                BooleanTrueValues.Contains(tagValue.ToLowerInvariant());
        }

        /// <summary>
        /// Returns true if the given tags key has an associated value that can be interpreted as false.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="tagKey"></param>
        /// <returns></returns>
        public static bool IsFalse(this ICollection<Tag> tags, string tagKey)
        {
            if (tags == null || IsNullOrWhiteSpace(tagKey))
                return false;
            string tagValue;
            return tags.TryGetValue(tagKey, out tagValue) &&
                BooleanFalseValues.Contains(tagValue.ToLowerInvariant());
        }

        /// <summary>
        /// Searches for the tags collection for the <c>Access</c>-Tags and returns the associated values.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Key:access
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="accessTagHierachy">The hierarchy of <c>Access</c>-Tags for different vehicle types.</param>
        /// <returns>The best fitting value is returned.</returns>
        public static string GetAccessTag(this ICollection<Tag> tags, IEnumerable<string> accessTagHierachy)
        {
            if (tags == null)
                return null;
            foreach (string s in accessTagHierachy)
            {
                string access;
                if (tags.TryGetValue(s, out access))
                    return access;
            }
            return null;
        }

        #region Reading Tags

        /// <summary>
        /// Searches for a maxspeed tag and returns the associated value.
        /// 
        ///  http://wiki.openstreetmap.org/wiki/Key:maxspeed
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxSpeed(this ICollection<Tag> tags, out double result)
        {
            result = double.MaxValue;
            string tagValue;
            if (tags == null || !tags.TryGetValue("maxspeed", out tagValue) || IsNullOrWhiteSpace(tagValue) ||
                tagValue == "none" || tagValue == "signals" || tagValue == "signs" || tagValue == "no")
                return false;
            return TagExtensions.TryParseSpeed(tagValue, out result);
        }

        /// <summary>
        /// Searches for a maxweight tag and returns the associated value.
        /// 
        ///  http://wiki.openstreetmap.org/wiki/Key:maxweight
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxWeight(this ICollection<Tag> tags, out double result)
        {
            result = double.MaxValue;
            string tagValue;
            if (tags == null || !tags.TryGetValue("maxweight", out tagValue) || IsNullOrWhiteSpace(tagValue))
                return false;
            return TagExtensions.TryParseWeight(tagValue, out result);
        }

        /// <summary>
        /// Searches for a max axle load tag and returns the associated value.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Key:maxaxleload
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxAxleLoad(this ICollection<Tag> tags, out double result)
        {
            result = double.MaxValue;
            string tagValue;
            if (tags == null || !tags.TryGetValue("maxaxleload", out tagValue) || IsNullOrWhiteSpace(tagValue))
                return false;
            return TagExtensions.TryParseWeight(tagValue, out result);
        }

        /// <summary>
        /// Searches for a max height tag and returns the associated value.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Maxheight
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxHeight(this ICollection<Tag> tags, out double result)
        {
            result = double.MaxValue;

            string tagValue;
            if (tags == null || !tags.TryGetValue("maxheight", out tagValue) || IsNullOrWhiteSpace(tagValue))
                return false;

            return TagExtensions.TryParseLength(tagValue, out result);
        }

        /// <summary>
        /// Searches for a max width tag and returns the associated value.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Key:maxwidth
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxWidth(this ICollection<Tag> tags, out double result)
        {
            result = double.MaxValue;
            string tagValue;
            if (tags == null || !tags.TryGetValue("maxwidth", out tagValue) || IsNullOrWhiteSpace(tagValue))
                return false;
            return TagExtensions.TryParseLength(tagValue, out result);
        }

        /// <summary>
        /// Searches for a max length tag and returns the associated value.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Key:maxlength
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxLength(this IDictionary<string, string> tags, out double result)
        {
            result = double.MaxValue;

            string tagValue;
            if (tags == null || !tags.TryGetValue("maxlength", out tagValue) || IsNullOrWhiteSpace(tagValue))
                return false;

            return TagExtensions.TryParseLength(tagValue, out result);
        }

        #endregion

        #region Parsing Units

        /// <summary>
        /// Tries to parse a speed value from a given tag-value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseSpeed(string s, out double result)
        {
            result = double.MaxValue;

            if (IsNullOrWhiteSpace(s))
                return false;

            if (s[0] != '0' && s[0] != '1' && s[0] != '2' && s[0] != '3' && s[0] != '4' &&
                s[0] != '5' && s[0] != '6' && s[0] != '7' && s[0] != '8' && s[0] != '9')
            { // performance inprovement, quick negative answer.
                return false;
            }

            if(s.Contains(","))
            { // refuse comma as a decimal seperator or anywhere else in the number.
                return false;
            }

            // try regular speed: convention in OSM is km/h in this case.
            double kmphDouble;
            if (double.TryParse(s, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out kmphDouble))
            {
                result = kmphDouble;
            }

            // try km/h
            if (double.TryParse(s, out result))
            {
                return true;
            }

            // try mph.
            double resultMph;
            if (double.TryParse(s, out resultMph))
            {
                result = resultMph;
                return true;
            }

            // try knots.
            double resultKnots;
            if (double.TryParse(s, out resultKnots))
            {
                result = resultKnots;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a weight value from a given tag-value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseWeight(string s, out double result)
        {
            result = double.MaxValue;

            if (IsNullOrWhiteSpace(s))
                return false;

            Regex tonnesRegex = new Regex("^" + REGEX_DECIMAL + REGEX_UNIT_TONNES + "$", RegexOptions.IgnoreCase);
            Match tonnesMatch = tonnesRegex.Match(s);
            if (tonnesMatch.Success)
            {
                result = (double.Parse(tonnesMatch.Groups[1].Value, CultureInfo.InvariantCulture) * 1000);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a distance measure from a given tag-value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseLength(string s, out double result)
        {
            result = double.MaxValue;

            if (IsNullOrWhiteSpace(s))
                return false;

            Regex metresRegex = new Regex("^" + REGEX_DECIMAL + REGEX_UNIT_METERS + "$", RegexOptions.IgnoreCase);
            Match metresMatch = metresRegex.Match(s);
            if (metresMatch.Success)
            {
                result = double.Parse(metresMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                return true;
            }

            Regex feetInchesRegex = new Regex("^(\\d+)\\'(\\d+)\\\"$", RegexOptions.IgnoreCase);
            Match feetInchesMatch = feetInchesRegex.Match(s);
            if (feetInchesMatch.Success)
            {
                int feet = int.Parse(feetInchesMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                int inches = int.Parse(feetInchesMatch.Groups[2].Value, CultureInfo.InvariantCulture);

                result = feet * 0.3048 + inches * 0.0254;
                return true;
            }

            return false;
        }

        #endregion

        #region Collection helpers

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        public static bool TryGetValue(this ICollection<Tag> tags, string key, out string value)
        {
            foreach (var tag in tags)
            {
                if (tag.Key == key)
                {
                    value = tag.Value;
                    return true;
                }
            }
            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Returns true if the given key is found in this tags collection.
        /// </summary>
        public static bool ContainsKey(this ICollection<Tag> tags, string key)
        {
            return tags != null && tags.Any(tag => tag.Key == key);
        }

        #endregion

        public static bool IsNullOrWhiteSpace(string str)
        {
            return str == null || str.Trim() == "";
        }

    }
}
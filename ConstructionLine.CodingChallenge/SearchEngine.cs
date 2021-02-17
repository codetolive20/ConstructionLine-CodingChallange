using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            if (shirts == null)
                throw new ArgumentNullException(nameof(shirts));

            _shirts = shirts;
        }


        public SearchResults Search(SearchOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));


            var results = _shirts.Where(shirt =>
                    (!options.Colors.Any() || options.Colors.Contains(shirt.Color)) &&
                    (!options.Sizes.Any() || options.Sizes.Contains(shirt.Size)))
                .ToList();


            return new SearchResults
            {
                Shirts = results,
                ColorCounts = Color.All.Select(color => new ColorCount
                {
                    Color = color,
                    Count = results.Count(shirt => shirt.Color == color)
                }).ToList(),
                SizeCounts = Size.All.Select(size => new SizeCount
                {
                    Size = size,
                    Count = results.Count(shirt => shirt.Size == size)
                }).ToList()
            };
        }

    }
}
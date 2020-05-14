using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Server.Configuration;
using WelcomeTo.Server.Extensions;
using WelcomeTo.Shared;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Server.Services
{
    public interface IGameBuilder
    {
        public Game Build(string id, string name);

        public Board StartingBoard { get; }

        public ScoreSheet StartingScoreSheet { get; }
    }

    public class GameBuilder : IGameBuilder
    {
        private readonly Dictionary<CardType, List<int>> _cardDistribution;
        private readonly Dictionary<StreetPosition, List<bool>> _poolPositions;
        private readonly Dictionary<StreetPosition, List<int>> _parkPoints;
        private readonly Dictionary<RealEstateSize, List<int>> _realEstateSizeValues;

        private readonly IEnumerable<int> _poolPoints;
        private readonly IEnumerable<int> _tempAgencyPoints;
        private readonly IEnumerable<int> _bisPoints;
        private readonly IEnumerable<int> _refusalPoints;

        private readonly List<CityPlan> _cityPlans1 = new List<CityPlan>();
        private readonly List<CityPlan> _cityPlans2 = new List<CityPlan>();
        private readonly List<CityPlan> _cityPlans3 = new List<CityPlan>();

        private readonly Random _random = new Random();

        public GameBuilder(IOptions<ApplicationOptions> options)
        {
            _cardDistribution = options.Value.CardDistribution;
            _poolPositions = options.Value.PoolPositions;
            _parkPoints = options.Value.ParkPoints;
            _realEstateSizeValues = options.Value.RealEstateSizes;
            _poolPoints = options.Value.PoolPoints;
            _tempAgencyPoints = options.Value.TempAgencyPoints;
            _bisPoints = options.Value.BisPoints;
            _refusalPoints = options.Value.RefusalPoints;
            _bisPoints = options.Value.BisPoints;
            foreach (var cityPlan in options.Value.CityPlans)
            {
                switch (cityPlan.Type)
                {
                    case PlanType.No1:
                        _cityPlans1.Add(cityPlan);
                        break;
                    case PlanType.No2:
                        _cityPlans2.Add(cityPlan);
                        break;
                    case PlanType.No3:
                        _cityPlans3.Add(cityPlan);
                        break;
                }
            }
        }

        public Game Build(string id, string name)
        {
            return new Game
            {
                Id = new Guid(id),
                Name = name ?? "Unnamed Game",
                Players = new List<Player> { new Player { Name = "Host", IsHost = true, Board = StartingBoard, ScoreSheet = StartingScoreSheet } },
                GameDeck = GetGameDeck(),
                CurrentTurn = new Turn(),
                Plans = new CityPlans
                {
                    Plan1 = _cityPlans1[_random.Next(_cityPlans1.Count)],
                    Plan2 = _cityPlans2[_random.Next(_cityPlans2.Count)],
                    Plan3 = _cityPlans3[_random.Next(_cityPlans3.Count)]
                },
                TempAgencyPoints = _tempAgencyPoints.ToList()
            };
        }

        public Board StartingBoard => new Board
        {
            TopStreet = BuildStreet(StreetPosition.Top),
            MiddleStreet = BuildStreet(StreetPosition.Middle),
            BottomStreet = BuildStreet(StreetPosition.Bottom)
        };

        public ScoreSheet StartingScoreSheet => new ScoreSheet
        {
            RealEstateValuesTable = _realEstateSizeValues.ToDictionary(s => s.Key, s => s.Value.Select(points => new PointsListItem { Points = points, IsCovered = false }).ToList()),
            PoolPoints = _poolPoints.Select(points => new PointsListItem { Points = points, IsCovered = false }).ToList(),
            BisPoints = _bisPoints.Select(points => new PointsListItem { Points = points, IsCovered = false }).ToList(),
            RefusalPoints = _refusalPoints.Select(points => new PointsListItem { Points = points, IsCovered = false }).ToList()
        };

        private GameDeck GetGameDeck()
        {
            var deck = new GameDeck()
            {
                Deck1 = new Stack<Card>(),
                Deck2 = new Stack<Card>(),
                Deck3 = new Stack<Card>(),
                Discard1 = new Stack<Card>(),
                Discard2 = new Stack<Card>(),
                Discard3 = new Stack<Card>()
            };

            var cards = _cardDistribution.SelectMany(x => x.Value.Select(number => new Card { Type = x.Key, HouseNumber = number })).Shuffle();
            cards.Distribute(deck.Deck1, deck.Deck2, deck.Deck3);
            return deck;
        }

        private Street BuildStreet(StreetPosition position)
        {
            return new Street
            {
                Position = position,
                Parks = _parkPoints[position].Select(p => new Park { Points = p, IsCovered = false }).ToList(),
                Houses = _poolPositions[position].Select((hasPool, index) => new House { Index = index, Pool = hasPool ? PoolType.Unbuilt : PoolType.None }).ToList()
            };
        }
    }
}